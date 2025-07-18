using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace BeeApp.Web.Services
{
    public class ImageService : IImageService
    {
        private readonly string _uploadPath;

        public ImageService(IWebHostEnvironment env)
        {
            _uploadPath = Path.Combine(env.WebRootPath, "uploads", "apiaries");
            Directory.CreateDirectory(_uploadPath);
        }

        public async Task<string> SaveImageAsync(IFormFile file, string folder = "apiaries", int maxWidth = 400)
        {
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var folderPath = Path.Combine(_uploadPath, "..", folder);
            Directory.CreateDirectory(folderPath);
            var savePath = Path.Combine(folderPath, fileName);

            using var stream = file.OpenReadStream();
            using var image = await Image.LoadAsync(stream);

            // Změní velikost, pokud je příliš široký
            if (image.Width > maxWidth)
            {
                var ratio = (double)maxWidth / image.Width;
                var newHeight = (int)(image.Height * ratio);

                image.Mutate(x => x.Resize(maxWidth, newHeight));
            }

            // Uloží jako zkomprimovaný JPEG
            await image.SaveAsJpegAsync(savePath, new JpegEncoder { Quality = 85 });

            return fileName;
        }

        public void DeleteImage(string fileName, string folder = "apiaries")
        {
            var path = Path.Combine(_uploadPath, "..", folder, fileName);
            if (File.Exists(path))
                File.Delete(path);
        }
    }
}
