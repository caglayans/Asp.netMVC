using System;
using AutoMapper;
using ShopApp.Entity;
using ShopApp.WebUI.Identity;
using ShopApp.WebUI.ViewModels;

namespace ShopApp.WebUI.Mapping
{
	public class ViewModelMapping :Profile
	{
		public ViewModelMapping()
		{
			CreateMap<Product, ProductViewModel>().ReverseMap();
			CreateMap<Category, CategoryViewModel>().ReverseMap();
			CreateMap<User, RegisterViewModel>().ReverseMap();
		}
	}
}

