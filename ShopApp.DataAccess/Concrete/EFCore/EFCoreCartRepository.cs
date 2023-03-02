using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using ShopApp.DataAccess.Abstract;
using ShopApp.Entity;

namespace ShopApp.DataAccess.Concrete.EFCore
{
	public class EFCoreCartRepository :EFCoreGenericRepository<Cart,ShopDbContext> ,ICartRepository
	{
		private readonly ShopDbContext _db;

        public EFCoreCartRepository(ShopDbContext db) :base(db)
        {
            _db = db;
        }

        public void ClearCart(int cartId)
        {
            var cmd = @"delete from CartItems where CartId=@p0";
            _db.Database.ExecuteSqlRaw(cmd, cartId);
        }

        public void DeleteFromCart(int cartId, int productId)
        {
            var cmd = @"delete from CartItems where CartId=@p0 and ProductId=@p1";
            _db.Database.ExecuteSqlRaw(cmd, cartId, productId);
        }

        public Cart GetByUserId(string userId)
        {
            return _db.Carts.Include(c => c.CartItems)
                            .ThenInclude(ci => ci.Product)
                            .FirstOrDefault(c=> c.UserId==userId);
        }

        public void AddToCart(string userId, int productId, int quantity)
        {
            var cart = GetByUserId(userId);

            if (cart != null)
            {
                var index = cart.CartItems.FindIndex(i => i.ProductId == productId);
                if (index < 0)
                {
                    cart.CartItems.Add(new CartItem()
                    {
                        Id= Guid.NewGuid().ToString(),
                        CartId = cart.Id,
                        ProductId = productId,
                        Quantity = quantity,
                    });
                }
                else
                {
                    cart.CartItems[index].Quantity += quantity;
                }

                _db.SaveChanges();
            }
        }
    }
}

