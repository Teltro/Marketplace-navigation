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

using MarketplaceNavigation.Orm;

using SQLite;

namespace MarketplaceNavigation.Models
{
    public class User : IDataModel
    {
        [PrimaryKey, AutoIncrement, Unique]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public int DbAccessLevel { get; set; }
        public AccessLevels AccessLevel
        {
            get => (AccessLevels)DbAccessLevel;
            set => DbAccessLevel = (int)value;
        }
        public User() { }
        public static User Visitor
        {
            get
            {
                var user = new User();
                user.Name = "";
                user.Password = "";
                user.Id = 0;
                user.AccessLevel = AccessLevels.Visitor;
                return user;
            }
        }
    }
}