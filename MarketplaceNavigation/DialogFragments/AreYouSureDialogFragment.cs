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

using v4DialogFragment = Android.Support.V4.App.DialogFragment;

namespace MarketplaceNavigation.DialogFragments
{
    public class AreYouSureDialogFragment : v4DialogFragment
    {
        public event EventHandler<bool> DialogResultHandler;

        private string message;

        public AreYouSureDialogFragment(string message)
        {
            this.message = message;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            var builder = new AlertDialog.Builder(Activity);
            builder.SetTitle("Warning");
            builder.SetMessage(message);
            builder.SetCancelable(false);

            builder.SetPositiveButton("OK", new DialogFragmentClickListener<bool>(DialogResultHandler, true));
            builder.SetNegativeButton("CANCEL", new DialogFragmentClickListener<bool>(DialogResultHandler, false));

            return builder.Create();
        }
    }
}