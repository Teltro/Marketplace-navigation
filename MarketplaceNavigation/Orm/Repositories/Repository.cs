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


namespace MarketplaceNavigation.Orm
{
    public abstract class Repository<T> : IRepository<T>
        where T: class, IDataModel, new()
    {
        protected SQLiteAsyncConnection db;
        public Repository(SQLiteAsyncConnection db)
        {
            this.db = db;
        }

        public virtual T Get(int id)
        {
            return db.Table<T>()
                .FirstOrDefaultAsync(f => f.Id == id)
                .Result;
        }

        public virtual IEnumerable<T> GetList()
        {
            return db.Table<T>()
                .ToListAsync()
                .Result;
        }

        public virtual T[] GetArray()
        {
            return db.Table<T>()
                .ToArrayAsync()
                .Result;
        }

        public virtual int Insert(T item)
        {
            return db.InsertAsync(item).Result;
        }

        public virtual int Update(T item)
        {
            return db.UpdateAsync(item).Result;
        }

        public virtual int Delete(int id)
        {
            return db.DeleteAsync<T>(id).Result;
        }

        public virtual int Save(T item)
        {
            return db.InsertOrReplaceAsync(item).Result;
        }

        public virtual int Count
        {
            get => db.Table<T>().CountAsync().Result;
        }
    }
}