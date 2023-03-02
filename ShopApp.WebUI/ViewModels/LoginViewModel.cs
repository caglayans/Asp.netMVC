using System;
using System.ComponentModel.DataAnnotations;

namespace ShopApp.WebUI.ViewModels
{
	public class LoginViewModel
	{
		[Required]
		[DataType(DataType.EmailAddress)]
		// public string UserName { get; set; }
		public string Email { get; set; }

		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		public string? ReturnUrl { get; set; }
	}
}

