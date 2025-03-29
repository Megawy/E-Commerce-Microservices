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
	public class CategoriesController(IMapper mapper, ICategory categoryInterface) : ControllerBase
	{
		[AllowAnonymous]
		[HttpGet]
		public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategories()
		{
			var categories = await categoryInterface.GetAllAsync();
			if (!categories.Any())
				return NotFound("No products detected in the database");

			//convert data from entity to dto

			var categoriesDTOs = mapper.Map<IEnumerable<CategoryDTO>>(categories);

			return !categoriesDTOs!.Any() ? NotFound() : Ok(categoriesDTOs);
		}

		[AllowAnonymous]
		[HttpGet("{id:Guid}")]
		public async Task<ActionResult<CategoryDTO>> GetCategory(Guid id)
		{
			if (id == Guid.Empty) return BadRequest("Invalid user Id");

			//Get singfle category from the Repo
			var category = await categoryInterface.FindByIdAsync(id);

			if (category is null) return NotFound("Category requested not found");

			var categorysDTOs = mapper.Map<CategoryDTO>(category);

			return categorysDTOs.Id != Guid.Empty ? Ok(categorysDTOs) : NotFound();
		}

		[HttpPost]
		public async Task<ActionResult<Response>> CreateCategory(CategoryDTO category)
		{
			//check model state is all data annotations are passed
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			//convert to entity
			var getEntity = mapper.Map<Category>(category);
			var response = await categoryInterface.CreateAsync(getEntity);
			return response.Flag is true ? Ok(response) : BadRequest(response);
		}

		[HttpPut]
		public async Task<ActionResult<Response>> UpdateCategory(CategoryDTO category)
		{
			// check model state is all data annotations are passed
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			//convert to entity
			var getEntity = mapper.Map<Category>(category);
			var response = await categoryInterface.UpdateAsync(getEntity);
			return response.Flag is true ? Ok(response) : BadRequest(response);
		}

		[HttpDelete]
		public async Task<ActionResult<Response>> DeleteCategory(CategoryDTO category)
		{
			//convert to entity
			var getEntity = mapper.Map<Category>(category);
			var response = await categoryInterface.DeleteAsync(getEntity);
			return response.Flag is true ? Ok(response) : BadRequest(response);
		}
	}
}
