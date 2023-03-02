using System;
using ShopApp.Entity;

namespace ShopApp.Business.Abstract
{
	public interface IProductService 
	{
        Product GetProductDetails(string url);
        Product GetById(int id);
        Product GetByIdWithCategories(int id);
        List<Product> GetProductsByCategory(string name, int pageSize, int page);
        int GetCountByCategory(string category);
        List<Product> GetAll();
        List<Product> GetHomePageProducts();
        List<Product> GetSearchResults(string searchString);
        void Create(Product entity);
        void Update(Product entity);
        void Delete(Product entity);
        Dictionary<bool, string> Update(Product product, int[] c_Ids);
    }
}

