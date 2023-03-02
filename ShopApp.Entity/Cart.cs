using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopApp.Entity
{
	public class Cart
	{      
        public int Id { get; set; }

        public string UserId { get; set; }

        public List<CartItem> CartItems { get; set; }= new List<CartItem>();
    }
}

