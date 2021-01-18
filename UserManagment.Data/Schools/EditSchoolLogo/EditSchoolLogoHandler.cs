using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.ResultErrors;
using MediatR;
using SchoolManagement.Core.SchoolAggregate.Schools;
using SchoolManagement.Core.SchoolAggregate.Users;
using SchoolManagement.Data.Database;
using SchoolManagement.Data.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Data.Schools.EditSchoolLogo
{
    public class EditSchoolLogoHandler : IRequestHandler<EditSchoolLogoCommand, Result<bool, RequestError>>
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IStorageService _storageService;
        private readonly SchoolContext _schoolContext;

        public EditSchoolLogoHandler(
            IAuthorizationService authorizationService,
            IStorageService storageService,
            SchoolContext schoolContext)
        {
            _authorizationService = authorizationService;
            _storageService = storageService;
            _schoolContext = schoolContext;
        }

        public async Task<Result<bool, RequestError>> Handle(EditSchoolLogoCommand request, CancellationToken cancellationToken)
        {
            Result<Tuple<School, User>, RequestError> authContext =
                await _authorizationService.GetAuthorizationContextAsync(request.SchoolId, request.AuthId);

            if (authContext.IsFailure)
                authContext.ConvertFailure<bool>();

            User currentUser = authContext.Value.Item2;
            School school = authContext.Value.Item1;
            string oldLogoId = school.LogoId;

            currentUser.EditSchoolLogo(school);

            if (!string.IsNullOrWhiteSpace(oldLogoId))
            {
                await _storageService.DeleteAsync(oldLogoId);
            }

            using (var logo = Image.Load(request.Logo.OpenReadStream()))
            {
                logo.Mutate(x => x.Resize(new ResizeOptions()
                {
                    Mode = ResizeMode.Min,
                    Size = new Size(250, 250)
                }
                ).Resize(new ResizeOptions()
                {
                    Mode = ResizeMode.BoxPad,
                    Size = new Size(250, 250)
                })
                .BackgroundColor(Color.Transparent));

                await _storageService.SaveAsync(logo, school, cancellationToken);
            }

            await _schoolContext.SaveChangesAsync(cancellationToken);

            return Result.Success<bool, RequestError>(true);
        }
    }
}
