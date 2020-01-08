using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;

using MarketplaceNavigation.Models;
using MarketplaceNavigation.Orm;

namespace MarketplaceNavigation.Adapters
{
    public class PavilionAdapter : RecyclerView.Adapter//, IFilterable
    {
        private const string tag = "PavilionAdapter";

        private IPavilionRepository pavilionRepository;
        private IShopRepository shopRepository;
        private IUserRepository userRepository;

        private User user;
        private Pavilion[] pavilions;

        public event EventHandler<Pavilion> OpenShop;
        public event EventHandler<PavilionEventsArgs> SeparatePavilion;
        public event EventHandler<PavilionEventsArgs> AddPavilionToShop;
        public event EventHandler<PavilionEventsArgs> CloseShop;

        public void RefreshData()
        {
            pavilions = pavilionRepository.GetArray();
            NotifyDataSetChanged();
        }

        public PavilionAdapter(DbHelper DAO, int floorId, string searchQuery)
        {
            pavilionRepository = DAO.CreateRepository<Pavilion>(floorId) as IPavilionRepository;
            shopRepository = DAO.CreateRepository<Shop>(null) as IShopRepository;
            userRepository = DAO.CreateRepository<User>(null) as IUserRepository;
            user = DAO.User;
            if (searchQuery == "" || searchQuery == null)
            {
                Log.Info(tag, "without search");
                pavilions = pavilionRepository.GetArray();
            }
            else
            {
                Log.Info(tag, "with search");
                pavilions = pavilionRepository.Search(searchQuery).ToArray();
            }
        }


        public override RecyclerView.ViewHolder
            OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context)
                                            .Inflate(Resource.Layout.pavilion_layout, parent, false);
            PavilionViewHolder holder = new PavilionViewHolder(itemView/*, OnClick*/);

            holder.BindViewClick(holder.ItemView, PavilionClick);
            holder.BindViewClick(holder.OpenShopButton, OpenShopClick);
            holder.BindViewClick(holder.SeparatePavilionButton, SeparatePavilionClick);
            holder.BindViewClick(holder.AddPavilionToShopButton, AddPavilionToShopClick);
            holder.BindViewClick(holder.CloseShopButton, CloseShopClick);

            return holder;
        }

        public override void
            OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            PavilionViewHolder _holder = holder as PavilionViewHolder;

            Shop shop;
            User owner;
            if (pavilions[position].IsEmpty)
            {
                shop = null;
                owner = null;
            }
            else
            {
                shop = shopRepository.GetShopByPavilion(pavilions[position]);
                Log.Info(tag, $"Binding shop name is: {shop.Name}");
                owner = userRepository.GetOwnerOfShop(shop);
            }

            _holder.BindPavilionView(pavilions[position], shop, owner, user);
        }

        private void PavilionClick(int position)
        {
            var pavilion = pavilions[position];
            pavilion.Check();
            NotifyItemChanged(position);
        }

        private void OpenShopClick(int position)
        {
            OpenShop?.Invoke(this, pavilions[position]);
        }

        private void SeparatePavilionClick(int position)
        {
            PavilionEventsArgs eventArgs
                = new PavilionEventsArgs(pavilions[position], position);
            SeparatePavilion?.Invoke(this, eventArgs);
        }

        private void AddPavilionToShopClick(int position)
        {
            PavilionEventsArgs eventsArgs
                = new PavilionEventsArgs(pavilions[position], position);
            AddPavilionToShop?.Invoke(this, eventsArgs);
        }

        private void CloseShopClick(int position)
        {
            PavilionEventsArgs eventArgs
                = new PavilionEventsArgs(pavilions[position], position);
            CloseShop?.Invoke(this, eventArgs);
        }

        public override int ItemCount => pavilions.Length;

        private class PavilionViewHolder : RecyclerView.ViewHolder
        {
            public TextView PavilionNum { get; private set; }
            public TextView ShopName { get; private set; }
            public TextView ShopInfo { get; private set; }
            public TextView ShopSize { get; private set; }
            public TextView ShopOwnerName { get; private set; }
            public LinearLayout ShopDetails { get; private set; }
            public LinearLayout EveryPavilionFunctional { get; private set; }
            public LinearLayout EmptyPavilionFunctional { get; private set; }
            public LinearLayout FilledPavilionFunctional { get; private set; }
            public Button OpenShopButton { get; private set; }
            public Button SeparatePavilionButton { get; private set; }
            public Button AddPavilionToShopButton { get; private set; }
            public Button CloseShopButton { get; private set; }


            public PavilionViewHolder(View itemView)
                : base(itemView)
            {
                PavilionNum = itemView.FindViewById<TextView>(Resource.Id.pavilionNumTextView);
                ShopName = itemView.FindViewById<TextView>(Resource.Id.shopNameTextView);

                ShopDetails = itemView.FindViewById<LinearLayout>(Resource.Id.shopDetails);
                ShopInfo = itemView.FindViewById<TextView>(Resource.Id.shopInfoTextView);
                ShopSize = itemView.FindViewById<TextView>(Resource.Id.shopSizeTextView);
                ShopOwnerName = itemView.FindViewById<TextView>(Resource.Id.shopOwnerNameTextView);

                EveryPavilionFunctional = itemView.FindViewById<LinearLayout>(Resource.Id.everyPavilionFunctional);
                SeparatePavilionButton = itemView.FindViewById<Button>(Resource.Id.separatePavilionButton);

                EmptyPavilionFunctional = itemView.FindViewById<LinearLayout>(Resource.Id.emptyPavilionFunctional);
                OpenShopButton = itemView.FindViewById<Button>(Resource.Id.openShopButton);

                FilledPavilionFunctional = itemView.FindViewById<LinearLayout>(Resource.Id.filledPavilionFunctional);
                CloseShopButton = itemView.FindViewById<Button>(Resource.Id.closeShopButton);
                AddPavilionToShopButton = itemView.FindViewById<Button>(Resource.Id.addPavilionToShopButton);
            }

            public void BindViewClick(View view, Action<int> clickListener)
            {
                view.Click += (sender, e) => clickListener(base.AdapterPosition);
            }

            public void BindPavilionView(Pavilion pavilion, Shop shop, User owner, User user)
            {
                PavilionNum.Text = pavilion.Number.ToString();
                if (pavilion.Size <= 3)
                    AddPavilionToShopButton.Enabled = true;
                else
                    AddPavilionToShopButton.Enabled = false;

                if (pavilion.Size > 1)
                    SeparatePavilionButton.Visibility = ViewStates.Visible;
                else
                    SeparatePavilionButton.Visibility = ViewStates.Gone;

                if (!pavilion.IsEmpty)
                {
                    ShopName.Text = shop.Name;
                    ShopInfo.Text = "Info: " + shop.Info;
                    ShopSize.Text = "Size: " + pavilion.Size.ToString();
                    ShopOwnerName.Text = "Owner: " + owner.Name;
                    PavilionNum.SetTextColor(Color.Black);
                }
                else
                {
                    ShopName.Text = "";
                    ShopInfo.Text = "";
                    ShopSize.Text = "";
                    ShopOwnerName.Text = "";
                    PavilionNum.SetTextColor(Color.LightGray);
                }

                SetPavilionVisibility(pavilion, shop, user);
            }

            private void SetPavilionVisibility(Pavilion pavilion, Shop shop, User user)
            {
                if (pavilion.IsChecked)
                    if (pavilion.IsEmpty)
                    {
                        ShopDetails.Visibility = ViewStates.Gone;
                        FilledPavilionFunctional.Visibility = ViewStates.Gone;
                        if (user.AccessLevel != AccessLevels.Visitor)
                        {
                            EmptyPavilionFunctional.Visibility = ViewStates.Visible;
                            if (user.AccessLevel == AccessLevels.Administrator)
                                EveryPavilionFunctional.Visibility = ViewStates.Visible;
                        }
                        else
                        {
                            EmptyPavilionFunctional.Visibility = ViewStates.Gone;
                            EveryPavilionFunctional.Visibility = ViewStates.Gone;
                        }
                    }
                    else
                    {
                        ShopDetails.Visibility = ViewStates.Visible;
                        EmptyPavilionFunctional.Visibility = ViewStates.Gone;
                        if (user.AccessLevel == AccessLevels.Administrator)
                        {
                            FilledPavilionFunctional.Visibility = ViewStates.Visible;
                            EveryPavilionFunctional.Visibility = ViewStates.Visible;
                        }
                        else if (user.AccessLevel == AccessLevels.Businessman && shop.OwnerId == user.Id)
                        {
                            FilledPavilionFunctional.Visibility = ViewStates.Visible;
                            EveryPavilionFunctional.Visibility = ViewStates.Visible;
                        }
                        else
                        {
                            FilledPavilionFunctional.Visibility = ViewStates.Gone;
                            EveryPavilionFunctional.Visibility = ViewStates.Gone;
                        }
                    }
                else
                {
                    ShopDetails.Visibility = ViewStates.Gone;
                    EmptyPavilionFunctional.Visibility = ViewStates.Gone;
                    FilledPavilionFunctional.Visibility = ViewStates.Gone;
                }
            }
        }
    }

    public class PavilionEventsArgs : EventArgs
    {
        public Pavilion pavilion { get; set; }
        public int position { get; set; }
        public PavilionEventsArgs() { }

        public PavilionEventsArgs(Pavilion pavilion, int position)
        {
            this.pavilion = pavilion;
            this.position = position;
        }
    }

}