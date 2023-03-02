using System;
using Microsoft.AspNetCore.Identity;

namespace ShopApp.WebUI.Identity
{
	public static class SeedIdentity
	{
       
        public static async Task Seed(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IConfiguration _configuration)
		{
			var userName = _configuration["Data:AdminUser:UserName"];
			var password = _configuration["Data:AdminUser:Password"];
			var email = _configuration["Data:AdminUser:Email"];
			var role = _configuration["Data:AdminUser:Role"];

			if (await userManager.FindByNameAsync(userName) == null)
			{
				await roleManager.CreateAsync(new IdentityRole(role));

				var user = new User()
				{
					UserName=userName,
					FirstName="admin",
					LastName="admin",
					Email=email,
					EmailConfirmed=true,
				};

				var result = await userManager.CreateAsync(user, password);
				if (result.Succeeded)
				{
					await userManager.AddToRoleAsync(user, role);
				}

			}
		}
	}
}

