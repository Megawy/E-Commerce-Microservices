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

	public class ProductsController(IProduct productInterface, IMapper mapper) : ControllerBase
	{
		[AllowAnonymous]
		[HttpGet]
		public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
		{
			//Get all products from repo
			var products = await productInterface.GetAllAsync();
			if (!products.Any())
				return NotFound("No products detected in the database");

			//convert data from entity to dto

			var ProductsDTOs = mapper.Map<IEnumerable<ProductDTO>>(products);

			return !ProductsDTOs!.Any() ? NotFound() : Ok(ProductsDTOs);
		}
		[AllowAnonymous]
		[HttpGet("{id:Guid}")]
		public async Task<ActionResult<ProductDTO>> GetProduct(Guid id)
		{
			if (id == Guid.Empty) return BadRequest("Invalid user Id");

			//Get singfle product from the Repo
			var product = await productInterface.FindByIdAsync(id);

			if (product is null) return NotFound("Product requested not found");

			//Convert From entity to DTO and return 
			var productsDTOs = mapper.Map<ProductDTO>(product);

			return productsDTOs.Id != Guid.Empty ? Ok(productsDTOs) : NotFound();
		}

		[HttpPost]
		public async Task<ActionResult<Response>> CreateProduct(ProductDTO product)
		{
			//check model state is all data annotations are passed
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			//convert to entity
			var getEntity = mapper.Map<Product>(product);
			var response = await productInterface.CreateAsync(getEntity);
			return response.Flag is true ? Ok(response) : BadRequest(response);
		}

		[HttpPut]
		public async Task<ActionResult<Response>> UpdateProduct(ProductDTO product)
		{
			// check model state is all data annotations are passed
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			//convert to entity
			var getEntity = mapper.Map<Product>(product);
			var response = await productInterface.UpdateAsync(getEntity);
			return response.Flag is true ? Ok(response) : BadRequest(response);
		}

		[HttpDelete]
		public async Task<ActionResult<Response>> DeleteProduct(ProductDTO product)
		{

			//convert to entity
			var getEntity = mapper.Map<Product>(product);
			var response = await productInterface.DeleteAsync(getEntity);
			return response.Flag is true ? Ok(response) : BadRequest(response);
		}
	}
}

