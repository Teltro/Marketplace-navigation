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
using Android.Widget;

using v4Fragment = Android.Support.V4.App.Fragment;

namespace MarketplaceNavigation.Fragments
{
    public class TextFragment : v4Fragment
    {
        private const string tag = "HelpFragment";

        private string title;
        private string text;

        private TextView HelpTextView;

        public TextFragment(string fragmentTitle, string fragmentText)
        {
            title = fragmentTitle;
            text = fragmentText;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            ((MainActivity)Activity).SupportActionBar.Title = title;

            View view = inflater.Inflate(Resource.Layout.text_layout, container, false);

            HelpTextView = view.FindViewById<TextView>(Resource.Id.textTextView);
            HelpTextView.Text = text;

            return view;
        }
    }
}