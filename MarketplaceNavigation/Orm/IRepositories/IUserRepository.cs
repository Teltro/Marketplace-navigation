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

using MarketplaceNavigation.Models;

namespace MarketplaceNavigation.Orm
{
    public interface IUserRepository : IRepository<User>
    {
        User Find(User userToFind);
        User FindByUserName(string userName);
        User GetOwnerOfShop(Shop shop);
    }
}