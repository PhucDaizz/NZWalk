using Microsoft.AspNetCore.Mvc;
using NZWalk.API.Models.Domain;

namespace NZWalk.API.Repositories.Interfaces
{
    public interface IWalkRepository
    {
        Task<Walk> CreateAsync(Walk walk);

        Task <List<Walk>> GetAllAsync(string? filterOn = null, string? filterQuery = null, 
                                    string? sortBy = null, bool isAsending = true,
                                    int pageNumber = 1, int pageSize = 10);

        Task<Walk?> GetByIdAsync(Guid id);

        Task<Walk?> UpdateAsync(Guid id, Walk walk);

        Task<Walk?> DeleteAsync(Guid id);
    }
}
