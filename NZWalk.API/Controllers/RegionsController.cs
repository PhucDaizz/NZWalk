using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalk.API.CustomActionFilters;
using NZWalk.API.Data;
using NZWalk.API.Models.Domain;
using NZWalk.API.Models.DTO;
using NZWalk.API.Repositories.Interfaces;

namespace NZWalk.API.Controllers
{

    // https://localhost:xxxx/api/regions
    [Route("api/[controller]")]
    [ApiController] 
    public class RegionsController : ControllerBase
    {
        private readonly NZWalkDbContext nZWalkDbContext;
        private readonly IRegionRepository regionRepository;
        private readonly IMapper mapper;
        private readonly ILogger<RegionsController> logger;

        public RegionsController(NZWalkDbContext nZWalkDbContext, 
            IRegionRepository regionRepository, 
            IMapper mapper,
            ILogger<RegionsController> logger)
        {
            this.nZWalkDbContext = nZWalkDbContext;
            this.regionRepository = regionRepository;
            this.mapper = mapper;
            this.logger = logger;
        }


        // Get all regions
        // Get https://localhost:xxxx/api/regions
        [HttpGet]
        //[Authorize(Roles = "Reader")]
        public async Task<IActionResult> GetAll()
        {
            // Get Data from database - domain model
            var regionsDomain = await regionRepository.GetAllAsync();

            // Map domain model to DTOs
            //var regionDto = new List<RegionDto>();
            //foreach (var regionDomain in regionsDomain)
            //{
            //    regionDto.Add(new RegionDto()
            //    {
            //        Id = regionDomain.Id,
            //        Name = regionDomain.Name,
            //        Code = regionDomain.Code,
            //        RegionImageUrl = regionDomain.RegionImageUrl,
            //    });
            //}

            // Map domain model to DTOs
            var regionDto = mapper.Map<List<RegionDto>>(regionsDomain);

            //return DTOs
            return Ok(regionDto);
        }

        // Get single region(get bt id)
        // Get https://localhost:xxxx/api/regions{id}
        [HttpGet]
        [Route("{id:Guid}")]
        //[Authorize(Roles = "Reader")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var regionDomain = await regionRepository.GetByIdAsync(id);
            if (regionDomain != null)
            {
                // Map/Convert region domain model to region DTO
                //var regionDTO = new RegionDto()
                //{
                //    Id = regionDomain.Id,
                //    Name = regionDomain.Name,
                //    Code = regionDomain.Code,
                //    RegionImageUrl = regionDomain.RegionImageUrl,
                //};


                // return Dto back to client 
                return Ok(mapper.Map<RegionDto>(regionDomain));
            }

            return NotFound();
        }


        // Post to create new region
        // https://localhost:xxxx/api/regions
        [HttpPost]
        [ValidateModel]
        //[Authorize(Roles = "Writer")]
        public async Task<IActionResult> Create([FromBody] AddRegionRequestDto addRegionRequestDto)
        {

            // Map or cover DTO to domain model
            //var regionDomainModel = new Region()
            //{
            //    Code = addRegionRequestDto.Code,
            //    Name = addRegionRequestDto.Name,
            //    RegionImageUrl = addRegionRequestDto.RegionImageUrl,
            //};
            var regionDomainModel = mapper.Map<Region>(addRegionRequestDto);

            // Use domain model to create region
            regionDomainModel = await regionRepository.CreateAsync(regionDomainModel);

            // Map to domain model back to DTO
            //var regionDto = new RegionDto
            //{
            //    Id = regionDomainModel.Id,
            //    Name = regionDomainModel.Name,
            //    Code = regionDomainModel.Code,
            //    RegionImageUrl = regionDomainModel.RegionImageUrl,
            //};
            var regionDto = mapper.Map<RegionDto>(regionDomainModel);

            return CreatedAtAction(nameof(GetById), new { id = regionDto.Id }, regionDto);
        }


        // Update region 
        // PUT: https://localhost:xxxx/api/regions{id}
        [HttpPut]
        [Route("{id:Guid}")]
        [ValidateModel]
        //[Authorize(Roles = "Writer")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRegionRequestDto updateRegionRequestDto)
        {
            // Map Dto to Domain model
            //var regionDomainModel = new Region
            //{
            //    Code = updateRegionRequestDto.Code,
            //    Name = updateRegionRequestDto.Name,
            //    RegionImageUrl = updateRegionRequestDto.RegionImageUrl,
            //};
            var regionDomainModel = mapper.Map<Region>(updateRegionRequestDto);

            // check if region exists
            regionDomainModel = await regionRepository.UpdateAsync(id, regionDomainModel);

            if (regionDomainModel == null)
            {
                return NotFound();
            }

            await nZWalkDbContext.SaveChangesAsync();

            // Convert domain model to Dto
            //var regionDto = new RegionDto
            //{
            //    Id = regionDomainModel.Id,
            //    Code = regionDomainModel.Code,
            //    RegionImageUrl = regionDomainModel.RegionImageUrl,
            //    Name = regionDomainModel.Name,
            //};

            return Ok(mapper.Map<RegionDto>(regionDomainModel));
        }


        // Delate Region
        [HttpDelete]
        [Route("{id:Guid}")]
        //[Authorize(Roles = "Writer,Reader")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var regionDomainModel = await regionRepository.DeleteAsync(id);
            if (regionDomainModel == null)
            {
                return NotFound();
            }

            // Return delate region back
            // Map domain model to DTO
            //var regionDto = new RegionDto
            //{
            //    Name = regionDomainModel.Name,
            //    Code = regionDomainModel.Code,
            //    RegionImageUrl = regionDomainModel.RegionImageUrl,
            //    Id = regionDomainModel.Id
            //};

            return Ok(mapper.Map<RegionDto>(regionDomainModel));
        }

    }
}

// reader

// eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJyZWFkZXJAbnp3YWxrcy5jb20iLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJSZWFkZXIiLCJleHAiOjE3MjI0ODI2NzQsImlzcyI6Imh0dHBzOi8vbG9jYWxob3N0OjcxMDQvIiwiYXVkIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NzEwNC8ifQ.KD8zIC-wRZuYCPnULfkSgJavs2yuY1F1E8RdhZDXH9A

// writer
// eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJ3cml0ZXJAbnp3YWxrcy5jb20iLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJXcml0ZXIiLCJleHAiOjE3MjI0ODI4MDEsImlzcyI6Imh0dHBzOi8vbG9jYWxob3N0OjcxMDQvIiwiYXVkIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NzEwNC8ifQ.PABfERYCAd1e1VD2qhO_yiqEFdSb-oaTEyJBx3YhRXQ

// id 
// cfa06ed2-bf65-4b65-93ed-c9d286ddb0de