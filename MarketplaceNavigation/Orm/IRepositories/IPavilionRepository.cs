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
    public interface IPavilionRepository : IRepository<Pavilion>
    {
        Pavilion GetByPosition(int position);
        Pavilion GetByNum(int num);
        int GetPavilionPosition(Pavilion pavilion);
        void SortWithUpdate(int beginPosition, int endPosition);
        //void SortWithUpdate(Pavilion pavilion, int endPosition);
        IEnumerable<Pavilion> Search(string query);
        bool CanInsert(Pavilion pavilion);
        bool CanUpdate(Pavilion pavilion);
    }
}