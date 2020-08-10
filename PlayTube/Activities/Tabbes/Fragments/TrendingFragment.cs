using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using PlayTube.Activities.Chat;
using PlayTube.Helpers.Controller;
using PlayTube.Helpers.Utils;
using PlayTubeClient.RestCalls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Android.Graphics;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Bumptech.Glide.Integration.RecyclerView;
using Bumptech.Glide.Util;
using PlayTube.Activities.Channel.ChannelPopular;
using PlayTube.Activities.Search;
using PlayTube.Activities.Tabbes.Adapters;
using PlayTube.Adapters;
using PlayTube.Helpers.Models;
using PlayTubeClient.Classes.Video;
using Fragment = Android.Support.V4.App.Fragment;
using SearchView = Android.Support.V7.Widget.SearchView;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace PlayTube.Activities.Tabbes.Fragments
{
    public class TrendingFragment : Fragment , View.IOnClickListener, View.IOnFocusChangeListener
    {
        #region Variables Basic

        private TrendingAdapter MAdapter;
        private SwipeRefreshLayout SwipeRefreshLayout;
        private RecyclerView MRecycler;
        private LinearLayoutManager LayoutManager;
        private ViewStub EmptyStateLayout;
        private View Inflated;
        private RecyclerViewOnScrollListener MainScrollEvent;
        private TabbedMainActivity GlobalContext; 
        private ImageView MessageButton;
        private SearchView SearchBox;
        public AllChannelPopularFragment AllChannelPopularFragment;

        #endregion

        #region General

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            GlobalContext = (TabbedMainActivity)Activity;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                // Use this to return your custom view for this Fragment
                View view = inflater.Inflate(Resource.Layout.TTrendingLayout, container, false);

                //Get Value And Set Toolbar
                InitComponent(view);
                InitToolbar(view);
                SetRecyclerViewAdapters();

                SwipeRefreshLayout.Refresh += SwipeRefreshLayoutOnRefresh; 
                MessageButton.Click += MessageButton_Click;
                 
                //Get Data Api
                StartApiService();
                 
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

                MessageButton = (ImageView)view.FindViewById(Resource.Id.Message_icon);

                if (!UserDetails.IsLogin)
                    MessageButton.Visibility = ViewStates.Invisible;
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
                MAdapter = new TrendingAdapter(Activity)
                {
                    TrendingList = new ObservableCollection<Classes.TrendingClass>()
                };
                LayoutManager = new LinearLayoutManager(Context);
                MRecycler.SetLayoutManager(LayoutManager);
                MRecycler.HasFixedSize = true;
                MRecycler.SetItemViewCacheSize(10);
                MRecycler.GetLayoutManager().ItemPrefetchEnabled = true;
                var sizeProvider = new FixedPreloadSizeProvider(10, 10);
                var preLoader = new RecyclerViewPreloader<Classes.TrendingClass>(Activity, MAdapter, sizeProvider, 10);
                MRecycler.AddOnScrollListener(preLoader);
                MRecycler.SetAdapter(MAdapter);
                MAdapter.VideoItemClick += MAdapterOnVideoItemClick;

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
                Toolbar toolbar = view.FindViewById<Toolbar>(Resource.Id.toolbar);
                GlobalContext.SetToolBar(toolbar, "", false);
                GlobalContext.SetSupportActionBar(toolbar);

                SearchBox = view.FindViewById<SearchView>(Resource.Id.TrendingSearchBox);
                SearchBox.SetIconifiedByDefault(false);
                SearchBox.SetOnClickListener(this);
                SearchBox.SetOnSearchClickListener(this);
                SearchBox.SetOnQueryTextFocusChangeListener(this);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Events

        private void MAdapterOnVideoItemClick(object sender, VideoRowAdapterClickEventArgs e)
        {
            try
            {
                if (e.Position <= -1) return;

                var item = MAdapter.GetItem(e.Position);
                if (item.VideoData == null) return;

                GlobalContext.StartPlayVideo(item.VideoData);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Open Chat
        private void MessageButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (UserDetails.IsLogin)
                {
                    Intent intent = new Intent(Activity, typeof(LastChatActivity));
                    StartActivity(intent);
                }
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
                MAdapter.TrendingList.Clear();
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
                if (!Methods.CheckConnectivity())
                    return;

                //Code get last id where LoadMore >>
                var checkList = MAdapter.TrendingList.FirstOrDefault(q => q.Type == ItemType.Video);  
                if (MainScrollEvent != null && checkList?.VideoData != null && !string.IsNullOrEmpty(checkList.VideoData?.Id) && !MainScrollEvent.IsLoading)
                {
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => LoadDataAsync(checkList.VideoData?.Id) });
                } 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        #region Load Data Api 

        private void StartApiService()
        {
            if (!Methods.CheckConnectivity())
                Toast.MakeText(Context, Context.GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
            else
                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { LoadDataChannelAsync ,() => LoadDataAsync()  });
        }

        private async Task LoadDataAsync(string offset = "0")
        {
            if (MainScrollEvent.IsLoading)
                return;

            if (Methods.CheckConnectivity())
            {
                MainScrollEvent.IsLoading = true;
                 
                var (apiStatus, respond) = await RequestsAsync.Video.Get_Trending_Videos_Http(offset ,"25");
                if (apiStatus != 200 || !(respond is GetVideosListObject result) || result.VideoList == null)
                {
                    MainScrollEvent.IsLoading = false; 
                    Methods.DisplayReportResult(Activity, respond);
                }
                else
                {
                    var respondList = result.VideoList.Count;
                    if (respondList > 0)
                    {
                        result.VideoList = AppTools.ListFilter(result.VideoList);

                        foreach (var users in from item in result.VideoList let check = MAdapter.TrendingList.FirstOrDefault(a => a.VideoData?.VideoId == item.VideoId) where check == null select new Classes.TrendingClass()
                        {
                            Id = Convert.ToInt32(item.Id),
                            VideoData = item,
                            Type = ItemType.Video
                        })
                        {
                            MAdapter.TrendingList.Add(users);
                        }
                    }
                    else
                    {
                        if (MAdapter.TrendingList.Count > 10 && !MRecycler.CanScrollVertically(1))
                            Toast.MakeText(Context, Context.GetText(Resource.String.Lbl_NoMoreVideos), ToastLength.Short).Show(); 
                    } 
                }
                 
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

        private async Task LoadDataChannelAsync()
        {
            if (!UserDetails.IsLogin)
                return;

            if (Methods.CheckConnectivity())
            {
                var (apiStatus, respond) = await RequestsAsync.Video.PopularChannels_Http();
                if (apiStatus != 200 || !(respond is GetPopularChannelsObject result) || result.Channels == null)
                {
                    MainScrollEvent.IsLoading = false;
                    Methods.DisplayReportResult(Activity, respond);
                }
                else
                {
                    var respondList = result.Channels.Count;
                    if (respondList > 0)
                    {
                        var checkList = MAdapter.TrendingList.FirstOrDefault(q => q.Type == ItemType.Channel);
                        if (checkList == null)
                        {
                            var channel = new Classes.TrendingClass()
                            {
                                Id = 200,
                                ChannelList = new List<GetPopularChannelsObject.Channel>(),
                                Type = ItemType.Channel
                            };

                            foreach (var item in from item in result.Channels let check = channel.ChannelList.FirstOrDefault(a => a.UserData?.Id == item.UserData?.Id) where check == null select item)
                            {
                                channel.ChannelList.Add(item);
                            }

                            MAdapter.TrendingList.Insert(0, channel);
                        }
                        else
                        {
                            foreach (var item in from item in result.Channels let check = checkList.ChannelList.FirstOrDefault(a => a.UserData?.Id == item.UserData?.Id) where check == null select item)
                            {
                                checkList.ChannelList.Add(item);
                            }
                        } 
                    } 
                }

                //Activity.RunOnUiThread(ShowEmptyPage);
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
        }
         
        private void ShowEmptyPage()
        {
            try
            {
                MainScrollEvent.IsLoading = false;
                SwipeRefreshLayout.Refreshing = false;

                if (MAdapter.TrendingList.Count > 0)
                {
                    MRecycler.Visibility = ViewStates.Visible;
                    EmptyStateLayout.Visibility = ViewStates.Gone;

                    var checkList = MAdapter.TrendingList.FirstOrDefault(q => q.Type == ItemType.Video);
                    if (checkList != null)
                    {
                        var emptyStateChecker = MAdapter.TrendingList.FirstOrDefault(a => a.Type == ItemType.EmptyPage);
                        if (emptyStateChecker != null)
                        {
                            MAdapter.TrendingList.Remove(emptyStateChecker);
                        }
                    }

                    MAdapter.NotifyDataSetChanged();
                }
                else
                {
                    var emptyStateChecker = MAdapter.TrendingList.FirstOrDefault(q => q.Type == ItemType.EmptyPage);
                    if (emptyStateChecker == null)
                    {
                        MAdapter.TrendingList.Add(new Classes.TrendingClass()
                        {
                            Id = 300,
                            Type = ItemType.EmptyPage
                        });
                        MAdapter.NotifyDataSetChanged();

                        EmptyStateLayout.Visibility = ViewStates.Gone;
                    } 
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

        #region Search View

        public void OnClick(View v)
        {

            if (v.Id != SearchBox.Id)
            {
                return;
            }

            SearchFragment searchFragment = new SearchFragment();
            GlobalContext.FragmentBottomNavigator.DisplayFragment(searchFragment);
            SearchBox.ClearFocus();

        }

        public void OnFocusChange(View v, bool hasFocus)
        {

            if (v.Id != SearchBox.Id || !hasFocus)
            {
                return;
            }

            SearchFragment searchFragment = new SearchFragment();
            GlobalContext.FragmentBottomNavigator.DisplayFragment(searchFragment);
            SearchBox.ClearFocus();

        }


        #endregion
    }
}