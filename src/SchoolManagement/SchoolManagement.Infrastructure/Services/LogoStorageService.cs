using Microsoft.AspNetCore.Hosting;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using SixLabors.ImageSharp;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Infrastructure.Services
{
    internal sealed class LogoStorageService : ILogoStorageService
    {
        private readonly IWebHostEnvironment _env;

        public LogoStorageService(IWebHostEnvironment environment)
        {
            _env = environment;
        }

        public Task DeleteLogoAsync(School school, CancellationToken cancellationToken = default)
        {
            if (school == null)
                throw new ArgumentNullException(nameof(school));

            if (!string.IsNullOrWhiteSpace(school.LogoId))
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), _env.WebRootPath, "logos");
                string[] files = Directory.GetFiles(path, school.LogoId + ".*");
                foreach (var file in files)
                    File.Delete(file);
            }

            return Task.CompletedTask;
        }

        public async Task UpsertLogoAsync(Image image, School school, CancellationToken cancellationToken = default)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            if (school == null)
                throw new ArgumentNullException(nameof(school));

            await DeleteLogoAsync(school);

            school.EditLogo();

            var path = Path.Combine(Directory.GetCurrentDirectory(), _env.WebRootPath, "logos", school.LogoId + ".png");

            await image.SaveAsPngAsync(path);
        }

        //private bool IsValidFileName(string fileName)
        //{
        //    if (string.IsNullOrWhiteSpace(fileName) || fileName.Any(x => Path.GetInvalidFileNameChars().AsSpan().Contains(x)))
        //        return false;

        //    return true;
        //}
    }
}
