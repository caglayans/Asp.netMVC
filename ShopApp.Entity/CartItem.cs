using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopApp.Entity
{
    public class CartItem
	{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }

        public int ProductId { get; set; }
		public Product Product { get; set; }

        public int CartId { get; set; }
		public Cart Cart { get; set; }

		public int Quantity { get; set; }

    }
}

