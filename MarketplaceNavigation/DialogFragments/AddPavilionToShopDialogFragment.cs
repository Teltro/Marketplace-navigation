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
using Android.Util;

using MarketplaceNavigation.Orm;
using MarketplaceNavigation.Models;

using v4DialogFragment = Android.Support.V4.App.DialogFragment;
using v4FragmentManager = Android.Support.V4.App.FragmentManager;

namespace MarketplaceNavigation.DialogFragments
{
    public class AddPavilionToShopDialogFragment : v4DialogFragment
    {
        private const string tag = "AppPavilionToShop";

        private Spinner pavilionsSpinner;
        private Button addButton;

        private Pavilion pavilionToUpdate;
        private Pavilion selectedPavilion;
        private Shop shopToUpdate;

        private IPavilionRepository pavilionRepository;
        private IShopRepository shopRepository;

        public event EventHandler AddPavilionToShopFinish;

        public AddPavilionToShopDialogFragment(DbHelper DAO, Pavilion pavilion)
        {
            pavilionRepository = DAO.CreateRepository<Pavilion>(pavilion.FloorId) as IPavilionRepository;
            shopRepository = DAO.CreateRepository<Shop>(null) as IShopRepository;

            pavilionToUpdate = pavilion;
            shopToUpdate = shopRepository.GetShopByPavilion(pavilion);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.add_pavilion_to_shop_layout, container, false);
            pavilionsSpinner = view.FindViewById<Spinner>(Resource.Id.pavilionsSpinner);
            addButton = view.FindViewById<Button>(Resource.Id.addPavilionToShopDialogButton);

            var spinnerData = pavilionRepository
                                .GetList()
                                .Where(p => p.IsEmpty)
                                .Where(p => p.Size + pavilionToUpdate.Size <= 3)
                                .Select(p => p.Number.ToString())
                                .ToList();
            spinnerData.Insert(0, "Choice");

            var spinnerAdapter
                = new ArrayAdapter<string>(this.Activity, Resource.Layout.support_simple_spinner_dropdown_item, spinnerData);
            pavilionsSpinner.Adapter = spinnerAdapter;
            pavilionsSpinner.ItemSelected += (sender, e) => SpinnerItemSelected();

            addButton.Enabled = false;
            addButton.Click += (sender, e) => AddClick(selectedPavilion);

            return view;
        }

        private void AddClick(Pavilion selectedPavilion)
        {
            int startPosition;
            if (selectedPavilion.Number > pavilionToUpdate.Number)
            {
                startPosition = pavilionRepository.GetPavilionPosition(selectedPavilion) - 1;

                pavilionToUpdate.Size++;
                pavilionRepository.Update(pavilionToUpdate);
                pavilionRepository.Delete(selectedPavilion.Id);
            }
            else
            {
                selectedPavilion.Size++;
                pavilionRepository.Update(selectedPavilion);

                shopToUpdate.PavilionId = selectedPavilion.Id;
                shopRepository.Update(shopToUpdate);

                startPosition = pavilionRepository.GetPavilionPosition(pavilionToUpdate) - 1; // if (-1)
                pavilionRepository.Delete(pavilionToUpdate.Id);
            }
            pavilionRepository.SortWithUpdate(startPosition, pavilionRepository.Count - startPosition - 1);

            if (startPosition == -1)
                startPosition++;
            Log.Info(tag, $"startPosToUpdate: {startPosition}, count: {pavilionRepository.Count}");
            var args = new AddPavilionToShopEventsArgs(startPosition, pavilionRepository.Count);
            AddPavilionToShopFinish?.Invoke(this, args);
            Dismiss();
        }

        private void SpinnerItemSelected()
        {
            int pavilionNum;
            bool isNum = Int32.TryParse(pavilionsSpinner.SelectedItem.ToString(), out pavilionNum);
            if (isNum)
            {
                selectedPavilion = pavilionRepository.GetByNum(pavilionNum);

                Log.Info(tag, $"selected pavilion num #{selectedPavilion.Number.ToString()}");

                if (selectedPavilion != null)
                    addButton.Enabled = true;
            }
            else
            {
                addButton.Enabled = false;
                Log.Info(tag, "pavilion is not selected");
            }
        }
    }

    public class AddPavilionToShopEventsArgs : EventArgs
    {
        public int startPosition { get; set; }
        public int count { get; set; }

        public AddPavilionToShopEventsArgs(int startPosition, int count)
        {
            this.startPosition = startPosition;
            this.count = count;
        }
    }
}