using System;
using ShopApp.Entity;

namespace ShopApp.WebUI.ViewModels
{
	public class ProductDetailsViewModel
	{
			public Product Product { get; set; }

			public List<Category> Categories { get; set; }

	}
}

