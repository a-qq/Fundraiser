using System;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Http;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Security;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using SharedKernel.Infrastructure.Errors;
using SharedKernel.Infrastructure.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace SchoolManagement.Application.Schools.Commands.EditSchoolLogo
{
    [Authorize(Policy = "MustBeAtLeastHeadmaster")]
    public sealed class EditSchoolLogoCommand : CommandRequest
    {
        public EditSchoolLogoCommand(IFormFile logo, Guid schoolId)
        {
            Logo = logo;
            SchoolId = schoolId;
        }

        public IFormFile Logo { get; }
        public Guid SchoolId { get; }
    }

    internal sealed class EditSchoolLogoHandler : IRequestHandler<EditSchoolLogoCommand, Result<Unit, RequestError>>
    {
        private readonly ISchoolContext _context;
        private readonly ISchoolRepository _schoolRepository;
        private readonly ILogoStorageService _storageService;

        public EditSchoolLogoHandler(
            ISchoolRepository schoolRepository,
            ILogoStorageService storageService,
            ISchoolContext schoolContext)
        {
            _schoolRepository = schoolRepository;
            _storageService = storageService;
            _context = schoolContext;
        }

        public async Task<Result<Unit, RequestError>> Handle(EditSchoolLogoCommand request,
            CancellationToken cancellationToken)
        {
            var schoolId = new SchoolId(request.SchoolId);

            var schoolOrNone = await _schoolRepository.GetByIdAsync(schoolId, cancellationToken);

            if (schoolOrNone.HasNoValue)
                return SharedRequestError.General.NotFound(schoolId, nameof(School));

            Image logo = null;
            using (logo = Image.Load(request.Logo.OpenReadStream()))
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
            }

            await _storageService.UpsertLogoAsync(logo, schoolOrNone.Value, cancellationToken);

            //as new photo has been saved and old photo already is deleted, don't allow cancellation of the flow by passing token
            await _context.SaveChangesAsync();

            return Unit.Value;
        }
    }
}