using Microsoft.EntityFrameworkCore;
using NZWalk.API.Data;
using NZWalk.API.Models.Domain;
using NZWalk.API.Repositories.Interfaces;

namespace NZWalk.API.Repositories
{
    public class WalkRepository : IWalkRepository
    {
        private readonly NZWalkDbContext dbContext;

        public WalkRepository(NZWalkDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<Walk> CreateAsync(Walk walk)
        {
            await dbContext.AddAsync(walk);
            await dbContext.SaveChangesAsync(); 

            return walk;
        }

        public async Task<Walk?> DeleteAsync(Guid id)
        {
            var existingWalk =  await dbContext.walks.FirstOrDefaultAsync(x => x.Id == id);
            if (existingWalk != null)
            {
                dbContext.walks.Remove(existingWalk);
                dbContext.SaveChanges();
                return existingWalk;
            }
            return null;
        }

        public async Task<List<Walk>> GetAllAsync()
        {
            return await dbContext.walks.Include("Difficulty").Include("Region").ToListAsync();
        }
         
        public async Task<Walk?> GetByIdAsync(Guid id)
        {
            return await dbContext.walks.Include("Difficulty").Include("Region").FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Walk?> UpdateAsync(Guid id, Walk walk)
        {
            var exisingWalk = await dbContext.walks.FirstOrDefaultAsync(x => x.Id == id);
            if (exisingWalk != null)
            {
                exisingWalk.Name = walk.Name;
                exisingWalk.Description = walk.Description;
                exisingWalk.LengthInKm = walk.LengthInKm;
                exisingWalk.WalkImageUrl = walk.WalkImageUrl;
                exisingWalk.DifficultyId = walk.DifficultyId;  
                exisingWalk.RegionId = walk.RegionId;

                await dbContext.SaveChangesAsync();
                return exisingWalk;
            }
            return null;
        }
    }
}
