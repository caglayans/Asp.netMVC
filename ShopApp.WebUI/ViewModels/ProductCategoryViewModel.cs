using ShopApp.Entity;

namespace ShopApp.WebUI.ViewModels
{
    public class ProductCategoryViewModel
    {
        public int CategoryId { get; set; }
        public CategoryViewModel Category { get; set; }

        public int ProductId { get; set; }
        public ProductViewModel Product { get; set; }
    }
}