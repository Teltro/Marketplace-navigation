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
    [Table("Shops")]
    public class Shop : IDataModel
    {
        [PrimaryKey, AutoIncrement, Unique]
        public int Id { get; set; }
        [Unique]
        public int PavilionId { get; set; }
        public int OwnerId { get; set; }
        public string Name { get; set; }
        public string Info { get; set; }
    }
}