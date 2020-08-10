using System;
using Android.App;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Bumptech.Glide.Integration.RecyclerView;
using Bumptech.Glide.Util;

namespace PlayTube.Helpers.Utils
{
    public class TemplateRecyclerInflater
    {
        public RelativeLayout MainLinear;
        public TextView TitleText, MoreText;
        public RecyclerView Recyler;
        public dynamic LayoutManager;

        public enum TypeLayoutManager
        {
            LinearLayoutManagerVertical,
            LinearLayoutManagerHorizontal,
            GridLayoutManagerVertical,
            GridLayoutManagerHorizontal,
            StaggeredGridLayoutManagerVertical,
            StaggeredGridLayoutManagerHorizontal
        }

        private void InitComponent(View inflated)
        {
            try
            {
                MainLinear = inflated.FindViewById<RelativeLayout>(Resource.Id.mainLinear);
                TitleText = inflated.FindViewById<TextView>(Resource.Id.textTitle);
                MoreText = inflated.FindViewById<TextView>(Resource.Id.textMore);
                Recyler = inflated.FindViewById<RecyclerView>(Resource.Id.recyler);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void InflateLayout<T>(Activity activity, View inflated, dynamic mAdapter, TypeLayoutManager manager, int spanCount = 0, bool showTitle = true, string titleText = "", bool showMore = false) where T : class
        {
            try
            {
                InitComponent(inflated);

                if (showTitle)
                {
                    MainLinear.Visibility = ViewStates.Visible;

                    if (string.IsNullOrEmpty(titleText))
                    {
                        TitleText.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        TitleText.Text = titleText; 
                    }

                    if (showMore)
                        MoreText.Visibility = ViewStates.Visible;
                }
                else
                    MainLinear.Visibility = ViewStates.Gone;

                switch (manager)
                {
                    case TypeLayoutManager.LinearLayoutManagerHorizontal:
                        LayoutManager = new LinearLayoutManager(activity, LinearLayoutManager.Horizontal, false);
                        Recyler.NestedScrollingEnabled = false;
                        break;
                    case TypeLayoutManager.LinearLayoutManagerVertical:
                        LayoutManager = new LinearLayoutManager(activity);
                        break;
                    case TypeLayoutManager.GridLayoutManagerVertical:
                        LayoutManager = new GridLayoutManager(activity, spanCount);
                        break;
                    case TypeLayoutManager.GridLayoutManagerHorizontal:
                        LayoutManager = new GridLayoutManager(activity, spanCount, LinearLayoutManager.Horizontal, false);
                        Recyler.NestedScrollingEnabled = false;
                        break;
                    case TypeLayoutManager.StaggeredGridLayoutManagerVertical:
                        LayoutManager = new StaggeredGridLayoutManager(spanCount, LinearLayoutManager.Vertical);
                        break;
                    case TypeLayoutManager.StaggeredGridLayoutManagerHorizontal:
                        LayoutManager = new StaggeredGridLayoutManager(spanCount, LinearLayoutManager.Horizontal);
                        Recyler.NestedScrollingEnabled = false;
                        break;
                    default:
                        LayoutManager = new LinearLayoutManager(activity);
                        break;
                }

                Recyler.SetLayoutManager(LayoutManager);
                Recyler.SetItemViewCacheSize(20);
                Recyler.HasFixedSize = true;
                Recyler.SetItemViewCacheSize(10);
                Recyler.GetLayoutManager().ItemPrefetchEnabled = true;

                var sizeProvider = new FixedPreloadSizeProvider(10, 10);
                var preLoader = new RecyclerViewPreloader<T>(activity, mAdapter, sizeProvider, 10);
                Recyler.AddOnScrollListener(preLoader);

                Recyler.SetAdapter(mAdapter);

                if (Recyler.Visibility != ViewStates.Visible)
                    Recyler.Visibility = ViewStates.Visible;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void SetTextViewDrawableColor(TextView textView, string color)
        {
            foreach (Drawable drawable in textView.GetCompoundDrawables())
            {
                if (drawable != null)
                    drawable.SetColorFilter(new PorterDuffColorFilter(Color.ParseColor(color), PorterDuff.Mode.SrcIn));
            }
        }
    }
}