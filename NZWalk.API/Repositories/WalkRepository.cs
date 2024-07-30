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

        public async Task<List<Walk>> GetAllAsync(string? filterOn = null, string? filterQuery = null,
                                        string? sortBy = null, bool isAsending = true,
                                        int pageNumber = 1, int pageSize = 10)
        {
            var walks = dbContext.walks.Include("Difficulty").Include("Region").AsQueryable();
            
            // Filtering 
            if(string.IsNullOrWhiteSpace(filterOn) == false && string.IsNullOrWhiteSpace(filterQuery) == false)
            {
                if(filterOn.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    walks = walks.Where(x => x.Name.Contains(filterQuery));
                }
            }

            // Sorting 
            if(string.IsNullOrWhiteSpace(sortBy) == false)
            {
                if(sortBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    walks = isAsending ? walks.OrderBy(x => x.Name) : walks.OrderByDescending(x => x.Name);
                }
                else if (sortBy.Equals("Length", StringComparison.OrdinalIgnoreCase))
                {
                    walks = isAsending ? walks.OrderBy(x => x.LengthInKm) : walks.OrderByDescending(x => x.LengthInKm);
                }

            }

            // Pagination
            var skipResult = (pageNumber - 1)*pageSize;

            return await walks.Skip(skipResult).Take(pageSize).ToListAsync();
            //return await dbContext.walks.Include("Difficulty").Include("Region").ToListAsync();
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
