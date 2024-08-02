using NZWalk.API.Models.Domain;

namespace NZWalk.API.Repositories.Interfaces
{
    public interface IImageRepository
    {
        Task<Image> Upload(Image image);
    }
}
