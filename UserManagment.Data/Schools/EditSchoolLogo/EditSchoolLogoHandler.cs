using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.RequestErrors;
using MediatR;
using SchoolManagement.Core.Interfaces;
using SchoolManagement.Core.SchoolAggregate.Members;
using SchoolManagement.Core.SchoolAggregate.Schools;
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
        private readonly ISchoolRepository _schoolRepository;
        private readonly IAuthorizationService _authService;
        private readonly IStorageService _storageService;
        private readonly SchoolContext _schoolContext;

        public EditSchoolLogoHandler(
            ISchoolRepository schoolRepository,
            IAuthorizationService authorizationService,
            IStorageService storageService,
            SchoolContext schoolContext)
        {
            _schoolRepository = schoolRepository;
            _authService = authorizationService;
            _storageService = storageService;
            _schoolContext = schoolContext;
        }

        public async Task<Result<bool, RequestError>> Handle(EditSchoolLogoCommand request, CancellationToken cancellationToken)
        {
            await _authService.VerifyAuthorizationAsync(request.SchoolId, request.AuthId, Role.Headmaster);

            Maybe<School> schoolOrNone = await _schoolRepository.GetByIdAsync(request.SchoolId);
            if(schoolOrNone.HasNoValue)
                return Result.Failure<bool, RequestError>(SharedRequestError.General.NotFound(request.SchoolId, nameof(School)));

            if (!string.IsNullOrWhiteSpace(schoolOrNone.Value.LogoId))
            {
                //TODO: is it possible to rollback static file deletion (?) + pass token for future AzureBlob implementation
                await _storageService.DeleteAsync(schoolOrNone.Value.LogoId);
            }

            schoolOrNone.Value.EditLogo();

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

                await _storageService.SaveAsync(logo, schoolOrNone.Value);
            }

            //as old photo already has been deleted, don't pass token
            await _schoolContext.SaveChangesAsync();

            return Result.Success<bool, RequestError>(true);
        }
    }
}
