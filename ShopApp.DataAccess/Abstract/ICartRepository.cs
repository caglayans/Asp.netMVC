using System;
using ShopApp.Entity;

namespace ShopApp.DataAccess.Abstract
{
	public interface ICartRepository :IRepository<Cart>
	{
        void ClearCart(int cartId);
        void DeleteFromCart(int cartId, int productId);
        Cart GetByUserId(string userId);
        void AddToCart(string userId, int productId, int quantity);
    }
}

