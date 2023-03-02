using System;
using System.Diagnostics;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ShopApp.Entity;

namespace ShopApp.DataAccess.Concrete.EFCore
{
    public class SeedDatabase
    {
        public static void Seed(IServiceProvider serviceProvider)
        {
            var _db = new ShopDbContext(serviceProvider.GetRequiredService<DbContextOptions<ShopDbContext>>());

                if (!_db.Database.GetPendingMigrations().Any())
                {
                    if (!_db.Categories.Any())
                    {
                        _db.Categories.AddRange(Categories);
                    }

                    if (!_db.Products.Any())
                    {
                        _db.Products.AddRange(Products);
                        _db.AddRange(ProductCategory);

                }
                }
            _db.SaveChanges();
        }
        private static List<Product> Products = new List<Product>
        {
            new Product() { Name = "Samsung s6",Url="telefon-samsung-s6", Price = 2000, Description = "iyi telefon", ImageUrl = "1.jpg", IsApproved = true, IsHome=true},
            new Product() { Name = "Samsung s7",Url="telefon-samsung-s7",Price = 3000, Description = "iyi telefon", ImageUrl = "2.jpg", IsApproved = false, IsHome=true },
            new Product() { Name = "Samsung s8",Url="telefon-samsung-s8",Price = 4000, Description = "iyi telefon", ImageUrl = "3.jpg", IsApproved = true },
            new Product() { Name = "Samsung s9",Url="telefon-samsung-s9",Price = 5000, Description = "iyi telefon", ImageUrl = "4.jpg", IsApproved = false,},
            new Product() { Name = "Samsung s10",Url="telefon-samsung-s10",Price = 6000, Description = "iyi telefon", ImageUrl = "5.jpg", IsApproved = true, IsHome=true},
            new Product() { Name = "Samsung s11",Url="telefon-samsung-s11",Price = 7000, Description = "iyi telefon", ImageUrl = "6.jpg", IsApproved = true, IsHome=true}
        };

        private static List<Category> Categories = new List<Category>
        {
            new Category(){Name="Telefon", Url="telefon" },
            new Category(){Name="Bilgisayar", Url="bilgisayar",},
            new Category(){Name="Elektronik", Url="elektronik"},
            new Category(){Name="Beyaz Eşya", Url="beyaz-esya"}
        };


        private static ProductCategory[] ProductCategory =
        {
            new ProductCategory() { Product= Products[0],Category= Categories[0]},
            new ProductCategory() { Product= Products[0],Category= Categories[2]},
            new ProductCategory() { Product= Products[1],Category= Categories[0]},
            new ProductCategory() { Product= Products[1],Category= Categories[1]},
            new ProductCategory() { Product= Products[2],Category= Categories[0]},
            new ProductCategory() { Product= Products[2],Category= Categories[2]},
            new ProductCategory() { Product= Products[3],Category= Categories[1]}
        };


    };
}




