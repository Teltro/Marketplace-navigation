using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite;

using MarketplaceNavigation.Models;
using Android.Util;

namespace MarketplaceNavigation.Orm
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(SQLiteAsyncConnection db) 
            : base(db) { }

        public User Find(User userToFind)
        {
            return db.Table<User>()
                .FirstOrDefaultAsync(u =>
                    u.Name == userToFind.Name
                    && u.Password == userToFind.Password)
                .Result;
        }

        public User FindByUserName(string userName)
        {
            return db.Table<User>()
                .FirstOrDefaultAsync(u => u.Name == userName)
                .Result;
        }
        public User GetOwnerOfShop(Shop shop)
        {
            return db.GetAsync<User>(shop.OwnerId).Result;
        }
    }
}