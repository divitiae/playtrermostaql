﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Android.Gms.Ads;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Bumptech.Glide.Integration.RecyclerView;
using Bumptech.Glide.Util;
using PlayTube.Activities.Tabbes;
using PlayTube.Adapters;
using PlayTube.Helpers.Ads;
using PlayTube.Helpers.Controller;
using PlayTube.Helpers.Utils;
using PlayTubeClient.Classes.Global;
using PlayTubeClient.Classes.Video;
using PlayTubeClient.RestCalls;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using PlayTube.Activities.Base;

namespace PlayTube.Activities.Library
{
    public class RecentlyWatchedVideosFragment : RecyclerViewDefaultBaseFragment
    {
        #region Variables Basic

        private VideoRowAdapter MAdapter;
        private SwipeRefreshLayout SwipeRefreshLayout;
        private RecyclerView MRecycler;
        private LinearLayoutManager LayoutManager;
        private ViewStub EmptyStateLayout;
        private View Inflated;
        private RecyclerViewOnScrollListener MainScrollEvent;
        private TabbedMainActivity GlobalContext;

        #endregion

        #region General

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            GlobalContext = (TabbedMainActivity)Activity;
            HasOptionsMenu = true;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                // Use this to return your custom view for this Fragment
                View view = inflater.Inflate(Resource.Layout.RecyclerDefaultLayout, container, false);

                //Get Value And Set Toolbar
                InitComponent(view);
                InitToolbar(view);
                SetRecyclerViewAdapters(); 

                SwipeRefreshLayout.Refresh += SwipeRefreshLayoutOnRefresh;
                MAdapter.ItemClick += MAdapterOnItemClick;

                //Get Data Api
                StartApiService();

                AdsGoogle.Ad_Interstitial(Activity);

                return view;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public override void OnLowMemory()
        {
            try
            {
                GC.Collect(GC.MaxGeneration);
                base.OnLowMemory();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        #region Menu

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    try
                    {
                        GlobalContext.FragmentNavigatorBack();
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                    }

                    return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        #endregion

        #region Functions

        private void InitComponent(View view)
        {
            try
            {
                MRecycler = (RecyclerView)view.FindViewById(Resource.Id.recyler);
                EmptyStateLayout = view.FindViewById<ViewStub>(Resource.Id.viewStub);

                SwipeRefreshLayout = (SwipeRefreshLayout)view.FindViewById(Resource.Id.swipeRefreshLayout);
                SwipeRefreshLayout.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight, Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight, Android.Resource.Color.HoloRedLight);
                SwipeRefreshLayout.Refreshing = true;
                SwipeRefreshLayout.Enabled = true;
                SwipeRefreshLayout.SetProgressBackgroundColorSchemeColor(AppSettings.SetTabDarkTheme ? Color.ParseColor("#424242") : Color.ParseColor("#f7f7f7"));

                ShowFacebookAds(view);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void SetRecyclerViewAdapters()
        {
            try
            {
                MAdapter = new VideoRowAdapter(Activity)
                {
                    VideoList = new ObservableCollection<VideoObject>()
                };
                LayoutManager = new LinearLayoutManager(Context);
                MRecycler.SetLayoutManager(LayoutManager);
                MRecycler.HasFixedSize = true;
                MRecycler.SetItemViewCacheSize(10);
                MRecycler.GetLayoutManager().ItemPrefetchEnabled = true;
                var sizeProvider = new FixedPreloadSizeProvider(10, 10);
                var preLoader = new RecyclerViewPreloader<VideoObject>(Activity, MAdapter, sizeProvider, 10);
                MRecycler.AddOnScrollListener(preLoader);
                MRecycler.SetAdapter(MAdapter);

                RecyclerViewOnScrollListener xamarinRecyclerViewOnScrollListener = new RecyclerViewOnScrollListener(LayoutManager);
                MainScrollEvent = xamarinRecyclerViewOnScrollListener;
                MainScrollEvent.LoadMoreEvent += MainScrollEventOnLoadMoreEvent;
                MRecycler.AddOnScrollListener(xamarinRecyclerViewOnScrollListener);
                MainScrollEvent.IsLoading = false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void InitToolbar(View view)
        {
            try
            {
                var toolbar = view.FindViewById<Toolbar>(Resource.Id.toolbar);
                if (toolbar != null)
                {
                    string title = Context.GetString(Resource.String.Lbl_RecentlyWatched_Videos);
                    GlobalContext.SetToolBar(toolbar, title);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
 
        #endregion

        #region Events

          
        private void MAdapterOnItemClick(object sender, VideoRowAdapterClickEventArgs e)
        {
            try
            {
                if (e.Position <= -1) return;

                var item = MAdapter.GetItem(e.Position);
                if (item == null) return;

                GlobalContext.StartPlayVideo(item);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void SwipeRefreshLayoutOnRefresh(object sender, EventArgs e)
        {
            try
            {
                //Get Data Api
                MAdapter.VideoList.Clear();
                MAdapter.NotifyDataSetChanged();

                MainScrollEvent.IsLoading = false;
                StartApiService();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        #region Scroll

        private void MainScrollEventOnLoadMoreEvent(object sender, EventArgs eventArgs)
        {
            try
            {
                //Code get last id where LoadMore >>
                //video "history_id" will be used as offset for load more instead of video "id"
                var item = MAdapter.VideoList.LastOrDefault();
                if (item != null && !string.IsNullOrEmpty(item.HistoryId) && !MainScrollEvent.IsLoading)
                    StartApiService(item.HistoryId);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        #region Load Data Api 

        private void StartApiService(string offset = "0")
        {
            if (!Methods.CheckConnectivity())
                Toast.MakeText(Context, Context.GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
            else
                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => LoadDataAsync(offset) });
        }

        private async Task LoadDataAsync(string offset = "0")
        {
            if (Methods.CheckConnectivity())
            {
                var (apiStatus, respond) = await RequestsAsync.Video.Get_History_Videos_Http("25", offset);
                if (apiStatus != 200 || !(respond is GetVideosListObject result) || result.VideoList == null)
                {
                    if (respond is ErrorObject errorMessage)
                    {
                        if (errorMessage.errors.ErrorText != "Bad Request, Invalid or missing parameter")
                            Methods.DisplayReportResult(Activity, respond);
                    }
                    else
                        Methods.DisplayReportResult(Activity, respond);
                }
                else
                {
                    int countList = MAdapter.VideoList.Count;
                    var respondList = result.VideoList.Count;
                    if (respondList > 0)
                    {
                        result.VideoList = AppTools.ListFilter(result.VideoList);
                        if (countList > 0)
                        {
                            foreach (var item in from item in result.VideoList let check = MAdapter.VideoList.FirstOrDefault(a => a.VideoId == item.VideoId) where check == null select item)
                            {
                                MAdapter.VideoList.Add(item);
                            }

                            Activity.RunOnUiThread(() => { MAdapter.NotifyItemRangeInserted(countList, MAdapter.VideoList.Count - countList); });
                        }
                        else
                        {
                            MAdapter.VideoList = new ObservableCollection<VideoObject>(result.VideoList);
                            Activity.RunOnUiThread(() => { MAdapter.NotifyDataSetChanged(); });
                        }
                    }
                    else
                    {
                        if (MAdapter.VideoList.Count > 10 && !MRecycler.CanScrollVertically(1))
                            Toast.MakeText(Context, Context.GetText(Resource.String.Lbl_NoMoreVideos), ToastLength.Short).Show();
                    }
                }

                GlobalContext.LibrarySynchronizer.AddToRecentlyWatched(MAdapter.VideoList.FirstOrDefault(), MAdapter.VideoList.Count);

                Activity.RunOnUiThread(ShowEmptyPage);
            }
            else
            {
                Inflated = EmptyStateLayout.Inflate();
                EmptyStateInflater x = new EmptyStateInflater();
                x.InflateLayout(Inflated, EmptyStateInflater.Type.NoConnection);
                if (!x.EmptyStateButton.HasOnClickListeners)
                {
                    x.EmptyStateButton.Click += null;
                    x.EmptyStateButton.Click += EmptyStateButtonOnClick;
                }

                Toast.MakeText(Context, Context.GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
            }
            MainScrollEvent.IsLoading = false;
        }

        private void ShowEmptyPage()
        {
            try
            {
                MainScrollEvent.IsLoading = false;
                SwipeRefreshLayout.Refreshing = false;

                if (MAdapter.VideoList.Count > 0)
                {
                    MRecycler.Visibility = ViewStates.Visible;
                    EmptyStateLayout.Visibility = ViewStates.Gone;
                }
                else
                {
                    MRecycler.Visibility = ViewStates.Gone;

                    if (Inflated == null)
                        Inflated = EmptyStateLayout.Inflate();

                    EmptyStateInflater x = new EmptyStateInflater();
                    x.InflateLayout(Inflated, EmptyStateInflater.Type.NoRecentlyWatched);
                    if (!x.EmptyStateButton.HasOnClickListeners)
                    {
                        x.EmptyStateButton.Click += null;
                    }
                    EmptyStateLayout.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception e)
            {
                MainScrollEvent.IsLoading = false;
                SwipeRefreshLayout.Refreshing = false;
                Console.WriteLine(e);
            }
        }

        //No Internet Connection 
        private void EmptyStateButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                StartApiService();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
        #endregion
    }
} 