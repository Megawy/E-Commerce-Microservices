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
	[Authorize]
	public class ProductReviewsController(IMapper mapper, IProductReview productReviewInterface) : ControllerBase
	{
		[HttpGet]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<IEnumerable<ProductReviewDTO>>> GetProductReviews()
		{
			var categories = await productReviewInterface.GetAllAsync();
			if (!categories.Any())
				return NotFound("No products detected in the database");

			//convert data from entity to dto

			var categoriesDTOs = mapper.Map<IEnumerable<ProductReviewDTO>>(categories);

			return !categoriesDTOs!.Any() ? NotFound() : Ok(categoriesDTOs);
		}

		[HttpGet("{id:Guid}")]
		public async Task<ActionResult<ProductReviewDTO>> GetProductReview(Guid id)
		{
			if (id == Guid.Empty) return BadRequest("Invalid user Id");

			//Get singfle category from the Repo
			var category = await productReviewInterface.FindByIdAsync(id);

			if (category is null) return NotFound("Category requested not found");

			var categorysDTOs = mapper.Map<ProductReviewDTO>(category);

			return categorysDTOs.Id != Guid.Empty ? Ok(categorysDTOs) : NotFound();
		}

		[HttpPost]
		public async Task<ActionResult<Response>> CreateProductReview(ProductReviewDTO entity)
		{
			//check model state is all data annotations are passed
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			//convert to entity
			var getEntity = mapper.Map<ProductReview>(entity);
			var response = await productReviewInterface.CreateAsync(getEntity);
			return response.Flag is true ? Ok(response) : BadRequest(response);
		}

		[HttpPut]
		public async Task<ActionResult<Response>> UpdateProductReview(ProductReviewDTO entity)
		{
			// check model state is all data annotations are passed
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			//convert to entity
			var getEntity = mapper.Map<ProductReview>(entity);
			var response = await productReviewInterface.UpdateAsync(getEntity);
			return response.Flag is true ? Ok(response) : BadRequest(response);
		}

		[HttpDelete]
		public async Task<ActionResult<Response>> DeleteProductReview(ProductReviewDTO entity)
		{
			//convert to entity
			var getEntity = mapper.Map<ProductReview>(entity);
			var response = await productReviewInterface.DeleteAsync(getEntity);
			return response.Flag is true ? Ok(response) : BadRequest(response);
		}
	}
}
