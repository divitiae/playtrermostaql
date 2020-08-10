using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Gms.Ads;
using Android.Gms.Ads.DoubleClick;
using Android.Gms.Ads.Formats;
using Android.Gms.Ads.Reward;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using PlayTube.Helpers.Models;
using PlayTube.Helpers.Utils;
using Exception = System.Exception;
using Object = Java.Lang.Object;

namespace PlayTube.Helpers.Ads
{
    public static class AdsGoogle
    {
        private static int CountInterstitial;
        private static int CountRewarded;

        #region Interstitial

        private class AdMobInterstitial
        {
            private InterstitialAd Ad;

            public void ShowAd(Context context)
            {
                try
                {
                    Ad = new InterstitialAd(context) { AdUnitId = AppSettings.AdInterstitialKey };

                    var listener = new InterstitialAdListener(Ad);
                    listener.OnAdLoaded();
                    Ad.AdListener = listener;

                    var requestBuilder = new AdRequest.Builder();
                    requestBuilder.AddTestDevice(UserDetails.AndroidId);
                    Ad.LoadAd(requestBuilder.Build());
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }
        }

        private class InterstitialAdListener : AdListener
        {
            private readonly InterstitialAd Ad;

            public InterstitialAdListener(InterstitialAd ad)
            {
                Ad = ad;
            }

            public override void OnAdLoaded()
            {
                base.OnAdLoaded();

                if (Ad.IsLoaded)
                    Ad.Show();
            }
        }


        public static void Ad_Interstitial(Activity context)
        {
            try
            {
                if (AppSettings.ShowAdMobInterstitial)
                {
                    if (CountInterstitial == AppSettings.ShowAdMobInterstitialCount)
                    {
                        CountInterstitial = 0;
                        AdMobInterstitial ads = new AdMobInterstitial();
                        ads.ShowAd(context);
                    }
                    else
                    {
                        AdsFacebook.InitInterstitial(context);
                    }

                    CountInterstitial++;
                }
                else
                {
                    AdsFacebook.InitInterstitial(context);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        #region Native

        private class AdMobNative : Object, UnifiedNativeAd.IOnUnifiedNativeAdLoadedListener
        {
            private TemplateView Template;
            private Activity Context;
            public void ShowAd(Activity context, TemplateView template = null)
            {
                try
                {
                    Context = context;

                    Template = template ?? Context.FindViewById<TemplateView>(Resource.Id.my_template);
                    Template.Visibility = ViewStates.Gone;

                    if (AppSettings.ShowAdMobNative)
                    {
                        AdLoader.Builder builder = new AdLoader.Builder(Context, AppSettings.AdAdMobNativeKey);
                        builder.ForUnifiedNativeAd(this);
                        VideoOptions videoOptions = new VideoOptions.Builder()
                            .SetStartMuted(true)
                            .Build();
                        NativeAdOptions adOptions = new NativeAdOptions.Builder()
                            .SetVideoOptions(videoOptions)
                            .Build();

                        builder.WithNativeAdOptions(adOptions);

                        AdLoader adLoader = builder.WithAdListener(new AdListener()).Build();
                        adLoader.LoadAd(new AdRequest.Builder().Build());
                    }
                    else
                    {
                        Template.Visibility = ViewStates.Gone;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            public void OnUnifiedNativeAdLoaded(UnifiedNativeAd ad)
            {
                try
                {
                    NativeTemplateStyle styles = new NativeTemplateStyle.Builder().Build();

                    if (Template.GetTemplateTypeName() == TemplateView.BigTemplate)
                    {
                        Template.PopulateUnifiedNativeAdView(ad);
                    }
                    else if (Template.GetTemplateTypeName() == TemplateView.NativeContentAd)
                    {
                        Template.NativeContentAdView(ad);
                    }
                    else if (Template.GetTemplateTypeName() == TemplateView.NativeAppInstallAd)
                    {
                        Template.NativeAppInstallAdView(ad);
                    }
                    else
                    {
                        Template.SetStyles(styles);
                        Template.SetNativeAd(ad);
                    }

                    Template.Visibility = ViewStates.Visible;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public static void Ad_AdMobNative(Activity context, TemplateView template = null)
        {
            try
            {
                if (AppSettings.ShowAdMobNative)
                {
                    AdMobNative ads = new AdMobNative();
                    ads.ShowAd(context, template);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        //Rewarded Video >>
        //===================================================

        #region Rewarded

        public class AdMobRewardedVideo : AdListener, IRewardedVideoAdListener
        {
            private IRewardedVideoAd Rad;

            public void ShowAd(Context context)
            {
                try
                {
                    // Use an activity context to get the rewarded video instance.
                    Rad = MobileAds.GetRewardedVideoAdInstance(context);
                    Rad.UserId = context.GetString(Resource.String.admob_app_id);
                    Rad.RewardedVideoAdListener = this;
                    AdRequest adRequest = new AdRequest.Builder().Build();
                    Rad.LoadAd(AppSettings.AdRewardVideoKey, adRequest);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }

            public void OnRewarded(IRewardItem reward)
            {
                //Toast.MakeText(Application.Context, "onRewarded! currency: " + reward.Type + "  amount: " + reward.Amount , ToastLength.Short).Show();
            }


            public void OnRewardedVideoAdClosed()
            {

            }

            public void OnRewardedVideoAdFailedToLoad(int errorCode)
            {
                //Toast.MakeText(Application.Context, "No ads currently available", ToastLength.Short).Show();
            }

            public void OnRewardedVideoAdLeftApplication()
            {

            }

            public void OnRewardedVideoAdLoaded()
            {
                try
                {
                    if (Rad != null && Rad.IsLoaded)
                        Rad.Show();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            public void OnRewardedVideoAdOpened()
            {

            }

            public void OnRewardedVideoCompleted()
            {

            }

            public void OnRewardedVideoStarted()
            {

            }

            public void OnResume(Context context)
            {
                Rad?.Resume(context);

            }

            public void OnPause(Context context)
            {
                Rad?.Pause(context);

            }

            public void OnDestroy(Context context)
            {
                Rad?.Destroy(context);
            }
        }

        public static AdMobRewardedVideo Ad_RewardedVideo(Activity context)
        {
            try
            {
                if (AppSettings.ShowAdMobRewardVideo)
                {
                    if (CountRewarded == AppSettings.ShowAdMobRewardedVideoCount)
                    {
                        CountRewarded = 0;
                        AdMobRewardedVideo ads = new AdMobRewardedVideo();
                        ads.ShowAd(context);
                        return ads;
                    }
                    else
                    {
                        AdsFacebook.InitRewardVideo(context);
                    }

                    CountRewarded++;
                }
                else
                {
                    AdsFacebook.InitRewardVideo(context);
                }
                return null;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return null;
            }
        }

        #endregion

        public static void InitAdView(AdView mAdView, RecyclerView mRecycler)
        {
            try
            {
                if (mAdView == null)
                    return;

                if (AppSettings.ShowAdMobBanner)
                {
                    mAdView.Visibility = ViewStates.Visible;
                    var adRequest = new AdRequest.Builder();
                    adRequest.AddTestDevice(UserDetails.AndroidId);
                    mAdView.LoadAd(adRequest.Build());
                    mAdView.AdListener = new MyAdListener(mAdView, mRecycler);
                }
                else
                {
                    mAdView.Pause();
                    mAdView.Visibility = ViewStates.Gone;
                    if (mRecycler != null) Methods.SetMargin(mRecycler, 0, 0, 0, 0);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void InitPublisherAdView(PublisherAdView mAdView)
        {
            try
            {
                if (mAdView == null)
                    return;

                if (AppSettings.ShowAdMobBanner)
                {
                    mAdView.Visibility = ViewStates.Visible;
                    var adRequest = new PublisherAdRequest.Builder();
                    adRequest.AddTestDevice(UserDetails.AndroidId);
                    mAdView.AdListener = new MyPublisherAdViewListener(mAdView);
                    mAdView.LoadAd(adRequest.Build());
                }
                else
                {
                    mAdView.Pause();
                    mAdView.Visibility = ViewStates.Gone;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        private class MyAdListener : AdListener
        {
            private readonly AdView MAdView;
            private readonly RecyclerView MRecycler;
            public MyAdListener(AdView mAdView, RecyclerView mRecycler)
            {
                MAdView = mAdView;
                MRecycler = mRecycler;
            }

            public override void OnAdFailedToLoad(int p0)
            {
                try
                {
                    MAdView.Visibility = ViewStates.Gone;
                    if (MRecycler != null) Methods.SetMargin(MRecycler, 0, 0, 0, 0);
                    base.OnAdFailedToLoad(p0);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            public override void OnAdLoaded()
            {
                try
                {
                    MAdView.Visibility = ViewStates.Visible;

                    Resources r = Application.Context.Resources;
                    int px = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, MAdView.AdSize.Height, r.DisplayMetrics);
                    if (MRecycler != null) Methods.SetMargin(MRecycler, 0, 0, 0, px);

                    base.OnAdLoaded();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        private class MyPublisherAdViewListener : AdListener
        {
            private readonly PublisherAdView MAdView;
            public MyPublisherAdViewListener(PublisherAdView mAdView)
            {
                MAdView = mAdView;
            }

            public override void OnAdFailedToLoad(int p0)
            {
                try
                {
                    MAdView.Visibility = ViewStates.Gone;
                    base.OnAdFailedToLoad(p0);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            public override void OnAdLoaded()
            {
                try
                {
                    MAdView.Visibility = ViewStates.Visible;
                    base.OnAdLoaded();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}