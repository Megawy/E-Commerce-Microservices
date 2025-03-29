using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Domain.Entites;
using AutoMapper;
using E_Commerce.SharedLibray.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationApi.Presentation.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class AddressController(IAddress addressInterface, IMapper mapper) : ControllerBase
	{
		[HttpGet("{id:Guid}")]
		public async Task<ActionResult<UserAddressDTO>> FindAddressById(Guid id)
		{
			if (id == Guid.Empty) return BadRequest("Invalid address Id");

			var address = await addressInterface.FindByIdAsync(id);

			if (address is null) return NotFound($"Address id  {id} not Found");

			var addressDTOs = mapper.Map<UserAddressDTO>(address);

			return addressDTOs.Id != Guid.Empty ? Ok(addressDTOs) : NotFound($"Address id  {id} not Found");
		}

		[Authorize(Roles = "Admin")]
		[HttpGet("get-all")]
		public async Task<ActionResult<IEnumerable<UserAddressDTO>>> GetAllAddress()
		{
			var addresses = await addressInterface.GetAllAsync();
			if (!addresses.Any())
				return NotFound("No Addresses detected in the database");

			var addressDTOs = mapper.Map<IEnumerable<UserAddressDTO>>(addresses);

			return !addressDTOs!.Any() ? NotFound() : Ok(addressDTOs);
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<UserAddressDTO>>> GetAddressByUserId()
		{
			var getAddresses = await addressInterface.GetAddressByUserId();

			if (!getAddresses.Any())
				return NotFound("No Addresses detected in the database");

			var addressDTOs = mapper.Map<IEnumerable<UserAddressDTO>>(getAddresses);

			return !addressDTOs!.Any() ? NotFound() : Ok(addressDTOs);
		}

		[HttpPost]
		public async Task<ActionResult<Response>> CreateAddress(UserAddressDTO entity)
		{   //check model state is all data annotations are passed
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			//convert to entity
			var getentity = mapper.Map<UserAddress>(entity);
			var response = await addressInterface.CreateAsync(getentity);
			return response.Flag is true ? Ok(response) : BadRequest(response);
		}

		[HttpPut]
		public async Task<ActionResult<Response>> UpdateAddress(UserAddressDTO entity)
		{
			// check model state is all data annotations are passed
			if (!ModelState.IsValid) return BadRequest(ModelState);

			//convert to entity
			var getEntity = mapper.Map<UserAddress>(entity);
			var response = await addressInterface.UpdateAsync(getEntity);
			return response.Flag is true ? Ok(response) : BadRequest(response);
		}

		[HttpDelete]
		public async Task<ActionResult<Response>> DeleteAddress(UserAddressDTO entity)
		{
			//convert to entity
			var getEntity = mapper.Map<UserAddress>(entity);
			var response = await addressInterface.DeleteAsync(getEntity);
			return response.Flag is true ? Ok(response) : BadRequest(response);
		}
	}
}
