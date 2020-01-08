using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;

using MarketplaceNavigation.Models;
using MarketplaceNavigation.Orm;
using MarketplaceNavigation.DialogFragments;
using MarketplaceNavigation.Fragments;
using MarketplaceNavigation.Adapters;

using v4Fragment = Android.Support.V4.App.Fragment;
using v4FragmentTransaction = Android.Support.V4.App.FragmentTransaction;
using v4FragmentManager = Android.Support.V4.App.FragmentManager;

namespace MarketplaceNavigation.Fragments
{
    public class FloorFragment : v4Fragment
    {
        private const string tag = "FloorFragment";

        private DbHelper DAO;
        private IPavilionRepository pavilionRepository;
        private IShopRepository shopRepository;
        private RecyclerView recyclerView;
        private LinearLayoutManager layoutManager;
        private string searchQuery;
        public PavilionAdapter pavilionAdapter { get; private set; }

        private int floorId;

        private bool dialogResult;

        public FloorFragment() { }
        public FloorFragment(DbHelper DAO, int floorId, string searchQuery)
        {
            this.DAO = DAO;
            this.floorId = floorId;
            this.searchQuery = searchQuery;
            pavilionRepository = DAO.CreateRepository<Pavilion>(floorId) as IPavilionRepository;
            shopRepository = DAO.CreateRepository<Shop>(null) as IShopRepository;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            RetainInstance = true;

            View view = inflater.Inflate(Resource.Layout.floor_layout, container, false);

            recyclerView = view.FindViewById<RecyclerView>(Resource.Id.pavilionsRecyclerView);
            layoutManager = new LinearLayoutManager(this.Context);
            recyclerView.SetLayoutManager(layoutManager);
            recyclerView.AddItemDecoration(new DividerItemDecoration(Activity));

            pavilionAdapter = new PavilionAdapter(DAO, floorId, searchQuery);
            pavilionAdapter.OpenShop += OpenShop;
            pavilionAdapter.SeparatePavilion += SeparatePavilion;
            pavilionAdapter.CloseShop += CloseShop;
            pavilionAdapter.AddPavilionToShop += AddPavilionToShop;
            recyclerView.SetAdapter(pavilionAdapter);

            return view;
        }

        private void OpenShop(object sender, Pavilion pavilion)
        {
            ShopRegisterFragment shopFragment = new ShopRegisterFragment(DAO, pavilion); // !!!
            v4FragmentManager fm = ParentFragment.FragmentManager;
            v4FragmentTransaction fragmentTransaction = fm.BeginTransaction();
            fragmentTransaction.Replace(Resource.Id.mainFrameLayout, shopFragment);
            fragmentTransaction.AddToBackStack(null);
            fragmentTransaction.Commit();
        }
        private void OnFinishSeparatePavilionDialog(object sneder, bool result) => dialogResult = result; // repeat
        private async void SeparatePavilion(object sender, PavilionEventsArgs e)
        {
            var dialog = new AreYouSureDialogFragment("Are you sure do want to separete this pavilion?");
            dialog.DialogResultHandler += OnFinishSeparatePavilionDialog;
            dialog.Show(FragmentManager, "AreYouSureDialogFragment");
            await System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                while (!dialog.IsRemoving) { }
            });

            if (dialogResult)
            {
                dialogResult = false;

                e.pavilion.Size--;
                pavilionRepository.Update(e.pavilion);

                var pavilionToInsert = new Pavilion();
                int pavilionToInsertPosition = pavilionAdapter.ItemCount;
                pavilionToInsert.IsEmpty = true;
                pavilionToInsert.Size = 1;
                pavilionToInsert.Number
                    = pavilionRepository
                    .GetByPosition(pavilionToInsertPosition - 1)
                    .Number + 1;

                pavilionRepository.Insert(pavilionToInsert);
                pavilionAdapter.RefreshData();
                Toast.MakeText(Activity, "Separated", ToastLength.Short).Show();
            }
            else
                Toast.MakeText(Activity, "Canceled", ToastLength.Short).Show();
        }
        private void OnFinishAddPavilionDialog(object sender, EventArgs e)
        {
            //pavilionAdapter.NotifyItemRangeChanged(e.beginPosition, e.count);
            Log.Info(tag, "datasetchanging");
            pavilionAdapter.RefreshData(/*e.startPosition, e.count*/);
        }
        private void AddPavilionToShop(object sender, PavilionEventsArgs e)
        {
            var dialog = new AddPavilionToShopDialogFragment(DAO, e.pavilion);
            dialog.AddPavilionToShopFinish += OnFinishAddPavilionDialog;
            dialog.Show(FragmentManager, "AddPavilionToShopFragment");
        }
        private void OnFinishCloseShopDialog(object sender, bool result) => dialogResult = result;
        private async void CloseShop(object sender, PavilionEventsArgs e)
        {
            AreYouSureDialogFragment dialog = new AreYouSureDialogFragment("Are you sure do you want to close this shop?");
            dialog.DialogResultHandler += OnFinishCloseShopDialog;
            dialog.Show(FragmentManager, "CloseShopDialogFragment");

            await System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                while (!dialog.IsRemoving) { }
            });

            Log.Info(tag, $"Dialog result = {dialogResult.ToString()}");

            if (dialogResult)
            {
                dialogResult = false;

                shopRepository.DeleteByPavilion(e.pavilion);
                e.pavilion.Check();
                e.pavilion.IsEmpty = true;
                pavilionAdapter.NotifyItemChanged(e.position);
                Toast.MakeText(Activity, "Deleted", ToastLength.Short).Show();
            }
            else
                Toast.MakeText(Activity, "Canceled", ToastLength.Short).Show();
        }
    }
}