using System;
using System.ComponentModel.DataAnnotations;
using ShopApp.Entity;

namespace ShopApp.WebUI.ViewModels
{
	public class CategoryViewModel
	{
        public int Id { get; set; }

        [Required(ErrorMessage ="İsim alanı boş bırakılamaz.")]
        [StringLength(60,MinimumLength =5,ErrorMessage ="Kategori ismi 5-60 karakter arasında olmalıdır.")]
        public string Name { get; set; }

        [Required(ErrorMessage ="Url alanı boş bırakılamaz.")]
        [StringLength(60, MinimumLength = 5, ErrorMessage = "Url 5-60 karakter arasında olmalıdır.")]
        public string Url { get; set; }

        public List<Product>? Products { get; set; }
    }
}

