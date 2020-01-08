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
    public class CallToAutorisationDialogFragment : v4DialogFragment
    {
        public event EventHandler<bool> DialogResultHandler;
        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            var builder = new AlertDialog.Builder(Activity);
            builder.SetTitle("Autorisation");
            builder.SetMessage("You can register to get more opportunities");
            builder.SetCancelable(false);

            builder.SetPositiveButton("Autorisation", new DialogFragmentClickListener<bool>(DialogResultHandler, true));
            builder.SetNegativeButton("Log in as visitor", new DialogFragmentClickListener<bool>(DialogResultHandler, false));

            return builder.Create();
        }
    }
}