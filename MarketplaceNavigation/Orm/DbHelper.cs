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

namespace MarketplaceNavigation.Orm
{
    public class DbHelper
    {
        private SQLiteAsyncConnection db;

        public event EventHandler<User> UserChange;
        //private AccessLevels accessLevel;
        private User user;
        public User User
        {
            get => user;
            set
            {
                user = value;
                UserChange?.Invoke(this, user);
            }
        }

        public DbHelper(SQLiteAsyncConnection db)
        {
            this.db = db;
            user = User.Visitor;
        }

        public IRepository<T> CreateRepository<T>(object data) where T : class, IDataModel
        {
            if (typeof(T).Name == typeof(Floor).Name)
                return (IRepository<T>)(new FloorRepository(db));
            else if (typeof(T).Name == typeof(Pavilion).Name)
                return (IRepository<T>)(new PavilionRepository(db, (int)data));
            else if (typeof(T).Name == typeof(Shop).Name)
                return (IRepository<T>)(new ShopRepository(db));
            else if (typeof(T).Name == typeof(User).Name)
                return (IRepository<T>)(new UserRepository(db));
            else return null;
        }

    }

    public enum AccessLevels
    {
        Visitor = 0,
        Businessman = 1,
        Administrator = 2
    }

}