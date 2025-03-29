using AutoMapper;
using E_Commerce.SharedLibray.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;

namespace ProductApi.Presentation.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(Roles = "Admin")]
	public class ProductTagsController(IMapper mapper, IProductTag productTagInterface) : ControllerBase
	{
		[HttpGet]
		public async Task<ActionResult<IEnumerable<ProductTagDTO>>> GetproductTags()
		{
			var product = await productTagInterface.GetAllAsync();
			if (!product.Any())
				return NotFound("No products detected in the database");

			//convert data from entity to dto

			var productDTOs = mapper.Map<IEnumerable<ProductTagDTO>>(product);

			return !productDTOs!.Any() ? NotFound() : Ok(productDTOs);
		}

		[HttpGet("{id:Guid}")]
		public async Task<ActionResult<ProductTagDTO>> GetproductTag(Guid id)
		{
			if (id == Guid.Empty) return BadRequest("Invalid user Id");

			//Get singfle category from the Repo
			var product = await productTagInterface.FindByIdAsync(id);

			if (product is null) return NotFound("product requested not found");

			var productDTOs = mapper.Map<ProductTagDTO>(product);

			return productDTOs.Id != Guid.Empty ? Ok(productDTOs) : NotFound();
		}

		[HttpPost]
		public async Task<ActionResult<Response>> CreateproductTag(ProductTagDTO entity)
		{
			//check model state is all data annotations are passed
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			//convert to entity
			var getEntity = mapper.Map<ProductTag>(entity);
			var response = await productTagInterface.CreateAsync(getEntity);
			return response.Flag is true ? Ok(response) : BadRequest(response);
		}

		[HttpPut]
		public async Task<ActionResult<Response>> UpdateproductTag(ProductTagDTO entity)
		{
			// check model state is all data annotations are passed
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			//convert to entity
			var getEntity = mapper.Map<ProductTag>(entity);
			var response = await productTagInterface.UpdateAsync(getEntity);
			return response.Flag is true ? Ok(response) : BadRequest(response);
		}

		[HttpDelete]
		public async Task<ActionResult<Response>> DeleteproductTag(ProductTagDTO entity)
		{
			//convert to entity
			var getEntity = mapper.Map<ProductTag>(entity);
			var response = await productTagInterface.DeleteAsync(getEntity);
			return response.Flag is true ? Ok(response) : BadRequest(response);
		}
	}
}
