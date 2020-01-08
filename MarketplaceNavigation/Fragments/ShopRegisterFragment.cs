using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;

using MarketplaceNavigation.Models;
using MarketplaceNavigation.Orm;

using v4Fragment = Android.Support.V4.App.Fragment;
using v4FragmentManager = Android.Support.V4.App.FragmentManager;
using v4FragmentTransaction = Android.Support.V4.App.FragmentTransaction;

namespace MarketplaceNavigation.Fragments
{
    public class ShopRegisterFragment : v4Fragment
    {
        private const string tag = "ShopRegisterFragment";

        private DbHelper DAO;
        private IShopRepository shopRepository;
        private IPavilionRepository pavilionRepository;
        private Pavilion pavilion;

        private EditText shopNameEditText;
        private EditText shopInfoEditText;
        private Button shopRegisterButton;


        public ShopRegisterFragment(DbHelper DAO, Pavilion pavilion)
        {

            this.DAO = DAO;
            shopRepository = DAO.CreateRepository<Shop>(null) as IShopRepository;
            pavilionRepository
                = DAO.CreateRepository<Pavilion>(pavilion.FloorId) as IPavilionRepository;
            this.pavilion = pavilion;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            ((MainActivity)Activity).SupportActionBar.Title = "Shop Register";

            View view = inflater.Inflate(Resource.Layout.shop_register_layout, container, false);

            shopNameEditText = view.FindViewById<EditText>(Resource.Id.shopNameEditText);
            shopInfoEditText = view.FindViewById<EditText>(Resource.Id.shopInfoEditText);
            shopRegisterButton = view.FindViewById<Button>(Resource.Id.shopRegisterButton);

            shopRegisterButton.Click += (sender, e) => ShopRegisterClick();

            return view;
        }

        private void ShopRegisterClick()
        {
            View.ClearFocus();

            Shop shop = new Shop();
            shop.Name = shopNameEditText.Text;
            shop.Info = shopInfoEditText.Text;
            shop.PavilionId = pavilion.Id;
            shop.OwnerId = DAO.User.Id;

            shopRepository.Insert(shop);

            this.Activity.OnBackPressed();
            
        }

    }
}