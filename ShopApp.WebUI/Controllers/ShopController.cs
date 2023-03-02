using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using ShopApp.Business.Abstract;
using ShopApp.Entity;
using ShopApp.WebUI.ViewModels;

namespace ShopApp.WebUI.Controllers
{
    public class ShopController : Controller
    {

        private IProductService _productService;

        public ShopController(IProductService productService)
        {
            _productService = productService;
        }


        [Route("/urunler/{category?}")]
        public IActionResult List(string category, int page = 1)
        {
            const int pageSize = 3;

            var productViewModel = new ListViewModel()
            {
                PageInfo = new PageInfo()
                {
                    TotalItems = _productService.GetCountByCategory(category),
                    ItemsPerPage = pageSize,
                    CurrentPage = page,
                    CurrentCategory = category
                },
                Products = _productService.GetProductsByCategory(category, page, pageSize)
            };

            return View(productViewModel);
        }


        [Route("/urun-incele/{url}")]
        public IActionResult Details(string url)
        {
            if (url == null)
            {
                return NotFound();
            }

            Product product = _productService.GetProductDetails(url);

            if (product == null)
            {
                return NotFound();
            }

            return View(new ProductDetailsViewModel()
            {
                Product = product,
                Categories = product.ProductCategories.Select(pc => pc.Category).ToList()
            }); ;
        }

        [Route("search")]
        public IActionResult Search(string q)
        {
            var ProductViewModel = new ListViewModel()
            {
                Products = _productService.GetSearchResults(q)
            };

            return View(ProductViewModel);
        }
    }
}



