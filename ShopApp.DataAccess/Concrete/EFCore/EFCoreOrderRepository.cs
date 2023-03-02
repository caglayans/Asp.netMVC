using System;
using Microsoft.EntityFrameworkCore;
using ShopApp.DataAccess.Abstract;
using ShopApp.Entity;

namespace ShopApp.DataAccess.Concrete.EFCore
{
	public class EFCoreOrderRepository: EFCoreGenericRepository<Order, ShopDbContext>, IOrderRepository
    {
        private readonly ShopDbContext _db;

        public EFCoreOrderRepository(ShopDbContext db) : base(db)
        {
            _db = db;
        }

        public List<Order> GetOrders(string userId)
        {
            var orders = _db.Orders
                                    .Include(i => i.OrderItems)
                                    .ThenInclude(i => i.Product)
                                    .AsQueryable();

            if (!string.IsNullOrEmpty(userId))
            {
                orders = orders.Where(i => i.UserId == userId);
            }

            return orders.ToList();
        }
    }
}

