using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Http;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Security;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using SharedKernel.Infrastructure.Abstractions.Requests;
using SharedKernel.Infrastructure.Errors;
using SharedKernel.Infrastructure.Utils;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Schools.Commands.EditSchoolLogo
{
    [Authorize(Policy = PolicyNames.MustBeAtLeastHeadmaster)]
    public sealed class EditSchoolLogoCommand : IUserCommand, ISchoolAuthorizationRequest
    {
        public EditSchoolLogoCommand(IFormFile logo, Guid schoolId)
        {
            Logo = logo;
            SchoolId = schoolId;
        }

        public IFormFile Logo { get; }
        public Guid SchoolId { get; }
    }

    internal sealed class EditSchoolLogoCommandHandler : IRequestHandler<EditSchoolLogoCommand, Result<Unit, RequestError>>
    {
        private readonly ISchoolRepository _schoolRepository;
        private readonly ILogoStorageService _storageService;

        public EditSchoolLogoCommandHandler(
            ISchoolRepository schoolRepository,
            ILogoStorageService storageService)
        {
            _schoolRepository = Guard.Against.Null(schoolRepository, nameof(schoolRepository));
            _storageService = Guard.Against.Null(storageService, nameof(storageService));
        }

        public async Task<Result<Unit, RequestError>> Handle(EditSchoolLogoCommand request,
            CancellationToken cancellationToken)
        {
            var schoolId = new SchoolId(request.SchoolId);

            var schoolOrNone = await _schoolRepository.GetByIdAsync(schoolId, cancellationToken);

            if (schoolOrNone.HasNoValue)
                return SharedRequestError.General.NotFound(schoolId, nameof(School));

            Image logo = null;
            using (logo = await Image.LoadAsync(request.Logo.OpenReadStream()))
            {
                logo.Mutate(x => x.Resize(new ResizeOptions
                {
                    Mode = ResizeMode.Min,
                    Size = new Size(250, 250)
                }
                    ).Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.BoxPad,
                        Size = new Size(250, 250)
                    })
                    .BackgroundColor(Color.Transparent));

                await _storageService.UpsertLogoAsync(logo, schoolOrNone.Value, cancellationToken);
            }

            return Unit.Value;
        }
    }
}