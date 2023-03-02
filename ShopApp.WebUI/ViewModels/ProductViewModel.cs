using System;
using System.ComponentModel.DataAnnotations;
using ShopApp.Entity;

namespace ShopApp.WebUI.ViewModels
{
	public class ProductViewModel
	{
        public int Id { get; set; }

        [Display(Name="İsim", Prompt ="Ürün İsmini giriniz.")]
        [Required(ErrorMessage ="İsim alanı zorunludur.")]
        [StringLength(30,MinimumLength =5,ErrorMessage ="Ürün ismi 5-30 karakter aralığında olmalıdır")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Url alanı zorunludur.")]
        public string Url { get; set; }

        [Required(ErrorMessage = "Fiyat alanı zorunludur.")]
        [Range(1,10000, ErrorMessage ="Fiyat için 1-10000 arasında bir değer girmelisiniz.")]
        public double? Price { get; set; }

        [Required(ErrorMessage = "Açıklama alanı zorunludur.")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Açıklama 5-100 karakter aralığında olmalıdır")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Resim Url alanı zorunludur.")]
        public string ImageUrl { get; set; }

        public bool IsApproved { get; set; }
        public bool IsHome { get; set; }

        public List<Category> SelectedCategories { get; set; }

    }
}

