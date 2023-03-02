using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShopApp.Business.Abstract;
using ShopApp.Entity;

namespace ShopApp.WebUI.ViewComponents
{
    public class CategoriesViewComponent : ViewComponent
    {

        private ICategoryService _categoryService;

        public CategoriesViewComponent(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (RouteData.Values["category"] != "null")
            {
                ViewBag.SelectedCategory = RouteData?.Values["category"];
            }
         
            ViewBag.SelectedAction = RouteData.Values["action"].ToString();

            return View(_categoryService.GetAll());

        }

    }
}


