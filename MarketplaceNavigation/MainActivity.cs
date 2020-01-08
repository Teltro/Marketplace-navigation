using Android.App;
using Android.OS;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Support.V4.View;
using Android.Support.V4.App;
using Android.Support.V4.Widget;
using Android.Support.Design.Widget;
using Android.Util;

using MarketplaceNavigation.Orm;
using MarketplaceNavigation.Models;
using MarketplaceNavigation.DialogFragments;
using MarketplaceNavigation.Fragments;

using v4FragmentTransaction = Android.Support.V4.App.FragmentTransaction;
using v4FragmentManager = Android.Support.V4.App.FragmentManager;
using v4Fragment = Android.Support.V4.App.Fragment;
using v7Toggle = Android.Support.V7.App.ActionBarDrawerToggle;
using v7Toolbar = Android.Support.V7.Widget.Toolbar;

namespace MarketplaceNavigation
{
    [Activity(Label = "@string/app_name", 
        Theme = "@style/AppTheme", 
        MainLauncher = true, 
        ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity
        : AppCompatActivity,
        NavigationView.IOnNavigationItemSelectedListener,
        v4FragmentManager.IOnBackStackChangedListener
    {
        private const string tag = "MainActvity";

        private v7Toolbar toolbar;
        private v7Toggle toggle;
        private DrawerLayout drawerLayout;
        private NavigationView navigationView;

        private DbHelper DAO;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            DAO = new MarketplaceDatabase().DAO;
            DAO.UserChange += NavigationViewUserChanged;
            DAO.UserChange += PagerFragmentUserChanged;

            SetContentView(Resource.Layout.activity_main);

            toolbar = FindViewById<v7Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = "Marketplace";
            SupportActionBar.SetHomeButtonEnabled(true);

            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.navigationDrawerLayout);
            toggle = new v7Toggle(this, drawerLayout, toolbar, Resource.String.open, Resource.String.close);
            drawerLayout.AddDrawerListener(toggle);
            toggle.SyncState();

            navigationView = FindViewById<NavigationView>(Resource.Id.navigationNavigationView);
            navigationView.SetNavigationItemSelectedListener(this);

            SupportFragmentManager.AddOnBackStackChangedListener(this);
            toolbar.NavigationClick += (sender, e) => OnNavigationClick();

            var dialog = new CallToAutorisationDialogFragment();
            dialog.DialogResultHandler += CallToRegisterDialogFinish;
            dialog.Show(SupportFragmentManager, "MainActivity");

            var pagerFragment = new PagerFragment(DAO);
            v4FragmentTransaction fragmentTransaction = SupportFragmentManager.BeginTransaction();
            fragmentTransaction.Replace(Resource.Id.mainFrameLayout, pagerFragment);
            fragmentTransaction.Commit();

        }
        private void OnNavigationClick()
        {
            if (drawerLayout.IsDrawerOpen(GravityCompat.Start))
            {
                drawerLayout.CloseDrawer(GravityCompat.Start);
                if (SupportFragmentManager.BackStackEntryCount > 0)
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            }
            else
            {
                if (SupportFragmentManager.BackStackEntryCount > 0)
                    this.OnBackPressed();
                else
                {
                    drawerLayout.OpenDrawer(GravityCompat.Start);
                    toggle.SyncState();
                }
            }
        }
        private void CallToRegisterDialogFinish(object sender, bool result)
        {
            if (result)
                CallAutorisationFragment();
            else
                DAO.User = User.Visitor;
        }
        private void CallAutorisationFragment()
        {
            var autorisationFragment = new AutorisationFragment(DAO);
            v4FragmentTransaction fragmentTransaction = SupportFragmentManager.BeginTransaction();
            fragmentTransaction.Replace(Resource.Id.mainFrameLayout, autorisationFragment);
            fragmentTransaction.AddToBackStack(null);
            fragmentTransaction.Commit();
        }
        private void CallTextFragment(string title, string text)
        {
            var textFragment = new TextFragment(title, text);
            v4FragmentTransaction fragmentTransaction = SupportFragmentManager.BeginTransaction();
            fragmentTransaction.Replace(Resource.Id.mainFrameLayout, textFragment);

            bool qwerty = SupportFragmentManager.PopBackStackImmediate("TextFragment", 0)
                || (SupportFragmentManager.FindFragmentByTag("TextFragment") == null);

            if (qwerty)
            {
                Log.Info(tag, "true");
                SupportFragmentManager.PopBackStack();
                fragmentTransaction.AddToBackStack("TextFragment");
            }
            else
            {
                Log.Info(tag, "false");
                fragmentTransaction.AddToBackStack("TextFragment");
            }

            fragmentTransaction.Commit();
        }
        public bool OnNavigationItemSelected(IMenuItem menuItem)
        {
            string navText = "";
            switch (menuItem.ItemId)
            {
                case Resource.Id.autorisationNavigationItem:
                    NavigationAutorisationClick(menuItem);
                    break;
                case Resource.Id.helpNavigationItem:
                    navText = "Here must be some text which can help you";
                    CallTextFragment("Help", navText);
                    break;
                case Resource.Id.aboutProgrammNavigationItem:
                    navText = "Here must be some text about this programm";
                    CallTextFragment("About programm", navText);
                    break;
                case Resource.Id.aboutAuthorNavigationItem:
                    navText = "Here must be some text about deleveper";
                    CallTextFragment("About developer", navText);
                    break;
            }
            drawerLayout.CloseDrawer(GravityCompat.Start);
            return true;
        }
        private void NavigationAutorisationClick(IMenuItem menuItem)
        {
            if (DAO.User.AccessLevel == AccessLevels.Visitor)
                CallAutorisationFragment();
            else
            {
                var dialog = new AreYouSureDialogFragment("Are you sure do want to log out?");
                dialog.DialogResultHandler += LogOutAccept;
                dialog.Show(SupportFragmentManager, "LogOutDialogFragment");
            }
        }
        private void PagerFragmentUserChanged(object sender, User user)
        {
            var fragmentToRefresh = SupportFragmentManager.FindFragmentById(Resource.Id.mainFrameLayout);
            if (fragmentToRefresh is PagerFragment)
            {
                var pagerFragment = new PagerFragment(DAO);
                v4FragmentTransaction fragmentTransaction = SupportFragmentManager.BeginTransaction();
                fragmentTransaction.Replace(Resource.Id.mainFrameLayout, pagerFragment);
                fragmentTransaction.Commit();
            }
        }
        private void NavigationViewUserChanged(object sender, User user)
        {
            navigationView
                .Menu
                .FindItem(Resource.Id.autorisationNavigationItem)
                .SetTitle(user.AccessLevel == AccessLevels.Visitor ? "Autorisation" : "Log Out");

            var navigationAccessLevel = navigationView.FindViewById<TextView>(Resource.Id.navigationUserAccessLevel);
            var navigationUserName = navigationView.FindViewById<TextView>(Resource.Id.navigationUserName);

            if (user.AccessLevel == AccessLevels.Visitor)
            {
                navigationUserName.Text = user.AccessLevel.ToString();
                navigationAccessLevel.Text = "";
            }
            else
            {
                navigationUserName.Text = user.Name;
                navigationAccessLevel.Text = user.AccessLevel.ToString();
            }
        }
        private void LogOutAccept(object Sender, bool result)
        {
            if (result)
                DAO.User = User.Visitor;
        }
        public void OnBackStackChanged()
        {
            if (SupportFragmentManager.BackStackEntryCount > 0)
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            else
            {
                SupportActionBar.SetDisplayHomeAsUpEnabled(false);
                toolbar.Title = "Marketplace";
                toggle.SyncState();
            }
        }
        public override void OnBackPressed()
        {
            if (drawerLayout.IsDrawerOpen(GravityCompat.Start))
            {
                drawerLayout.CloseDrawer(GravityCompat.Start);
                if (SupportFragmentManager.BackStackEntryCount > 0)
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                else
                    toggle.SyncState();
                return;
            }
            else
                base.OnBackPressed();
        }
    }
}