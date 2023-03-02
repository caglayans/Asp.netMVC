using ShopApp.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopApp.DataAccess.Abstract
{
    public interface IProductRepository : IRepository<Product>
    {
        Product GetProductDetails(string url);
        int GetCountByCategory(string category);
        List<Product> GetProductsByCategory(string name, int pageSize, int page);
        List<Product> GetSearchResults(string searchString);
        List<Product> GetHomePageProducts();
        Product GetByIdWithCategories(int id);
        void Update(Product product, int[] c_Ids);
    }
}
