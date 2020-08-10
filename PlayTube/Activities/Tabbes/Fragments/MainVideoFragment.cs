using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Bumptech.Glide.Integration.RecyclerView;
using Bumptech.Glide.Util;
using Liaoinstan.SpringViewLib.Widgets;
using PlayTube.Activities.Default;
using PlayTube.Activities.Tabbes.Adapters;
using PlayTube.Adapters;
using PlayTube.Helpers.CacheLoaders;
using PlayTube.Helpers.Controller;
using PlayTube.Helpers.Models;
using PlayTube.Helpers.Utils;
using PlayTube.SQLite;
using PlayTubeClient.Classes.Global;
using PlayTubeClient.Classes.Video;
using PlayTubeClient.RestCalls;
using Fragment = Android.Support.V4.App.Fragment;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace PlayTube.Activities.Tabbes.Fragments
{
    public class MainVideoFragment : Fragment, SpringView.IOnFreshListener, AppBarLayout.IOnOffsetChangedListener
    {
        #region  Variables Basic

        public MainVideoAdapter MAdapter;
        private SpringView SwipeRefreshLayout;
        private RecyclerView MRecycler;
        private LinearLayoutManager LayoutManager;
        private ViewStub EmptyStateLayout;
        private View Inflated;
        private RecyclerViewOnScrollListener MainScrollEvent;
        private TabbedMainActivity GlobalContext;
        private AppBarLayout MainAppBarLayout;
        private CollapsingToolbarLayout CollapsingToolbarLayout;
        private ImageView ToolbarLogo;
        private ViewGroup ToolbarLogoLinearLayout;
        private ViewPager ViewPagerView;
        private LinearLayout LoadingLinear;
        private RelativeLayout MainAlert;
        private readonly List<VideoObject> OtherVideosList = new List<VideoObject>();
        private bool IsShowing = true;
        private int ScrollRange = -1;

        #endregion

        #region General

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            HasOptionsMenu = true;
            // Create your fragment here
            GlobalContext = (TabbedMainActivity)Activity;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                View view = inflater.Inflate(Resource.Layout.TMainVideoLayout, container, false);

                InitComponent(view);
                InitToolbar(view);
                SetRecyclerViewAdapters();

                GetNotInterestedVideos();

                StartApiService();

                GlobalContext.GetDataOneSignal();
                return view;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
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
                 
                LoadingLinear = (LinearLayout)view.FindViewById(Resource.Id.Loading_LinearLayout);
                LoadingLinear.Visibility = ViewStates.Visible;

                ViewPagerView = view.FindViewById<ViewPager>(Resource.Id.viewpager2);
                
                MainAppBarLayout = view.FindViewById<AppBarLayout>(Resource.Id.mainAppBarLayout);
                MainAppBarLayout.SetExpanded(false);

                CollapsingToolbarLayout = (CollapsingToolbarLayout)view.FindViewById(Resource.Id.collapsingToolbar);
                ToolbarLogo = view.FindViewById<ImageView>(Resource.Id.ToolbarLogo);
                ToolbarLogoLinearLayout = view.FindViewById<LinearLayout>(Resource.Id.ToolbarLogoLinearLayout);

                if (AppSettings.ShowAppLogoInToolbar)
                {
                    CollapsingToolbarLayout.SetTitle(" ");
                    MainAppBarLayout.AddOnOffsetChangedListener(this);

                    ToolbarLogoLinearLayout.BringToFront();
                    ViewCompat.SetTranslationZ(ToolbarLogoLinearLayout, 100);
                    ((View)ToolbarLogoLinearLayout.Parent).RequestLayout();
                }
                else
                {
                    CollapsingToolbarLayout.Title = AppSettings.ApplicationName;
                }

                SwipeRefreshLayout = (SpringView)view.FindViewById(Resource.Id.material_style_ptr_frame);
                SwipeRefreshLayout.SetType(SpringView.Type.Overlap);
                SwipeRefreshLayout.Header = new Helpers.PullSwipeStyles.DefaultHeader(Activity);
                SwipeRefreshLayout.Footer = new Helpers.PullSwipeStyles.DefaultFooter(Activity);
                SwipeRefreshLayout.Enable = true;
                SwipeRefreshLayout.SetListener(this);

                MainAlert = (RelativeLayout)view.FindViewById(Resource.Id.mainAlert); 
                MainAlert.Visibility = !UserDetails.IsLogin ? ViewStates.Visible : ViewStates.Gone;
                MainAlert.Click += MainAlertOnClick;
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
                GlobalContext.SetToolBar(toolbar, AppSettings.ApplicationName, false);
                GlobalContext.SetSupportActionBar(toolbar); 
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
                MAdapter = new MainVideoAdapter(Activity)
                {
                    MainVideoList = new ObservableCollection<Classes.MainVideoClass>()
                };
                LayoutManager = new LinearLayoutManager(Context);
                MRecycler.SetLayoutManager(LayoutManager);
                MRecycler.HasFixedSize = true;
                MRecycler.SetItemViewCacheSize(10);
                MRecycler.GetLayoutManager().ItemPrefetchEnabled = true;
                var sizeProvider = new FixedPreloadSizeProvider(10, 10);
                var preLoader = new RecyclerViewPreloader<Classes.MainVideoClass>(Activity, MAdapter, sizeProvider, 10);
                MRecycler.AddOnScrollListener(preLoader);
                MRecycler.SetAdapter(MAdapter);
                MAdapter.OtherVideosItemClick += OnVideoItemClick;

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

        private void AddCategory()
        {
            try
            {
                //Category
                var respondList = CategoriesController.ListCategories.Count;
                if (respondList > 0)
                {
                    var checkList = MAdapter.MainVideoList.FirstOrDefault(q => q.Type == ItemType.Category);
                    if (checkList == null)
                    {
                        var category = new Classes.MainVideoClass()
                        {
                            Id = 400,
                            CategoryList = new List<Classes.Category>(),
                            Type = ItemType.Category
                        };

                        category.CategoryList = new List<Classes.Category>(CategoriesController.ListCategories);
                        MAdapter.MainVideoList.Insert(0 ,category);
                    }
                    else
                    {
                        checkList.CategoryList = new List<Classes.Category>(CategoriesController.ListCategories);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
       
        #endregion

        #region Events
          
        private void MainAlertOnClick(object sender, EventArgs e)
        {
            try
            {
                Activity.StartActivity(new Intent(Activity, typeof(LoginActivity)));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Open Video from Other
        private void OnVideoItemClick(object sender, VideoAdapterClickEventArgs args)
        {
            try
            { 
                if (args.Position <= -1) return;

                var item = MAdapter.GetItem(args.Position);
                if (item.VideoData == null) return;

                GlobalContext.StartPlayVideo(item.VideoData); 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
        #endregion

        #region Refresh

        public void OnLoadMore()
        {
            try
            {
                if (MainScrollEvent.IsLoading)
                    return;

                string idFeatured = ListUtils.FeaturedVideosList.LastOrDefault()?.Id ?? "0";

                string idTop = "0";
                string idLatest = "0";
                string idFav = "0";

                var checkTopList = MAdapter.MainVideoList.FirstOrDefault(q => q.Type == ItemType.TopVideos);
                if (checkTopList != null)
                    idTop = checkTopList.TopVideoList.LastOrDefault()?.Id ?? "0";

                var checkLatestList = MAdapter.MainVideoList.FirstOrDefault(q => q.Type == ItemType.LatestVideos);
                if (checkLatestList != null)
                    idLatest = checkLatestList.LatestVideoList.LastOrDefault()?.Id ?? "0";

                var checkFavList = MAdapter.MainVideoList.FirstOrDefault(q => q.Type == ItemType.FavVideos);
                if (checkFavList != null)
                    idFav = checkFavList.FavVideoList.LastOrDefault()?.Id ?? "0";

                StartApiService(idFeatured, idTop, idLatest, idFav); 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnRefresh()
        {
            try
            {
                MainScrollEvent.IsLoading = false;

                MainAppBarLayout.SetExpanded(false);
                ListUtils.FeaturedVideosList.Clear();
                ViewPagerView.Adapter = null;

                MAdapter.MainVideoList.Clear();
                MAdapter.NotifyDataSetChanged();

                LoadingLinear.Visibility = ViewStates.Visible;

                EmptyStateLayout.Visibility = ViewStates.Gone;

                StartApiService();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Scroll
           
        private void MainScrollEventOnLoadMoreEvent(object sender, EventArgs e)
        {
            try
            {
                if (MainScrollEvent.IsLoading)
                    return;

                string idFeatured = ListUtils.FeaturedVideosList.LastOrDefault()?.Id ?? "0";

                string idTop = "0"; 
                string idLatest = "0"; 
                string idFav = "0"; 

                var checkTopList = MAdapter.MainVideoList.FirstOrDefault(q => q.Type == ItemType.TopVideos);
                if (checkTopList != null)
                    idTop = checkTopList.TopVideoList.LastOrDefault()?.Id ?? "0";

                var checkLatestList = MAdapter.MainVideoList.FirstOrDefault(q => q.Type == ItemType.LatestVideos);
                if (checkLatestList != null)
                    idLatest = checkLatestList.LatestVideoList.LastOrDefault()?.Id ?? "0";
                 
                var checkFavList = MAdapter.MainVideoList.FirstOrDefault(q => q.Type == ItemType.FavVideos);
                if (checkFavList != null)
                    idFav = checkFavList.FavVideoList.LastOrDefault()?.Id ?? "0";
                 
                StartApiService(idFeatured, idTop, idLatest , idFav);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
        #endregion

        #region LoadData 

        private void StartApiService(string featuredOffset = "0", string topOffset = "0", string latestOffset = "0", string favOffset = "0")
        {
            if (Methods.CheckConnectivity())
            {
                PollyController.RunRetryPolicyFunction(new List<Func<Task>>{() => LoadVideos(featuredOffset, topOffset, latestOffset, favOffset) });
            }
            else
            {
                SwipeRefreshLayout.OnFinishFreshAndLoad(); 
                Toast.MakeText(Context, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long).Show();
            }
        }
     
        private async Task LoadVideos(string featuredOffset = "0", string topOffset = "0", string latestOffset = "0", string favOffset = "0")
        {
            if (MainScrollEvent.IsLoading)
                return;

            if (Methods.CheckConnectivity())
            {
                MainScrollEvent.IsLoading = true;

                var (apiStatus, respond) = await RequestsAsync.Video.Get_Videos_Http(featuredOffset, topOffset, latestOffset, favOffset, "20");
                if (apiStatus == 200)
                {
                    if (respond is GetVideosObject result)
                    {
                        if (result.DataResult.Featured?.Count > 0)
                        {
                            result.DataResult.Featured = AppTools.ListFilter(result.DataResult.Featured);
                             
                            if (ListUtils.FeaturedVideosList.Count > 0)
                            {
                                foreach (var item in from item in result.DataResult.Featured let check = ListUtils.FeaturedVideosList.FirstOrDefault(a => a.VideoId == item.VideoId) where check == null select item)
                                {
                                    ListUtils.FeaturedVideosList.Add(item);
                                }
                            }
                            else
                            {
                                var result2 = result.DataResult.Featured.GroupBy(x => x.VideoId).Where(x => x.Count() == 1).Select(x => x.First());
                                ListUtils.FeaturedVideosList = new ObservableCollection<VideoObject>(result2);
                            }
                        }
                         
                        //Top
                        var respondList = result.DataResult.Top.Count;
                        if (respondList > 0)
                        {
                            result.DataResult.Top = AppTools.ListFilter(result.DataResult.Top);

                            var checkList = MAdapter.MainVideoList.FirstOrDefault(q => q.Type == ItemType.TopVideos);
                            if (checkList == null)
                            {
                                var topVideos = new Classes.MainVideoClass()
                                {
                                    Id = 101,
                                    TopVideoList = new List<VideoObject>(),
                                    Type = ItemType.TopVideos
                                };

                                foreach (var item in from item in result.DataResult.Top let check = topVideos.TopVideoList.FirstOrDefault(a => a.VideoId == item.VideoId) where check == null select item)
                                {
                                    if (topVideos.TopVideoList.Count <= AppSettings.CountVideosTop)
                                        topVideos.TopVideoList.Add(item);
                                    else
                                    {
                                        var c = OtherVideosList.FirstOrDefault(a => a.VideoId == item.VideoId);
                                        if (c == null)
                                            OtherVideosList.Add(item);
                                    } 
                                }

                                MAdapter.MainVideoList.Add(topVideos);
                            }
                            else
                            {
                                foreach (var item in from item in result.DataResult.Top let check = checkList.TopVideoList.FirstOrDefault(a => a.VideoId == item.VideoId) where check == null select item)
                                {
                                    if (checkList.TopVideoList.Count <= AppSettings.CountVideosTop)
                                        checkList.TopVideoList.Add(item);
                                    else
                                    {
                                        var c = OtherVideosList.FirstOrDefault(a => a.VideoId == item.VideoId);
                                        if (c == null)
                                            OtherVideosList.Add(item);
                                    } 
                                }
                            }
                        }

                        //Latest
                        var respondLatestList = result.DataResult.Latest.Count;
                        if (respondLatestList > 0)
                        {
                            result.DataResult.Latest = AppTools.ListFilter(result.DataResult.Latest);

                            var checkList = MAdapter.MainVideoList.FirstOrDefault(q => q.Type == ItemType.LatestVideos);
                            if (checkList == null)
                            {
                                var latestVideos = new Classes.MainVideoClass()
                                {
                                    Id = 102,
                                    LatestVideoList = new List<VideoObject>(),
                                    Type = ItemType.LatestVideos
                                };

                                foreach (var item in from item in result.DataResult.Latest let check = latestVideos.LatestVideoList.FirstOrDefault(a => a.VideoId == item.VideoId) where check == null select item)
                                {
                                    if (latestVideos.LatestVideoList.Count <= AppSettings.CountVideosLatest)
                                        latestVideos.LatestVideoList.Add(item);
                                    else
                                    {
                                        var c = OtherVideosList.FirstOrDefault(a => a.VideoId == item.VideoId);
                                        if (c == null)
                                            OtherVideosList.Add(item);
                                    } 
                                }

                                MAdapter.MainVideoList.Add(latestVideos);
                            }
                            else
                            {
                                foreach (var item in from item in result.DataResult.Latest let check = checkList.LatestVideoList.FirstOrDefault(a => a.VideoId == item.VideoId) where check == null select item)
                                {
                                    if (checkList.LatestVideoList.Count <= AppSettings.CountVideosLatest)
                                        checkList.LatestVideoList.Add(item);
                                    else
                                    {
                                        var c = OtherVideosList.FirstOrDefault(a => a.VideoId == item.VideoId);
                                        if (c == null)
                                            OtherVideosList.Add(item);
                                    } 
                                }
                            }
                        }

                        //Fav
                        var respondFavList = result.DataResult.Fav.Count;
                        if (respondFavList > 0)
                        {
                            result.DataResult.Fav = AppTools.ListFilter(result.DataResult.Fav);

                            var checkList = MAdapter.MainVideoList.FirstOrDefault(q => q.Type == ItemType.FavVideos);
                            if (checkList == null)
                            {
                                var favVideos = new Classes.MainVideoClass()
                                {
                                    Id = 103,
                                    FavVideoList = new List<VideoObject>(),
                                    Type = ItemType.FavVideos
                                };

                                foreach (var item in from item in result.DataResult.Fav let check = favVideos.FavVideoList.FirstOrDefault(a => a.VideoId == item.VideoId) where check == null select item)
                                {
                                    if (favVideos.FavVideoList.Count <= AppSettings.CountVideosFav)
                                        favVideos.FavVideoList.Add(item);
                                    else
                                    {
                                        var c = OtherVideosList.FirstOrDefault(a => a.VideoId == item.VideoId);
                                        if (c == null)
                                            OtherVideosList.Add(item);
                                    } 
                                }

                                MAdapter.MainVideoList.Add(favVideos);
                            }
                            else
                            {
                                foreach (var item in from item in result.DataResult.Fav let check = checkList.FavVideoList.FirstOrDefault(a => a.VideoId == item.VideoId) where check == null select item)
                                {
                                    if (checkList.FavVideoList.Count <= AppSettings.CountVideosFav)
                                        checkList.FavVideoList.Add(item);
                                    else
                                    {
                                        var c = OtherVideosList.FirstOrDefault(a => a.VideoId == item.VideoId);
                                        if (c == null)
                                            OtherVideosList.Add(item);
                                    } 
                                }
                            }
                        }

                        //Other
                        var respondOtherList = OtherVideosList.Count;
                        if (respondOtherList > 0)
                        {
                            foreach (var users in from item in OtherVideosList let check = MAdapter.MainVideoList.FirstOrDefault(a => a.VideoData?.VideoId == item.VideoId) where check == null select new Classes.MainVideoClass()
                            {
                                Id = Convert.ToInt32(item.Id),
                                VideoData = item,
                                Type = ItemType.OtherVideos
                            })
                            {
                                MAdapter.MainVideoList.Add(users);
                            }
                        }
                        else
                        {
                            if (OtherVideosList.Count > 10 && !MRecycler.CanScrollVertically(1))
                                Toast.MakeText(Context, Context.GetText(Resource.String.Lbl_NoMoreVideos), ToastLength.Short).Show();
                        } 
                    }
                }
                else 
                {
                    MainScrollEvent.IsLoading = false; 
                    Methods.DisplayReportResult(Activity, respond);
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

                Toast.MakeText(Context, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
            }

            MainScrollEvent.IsLoading = false;
        }
         
        private void ShowEmptyPage()
        {
            try
            {
                MainScrollEvent.IsLoading = false;
                
                SwipeRefreshLayout.OnFinishFreshAndLoad();

                if (LoadingLinear.Visibility == ViewStates.Visible)
                    LoadingLinear.Visibility = ViewStates.Gone;

                //Add Featured Videos
                if (ListUtils.FeaturedVideosList.Count > 0)
                {
                    if (ViewPagerView.Adapter == null)
                    {
                        MainAppBarLayout.SetExpanded(true);
                        ViewPagerView.Adapter = new ImageCoursalViewPager(Activity, ListUtils.FeaturedVideosList);
                        ViewPagerView.CurrentItem = 0;
                    }
                    ViewPagerView.Adapter.NotifyDataSetChanged(); 
                }
                 
                if (CategoriesController.ListCategories.Count > 0 && AppSettings.ShowCategoriesInHome)
                    AddCategory();

                if (MAdapter.MainVideoList.Count > 0)
                {
                    MRecycler.Visibility = ViewStates.Visible;
                    EmptyStateLayout.Visibility = ViewStates.Gone;

                    var checkList = MAdapter.MainVideoList.FirstOrDefault(q => q.Type == ItemType.TopVideos || q.Type == ItemType.LatestVideos || q.Type == ItemType.FavVideos || q.Type == ItemType.OtherVideos );
                    if (checkList != null)
                    {
                        var emptyStateChecker = MAdapter.MainVideoList.FirstOrDefault(a => a.Type == ItemType.EmptyPage);
                        if (emptyStateChecker != null)
                        {
                            MAdapter.MainVideoList.Remove(emptyStateChecker);
                        }
                    } 

                    MAdapter.NotifyDataSetChanged();
                }
                else
                {
                    var emptyStateChecker = MAdapter.MainVideoList.FirstOrDefault(a => a.Type == ItemType.EmptyPage);
                    if (emptyStateChecker == null)
                    {
                        MAdapter.MainVideoList.Add(new Classes.MainVideoClass()
                        {
                            Id = 300,
                            Type = ItemType.EmptyPage
                        });
                        MAdapter.NotifyDataSetChanged();
                    }
                     
                    EmptyStateLayout.Visibility = ViewStates.Gone;
                } 
            }
            catch (Exception e)
            {
                MainScrollEvent.IsLoading = false;

                SwipeRefreshLayout.OnFinishFreshAndLoad();
                if (LoadingLinear.Visibility == ViewStates.Visible)
                    LoadingLinear.Visibility = ViewStates.Gone;

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

        private void GetNotInterestedVideos()
        {
            try
            {
                var sqlEntity = new SqLiteDatabase();
                var globalNotInterestedList = sqlEntity.Get_NotInterestedVideos();
                if (globalNotInterestedList?.Count > 0)
                {
                    ListUtils.GlobalNotInterestedList = globalNotInterestedList;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void OnOffsetChanged(AppBarLayout appBarLayout, int verticalOffset)
        {
            if (ScrollRange == -1)
            {
                ScrollRange = appBarLayout.TotalScrollRange;
            }
            if (ScrollRange + verticalOffset == 0)
            {
                ToolbarLogoLinearLayout.Visibility = ViewStates.Visible;
                IsShowing = true;
            }
            else if (IsShowing)
            {
                ToolbarLogoLinearLayout.Visibility = ViewStates.Gone;
                IsShowing = false;
            }
        }
    }
}