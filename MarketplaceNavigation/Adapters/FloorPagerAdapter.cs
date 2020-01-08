using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;

using MarketplaceNavigation.Models;
using MarketplaceNavigation.Orm;
using MarketplaceNavigation.Fragments;

using v4Fragment = Android.Support.V4.App.Fragment;
using v4FragmentManager = Android.Support.V4.App.FragmentManager;

namespace MarketplaceNavigation.Adapters
{
    public class FloorPagerAdapter : FragmentStatePagerAdapter, IFilterable
    {
        private const string tag = "FloorPagerAdapter";

        private DbHelper DAO;
        private IFloorRepository floorRepository;

        private Floor[] floors;
        private string searchQuery;
        
        public Filter Filter { get; private set; }

        public FloorPagerAdapter(v4FragmentManager fm, DbHelper DAO)
            : base(fm)
        {

            Log.Info(tag, "FloorPagerAdapter constructor calling");
            this.DAO = DAO;
            floorRepository = DAO.CreateRepository<Floor>(null) as IFloorRepository;
            floors = floorRepository.GetArray();

            Filter = new FloorFilter(this);
        }

        public override int GetItemPosition(Java.Lang.Object @object)
        {
            Log.Info(tag, "call GetItemPosition");
            return PagerAdapter.PositionNone;
        }

        public override v4Fragment GetItem(int position)
        {
            Log.Info(tag, $"calling GetItem for {position} position");

            FloorFragment fragment
                = new FloorFragment(DAO, floors[position].Id, searchQuery);
            return fragment;
        }

        public override Java.Lang.ICharSequence GetPageTitleFormatted(int position)
        {
            Log.Info(tag, $"calling GetPageTitleFormatted for {position} position");

            return new
                Java.Lang.String("Floor #" + floors[position].Number);
        }

        public override int Count => floors.Length;

        private class FloorFilter : Filter
        {
            private const string tag = "FloorFilter";

            private readonly FloorPagerAdapter adapter;
            public FloorFilter(FloorPagerAdapter adapter)
            {
                this.adapter = adapter;
            }

            protected override FilterResults PerformFiltering(ICharSequence constraint)
            {
                Log.Info(tag, constraint.ToString());

                adapter.searchQuery = constraint.ToString();

                var returnObject = new FilterResults();
                IEnumerable<Floor> results;

                if (constraint == null)
                    return returnObject;

                results = adapter.floorRepository.PavilionSearching(constraint.ToString());

                Log.Info(tag, $"results count = {results.Count()}");

                returnObject.Values
                    = FromArray(results.Select(res => res.ToJavaObject()).ToArray());
                returnObject.Count = results.Count();

                constraint.Dispose();

                return returnObject;
            }

            protected override void PublishResults(ICharSequence constraint, FilterResults results)
            {
                adapter.floors = results.Values.ToArray<Java.Lang.Object>()
                    .Select(res => res.ToNetObject<Floor>()).ToArray();

                Log.Info(tag, $"filterfinish floors count = {adapter.floors.Count()}, adapter count = {adapter.Count}");

                adapter.NotifyDataSetChanged();

                constraint.Dispose();
                results.Dispose();
            }

        }

    }
}