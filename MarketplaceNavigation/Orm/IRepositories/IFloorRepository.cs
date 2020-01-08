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
    public interface IFloorRepository : IRepository<Floor>
    {
        Floor GetByPosition(int position);
        IEnumerable<Floor> PavilionSearching(string query);
    }
}