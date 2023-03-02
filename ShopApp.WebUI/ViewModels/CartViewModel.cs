using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ShopApp.Entity;

namespace ShopApp.WebUI.ViewModels
{
	public class CartViewModel
	{
        public int Id { get; set; }
        public List<CartItemViewModel> CartItems { get; set; }= new List<CartItemViewModel>();

        public double TotalPrice()
        {
            return CartItems.Sum(x => x.Price * x.Quantity);
        }
       
	}

    public class CartItemViewModel
    {
        public string Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public double Price { get; set; }
        public string ImageUrl { get; set; }
        public int Quantity { get; set; }

    }
}

