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

using MarketplaceNavigation.Orm;

namespace MarketplaceNavigation.Models
{
    public class Category : IDataModel
    {
        [PrimaryKey, AutoIncrement, Unique]
        public int Id { get; set; }
        public string CategoryName { get; set; }
    }
}