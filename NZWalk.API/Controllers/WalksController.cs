using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NZWalk.API.CustomActionFilters;
using NZWalk.API.Data;
using NZWalk.API.Models.Domain;
using NZWalk.API.Models.DTO;
using NZWalk.API.Repositories.Interfaces;

namespace NZWalk.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalksController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly NZWalkDbContext nZWalkDbContext;
        private readonly IWalkRepository walkRepository;

        public WalksController(IMapper mapper, 
            NZWalkDbContext nZWalkDbContext, 
            IWalkRepository walkRepository)
        {
            this.mapper = mapper;
            this.nZWalkDbContext = nZWalkDbContext;
            this.walkRepository = walkRepository;
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Create([FromBody] AddWalkRequestDto addWalkRequestDto)
        {
            // Map Dto to domain model
            var walkDomainModel = mapper.Map<Walk>(addWalkRequestDto);
            await walkRepository.CreateAsync(walkDomainModel);

            // Map domain model to DTo
            return Ok(mapper.Map<WalkDto>(walkDomainModel));
        }


        // GET Walk 
        // GET: api/walks?filterOn=Name&filterQuery=Track&sortBy=Name&isAsending=True&pageNumber=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? filterOn, [FromQuery] string? filterQuery,
                                [FromQuery] string? sortBy, [FromQuery] bool? isAsending,
                                [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var walksDomainModel = await walkRepository.GetAllAsync(filterOn, filterQuery, sortBy, isAsending ?? true, 
                                                                pageNumber, pageSize);

            // Map domain model to Dto
            return Ok(mapper.Map<List<WalkDto>>(walksDomainModel));
        }

        // Get walk by id
        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var walkDomainModel = await walkRepository.GetByIdAsync(id);
            if (walkDomainModel == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<WalkDto>(walkDomainModel));
        }

        // Update walk by id
        [HttpPut]
        [Route("{id:Guid}")]
        [ValidateModel]
        public async Task<IActionResult> Update([FromRoute] Guid id, UpdateWalkRequestDto updateWalkRequestDto)
        {
            // Map Dto to domain model 
            var walkDomainModel = mapper.Map<Walk>(updateWalkRequestDto);
            walkDomainModel = await walkRepository.UpdateAsync(id, walkDomainModel);

            if (walkDomainModel == null)
            {
                return NotFound();
            }

            // Map Domain model to Dto
            return Ok(mapper.Map<WalkDto>(walkDomainModel));
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deletedWalkDomainModel = await walkRepository.DeleteAsync(id);
            if (deletedWalkDomainModel == null)
            {
                return NotFound();
            }

            // Map Domain model to Dto
            return Ok(mapper.Map<WalkDto>(deletedWalkDomainModel));
        }
    }
}
