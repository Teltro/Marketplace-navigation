using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace MarketplaceNavigation
{
    public class DividerItemDecoration : RecyclerView.ItemDecoration
    {
        private static int[] ATTRS = new int[] { Android.Resource.Attribute.ListDivider }; 
        private Drawable divider;
        public DividerItemDecoration(Context context)
        {
            var styledAttributes = context.ObtainStyledAttributes(ATTRS);
            divider = styledAttributes.GetDrawable(0);
            styledAttributes.Recycle();
        }

        public override void OnDraw(Canvas c, RecyclerView parent, RecyclerView.State state)
        {
            int left = parent.PaddingLeft;
            int right = parent.Width - parent.PaddingRight;

            int childCount = parent.ChildCount;
            for(int i = 0; i < childCount; i++)
            {
                View child = parent.GetChildAt(i);
                RecyclerView.LayoutParams parametres = (RecyclerView.LayoutParams)child.LayoutParameters;
                int top = child.Bottom + parametres.BottomMargin;
                int bottom = top + divider.IntrinsicHeight;
                divider.SetBounds(left, top, right, bottom);
                divider.Draw(c);
            }
        }
    }
}