using System;
using Android.Gms.Ads;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using PlayTube.Helpers.Ads;
using Fragment = Android.Support.V4.App.Fragment;

namespace PlayTube.Activities.Base
{
    public class RecyclerViewDefaultBaseFragment : Fragment
    {
        protected void ShowGoogleAds(View view, RecyclerView recyclerView)
        {
            try
            {
                var adView = view.FindViewById<AdView>(Resource.Id.adView);
                AdsGoogle.InitAdView(adView, recyclerView);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected void ShowFacebookAds(View view)
        {
            try
            {
                var mRecycler = (RecyclerView)view.FindViewById(Resource.Id.recyler);
                var containerLayout = view.FindViewById<LinearLayout>(Resource.Id.bannerContainer);
                AdsFacebook.InitAdView(Activity, containerLayout, mRecycler);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}