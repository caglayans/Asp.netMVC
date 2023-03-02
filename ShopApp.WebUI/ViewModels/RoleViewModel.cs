using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using ShopApp.WebUI.Identity;

namespace ShopApp.WebUI.ViewModels
{
	public class RoleViewModel
	{
		[Required]
		public string Name { get; set; }
	}

	public class RoleDetailsViewModel
	{
		public IdentityRole Role{ get; set; }
		public IEnumerable<User> Members { get; set; }
        public IEnumerable<User> NonMembers { get; set; }

    }

    public class EditRoleViewModel
    {
        public EditRoleViewModel()
        {
            Users = new List<string>();
        }

        public string Id { get; set; }

        [Required]
        public string RoleName { get; set; }

        public List<string> Users { get; set; }

    }

    public class UserRoleViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public bool IsSelected { get; set; }
    }

}

