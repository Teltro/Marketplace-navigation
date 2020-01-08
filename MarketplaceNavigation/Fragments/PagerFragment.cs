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
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Util;
using Android.Support.Design.Widget;

using MarketplaceNavigation.Orm;
using MarketplaceNavigation.Models;
using MarketplaceNavigation.Adapters;

using v4Fragment = Android.Support.V4.App.Fragment;
using v7SearchView = Android.Support.V7.Widget.SearchView;

namespace MarketplaceNavigation.Fragments
{
    public class PagerFragment : v4Fragment
    {
        private const string tag = "PagerFragment";

        private ViewPager viewPager;
        private TabLayout pagerTabLayout;

        private FloorPagerAdapter floorPagerAdapter;

        private DbHelper DAO;
        public PagerFragment(DbHelper DAO)
        {
            this.DAO = DAO;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.pager_layout, container, false);

            HasOptionsMenu = true;

            viewPager = view.FindViewById<ViewPager>(Resource.Id.floorPager);
            floorPagerAdapter = new FloorPagerAdapter(this.ChildFragmentManager, DAO);
            viewPager.Adapter = floorPagerAdapter;
            pagerTabLayout = view.FindViewById<TabLayout>(Resource.Id.pagerTabLayout);
            pagerTabLayout.SetupWithViewPager(viewPager);

            return view;
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.toolbar_menu, menu);

            IMenuItem searchItem = menu.FindItem(Resource.Id.toolbarSearch);
            SearchManager searchManager = (SearchManager)Activity.GetSystemService(Context.SearchService);
            v7SearchView searchView = (v7SearchView)menu.FindItem(Resource.Id.toolbarSearch).ActionView;
            searchView.SetSearchableInfo(searchManager.GetSearchableInfo(Activity.ComponentName));

            searchView.QueryTextSubmit += (sender, e) =>
            {
                Log.Info(tag, $"searching for {e.NewText}");
                floorPagerAdapter.Filter.InvokeFilter(e.NewText);
                e.Handled = true;
            };

            searchItem.SetOnActionExpandListener(new SearchViewExpandListener(floorPagerAdapter));

            base.OnCreateOptionsMenu(menu, inflater);
        }

        private class SearchViewExpandListener : Java.Lang.Object, IMenuItemOnActionExpandListener
        {
            private readonly IFilterable adapter;
            public SearchViewExpandListener(IFilterable adapter)
            {
                this.adapter = adapter;
            }

            public bool OnMenuItemActionExpand(IMenuItem item)
            {
                return true;
            }

            public bool OnMenuItemActionCollapse(IMenuItem item)
            {
                adapter.Filter.InvokeFilter("");
                return true;
            }
        }
    }
}