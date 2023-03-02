using ShopApp.Entity;

namespace ShopApp.WebUI.ViewModels
{
    public class ListViewModel
    {
        public PageInfo PageInfo { get; set; }
        public List<Product> Products { get; set; }
        public List<Category> Categories { get; set; }
    }
}
