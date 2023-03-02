using Microsoft.EntityFrameworkCore;
using ShopApp.DataAccess.Abstract;
using ShopApp.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopApp.DataAccess.Concrete.EFCore
{
    public class EFCoreCategoryRepository : EFCoreGenericRepository<Category, ShopDbContext>, ICategoryRepository
    {

        private ShopDbContext _db;

        public EFCoreCategoryRepository(ShopDbContext db) : base(db)
        {
            _db = db;
        }

        public void DeleteFromCategory(int Cid, int Pid)
        {
            var cmd = "delete from productCategory where ProductId=@P0 and CategoryId=@P1";
            _db.Database.ExecuteSqlRaw(cmd, Pid, Cid);
            /*var category= _db.Categories.Where(c => c.Id == Cid)
                                        .Include(c => c.ProductCategories)
                                        .ThenInclude(pc => pc.ProductId == Pid)
                                        .FirstOrDefault();
            _db.SaveChanges();*/
        }

        public Category GetByIdWithProducts(int id)
        {
            return _db.Categories.Where(c => c.Id == id)
                                 .Include(c => c.ProductCategories)
                                 .ThenInclude(pc => pc.Product)
                                 .FirstOrDefault();
        }
    }
}

