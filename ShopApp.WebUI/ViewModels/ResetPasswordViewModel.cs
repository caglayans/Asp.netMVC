using System;
using System.ComponentModel.DataAnnotations;

namespace ShopApp.WebUI.ViewModels
{
	public class ResetPasswordViewModel
	{
		[Required]
		public string Token { get; set; }

		[Required]
		[DataType(DataType.EmailAddress)]
		public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
	}
}

