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
	public class ProductDiscountsController(IMapper mapper, IProductDiscount productDiscountInterface) : ControllerBase
	{
		[HttpGet]
		public async Task<ActionResult<IEnumerable<ProductDiscountDTO>>> GetProductDiscounts()
		{
			var categories = await productDiscountInterface.GetAllAsync();
			if (!categories.Any())
				return NotFound("No products detected in the database");

			//convert data from entity to dto

			var categoriesDTOs = mapper.Map<IEnumerable<ProductDiscountDTO>>(categories);

			return !categoriesDTOs!.Any() ? NotFound() : Ok(categoriesDTOs);
		}

		[HttpGet("{id:Guid}")]
		public async Task<ActionResult<ProductDiscountDTO>> GetProductDiscount(Guid id)
		{
			if (id == Guid.Empty) return BadRequest("Invalid user Id");

			//Get singfle category from the Repo
			var category = await productDiscountInterface.FindByIdAsync(id);

			if (category is null) return NotFound("Category requested not found");

			var categorysDTOs = mapper.Map<ProductDiscountDTO>(category);

			return categorysDTOs.Id != Guid.Empty ? Ok(categorysDTOs) : NotFound();
		}

		[HttpPost]
		public async Task<ActionResult<Response>> CreateProductDiscount(ProductDiscountDTO entity)
		{
			//check model state is all data annotations are passed
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			//convert to entity
			var getEntity = mapper.Map<ProductDiscount>(entity);
			var response = await productDiscountInterface.CreateAsync(getEntity);
			return response.Flag is true ? Ok(response) : BadRequest(response);
		}

		[HttpPut]
		public async Task<ActionResult<Response>> UpdateProductDiscount(ProductDiscountDTO entity)
		{
			// check model state is all data annotations are passed
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			//convert to entity
			var getEntity = mapper.Map<ProductDiscount>(entity);
			var response = await productDiscountInterface.UpdateAsync(getEntity);
			return response.Flag is true ? Ok(response) : BadRequest(response);
		}

		[HttpDelete]
		public async Task<ActionResult<Response>> DeleteProductDiscount(ProductDiscountDTO entity)
		{
			//convert to entity
			var getEntity = mapper.Map<ProductDiscount>(entity);
			var response = await productDiscountInterface.DeleteAsync(getEntity);
			return response.Flag is true ? Ok(response) : BadRequest(response);
		}
	}
}
