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

namespace MarketplaceNavigation.DialogFragments
{
    public class DialogFragmentClickListener<ResultType> : Java.Lang.Object, IDialogInterfaceOnClickListener
    {
        private event EventHandler<ResultType> DialogResultHandler;
        private ResultType result;
        public DialogFragmentClickListener(EventHandler<ResultType> dialogResultHandler, ResultType result)
        {
            this.DialogResultHandler = dialogResultHandler;
            this.result = result;
        }

        public void OnClick(IDialogInterface dialog, int which)
        {
            DialogResultHandler?.Invoke(this, result);
        }
    }
}
