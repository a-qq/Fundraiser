using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.ResultErrors;
using MediatR;
using SchoolManagement.Core.SchoolAggregate.Schools;
using SchoolManagement.Core.SchoolAggregate.Members;
using SchoolManagement.Data.Database;
using SchoolManagement.Data.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Data.Schools.EditSchoolLogo
{
    public class EditSchoolLogoHandler : IRequestHandler<EditSchoolLogoCommand, Result<bool, RequestError>>
    {
        private readonly IAuthorizationService _authService;
        private readonly IStorageService _storageService;
        private readonly SchoolContext _schoolContext;

        public EditSchoolLogoHandler(
            IAuthorizationService authorizationService,
            IStorageService storageService,
            SchoolContext schoolContext)
        {
            _authService = authorizationService;
            _storageService = storageService;
            _schoolContext = schoolContext;
        }

        public async Task<Result<bool, RequestError>> Handle(EditSchoolLogoCommand request, CancellationToken cancellationToken)
        {
            Result<School, RequestError> schoolOrError =
                await _authService.VerifyAuthorizationAsync(request.SchoolId, request.AuthId, Role.Headmaster);

            if (schoolOrError.IsFailure)
                schoolOrError.ConvertFailure<bool>();

            if (!string.IsNullOrWhiteSpace(schoolOrError.Value.LogoId))
            {
                //TODO: is it possible to rollback static file deletion (?) + pass token for future AzureBlob implementation
                await _storageService.DeleteAsync(schoolOrError.Value.LogoId);
            }

            schoolOrError.Value.EditLogo();

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

                await _storageService.SaveAsync(logo, schoolOrError.Value);
            }

            //as old photo already has been deleted, don't pass token
            await _schoolContext.SaveChangesAsync();

            return Result.Success<bool, RequestError>(true);
        }
    }
}
