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
	public class ProductImagesController(IMapper mapper, IProductImage productImageInterface) : ControllerBase
	{
		[HttpGet]
		public async Task<ActionResult<IEnumerable<ProductImageDTO>>> GetProductImages()
		{
			var categories = await productImageInterface.GetAllAsync();
			if (!categories.Any())
				return NotFound("No products detected in the database");

			//convert data from entity to dto

			var categoriesDTOs = mapper.Map<IEnumerable<ProductImageDTO>>(categories);

			return !categoriesDTOs!.Any() ? NotFound() : Ok(categoriesDTOs);
		}

		[HttpGet("{id:Guid}")]
		public async Task<ActionResult<ProductImageDTO>> GetProductImage(Guid id)
		{
			if (id == Guid.Empty) return BadRequest("Invalid user Id");

			//Get singfle category from the Repo
			var category = await productImageInterface.FindByIdAsync(id);

			if (category is null) return NotFound("Category requested not found");

			var categorysDTOs = mapper.Map<ProductImageDTO>(category);

			return categorysDTOs.Id != Guid.Empty ? Ok(categorysDTOs) : NotFound();
		}

		[HttpPost]
		public async Task<ActionResult<Response>> CreateProductImage(ProductImageDTO entity)
		{
			//check model state is all data annotations are passed
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			//convert to entity
			var getEntity = mapper.Map<ProductImage>(entity);
			var response = await productImageInterface.CreateAsync(getEntity);
			return response.Flag is true ? Ok(response) : BadRequest(response);
		}

		[HttpPut]
		public async Task<ActionResult<Response>> UpdateProductImage(ProductImageDTO entity)
		{
			// check model state is all data annotations are passed
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			//convert to entity
			var getEntity = mapper.Map<ProductImage>(entity);
			var response = await productImageInterface.UpdateAsync(getEntity);
			return response.Flag is true ? Ok(response) : BadRequest(response);
		}

		[HttpDelete]
		public async Task<ActionResult<Response>> DeleteProductImage(ProductImageDTO entity)
		{
			//convert to entity
			var getEntity = mapper.Map<ProductImage>(entity);
			var response = await productImageInterface.DeleteAsync(getEntity);
			return response.Flag is true ? Ok(response) : BadRequest(response);
		}
	}
}
