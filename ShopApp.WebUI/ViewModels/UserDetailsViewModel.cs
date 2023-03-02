using System;
using System.ComponentModel.DataAnnotations;

namespace ShopApp.WebUI.ViewModels
{
	public class UserDetailsViewModel
	{

        public UserDetailsViewModel()
        {
            SelectedRoles = new List<string>();
        }

		public string UserId { get; set; }

        [Required]
        public string UserName { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }
        public IEnumerable<string> SelectedRoles { get; set; }

    }

    public class UserRolesViewModel
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public bool IsSelected { get; set; }
    }
}

