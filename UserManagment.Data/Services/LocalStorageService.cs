using Microsoft.AspNetCore.Hosting;
using SchoolManagement.Core.SchoolAggregate.Schools;
using SixLabors.ImageSharp;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Data.Services
{
    public sealed class LocalStorageService : IStorageService
    {
        private readonly IWebHostEnvironment _env;

        public LocalStorageService(IWebHostEnvironment environment)
        {
            _env = environment;
        }
        public Task DeleteAsync(string fileName)
        {
            
            var path = Path.Combine(Directory.GetCurrentDirectory(), _env.WebRootPath, "logos");
            string[] files = Directory.GetFiles(path, fileName + ".*");
            foreach(var file in files)
            {
                File.Delete(file);
            }

            return Task.CompletedTask;
        }

        public async Task SaveAsync(Image image, School school, CancellationToken cancellationToken = default)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), _env.WebRootPath, "logos", school.LogoId + ".png");
            await DeleteAsync(school.LogoId);
            await image.SaveAsPngAsync(path, cancellationToken);
        }
    }
}
