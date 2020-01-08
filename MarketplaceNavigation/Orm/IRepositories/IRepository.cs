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

namespace MarketplaceNavigation.Orm
{
    public interface IRepository<T>
        where T : class
    {
        T Get(int id);
        IEnumerable<T> GetList();
        T[] GetArray();
        int Insert(T item);
        int Update(T item);
        int Delete(int id);
        int Save(T item);
        int Count { get; }
    }
}