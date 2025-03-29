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
	public class ProductColorsController(IMapper mapper, IProductColor productColorInterface) : ControllerBase
	{
		[HttpGet]
		public async Task<ActionResult<IEnumerable<ProductColorDTO>>> GetProductColors()
		{
			var categories = await productColorInterface.GetAllAsync();
			if (!categories.Any())
				return NotFound("No products detected in the database");

			//convert data from entity to dto

			var categoriesDTOs = mapper.Map<IEnumerable<ProductColorDTO>>(categories);

			return !categoriesDTOs!.Any() ? NotFound() : Ok(categoriesDTOs);
		}

		[HttpGet("{id:Guid}")]
		public async Task<ActionResult<ProductColorDTO>> GetProductColor(Guid id)
		{
			if (id == Guid.Empty) return BadRequest("Invalid user Id");

			//Get singfle category from the Repo
			var category = await productColorInterface.FindByIdAsync(id);

			if (category is null) return NotFound("Category requested not found");

			var categorysDTOs = mapper.Map<ProductColorDTO>(category);

			return categorysDTOs.Id != Guid.Empty ? Ok(categorysDTOs) : NotFound();
		}

		[HttpPost]
		public async Task<ActionResult<Response>> CreateProductColor(ProductColorDTO entity)
		{
			//check model state is all data annotations are passed
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			//convert to entity
			var getEntity = mapper.Map<ProductColor>(entity);
			var response = await productColorInterface.CreateAsync(getEntity);
			return response.Flag is true ? Ok(response) : BadRequest(response);
		}

		[HttpPut]
		public async Task<ActionResult<Response>> UpdateProductColor(ProductColorDTO entity)
		{
			// check model state is all data annotations are passed
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			//convert to entity
			var getEntity = mapper.Map<ProductColor>(entity);
			var response = await productColorInterface.UpdateAsync(getEntity);
			return response.Flag is true ? Ok(response) : BadRequest(response);
		}

		[HttpDelete]
		public async Task<ActionResult<Response>> DeleteProductColor(ProductColorDTO entity)
		{
			//convert to entity
			var getEntity = mapper.Map<ProductColor>(entity);
			var response = await productColorInterface.DeleteAsync(getEntity);
			return response.Flag is true ? Ok(response) : BadRequest(response);
		}
	}
}
