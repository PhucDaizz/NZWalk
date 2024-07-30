using Microsoft.EntityFrameworkCore;
using NZWalk.API.Data;
using NZWalk.API.Models.Domain;
using NZWalk.API.Repositories.Interfaces;

namespace NZWalk.API.Repositories
{
    public class RegionRepository : IRegionRepository
    {
        private readonly NZWalkDbContext dbContext;

        public RegionRepository(NZWalkDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Region> CreateAsync(Region region)
        {
            await dbContext.regions.AddAsync(region);
            await dbContext.SaveChangesAsync();
            return region;
        }

        public async Task<Region?> DeleteAsync(Guid id)
        {
            var existingRegion = await dbContext.regions.FirstOrDefaultAsync(x => x.Id == id);
            if (existingRegion != null)
            {
                dbContext.regions.Remove(existingRegion);
                await dbContext.SaveChangesAsync();
                return existingRegion;
            }
            return null;
        }

        public async Task<List<Region>> GetAllAsync()
        {
            return await dbContext.regions.ToListAsync();
        }

        public Task<Region?> GetByIdAsync(Guid id)
        {
            return dbContext.regions.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Region?> UpdateAsync(Guid id, Region region)
        {
            var existingRegion = await dbContext.regions.FirstOrDefaultAsync(x => x.Id == id);
            if (existingRegion == null)
            {
                return null;
            }
            existingRegion.Code = region.Code;
            existingRegion.Name = region.Name;
            existingRegion.RegionImageUrl = region.RegionImageUrl;
           
            await dbContext.SaveChangesAsync();
            return existingRegion;
        }
    }
}
