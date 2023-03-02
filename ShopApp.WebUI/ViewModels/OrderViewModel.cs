using System;
namespace ShopApp.WebUI.ViewModels
{
	public class OrderViewModel
	{
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public string Note { get; set; }
        public string Email { get; set; }
        public string CardHolderName { get; set; }
        public string CardNumber { get; set; }
        public string ExpireMonth { get; set; }
        public string ExpireYear { get; set; }
        public string Cvv { get; set; }

        public CartViewModel CartViewModel { get; set; } = new CartViewModel();
    }
}

