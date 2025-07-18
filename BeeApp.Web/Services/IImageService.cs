namespace BeeApp.Web.Services
{
    public interface IImageService
    {
        Task<string> SaveImageAsync(IFormFile file, string folder = "apiaries", int maxWidth = 400);
        void DeleteImage(string fileName, string folder = "apiaries");
    }
}
