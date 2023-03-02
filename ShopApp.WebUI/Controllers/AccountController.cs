using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShopApp.Business.Abstract;
using ShopApp.WebUI.EmailServices;
using ShopApp.WebUI.Identity;
using ShopApp.WebUI.Models;
using ShopApp.WebUI.ViewModels;

namespace ShopApp.WebUI.Controllers
{
    [AutoValidateAntiforgeryToken]
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private SignInManager<User> _signInManager;
        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;
        private readonly ICartService _cartService;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IMapper mapper, IEmailSender emailSender, ICartService cartService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _emailSender = emailSender;
            _cartService = cartService;
        }

        public IActionResult Login(string ReturnUrl=null)
        {
            return View(new LoginViewModel()
            {
                ReturnUrl = ReturnUrl
            });
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //var user = await _userManager.FindByNameAsync(model.UserName);
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Bu kullanıcı adı ile ilgili hesap bulunamadı.");
                return View(model);
            }

            if(!await _userManager.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelError("", "Lütfen e-postanıza gelen link ile hesabınızı onaylayınız.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password,true,false);

            if (result.Succeeded)
            {
                if(model.ReturnUrl==null)
                return RedirectToAction("Index", "Home");
                return Redirect(model.ReturnUrl);
            }

            ModelState.AddModelError("", "Girilen kullanıcı adı veya parola yanlış.");
            return View(model);
        }


        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _mapper.Map<User>(model);

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var url = Url.Action("ConfirmEmail", "Account", new
                {
                    userId = user.Id,
                    token = code
                },  protocol: "https", host: "localhost:7190");

                await _emailSender.SendEmailAsync(model.Email, "Hesabınızı onaylayınız.", "Lütfen E-posta hesabınızı onaylamak için linke <a href=\"" + url +"\">tıklayınız.</a>");

                return RedirectToAction("Login");
            }
            

            ModelState.AddModelError("", "Bilinmeyen bir hata oluştu.Lütfen tekrar deneyiniz.");
            //ModelState.AddModelError("Password", "Parola en az 6 karakterden oluşmalı. Büyük-küçük harf, rakam ve “?, @, !, #, %, +, -, *, %” gibi özel karakterler içermelidir.");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Redirect("~/");
        }


        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if(userId==null || token == null)
            {
                CreateMessage("Geçersiz token.", "danger");
                return View();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    //cart objesini oluşturma
                    _cartService.InitializeCart(userId);
                    CreateMessage("Hesabınız onaylandı.", "success");
                    return View();
                }               
            }
            CreateMessage("Hesabınız onaylanmadı.", "warning");
            return View();
        }


        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return View();
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return View();

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var url = Url.Action("ResetPassword", "Account", new
            {
                userId = user.Id,
                token = code
            }, protocol: "https", host: "localhost:7190");

            await _emailSender.SendEmailAsync(email, "Parola sıfırlama.", "Lütfen parolanızı yenilemek için linke <a href=\"" + url + "\">tıklayınız.</a>");
            return View();
        }

        public IActionResult ResetPassword(string userId, string token)
        {
            if(userId==null || token==null)
            {
                return RedirectToAction("Home", "Index");
            }
            var model = new ResetPasswordViewModel { Token = token };

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                CreateMessage("Daha önce bu e-posta hesabı ile bir kullanıcı oluşturulmamış", "warning");
                return View();
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded) { return RedirectToAction("Login", "Account"); }
            return View(model);
        }

        public IActionResult AccessDenied()
        {
            return View();
        }


        private void CreateMessage(string message, string alertType)
        {
            var status = new AlertStatus()
            {
                Message = message,
                AlertType = alertType
            };

            TempData["status"] = JsonConvert.SerializeObject(status);
        }
    }
}

