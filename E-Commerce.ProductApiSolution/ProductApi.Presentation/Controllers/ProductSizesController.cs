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
	public class ProductSizesController(IMapper mapper, IProductSize productSizeInterface) : ControllerBase
	{
		[HttpGet]
		public async Task<ActionResult<IEnumerable<ProductSizeDTO>>> GetproductSizes()
		{
			var productSize = await productSizeInterface.GetAllAsync();
			if (!productSize.Any())
				return NotFound("No products detected in the database");

			//convert data from entity to dto

			var productSizesDTOs = mapper.Map<IEnumerable<ProductSizeDTO>>(productSize);

			return !productSizesDTOs!.Any() ? NotFound() : Ok(productSizesDTOs);
		}

		[HttpGet("{id:Guid}")]
		public async Task<ActionResult<ProductSizeDTO>> GetproductSize(Guid id)
		{
			if (id == Guid.Empty) return BadRequest("Invalid user Id");

			//Get singfle category from the Repo
			var productSize = await productSizeInterface.FindByIdAsync(id);

			if (productSize is null) return NotFound("product requested not found");

			var productSizeDTOs = mapper.Map<ProductSizeDTO>(productSize);

			return productSizeDTOs.Id != Guid.Empty ? Ok(productSizeDTOs) : NotFound();
		}

		[HttpPost]
		public async Task<ActionResult<Response>> CreateproductSize(ProductSizeDTO entity)
		{
			//check model state is all data annotations are passed
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			//convert to entity
			var getEntity = mapper.Map<ProductSize>(entity);
			var response = await productSizeInterface.CreateAsync(getEntity);
			return response.Flag is true ? Ok(response) : BadRequest(response);
		}

		[HttpPut]
		public async Task<ActionResult<Response>> UpdateproductSize(ProductSizeDTO entity)
		{
			// check model state is all data annotations are passed
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			//convert to entity
			var getEntity = mapper.Map<ProductSize>(entity);
			var response = await productSizeInterface.UpdateAsync(getEntity);
			return response.Flag is true ? Ok(response) : BadRequest(response);
		}

		[HttpDelete]
		public async Task<ActionResult<Response>> DeleteproductSize(ProductSizeDTO entity)
		{
			//convert to entity
			var getEntity = mapper.Map<ProductSize>(entity);
			var response = await productSizeInterface.DeleteAsync(getEntity);
			return response.Flag is true ? Ok(response) : BadRequest(response);
		}
	}
}
