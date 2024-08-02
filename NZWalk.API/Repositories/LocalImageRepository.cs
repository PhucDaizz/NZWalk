using NZWalk.API.Data;
using NZWalk.API.Models.Domain;
using NZWalk.API.Repositories.Interfaces;

namespace NZWalk.API.Repositories
{
    public class LocalImageRepository : IImageRepository
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly NZWalkDbContext dbContext;

        public LocalImageRepository(IWebHostEnvironment webHostEnvironment, 
            IHttpContextAccessor httpContextAccessor,
            NZWalkDbContext dbContext)
        {
            this.webHostEnvironment = webHostEnvironment;
            this.httpContextAccessor = httpContextAccessor;
            this.dbContext = dbContext;
        }

        public async Task<Image> Upload(Image image)
        {
            var localFilePath = Path.Combine(webHostEnvironment.ContentRootPath, "Images",$"{image.Filename}{image.FileExtension}");

            //Upload Image to local Path
            using var stream = new  FileStream(localFilePath, FileMode.Create);
            await image.Name.CopyToAsync(stream);

            // https://localhost:xxxx/images/image.png
            var urlFilePath = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}{httpContextAccessor.HttpContext.Request.PathBase}/Images/{image.Filename}{image.FileExtension}";

            image.FilePath = urlFilePath;

            // Add Image to the Image table 
            await dbContext.images.AddAsync(image);
            await dbContext.SaveChangesAsync();
            
            return image;
        }
    }
}
