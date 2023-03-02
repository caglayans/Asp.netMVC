using System;
using System.Linq;
using System.Net.NetworkInformation;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using NuGet.Packaging.Signing;
using ShopApp.Business.Abstract;
using ShopApp.DataAccess.Concrete.EFCore;
using ShopApp.Entity;
using ShopApp.WebUI.Identity;
using ShopApp.WebUI.Models;
using ShopApp.WebUI.ViewModels;

namespace ShopApp.WebUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
	{
		private readonly IProductService _productService;
		private readonly ICategoryService _categoryService;
		private readonly IMapper _mapper;
		private readonly IFileProvider _fileProvider;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;

        public AdminController(IProductService productService, IMapper mapper, ICategoryService categoryService, IFileProvider fileProvider, RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            _productService = productService;
            _mapper = mapper;
            _categoryService = categoryService;
            _fileProvider = fileProvider;
            _roleManager = roleManager;
            _userManager = userManager;
        }



        public async Task<IActionResult> ManageUserRoles(string userId)
        {
            ViewBag.userId = userId;

            var user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                var model = new List<UserRolesViewModel>();

                foreach (var role in _roleManager.Roles.ToList())
                {
                    var userRolesViewModel = new UserRolesViewModel
                    {
                        RoleId = role.Id,
                        RoleName = role.Name
                    };

                    if (await _userManager.IsInRoleAsync(user, role.Name))
                    {
                        userRolesViewModel.IsSelected = true;
                    }
                    else
                    {
                        userRolesViewModel.IsSelected = false;
                    }

                    model.Add(userRolesViewModel);
                }

                return View(model);
            }

            return NotFound();
        }


        [HttpPost]
        public async Task<IActionResult> ManageUserRoles(List<UserRolesViewModel> model, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var result = await _userManager.RemoveFromRolesAsync(user, roles);

                if (result.Succeeded)
                {
                    result = await _userManager.AddToRolesAsync(user,
                    model.Where(x => x.IsSelected).Select(y => y.RoleName));

                    if (result.Succeeded)
                    {
                        return RedirectToAction("EditUser", new { Id = userId });                       
                    }

                    ModelState.AddModelError("", "Seçilen roller kullanıcıya eklenemez.");
                    return View(model);
                }
                ModelState.AddModelError("", "Kullanıcının mevcut rolleri kaldırılamaz.");
                return View(model);
            }
            return NotFound();
        }




        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var SelectedRoles = await _userManager.GetRolesAsync(user);
                var roles = _roleManager.Roles.Select(x => x.Name).ToList();
                
                return View(new UserDetailsViewModel()
                {
                    UserId=user.Id,
                    UserName=user.UserName,
                    FirstName=user.FirstName,
                    LastName=user.LastName,
                    Email=user.Email, 
                    EmailConfirmed=user.EmailConfirmed,
                    SelectedRoles=SelectedRoles.ToList()
                });
            }
            return RedirectToAction("UserList");
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(UserDetailsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user != null)
                {
                    user.UserName = model.UserName;
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Email = model.Email;
                    user.EmailConfirmed = model.EmailConfirmed;

                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("UserList");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
                return NotFound();       
            }
            
            return View(model);
        }



        public IActionResult UserList()
        {
            return View(_userManager.Users);
        }


        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.roleId = roleId;

            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
                return NotFound();

            var model = new List<UserRoleViewModel>();

            foreach (var user in _userManager.Users.ToList())
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };

                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }

                model.Add(userRoleViewModel);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
                return NotFound();

            for (int i = 0; i < model.Count; i++)
            {
                var user = await _userManager.FindByIdAsync(model[i].UserId);

                IdentityResult result = null;

                if (model[i].IsSelected && !(await _userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await _userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && await _userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }

                if (result.Succeeded)
                {
                    if (i < (model.Count - 1))
                        continue;
                    else
                        return RedirectToAction("EditRole", new { Id = roleId });
                }
            }

            return RedirectToAction("EditRole", new { Id = roleId });
        }


        
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                return NotFound();
            }

            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name
            };

            foreach (var user in _userManager.Users.ToList())
            {
                
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.Id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {model.Id} cannot be found";
                return NotFound();
            }
            else
            {
                role.Name = model.RoleName;

                var result = await _roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("RoleList");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
        }



        public IActionResult RoleList()
        {
            return View(_roleManager.Roles);
        }



        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(model.Name));
                if (result.Succeeded)
                {
                    return RedirectToAction("RoleList");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(model);
        }



        [Route("/yonetici/urunler")]
        public IActionResult ProductList()
		{

			return View(new ListViewModel()
			{
				Products=_productService.GetAll()
            });
		}

		[Route("/yonetici/kategoriler")]
		public IActionResult CategoryList()
		{
            return View(new ListViewModel()
            {
                Categories = _categoryService.GetAll()
            });
        }



		public IActionResult CreateProduct()
		{
            var CategoryList = new List<Category>();

            foreach (var category in _categoryService.GetAll())
            {
                CategoryList.Add(new Category() { Id = category.Id, Name = category.Name, Url = category.Url, ProductCategories=category.ProductCategories});
            };

			ViewBag.CategoriesSelectList = CategoryList;
            return View();
		}

		[HttpPost]
		public IActionResult CreateProduct(ProductViewModel model)
		{
			if (ModelState.IsValid)
			{
                _productService.Create(_mapper.Map<Product>(model));
                var status = new AlertStatus()
                {
                    Message = "Ürün başarıyla oluşturuldu.",
                    AlertType = "success"
                };
                TempData["status"] = JsonConvert.SerializeObject(status);
                return RedirectToAction("ProductList");
            }
			return View(model);
   
		}



		public IActionResult CreateCategory()
		{
			return View();
		}

		[HttpPost]
		public IActionResult CreateCategory(CategoryViewModel model)
		{
			if (ModelState.IsValid)
			{
                _categoryService.Create(_mapper.Map<Category>(model));
                var status = new AlertStatus()
                {
                    Message = "Kategori başarıyla oluşturuldu",
                    AlertType = "success"
                };

                TempData["status"] = JsonConvert.SerializeObject(status);
                return RedirectToAction("CategoryList");
            }
			return View(model);
		}



		public IActionResult EditProduct(int? id)
		{
			if (id == null)
			{ return NotFound(); }

			var entity = _productService.GetByIdWithCategories((int)id);

			if (entity == null)
			{ return NotFound(); }

			ViewBag.Categories = _categoryService.GetAll();
			var model = _mapper.Map<ProductViewModel>(entity);
			model.SelectedCategories = entity.ProductCategories.Select(pc => pc.Category).ToList();
            return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> EditProduct(ProductViewModel model, int[] C_Ids, IFormFile? file)
		{
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    var root = _fileProvider.GetDirectoryContents("wwwroot");

                    var images = root.First(x => x.Name == "images");

                    var randomImageName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    model.ImageUrl = randomImageName;

                    var path = Path.Combine(images.PhysicalPath, randomImageName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }

                var valdtn = _productService.Update(_mapper.Map<Product>(model), C_Ids);

                foreach (KeyValuePair<bool, string> item in valdtn)
                {
                    if (!item.Key)
                    {
                        var a = new AlertStatus()
                        {
                            Message = item.Value,
                            AlertType = "danger"
                        };
                        TempData["status"] = JsonConvert.SerializeObject(a);
                        ViewBag.Categories = _categoryService.GetAll();
                        return View(model);
                    }

                    var b = new AlertStatus()
                    {
                        Message = "Ürün başarıyla güncellendi.",
                        AlertType = "success"
                    };

                    TempData["status"] = JsonConvert.SerializeObject(b);
                    return RedirectToAction("ProductList");

                }
            }
            ViewBag.Categories = _categoryService.GetAll();
            return View(model);

        }



		public IActionResult EditCategory(int? id)
		{
			if (id == null) return NotFound();
			var entity = _categoryService.GetByIdWithProducts((int)id);

			if (entity == null) return NotFound();
			var model = _mapper.Map<CategoryViewModel>(entity);
			model.Products = entity.ProductCategories.Select(p => p.Product).ToList();

            return View(model);
		}

		[HttpPost]
		public IActionResult EditCategory(CategoryViewModel model)
		{
			if (ModelState.IsValid)
			{
                if (_categoryService.GetById(model.Id) == null)
                {return NotFound();}
                _categoryService.Update(_mapper.Map<Category>(model));

                var status = new AlertStatus()
                {
                    Message = "Kategori başarıyla güncellendi.",
                    AlertType = "success"
                };
                TempData["status"] = JsonConvert.SerializeObject(status);
                return RedirectToAction("CategoryList");
            }
			return View(model);
        }



		public IActionResult DeleteProduct(int id)
		{
			var entity=_productService.GetById(id);
			if (entity != null) _productService.Delete(entity);

            var status = new AlertStatus()
            {
                Message = "Ürün başarıyla silindi.",
                AlertType = "danger"
            };

            TempData["status"] = JsonConvert.SerializeObject(status);

            return RedirectToAction("ProductList");
		}

		public IActionResult DeleteCategory(int id)
		{
			var entity = _categoryService.GetById(id);

			if (entity != null) _categoryService.Delete(entity);

			var status = new AlertStatus()
			{
				Message = "Kategori başarıyla silindi.",
				AlertType = "danger"
			};

			TempData["status"] = JsonConvert.SerializeObject(status);

			return RedirectToAction("CategoryList");
		}


        [Route("{Cid}-{Pid}")]
		public IActionResult DeleteFromCategory(int Cid,int Pid)
		{
            _categoryService.DeleteFromCategory(Cid, Pid);

			return Redirect("/admin/EditCategory/"+Cid);
		}
	}
}

