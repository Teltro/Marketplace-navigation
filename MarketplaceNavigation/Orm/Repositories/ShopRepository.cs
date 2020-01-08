using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using SQLite;

using MarketplaceNavigation.Models;

namespace MarketplaceNavigation.Orm
{
    public class ShopRepository : Repository<Shop>, IShopRepository
    {
        private const string tag = "ShopRepository";
        public ShopRepository(SQLiteAsyncConnection db)
            : base(db) { }

        public override int Insert(Shop shop)
        {
            PavilionSpaceChange(shop.PavilionId, false);
            return base.Insert(shop);
        }

        public override int Update(Shop shop)
        {
            var originalShop
                = db.Table<Shop>()
                .FirstOrDefaultAsync(s => s.Id == shop.Id)
                .Result;
            PavilionSpaceChange(originalShop.PavilionId, true);
            PavilionSpaceChange(shop.PavilionId, false);
            return base.Update(shop);
        }

        public override int Delete(int id)
        {
            var shop = db.Table<Shop>()
                .FirstOrDefaultAsync(s => s.Id == id)
                .Result;
            PavilionSpaceChange(shop.PavilionId, true);
            return base.Delete(id);
        }

        public int DeleteByPavilion(Pavilion pavilion)
        {
            var shop = db.Table<Shop>()
                .FirstOrDefaultAsync(s => s.PavilionId == pavilion.Id)
                .Result;
            return base.Delete(shop.Id);
        }

        public int DeleteByPavilionId(int pavilionId)
        {
            PavilionSpaceChange(pavilionId, true);
            return db.Table<Shop>()
                .Where(s => s.PavilionId == pavilionId)
                .DeleteAsync()
                .Result;
        }

        public override int Save(Shop shop)
        {
            var originalShop
               = db.Table<Shop>()
               .FirstOrDefaultAsync(s => s.Id == shop.Id)
               .Result;
            if (originalShop != null)
                PavilionSpaceChange(originalShop.PavilionId, true);
            PavilionSpaceChange(shop.PavilionId, false);
            return base.Save(shop);
        }

        public Shop GetShopByPavilion(Pavilion pavilion)
        {
            return db.Table<Shop>()
                .FirstOrDefaultAsync(s => s.PavilionId == pavilion.Id).Result;
          
        }

        public Shop GetShopByPavilionId(int pavilionId)
        {
            return db.Table<Shop>().FirstOrDefaultAsync(s => s.PavilionId == pavilionId).Result;
        }

        private int PavilionSpaceChange(int pavilionId, bool IsEmpty)
        {
            var pavilion = db.Table<Pavilion>()
                .FirstOrDefaultAsync(p => p.Id == pavilionId)
                .Result;
            pavilion.IsEmpty = IsEmpty;
            return db.UpdateAsync(pavilion).Result;
        }
    }
}