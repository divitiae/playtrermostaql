using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AFollestad.MaterialDialogs;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Bumptech.Glide.Integration.RecyclerView;
using Bumptech.Glide.Util;
using Java.Lang;
using PlayTube.Activities.Tabbes;
using PlayTube.Activities.Videos;
using PlayTube.Adapters;
using PlayTube.Helpers.Ads;
using PlayTube.Helpers.Controller;
using PlayTube.Helpers.Fonts;
using PlayTube.Helpers.Models;
using PlayTube.Helpers.Utils;
using PlayTubeClient.Classes.Global;
using PlayTubeClient.Classes.Video;
using PlayTubeClient.RestCalls;
using Exception = System.Exception;
using Fragment = Android.Support.V4.App.Fragment;
using SearchView = Android.Support.V7.Widget.SearchView;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace PlayTube.Activities.Search
{
    public class SearchFragment : Fragment ,MaterialDialog.IListCallback, MaterialDialog.ISingleButtonCallback
    { 
        #region Variables Basic

        private CategoryAdapter MCatAdapter;
        private VideoRowAdapter MAdapter;
        private SwipeRefreshLayout SwipeRefreshLayout;
        private RecyclerView MRecycler, CatRecycler;
        private LinearLayoutManager LayoutManager, CatLayoutManager;
        private ViewStub EmptyStateLayout;
        private View Inflated;
        private RecyclerViewOnScrollListener MainScrollEvent;
        private TabbedMainActivity GlobalContext;
        private SearchView SearchBox;
        private string SearchKey = "" , DateId = "";
        private TextView FilterButton;
        private AdsGoogle.AdMobRewardedVideo RewardedVideoAd;

        #endregion

        #region General

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            GlobalContext = TabbedMainActivity.GetInstance();
            HasOptionsMenu = true;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                // Use this to return your custom view for this Fragment
                View view = inflater.Inflate(Resource.Layout.Search_Layout, container, false);

                //Get Value And Set Toolbar
                InitComponent(view);
                InitToolbar(view);
                SetRecyclerViewAdapters();
                
                SwipeRefreshLayout.Refresh += SwipeRefreshLayoutOnRefresh;
                MAdapter.ItemClick += MAdapterOnItemClick;
                MCatAdapter.ItemClick += MCatAdapterOnItemClick;
                FilterButton.Click += FilterButtonOnClick;

                RewardedVideoAd = AdsGoogle.Ad_RewardedVideo(Activity);

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


        public override void OnResume()
        {
            try
            {
                base.OnResume();
                RewardedVideoAd?.OnResume(Context);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override void OnPause()
        {
            try
            {
                base.OnPause();
                RewardedVideoAd?.OnPause(Context);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override void OnDestroy()
        {
            try
            {
                RewardedVideoAd?.OnDestroy(Context);

                base.OnDestroy();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
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
                CatRecycler = (RecyclerView)view.FindViewById(Resource.Id.catigoriesRecyler);

                MRecycler = (RecyclerView)view.FindViewById(Resource.Id.recyler);
                EmptyStateLayout = view.FindViewById<ViewStub>(Resource.Id.viewStub);

                SwipeRefreshLayout = (SwipeRefreshLayout)view.FindViewById(Resource.Id.swipeRefreshLayout);
                SwipeRefreshLayout.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight, Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight, Android.Resource.Color.HoloRedLight);
                SwipeRefreshLayout.Refreshing = false;
                SwipeRefreshLayout.Enabled = false;
                SwipeRefreshLayout.SetProgressBackgroundColorSchemeColor(AppSettings.SetTabDarkTheme ? Color.ParseColor("#424242") : Color.ParseColor("#f7f7f7"));
                  
                FilterButton = (TextView)view.FindViewById(Resource.Id.filter_icon);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, FilterButton, IonIconsFonts.AndroidOptions);
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

                //======== Category =======
                CatLayoutManager = new LinearLayoutManager(Activity);
                CatRecycler.SetLayoutManager(CatLayoutManager);
                MCatAdapter = new CategoryAdapter(Activity)
                {
                    CategoryList = new ObservableCollection<Classes.Category>()
                };
                CatRecycler.SetAdapter(MCatAdapter);

                if (MCatAdapter.CategoryList.Count == 0 && CategoriesController.ListCategories.Count > 0)
                {
                    MCatAdapter.CategoryList = CategoriesController.ListCategories;
                    MCatAdapter.NotifyDataSetChanged();

                    CatRecycler.Visibility = ViewStates.Visible;
                    MRecycler.Visibility = ViewStates.Gone;
                }
                else
                {
                    CatRecycler.Visibility = ViewStates.Gone;
                    MRecycler.Visibility = ViewStates.Visible;

                    Inflated = EmptyStateLayout.Inflate();
                    EmptyStateInflater x = new EmptyStateInflater();
                    x.InflateLayout(Inflated, EmptyStateInflater.Type.NoSearchResult);
                    if (!x.EmptyStateButton.HasOnClickListeners)
                    {
                        x.EmptyStateButton.Click += null;
                    }
                } 
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
                    GlobalContext.SetToolBar(toolbar, "");

                    SearchBox = view.FindViewById<SearchView>(Resource.Id.searchBox);
                    SearchBox.SetIconifiedByDefault(false);
                    SearchBox.OnActionViewExpanded();
                    SearchBox.Iconified = false;
                    SearchBox.QueryTextSubmit += SearchViewOnQueryTextSubmit;
                    SearchBox.QueryTextChange += SearchViewOnQueryTextChange;
                    SearchBox.ClearFocus();

                    //Change text colors
                    var editText = (EditText)SearchBox.FindViewById(Resource.Id.search_src_text);
                    editText.SetHintTextColor(Color.White);
                    editText.SetTextColor(Color.White);

                    //Remove Icon Search
                    ImageView searchViewIcon = (ImageView)SearchBox.FindViewById(Resource.Id.search_mag_icon);
                    ViewGroup linearLayoutSearchView = (ViewGroup)searchViewIcon.Parent;
                    linearLayoutSearchView.RemoveView(searchViewIcon);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        #endregion

        #region Events

        // Open Video  
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
         
        private void SearchViewOnQueryTextSubmit(object sender, SearchView.QueryTextSubmitEventArgs e)
        {
            try
            {
                SearchKey = e.NewText;
                if (SearchKey.Length >= 4)
                {
                    CatRecycler.Visibility = ViewStates.Gone;
                    MRecycler.Visibility = ViewStates.Visible;

                    MAdapter.VideoList.Clear();
                    SwipeRefreshLayout.Refreshing = true;
                    SwipeRefreshLayout.Enabled = true;

                    StartApiService();

                    e.Handled = true;
                    SearchBox.ClearFocus(); 
                }
                else
                {
                    Toast.MakeText(Activity, GetText(Resource.String.Lbl_The_search_term), ToastLength.Short).Show();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void SearchViewOnQueryTextChange(object sender, SearchView.QueryTextChangeEventArgs e)
        {
            try
            {
                SearchKey = e.NewText;

                if (SearchKey != "")
                    return;

                CatRecycler.Visibility = ViewStates.Visible;
                MRecycler.Visibility = ViewStates.Gone;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Event get video by Category
        private void MCatAdapterOnItemClick(object sender, CategoryAdapterClickEventArgs e)
        {
            try
            {
                if (e.Position <= -1) return;

                var item = MCatAdapter.GetItem(e.Position);
                if (item == null) return;

                Bundle bundle = new Bundle();
                bundle.PutString("CatId", item.Id);
                bundle.PutString("CatName", item.Name);

                VideosByCategoryFragment videoViewerFragment = new VideosByCategoryFragment()
                {
                    Arguments = bundle
                };

                GlobalContext.FragmentBottomNavigator.DisplayFragment(videoViewerFragment);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        // Filter >> Get video by date (last_hour','today','this_week','this_month','this_year)
        private void FilterButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                var dialogList = new MaterialDialog.Builder(Activity).Theme(AppSettings.SetTabDarkTheme ? Theme.Dark : Theme.Light);

                var arrayAdapter = new List<string>
                {
                    Activity.GetText(Resource.String.Lbl_LastHour),
                    Activity.GetText(Resource.String.Lbl_Today),
                    Activity.GetText(Resource.String.Lbl_ThisWeek),
                    Activity.GetText(Resource.String.Lbl_ThisMonth),
                    Activity.GetText(Resource.String.Lbl_ThisYear)
                };

                dialogList.Title(GetText(Resource.String.Lbl_Date));
                dialogList.Items(arrayAdapter);
                dialogList.NegativeText(GetText(Resource.String.Lbl_Close)).OnNegative(this);
                dialogList.AlwaysCallSingleChoiceCallback();
                dialogList.ItemsCallback(this).Build().Show();
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
                var item = MAdapter.VideoList.LastOrDefault();
                if (item != null && !string.IsNullOrEmpty(item.Id) && !MainScrollEvent.IsLoading)
                    StartApiService(item.Id);
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
            if (MainScrollEvent.IsLoading)
                return;

            if (Methods.CheckConnectivity())
            {
                MainScrollEvent.IsLoading = true;
                int countList = MAdapter.VideoList.Count;

                var (apiStatus, respond) = await RequestsAsync.Video.Search_Videos_Http(SearchKey,"25", offset, DateId);
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
                    x.InflateLayout(Inflated, EmptyStateInflater.Type.NoSearchResult);
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
         
        #region MaterialDialog

        public void OnSelection(MaterialDialog p0, View p1, int itemId, ICharSequence itemString)
        {
            try
            {  
                if (itemString.ToString() == Activity.GetString(Resource.String.Lbl_LastHour))
                {
                    DateId = "last_hour";
                }
                else if (itemString.ToString() == Activity.GetString(Resource.String.Lbl_Today))
                {
                    DateId = "today";
                } 
                else if (itemString.ToString() == Activity.GetString(Resource.String.Lbl_ThisWeek))
                {
                    DateId = "this_week";
                } 
                else if (itemString.ToString() == Activity.GetString(Resource.String.Lbl_ThisMonth))
                {
                    DateId = "this_month";
                } 
                else if (itemString.ToString() == Activity.GetString(Resource.String.Lbl_ThisYear))
                {
                    DateId = "this_year";
                } 

                MAdapter.VideoList.Clear();
                MAdapter.NotifyDataSetChanged();

                SwipeRefreshLayout.Refreshing = true; 
                MainScrollEvent.IsLoading = false;

                StartApiService();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnClick(MaterialDialog p0, DialogAction p1)
        {
            try
            {
                if (p1 == DialogAction.Positive)
                {
                }
                else if (p1 == DialogAction.Negative)
                {
                    p0.Dismiss();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion


    }
} 