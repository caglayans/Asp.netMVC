using Microsoft.EntityFrameworkCore;
using ShopApp.DataAccess.Abstract;
using ShopApp.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopApp.DataAccess.Concrete.EFCore
{
    public class EFCoreProductRepository : EFCoreGenericRepository<Product, ShopDbContext>, IProductRepository
    {


        private ShopDbContext _db;

        public EFCoreProductRepository(ShopDbContext db) : base(db)
        {
            _db = db;
        }

        public Product GetByIdWithCategories(int id)
        {
            return _db.Products.Where(p => p.Id == id)
                        .Include(p => p.ProductCategories)
                        .ThenInclude(pc => pc.Category)
                        .FirstOrDefault();
        }

        public int GetCountByCategory(string category)
        {
            var products = _db.Products.AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                products = products.Include(p => p.ProductCategories)
                                   .ThenInclude(pc=> pc.Category)
                                   .Where(p => p.ProductCategories.Any(pc => pc.Category.Name.ToLower() == category.ToLower()));
            }

            return products.Count();
        }

        public List<Product> GetHomePageProducts()
        {
            return _db.Products
                .Where(p => p.IsApproved && p.IsHome)
                .ToList();
        }

        public Product GetProductDetails(string url)
        {
            return _db.Products.Where(p => p.Url == url)
                               .Include(p => p.ProductCategories)
                               .ThenInclude(pc => pc.Category)
                               .FirstOrDefault();

        }

        public List<Product> GetProductsByCategory(string categoryUrl, int pageSize, int page)
        {
            var products = _db.Products
                              .Where(p => p.IsApproved)
                              .AsQueryable();

            if (!string.IsNullOrEmpty(categoryUrl))
            {
                products = products.Include(p => p.ProductCategories).ThenInclude(pc => pc.Category)
                                 .Where(p => p.ProductCategories.Any(pc => pc.Category.Url.ToLower() == categoryUrl.ToLower()));
            }

            return products.Skip(pageSize * (page - 1)).Take(pageSize).ToList();
        }

        public List<Product> GetSearchResults(string searchString)
        {
            var products = _db.Products
                              .Where(p => p.IsApproved && (p.Name.ToLower().Contains(searchString.ToLower()) || p.Description.ToLower().Contains(searchString.ToLower())))
                              .AsQueryable();

            return products.ToList();
        }

        public void Update(Product product, int[] C_Ids)
        {
            var prdct = _db.Products
                                   .Include(i => i.ProductCategories)
                                   .FirstOrDefault(i => i.Id == product.Id);

            if (prdct != null)
            {
                prdct.Name = product.Name;
                prdct.Description = product.Description;
                prdct.ImageUrl = product.ImageUrl;
                prdct.Price = product.Price;
                prdct.Url = product.Url;
                prdct.ProductCategories = C_Ids.Select(Cid => new ProductCategory()
                {
                    CategoryId = Cid,
                    ProductId = product.Id
                }).ToList();

                _db.SaveChanges();
            }
        }
    }
}