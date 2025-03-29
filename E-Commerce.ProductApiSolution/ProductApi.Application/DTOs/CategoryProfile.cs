using AutoMapper;
using ProductApi.Domain.Entities;

namespace ProductApi.Application.DTOs
{
	public class CategoryProfile : Profile
	{
		public CategoryProfile()
		{
			CreateMap<Category, CategoryDTO>().ReverseMap();
		}
	}
}
