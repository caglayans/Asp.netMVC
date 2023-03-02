using System;
using ShopApp.Business.Abstract;
using ShopApp.DataAccess.Abstract;
using ShopApp.Entity;

namespace ShopApp.Business.Concrete
{
	public class ProductManager :IProductService
	{

        private IProductRepository _productRepository;

        public ProductManager(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public void Create(Product entity)
        {
            _productRepository.Create(entity);
        }

        public void Delete(Product entity)
        {
            _productRepository.Delete(entity);
        }

        public List<Product> GetAll()
        {
            return _productRepository.GetAll();
        }

        public Product GetById(int id)
        {
            return _productRepository.GetById(id);
        }

        public Product GetByIdWithCategories(int id)
        {
            return _productRepository.GetByIdWithCategories(id);
        }

        public int GetCountByCategory(string category)
        {
            return _productRepository.GetCountByCategory(category);
        }

        public List<Product> GetHomePageProducts()
        {
            return _productRepository.GetHomePageProducts();
        }

        public Product GetProductDetails(string url)
        {
            return _productRepository.GetProductDetails(url);
        }

        public List<Product> GetProductsByCategory(string name, int pageSize, int page)
        {
            return _productRepository.GetProductsByCategory(name,page, pageSize);
        }

        public List<Product> GetSearchResults(string searchString)
        {
            return _productRepository.GetSearchResults(searchString);
        }

        public void Update(Product entity)
        {
            _productRepository.Update(entity);
        }

        public Dictionary<bool, string> Update(Product product, int[] c_Ids)
        {
            Dictionary<bool, string> valdtn = new Dictionary<bool, string>() { };
            if (c_Ids.Length == 0)
            {
                valdtn.Add(false, "Ürün için en az bir kategori seçmelisiniz.");
                return valdtn;
            }

            _productRepository.Update(product, c_Ids);
            valdtn.Add(true, null);
            return valdtn;
        }
        
    }
}

