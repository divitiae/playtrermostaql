//###############################################################
// Author >> Elin Doughouz
// Copyright (c) PlayTube 12/07/2018 All Right Reserved
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// Follow me on facebook >> https://www.facebook.com/Elindoughous
//=========================================================

using AFollestad.MaterialDialogs;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Load.Engine;
using Bumptech.Glide.Request;
using Clans.Fab;
using Com.Google.Android.Youtube.Player;
using Com.Sothree.Slidinguppanel;
using Com.Theartofdev.Edmodo.Cropper;
using Irozon.SneakerLib;
using Java.IO;
using Java.Lang;
using Newtonsoft.Json;
using PlayTube.Activities.Article;
using PlayTube.Activities.Channel;
using PlayTube.Activities.Chat;
using PlayTube.Activities.Chat.Service;
using PlayTube.Activities.Comments;
using PlayTube.Activities.Models;
using PlayTube.Activities.PlayersView;
using PlayTube.Activities.Playlist;
using PlayTube.Activities.Tabbes.Fragments;
using PlayTube.Activities.Upload;
using PlayTube.Activities.Videos;
using PlayTube.Adapters;
using PlayTube.Helpers.Controller;
using PlayTube.Helpers.Models;
using PlayTube.Helpers.Utils;
using PlayTube.Helpers.Views;
using PlayTube.OneSignal;
using PlayTube.Payment;
using PlayTube.PaymentGoogle;
using PlayTube.SQLite;
using PlayTubeClient.Classes.Global;
using PlayTubeClient.Classes.Messages;
using PlayTubeClient.Classes.Playlist;
using PlayTubeClient.RestCalls;
using Q.Rorbin.Badgeview;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Android.Util;
using PlayTube.Activities.SettingsPreferences;
using Xamarin.PayPal.Android;
using Console = System.Console;
using Exception = System.Exception;
using Extensions = Android.Runtime.Extensions;
using FloatingActionButton = Clans.Fab.FloatingActionButton;
using Math = System.Math;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using VideoController = PlayTube.Helpers.Controller.VideoController;
using Fragment = Android.Support.V4.App.Fragment;
using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;
using static PlayTube.Activities.Models.VideoDataWithEventsLoader;
using PlayTube.Helpers.CacheLoaders;
using PlayTube.MediaPlayer;
using PlayTubeClient;

namespace PlayTube.Activities.Tabbes
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Keyboard | ConfigChanges.Orientation | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenLayout | ConfigChanges.ScreenSize | ConfigChanges.SmallestScreenSize | ConfigChanges.UiMode | ConfigChanges.Locale, SupportsPictureInPicture = true)]
    public class TabbedMainActivity : AppCompatActivity, MaterialDialog.ISingleButtonCallback, MaterialDialog.IListCallback, ServiceResultReceiver.IReceiver, SlidingPaneLayout.IPanelSlideListener, SlidingUpPanelLayout.IPanelSlideListener, IYouTubePlayerOnInitializedListener, AppBarLayout.IOnOffsetChangedListener, View.IOnTouchListener, View.IOnClickListener, IYouTubePlayerOnFullscreenListener
    {
        #region Variables Basic
        private static TabbedMainActivity Instance;
        public SlidingUpPanelLayout SlidingUpPanel;
        public FloatingActionButton ButtonImport;
        public FloatingActionButton ButtonUpload;
        public VideoController VideoActionsController;
        public FloatingActionMenu MoreMultiButtons;
        private AppBarLayout AppBarLayoutView;
        public MainVideoFragment MainVideoFragment;
        public TrendingFragment TrendingFragment;
        public LibraryFragment LibraryFragment;
        private ArticlesFragment ArticlesFragment;
        public MyChannelFragment MyChannelFragment;
        public UserChannelFragment UserChannelFragment;
        public CommentsFragment CommentsFragment;
        public NextToFragment NextToFragment;
        public YouTubePlayerSupportFragment YouTubeFragment;
        public RestrictedVideoFragment RestrictedVideoPlayerFragment;
        private ThirdPartyPlayersFragment ThirdPartyPlayersFragment;
        private CoordinatorLayout CoordinatorLayoutView;
        private string VideoIdYoutube;
        public IYouTubePlayer YoutubePlayer { get; private set; }
        public VideoDataWithEventsLoader VideoDataWithEventsLoader;
        public LibrarySynchronizer LibrarySynchronizer;
        private PowerManager.WakeLock Wl;
        private VideoObject VideoData;
        private readonly Handler ExitHandler = new Handler();
        private bool RecentlyBackPressed;
        private ServiceResultReceiver Receiver;
        public InitPayPalPayment InitPayPalPayment;
        private string DialogType;
        public InitInAppBillingPayment BillingPayment;
        public CustomNavigationController FragmentBottomNavigator;
        public FrameLayout NavigationTabBar;
        private CardView VideoButtomStyle;
        private FrameLayout MainVideoRoot;
        public TextView VideoTitleText, VideoChannelText;
        private CustomTouchLayout VideoSmallFrameLayout;
        private float X, Dx;
        private LinearLayout VideoTextContainer;
        private bool OnStopCalled;
        private bool IsYoutubeOnFullScreen;

        #endregion  

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                Methods.App.FullScreenApp(this);

                SetTheme(AppSettings.SetTabDarkTheme ? Resource.Style.MyTheme_Dark_Base : Resource.Style.MyTheme_Base);

                // Create your application here
                SetContentView(Resource.Layout.Tabbed_Main_Layout);

                Instance = this;
                OnStopCalled = false;

                SetVideoPlayerFragmentAdapters();

                //Get Value And Set Toolbar
                InitComponent();


                SetupFloatingActionMenus();
                SetupBottomNavigationView();

                GetGeneralAppData();

                if (AppSettings.EnablePictureToPictureMode)
                {
                    var pipIsChecked = AppTools.CheckPictureInPictureAllowed(this);

                    if (!PackageManager.HasSystemFeature(PackageManager.FeaturePictureInPicture) || !pipIsChecked)
                    {
                        var intent = new Intent("android.settings.PICTURE_IN_PICTURE_SETTINGS", Android.Net.Uri.Parse("package:" + PackageName));
                        StartActivityForResult(intent, 8520);
                    }
                }

                if (UserDetails.IsLogin)
                    SetService();

                GetDataOneSignal();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        protected override void OnResume()
        {
            try
            {
                base.OnResume();
                ToggleKeepSceenOnFeature(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected override void OnPause()
        {
            try
            {
                base.OnPause();
                ToggleKeepSceenOnFeature(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            try
            {
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                base.OnTrimMemory(level);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override void OnLowMemory()
        {
            try
            {
                GC.Collect(GC.MaxGeneration);
                base.OnLowMemory();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected override void OnStop()
        {
            try
            {
                OnStopCalled = true;
                base.OnStop();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected override void OnDestroy()
        {
            try
            {
                VideoActionsController?.SetStopvideo();

                if (YoutubePlayer != null && YoutubePlayer.IsPlaying)
                    YoutubePlayer?.Pause();

                InitPayPalPayment?.StopPayPalService();
                BillingPayment?.DisconnectInAppBilling();

                base.OnDestroy();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            try
            {
                base.OnConfigurationChanged(newConfig);

                var currentNightMode = newConfig.UiMode & UiMode.NightMask;
                switch (currentNightMode)
                {
                    case UiMode.NightNo:
                        // Night mode is not active, we're using the light theme
                        AppSettings.SetTabDarkTheme = false;
                        break;
                    case UiMode.NightYes:
                        // Night mode is active, we're using dark theme
                        AppSettings.SetTabDarkTheme = true;
                        break;
                }

                SetTheme(AppSettings.SetTabDarkTheme ? Resource.Style.MyTheme_Dark_Base : Resource.Style.MyTheme_Base);
                FragmentBottomNavigator?.DisableAllNavigationButton();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Functions

        private void InitComponent()
        {
            try
            {
                CoordinatorLayoutView = FindViewById<CoordinatorLayout>(Resource.Id.parent);
                NavigationTabBar = FindViewById<FrameLayout>(Resource.Id.buttomnavigationBar);
                VideoSmallFrameLayout = FindViewById<CustomTouchLayout>(Resource.Id.Vcontainer);
                MainVideoRoot = FindViewById<FrameLayout>(Resource.Id.Mainroot);
                VideoTitleText = FindViewById<TextView>(Resource.Id.videoTitileText);
                VideoChannelText = FindViewById<TextView>(Resource.Id.videoChannelText);
                VideoButtomStyle = FindViewById<CardView>(Resource.Id.VideoButtomStyle);
                VideoTextContainer = FindViewById<LinearLayout>(Resource.Id.videoTextcontainer);
                AppBarLayoutView = FindViewById<AppBarLayout>(Resource.Id.appBarLayout);
                SlidingUpPanel = FindViewById<SlidingUpPanelLayout>(Resource.Id.sliding_layout);
                SlidingUpPanel.SetPanelState(SlidingUpPanelLayout.PanelState.Hidden);
                SlidingUpPanel.AddPanelSlideListener(this);

                SlidingUpPanel.Tag = "Hidden";
                AppBarLayoutView.AddOnOffsetChangedListener(this);

                VideoDataWithEventsLoader = new VideoDataWithEventsLoader(this);
                VideoDataWithEventsLoader.SetViews();

                LibrarySynchronizer = new LibrarySynchronizer(this);

                VideoActionsController = new VideoController(this, "Main");
                VideoActionsController.ExoBackButton.Click += BackIcon_Click;

                if (AppSettings.ShowPaypal)
                {
                    InitPayPalPayment = new InitPayPalPayment(this);
                }

                if (AppSettings.ShowInAppBilling && Client.IsExtended)
                {
                    BillingPayment = new InitInAppBillingPayment(this);
                }

                VideoButtomStyle.SetOnClickListener(this);
                VideoButtomStyle.SetOnTouchListener(this);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnClick(View v)
        {
            try
            {
                if (v.Id == VideoButtomStyle.Id)
                {
                    if (SlidingUpPanel != null && SlidingUpPanel.GetPanelState() != SlidingUpPanelLayout.PanelState.Expanded)
                    {
                        HideVideoButtomStyle();
                        SlidingUpPanel?.SetPanelState(SlidingUpPanelLayout.PanelState.Expanded);
                    }
                }
                else if (v is FloatingActionButton)
                {
                    ToggleFloatingMenuItems();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            try
            {
                switch (e.Action)
                {
                    case MotionEventActions.Down:
                        X = e.RawX;
                        Dx = X - v.GetX();
                        return false;
                    case MotionEventActions.Move:
                        VideoTextContainer.Alpha = 1 - Math.Abs(1 / e.RawX * 100);
                        VideoButtomStyle.SetX(e.RawX - Dx);
                        return false;
                    case MotionEventActions.Up:
                        {
                            if (e.RawX - Dx > v.Width / 2)
                            {
                                VideoButtomStyle.Animate().TranslationX(v.Width).TranslationY(0).Alpha(0).SetDuration(300);
                                GlobalVideosRelease("All");
                            }
                            else if (Math.Abs(e.RawX - Dx) > v.Width / 2)
                            {
                                VideoButtomStyle.Animate().TranslationX(-v.Width).TranslationY(0).Alpha(0).SetDuration(300);
                                GlobalVideosRelease("All");
                            }
                            else if (Math.Abs((int)e.RawX - (int)Dx) < 30) //Click Event 
                            {
                                v.PerformClick();
                                return false;
                            }
                            else
                            {
                                VideoButtomStyle.Animate().TranslationX(0).TranslationY(0).Alpha(1).SetDuration(300);
                            }
                            return true;
                        }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
            return false;
        }

        public static TabbedMainActivity GetInstance()
        {
            try
            {
                return Instance;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        private void SetupFloatingActionMenus()
        {
            try
            {
                MoreMultiButtons = FindViewById<FloatingActionMenu>(Resource.Id.multistroybutton);
                MoreMultiButtons.GetChildAt(0).Click += BtnImportOnClick;
                MoreMultiButtons.GetChildAt(1).Click += BtnUploadOnClick;
                MoreMultiButtons.SetOnMenuButtonClickListener(this);

                MoreMultiButtons.Visibility = ViewStates.Invisible;

                ButtonImport = FindViewById<FloatingActionButton>(Resource.Id.item_Import);
                ButtonUpload = FindViewById<FloatingActionButton>(Resource.Id.item_Upload);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void ToggleFloatingMenuItems()
        {
            try
            {
                var showImportFunctionality = AppSettings.ShowButtonImport;
                ButtonImport.Visibility = showImportFunctionality ? ViewStates.Visible : ViewStates.Gone;
                MoreMultiButtons.GetChildAt(0).Visibility = showImportFunctionality ? ViewStates.Visible : ViewStates.Gone;

                var showUploadFunctionality = AppSettings.ShowButtonUpload;
                ButtonUpload.Visibility = showUploadFunctionality ? ViewStates.Visible : ViewStates.Gone;
                MoreMultiButtons.GetChildAt(1).Visibility = showUploadFunctionality ? ViewStates.Visible : ViewStates.Gone;

                var showFloatingButton = AppSettings.ShowButtonImport || AppSettings.ShowButtonUpload;
                MoreMultiButtons.Visibility = showFloatingButton ? ViewStates.Visible : ViewStates.Gone;

                MoreMultiButtons.Toggle(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void SetToolBar(Toolbar toolbar, string title, bool showIconBack = true)
        {
            try
            {
                if (toolbar != null)
                {
                    if (!string.IsNullOrEmpty(title))
                        toolbar.Title = title;

                    SetSupportActionBar(toolbar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(showIconBack);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true);
                }
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
                    FragmentNavigatorBack();
                    return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        #endregion

        #region Events

        public void ShowUserChannelFragment(UserDataObject userDataHandler, string userid)
        {
            try
            {
                if (userid != UserDetails.UserId)
                {
                    Bundle bundle = new Bundle();

                    if (userDataHandler != null)
                        bundle.PutString("Object", JsonConvert.SerializeObject(userDataHandler));

                    UserChannelFragment = new UserChannelFragment { Arguments = bundle };

                    FragmentBottomNavigator.DisplayFragment(UserChannelFragment);
                }
                else
                {
                    if (UserDetails.IsLogin)
                    {
                        // Allen NavigationTabBar.SetModelIndex(4, true);
                    }
                    else
                    {
                        PopupDialogController dialog = new PopupDialogController(this, null, "Login");
                        dialog.ShowNormalDialog(GetText(Resource.String.Lbl_Warning), GetText(Resource.String.Lbl_Start_signin), GetText(Resource.String.Lbl_Yes), GetText(Resource.String.Lbl_No));
                    }
                }

                if (SlidingUpPanel != null && SlidingUpPanel.GetPanelState() == SlidingUpPanelLayout.PanelState.Expanded)
                    SlidingUpPanel.SetPanelState(SlidingUpPanelLayout.PanelState.Collapsed);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void ShowReplyCommentFragment(dynamic comment, string type)
        {
            try
            {
                ReplyCommentBottomSheet replyFragment = new ReplyCommentBottomSheet();
                Bundle bundle = new Bundle();

                bundle.PutString("Type", type);
                bundle.PutString("Object", JsonConvert.SerializeObject(comment));
                replyFragment.Arguments = bundle;

                replyFragment.Show(SupportFragmentManager, replyFragment.Tag);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //VideoObject Import
        private void BtnImportOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                if (AppSettings.ShowButtonImport)
                {
                    MoreMultiButtons.Close(true);
                    StartActivity(new Intent(this, typeof(VideoImportActivity)));

                    VideoActionsController.SetStopvideo();

                    if (YoutubePlayer != null && YoutubePlayer.IsPlaying)
                        YoutubePlayer?.Pause();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //VideoObject Upload
        private void BtnUploadOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                if (AppSettings.ShowButtonUpload)
                {
                    MoreMultiButtons.Close(true);
                    StartActivityForResult(new Intent(this, typeof(VideoUploadActivity)), 3000);
                    VideoActionsController.SetStopvideo();

                    if (YoutubePlayer != null && YoutubePlayer.IsPlaying)
                        YoutubePlayer?.Pause();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void EditPlaylistOnClick(PlayListVideoObject playListVideoObject)
        {
            try
            {
                Intent intent = new Intent(this, typeof(EditPlaylistActivity));
                intent.PutExtra("Item", JsonConvert.SerializeObject(playListVideoObject));
                intent.PutExtra("IdList", playListVideoObject.ListId);
                StartActivityForResult(intent, 528);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        #region Refresh

        #endregion

        #region Set Tab

        private void SetupBottomNavigationView()
        {
            try
            {
                FragmentBottomNavigator = new CustomNavigationController(this);

                MainVideoFragment = new MainVideoFragment();
                TrendingFragment = new TrendingFragment();
                LibraryFragment = new LibraryFragment();
                MyChannelFragment = new MyChannelFragment();
                ArticlesFragment = new ArticlesFragment();

                FragmentBottomNavigator.FragmentListTab0.Add(MainVideoFragment);
                FragmentBottomNavigator.FragmentListTab1.Add(TrendingFragment);

                if (AppSettings.ShowArticle)
                    FragmentBottomNavigator.FragmentListTab2.Add(ArticlesFragment);

                FragmentBottomNavigator.FragmentListTab3.Add(LibraryFragment);
                FragmentBottomNavigator.FragmentListTab4.Add(MyChannelFragment);

                if (LibraryFragment.MAdapter == null)
                    LibraryFragment.MAdapter = new LibraryAdapter(this);

                FragmentBottomNavigator.ShowFragment0();

                GlideImageLoader.LoadImage(this, UserDetails.Avatar, FragmentBottomNavigator.ProfileImage, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);
            }
            catch (Exception e)
            {
                FragmentBottomNavigator.ShowFragment0();
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Set Video Player

        private void SetVideoPlayerFragmentAdapters()
        {
            try
            {
                CommentsFragment = new CommentsFragment();
                NextToFragment = new NextToFragment();

                FragmentTransaction ftvideo = SupportFragmentManager.BeginTransaction();
                ftvideo.Add(Resource.Id.videoButtomLayout, NextToFragment, NextToFragment.Id.ToString() + DateTime.Now).Commit();

                if (YouTubeFragment == null)
                {
                    FragmentTransaction ft = SupportFragmentManager.BeginTransaction();
                    YouTubeFragment = new YouTubePlayerSupportFragment();
                    YouTubeFragment.Initialize(AppSettings.YoutubePlayerKey, this);
                    ft.Add(Resource.Id.root, YouTubeFragment, YouTubeFragment.Id.ToString() + DateTime.Now).Commit();

                    if (!VideoFrameLayoutFragments.Contains(YouTubeFragment))
                        VideoFrameLayoutFragments.Add(YouTubeFragment);
                }
                if (ThirdPartyPlayersFragment == null)
                {
                    FragmentTransaction ft1 = SupportFragmentManager.BeginTransaction();
                    ThirdPartyPlayersFragment = new ThirdPartyPlayersFragment();
                    ft1.Add(Resource.Id.root, ThirdPartyPlayersFragment, DateTime.Now.ToString(CultureInfo.InvariantCulture)).Commit();
                    if (!VideoFrameLayoutFragments.Contains(ThirdPartyPlayersFragment))
                        VideoFrameLayoutFragments.Add(ThirdPartyPlayersFragment);
                }
                if (RestrictedVideoPlayerFragment == null)
                {
                    FragmentTransaction ft2 = SupportFragmentManager.BeginTransaction();
                    RestrictedVideoPlayerFragment = new RestrictedVideoFragment();
                    ft2.Add(Resource.Id.root, RestrictedVideoPlayerFragment, DateTime.Now.ToString(CultureInfo.InvariantCulture)).Commit();
                    if (!VideoFrameLayoutFragments.Contains(RestrictedVideoPlayerFragment))
                        VideoFrameLayoutFragments.Add(RestrictedVideoPlayerFragment);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public List<Fragment> VideoFrameLayoutFragments = new List<Fragment>();
        public void StartPlayVideo(VideoObject videoObject)
        {
            try
            {
                RestrictedVideoPlayerFragment.HideRestrictedInfo(true);
                UpdateMainRootDefaultSize();

                if (videoObject != null)
                {
                    VideoData = videoObject;

                    if (ListUtils.ArrayListPlay.Count > 0)
                        ListUtils.ArrayListPlay.Remove(videoObject);

                    if (AppSettings.EnablePictureToPictureMode && UserDetails.PipIsChecked)
                    {
                        if (GlobalPlayerActivity.OnOpenPage)
                        {
                            GlobalPlayerActivity.OnOpenPage = false;
                            GlobalPlayerActivity.GetInstance()?.FinishActivityAndTask();
                        }
                        var intent = new Intent(this, typeof(GlobalPlayerActivity));
                        intent.PutExtra("VideoObject", JsonConvert.SerializeObject(VideoData));
                        intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask);
                        intent.AddCategory(Intent.CategoryDefault);
                        intent.AddFlags(ActivityFlags.ReorderToFront);
                        intent.AddFlags(ActivityFlags.SingleTop);
                        StartActivityForResult(intent, 5000);
                        return;
                    }
                    VideoDataWithEventsLoader.LoadVideoData(videoObject);
                    VideoDataWithEventsLoader.HideCommentsAndShowNextTo();

                    if (SlidingUpPanel != null && SlidingUpPanel.GetPanelState() == SlidingUpPanelLayout.PanelState.Collapsed || SlidingUpPanel?.GetPanelState() == SlidingUpPanelLayout.PanelState.Hidden || SlidingUpPanel?.GetPanelState() == SlidingUpPanelLayout.PanelState.Expanded)
                    {
                        HideVideoButtomStyle();
                        SlidingUpPanel?.SetPanelState(SlidingUpPanelLayout.PanelState.Expanded);
                        VideoActionsController.ExoBackButton.SetImageResource(Resource.Drawable.ic_action_arrow_down_sign);
                        VideoActionsController.ExoBackButton.Tag = "Open";

                        if (VideoDataWithEventsLoader.VideoType == VideoEnumTypes.AgeRestricted)
                        {
                            GlobalVideosRelease("All");
                            CustomNavigationController.BringFragmentToTop(RestrictedVideoPlayerFragment, SupportFragmentManager, VideoFrameLayoutFragments);
                            RestrictedVideoPlayerFragment.LoadRestriction("AgeRestriction", videoObject.Thumbnail, videoObject);
                        }
                        else if (VideoDataWithEventsLoader.VideoType == VideoEnumTypes.GeoBlocked)
                        {
                            GlobalVideosRelease("All");
                            CustomNavigationController.BringFragmentToTop(RestrictedVideoPlayerFragment, SupportFragmentManager, VideoFrameLayoutFragments);
                            RestrictedVideoPlayerFragment.LoadRestriction("GeoRestriction", videoObject.Thumbnail, videoObject);
                        }
                        else if (VideoDataWithEventsLoader.VideoType == VideoEnumTypes.Facebook || VideoDataWithEventsLoader.VideoType == VideoEnumTypes.Twitch | VideoDataWithEventsLoader.VideoType == VideoEnumTypes.DailyMotion | VideoDataWithEventsLoader.VideoType == VideoEnumTypes.Ok | VideoDataWithEventsLoader.VideoType == VideoEnumTypes.Vimeo)
                        {
                            GlobalVideosRelease("All");
                            CustomNavigationController.BringFragmentToTop(ThirdPartyPlayersFragment, SupportFragmentManager, VideoFrameLayoutFragments);
                            ThirdPartyPlayersFragment.SetVideoIframe(videoObject);
                        }
                        else if (VideoDataWithEventsLoader.VideoType == VideoEnumTypes.Youtube)
                        {
                            VideoIdYoutube = videoObject.VideoLocation.Split(new[] { "v=" }, StringSplitOptions.None).LastOrDefault();
                            GlobalVideosRelease("Youtube");
                            CustomNavigationController.BringFragmentToTop(YouTubeFragment, SupportFragmentManager, VideoFrameLayoutFragments);
                            YoutubePlayer?.LoadVideo(VideoIdYoutube);
                        }
                        else
                        {
                            GlobalVideosRelease("exo");
                            CustomNavigationController.BringFragmentToTop(null, SupportFragmentManager, VideoFrameLayoutFragments);
                            VideoActionsController.PlayVideo(videoObject.VideoLocation, videoObject, RestrictedVideoPlayerFragment, this);
                        }
                    }

                    SetOnWakeLock();
                    LibrarySynchronizer.AddToRecentlyWatched(videoObject);
                }
            }
            catch (Exception exception)
            {
                if (videoObject != null)
                    VideoActionsController.PlayVideo(videoObject.VideoLocation, videoObject, RestrictedVideoPlayerFragment, this);
                Console.WriteLine(exception);
            }
        }

        private void UpdateMainRootDefaultSize()
        {
            if (AppSettings.ShowVideoWithDynamicHeight)
            {
                var p = MainVideoRoot.LayoutParameters;
                int currWidth = MainVideoRoot.Width;
                p.Width = currWidth;
                p.Height = CovertDpToPixel(220);
                MainVideoRoot.RequestLayout();
            }
        }

        public int CovertDpToPixel(int dp)
        {
            var displayMetrics = new DisplayMetrics();
            WindowManager.DefaultDisplay.GetMetrics(displayMetrics);
            return (int)(dp * displayMetrics.Density);
        }

        public void GlobalVideosRelease(string exepttype)
        {
            try
            {
                if (exepttype == "exo")
                {
                    if (YoutubePlayer != null && YoutubePlayer.IsPlaying)
                        YoutubePlayer?.Pause();

                    if (VideoActionsController.SimpleExoPlayerView.Visibility == ViewStates.Gone)
                        VideoActionsController.SimpleExoPlayerView.Visibility = ViewStates.Visible;
                }
                if (exepttype == "Youtube")
                {
                    VideoActionsController.SimpleExoPlayerView.Visibility = ViewStates.Gone;
                    VideoActionsController.ReleaseVideo();
                    YouTubeFragment.View.Visibility = ViewStates.Visible;
                }

                if (exepttype == "All")
                {
                    if (YoutubePlayer != null && YoutubePlayer.IsPlaying)
                        YoutubePlayer?.Pause();
                    YouTubeFragment.View.Visibility = ViewStates.Gone;

                    VideoActionsController.SimpleExoPlayerView.Visibility = ViewStates.Gone;
                    VideoActionsController.ReleaseVideo();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void StopFragmentVideo()
        {
            try
            {
                if (SlidingUpPanel != null && SlidingUpPanel.GetPanelState() == SlidingUpPanelLayout.PanelState.Collapsed)
                {

                    if (VideoDataWithEventsLoader.VideoType == VideoEnumTypes.Normal)
                        VideoActionsController.ReleaseVideo();
                    else if (VideoDataWithEventsLoader.VideoType == VideoEnumTypes.Youtube)
                        YoutubePlayer?.Pause();

                    // ..screen will stay on during this section..
                    Wl?.Release();
                    Wl = null;

                    SlidingUpPanel.SetPanelState(SlidingUpPanelLayout.PanelState.Hidden);
                }
                else
                {
                    if (!AppSettings.EnablePictureToPictureMode || !UserDetails.PipIsChecked || !GlobalPlayerActivity.OnOpenPage) return;
                    GlobalPlayerActivity.OnOpenPage = false;
                    GlobalPlayerActivity.GetInstance()?.FinishActivityAndTask();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        #region Event Back

        private void BackIcon_Click(object sender, EventArgs e)
        {
            try
            {
                if (SlidingUpPanel != null && (SlidingUpPanel.GetPanelState() == SlidingUpPanelLayout.PanelState.Expanded || SlidingUpPanel.GetPanelState() == SlidingUpPanelLayout.PanelState.Anchored))
                {
                    ShowVideoButtomStyle();
                    SlidingUpPanel.SetPanelState(SlidingUpPanelLayout.PanelState.Collapsed);
                }
                else if (SlidingUpPanel != null && SlidingUpPanel.GetPanelState() == SlidingUpPanelLayout.PanelState.Collapsed)
                {
                    VideoButtomStyle.Animate().TranslationX(-VideoButtomStyle.Width).TranslationY(0).SetDuration(300);
                    GlobalVideosRelease("All");
                    StopFragmentVideo();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public override void OnBackPressed()
        {
            try
            {
                if (IsYoutubeOnFullScreen)
                {
                    YoutubePlayer?.SetFullscreen(false);
                    return;
                }

                if (SlidingUpPanel != null && (SlidingUpPanel.GetPanelState() == SlidingUpPanelLayout.PanelState.Expanded || SlidingUpPanel.GetPanelState() == SlidingUpPanelLayout.PanelState.Anchored))
                {
                    ShowVideoButtomStyle();
                    SlidingUpPanel.SetPanelState(SlidingUpPanelLayout.PanelState.Collapsed);
                }
                else
                {
                    if (FragmentBottomNavigator.GetCountFragment() > 0)
                    {
                        FragmentNavigatorBack();
                    }
                    else
                    {
                        if (RecentlyBackPressed)
                        {
                            ExitHandler.RemoveCallbacks(() => { RecentlyBackPressed = false; });
                            RecentlyBackPressed = false;
                            MoveTaskToBack(true);
                        }
                        else
                        {
                            RecentlyBackPressed = true;
                            Toast.MakeText(this, GetString(Resource.String.press_again_exit), ToastLength.Long).Show();
                            ExitHandler.PostDelayed(() => { RecentlyBackPressed = false; }, 2000L);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void FragmentNavigatorBack()
        {
            try
            {
                FragmentBottomNavigator.OnBackStackClickFragment();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Listener Panel Layout

        private void ToggleKeepSceenOnFeature(bool keepScreenOn)
        {
            if (keepScreenOn)
            {
                SetOnWakeLock();
                AddFlagsWakeLock();
            }
            else
            {
                SetOffWakeLock();
                ClearFlagsWakeLock();
            }

            VideoActionsController.ToggleExoPlayerKeepScreenOnFeature(keepScreenOn);
        }

        public void OnPanelClosed(View panel)
        {

        }

        public void OnPanelOpened(View panel)
        {

        }

        public void OnPanelSlide(View panel, float slideOffset)
        {
            try
            {
                NavigationTabBar.Alpha = 1 - slideOffset;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void OnPanelStateChanged(View p0, SlidingUpPanelLayout.PanelState p1, SlidingUpPanelLayout.PanelState p2)
        {
            try
            {
                if (p1 == SlidingUpPanelLayout.PanelState.Expanded && p2 == SlidingUpPanelLayout.PanelState.Dragging)
                {
                    if (VideoActionsController.ExoBackButton.Tag.ToString() == "Close")
                    {
                        VideoActionsController.ExoBackButton.SetImageResource(Resource.Drawable.ic_action_arrow_down_sign);
                        VideoActionsController.ExoBackButton.Tag = "Open";
                        VideoActionsController.ExoTopLayout.SetPadding(3, 5, 3, 0);

                    }

                    HideVideoButtomStyle();

                }
                else if (p1 == SlidingUpPanelLayout.PanelState.Hidden && p2 == SlidingUpPanelLayout.PanelState.Dragging)
                {
                    if (VideoActionsController.ExoBackButton != null && VideoActionsController.ExoBackButton.Tag.ToString() == "Open")
                    {
                        VideoActionsController.ExoBackButton.SetImageResource(Resource.Drawable.ic_action_arrow_down_sign);
                        VideoActionsController.ExoBackButton.Tag = "Close";
                        HideVideoButtomStyle();
                        NavigationTabBar.Visibility = ViewStates.Gone;
                    }



                }
                else if (p1 == SlidingUpPanelLayout.PanelState.Expanded && p2 == SlidingUpPanelLayout.PanelState.Anchored)
                {
                }
                else if (p1 == SlidingUpPanelLayout.PanelState.Expanded && p2 == SlidingUpPanelLayout.PanelState.Expanded)
                {
                }
                else if (p1 == SlidingUpPanelLayout.PanelState.Expanded && p2 == SlidingUpPanelLayout.PanelState.Hidden)
                {
                }
                else if (p1 == SlidingUpPanelLayout.PanelState.Dragging && p2 == SlidingUpPanelLayout.PanelState.Expanded)
                {
                    if (VideoActionsController.ExoBackButton != null && VideoActionsController.ExoBackButton.Tag.ToString() == "Close")
                    {
                        VideoActionsController.ExoBackButton.SetImageResource(Resource.Drawable.ic_action_arrow_down_sign);
                        VideoActionsController.ExoBackButton.Tag = "Open";
                        NavigationTabBar.Visibility = ViewStates.Gone;
                    }

                    ToggleKeepSceenOnFeature(true);
                }
                else if (p1 == SlidingUpPanelLayout.PanelState.Dragging && p2 == SlidingUpPanelLayout.PanelState.Hidden)
                {
                    //Toast.MakeText(this, "p1 Anchored + Anchored ", ToastLength.Short).Show();
                    ShowVideoButtomStyle();
                }


                if (p1 == SlidingUpPanelLayout.PanelState.Collapsed && p2 == SlidingUpPanelLayout.PanelState.Dragging)
                {
                    if (VideoActionsController.ExoBackButton != null && VideoActionsController.ExoBackButton.Tag.ToString() == "Open")
                    {
                        VideoActionsController.ExoBackButton.SetImageResource(Resource.Drawable.ic_action_close);
                        VideoActionsController.ExoBackButton.Tag = "Close";
                        VideoActionsController.ExoTopLayout.SetPadding(3, 25, 3, 0);
                        NavigationTabBar.Visibility = ViewStates.Visible;
                    }

                }


                if (p1 == SlidingUpPanelLayout.PanelState.Dragging && p2 == SlidingUpPanelLayout.PanelState.Collapsed)
                {
                    if (VideoActionsController.ExoBackButton != null && VideoActionsController.ExoBackButton.Tag.ToString() == "Open")
                    {
                        VideoActionsController.ExoBackButton.SetImageResource(Resource.Drawable.ic_action_close);
                        VideoActionsController.ExoBackButton.Tag = "Close";
                        VideoActionsController.ExoTopLayout.SetPadding(3, 25, 3, 0);
                        ShowVideoButtomStyle();
                        NavigationTabBar.Visibility = ViewStates.Visible;
                    }

                    ToggleKeepSceenOnFeature(false);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion Listener Panel Layout

        #region Get Notifications

        private async Task GetNotifications()
        {
            if (!UserDetails.IsLogin) return;
            if (Methods.CheckConnectivity())
            {
                var (apiStatus, respond) = await RequestsAsync.Notifications.Get_Count_Notifications_Http();
                if (apiStatus == 200)
                {
                    if (respond is GetNotificationsObject result)
                    {
                        var count = result.Notifications.Count;
                        if (count != 0)
                        {
                            ShowOrHideBadgeViewMessenger(count, true);
                        }
                        else
                        {
                            ShowOrHideBadgeViewMessenger();
                        }
                    }
                }
                else Methods.DisplayReportResult(this, respond);
            }
        }

        private void ShowOrHideBadgeViewMessenger(int countMessages = 0, bool show = false)
        {
            try
            {
                RunOnUiThread(() =>
                {
                    try
                    {
                        if (show)
                        {
                            if (LibraryFragment?.NotificationButton != null)
                            {
                                int gravity = (int)(GravityFlags.End | GravityFlags.Bottom);
                                QBadgeView badge = new QBadgeView(this);
                                badge.BindTarget(LibraryFragment?.NotificationButton);
                                badge.SetBadgeNumber(countMessages);
                                badge.SetBadgeGravity(gravity);
                                badge.SetBadgeBackgroundColor(Color.ParseColor(AppSettings.MainColor));
                                badge.SetGravityOffset(10, true);
                            }
                        }
                        else
                        {
                            new QBadgeView(this).BindTarget(LibraryFragment?.NotificationButton).Hide(true);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion Get Notifications

        #region Permissions && Result


        public void ShowVideoButtomStyle()
        {
            try
            {

                if (SlidingUpPanel.Tag?.ToString() == "Hidden")
                {
                    if (VideoButtomStyle.Alpha != 1)
                        VideoButtomStyle.Alpha = 1;

                    if (VideoButtomStyle.Visibility != ViewStates.Visible)
                        VideoButtomStyle.Visibility = ViewStates.Visible;
                    else
                        return;

                    View namebar = VideoActionsController.MainVideoFrameLayout;
                    ViewGroup parent = (ViewGroup)namebar.Parent;
                    if (parent != null)
                    {
                        parent.RemoveView(namebar);
                        VideoSmallFrameLayout.AddView(namebar);
                    }

                    if (VideoData.VideoType == "VideoObject/youtube" || VideoData.VideoLocation.Contains("Youtube") || VideoData.VideoLocation.Contains("youtu"))
                    {
                        if (YoutubePlayer != null)
                        {
                            YoutubePlayer.SetPlayerStyle(YouTubePlayerPlayerStyle.Minimal);
                            YoutubePlayer.LoadVideo(VideoIdYoutube, YoutubePlayer.CurrentTimeMillis);
                        }

                    }
                    else if (VideoData.VideoType == "Vimeo" || VideoData.VideoType == "Twitch" || VideoData.VideoType == "Daily" || VideoData.VideoType == "Ok" || VideoData.VideoType == "Facebook")
                    {

                    }
                    else
                    {
                        VideoActionsController.SimpleExoPlayerView.HideController();

                        //small size 
                        if (RestrictedVideoPlayerFragment != null && VideoFrameLayoutFragments.Contains(RestrictedVideoPlayerFragment))
                        {
                            RestrictedVideoPlayerFragment.RestrictedTextView.SetTextSize(ComplexUnitType.Sp, 10F);
                            RestrictedVideoPlayerFragment.RestrictedIcon.SetPadding(10, 10, 10, 10);
                            if (!string.IsNullOrEmpty(RestrictedVideoPlayerFragment.PurchaseButton?.Tag?.ToString()))
                                RestrictedVideoPlayerFragment.PurchaseButton.Visibility = ViewStates.Gone;
                        }
                    }

                    SlidingUpPanel.Tag = "Shown";
                    VideoButtomStyle.Animate().TranslationY(0).TranslationX(0).Alpha(1).SetDuration(100);
                    VideoButtomStyle.Alpha = 1;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        public void HideVideoButtomStyle()
        {
            try
            {
                if (SlidingUpPanel.Tag?.ToString() == "Shown")
                {

                    if (VideoButtomStyle.Visibility != ViewStates.Gone)
                        VideoButtomStyle.Visibility = ViewStates.Gone;
                    else
                        return;

                    if (VideoButtomStyle.TranslationY != 100)
                        VideoButtomStyle.Animate().TranslationY(100).SetDuration(50);


                    View namebar = VideoActionsController.MainVideoFrameLayout;
                    ViewGroup parent = (ViewGroup)namebar.Parent;
                    if (parent != null)
                    {
                        parent.RemoveView(namebar);
                        MainVideoRoot.AddView(namebar);
                    }

                    if (VideoData.VideoType == "VideoObject/youtube" || VideoData.VideoLocation.Contains("Youtube") || VideoData.VideoLocation.Contains("youtu"))
                    {
                        if (YoutubePlayer != null)
                        {

                            if (!string.IsNullOrEmpty(VideoIdYoutube))
                            {
                                YoutubePlayer.SetPlayerStyle(YouTubePlayerPlayerStyle.Default);
                                YoutubePlayer.LoadVideo(VideoIdYoutube, YoutubePlayer.CurrentTimeMillis);
                            }

                        }
                        else
                        {
                            VideoIdYoutube = VideoData.VideoLocation.Split(new[] { "v=" }, StringSplitOptions.None).LastOrDefault();
                            var ft = SupportFragmentManager.BeginTransaction();
                            YouTubeFragment = new YouTubePlayerSupportFragment();
                            YouTubeFragment.Initialize(AppSettings.YoutubePlayerKey, this);
                            ft.Add(Resource.Id.root, YouTubeFragment, YouTubeFragment.Id.ToString() + DateTime.Now).Commit();

                            VideoActionsController.SimpleExoPlayerView.Visibility = ViewStates.Gone;
                            VideoActionsController.ReleaseVideo();
                        }

                    }
                    else if (VideoData.VideoType == "Vimeo" || VideoData.VideoType == "Twitch" || VideoData.VideoType == "Daily" || VideoData.VideoType == "Ok" || VideoData.VideoType == "Facebook")
                    {

                    }
                    else
                    {
                        //big size 
                        if (RestrictedVideoPlayerFragment != null && VideoFrameLayoutFragments.Contains(RestrictedVideoPlayerFragment))
                        {
                            RestrictedVideoPlayerFragment.RestrictedTextView.SetTextSize(ComplexUnitType.Sp, 14F);
                            RestrictedVideoPlayerFragment.RestrictedIcon.SetPadding(0, 0, 0, 0);
                            if (!string.IsNullOrEmpty(RestrictedVideoPlayerFragment.PurchaseButton?.Tag?.ToString()))
                                RestrictedVideoPlayerFragment.PurchaseButton.Visibility = ViewStates.Visible;
                        }
                    }

                    SlidingUpPanel.Tag = "Hidden";
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected override async void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);
                BillingPayment?.Handler?.HandleActivityResult(requestCode, resultCode, data);

                if (requestCode == 2000)
                {
                    if (resultCode == Result.Ok)
                    {
                        VideoActionsController.RestartPlayAfterShrinkScreen();
                    }
                }
                else if (requestCode == 5000)
                {
                    if (resultCode == Result.Ok)
                    {
                        var type = data.GetStringExtra("Open") ?? "";
                        if (type == "UserProfile")
                        {
                            var userObject = JsonConvert.DeserializeObject<UserDataObject>(data.GetStringExtra("UserObject"));
                            if (userObject != null)
                                ShowUserChannelFragment(userObject, userObject.Id);
                        }
                        else if (type == "VideosByCategory")
                        {
                            var categoryId = data.GetStringExtra("CatId") ?? "";
                            var categoryName = data.GetStringExtra("CatName") ?? "";

                            Bundle bundle = new Bundle();
                            bundle.PutString("CatId", categoryId);
                            bundle.PutString("CatName", categoryName);

                            var videoViewerFragment = new VideosByCategoryFragment()
                            {
                                Arguments = bundle
                            };

                            FragmentBottomNavigator.DisplayFragment(videoViewerFragment);
                        }
                        else if (type == "EditVideo")
                        {
                            var videoObject = JsonConvert.DeserializeObject<VideoObject>(data.GetStringExtra("ItemDataVideo"));
                            if (videoObject != null)
                            {
                                Bundle bundle = new Bundle();
                                bundle.PutString("ItemDataVideo", JsonConvert.SerializeObject(videoObject));

                                var editVideoFragment = new EditVideoFragment()
                                {
                                    Arguments = bundle
                                };

                                FragmentBottomNavigator.DisplayFragment(editVideoFragment);
                            }
                        }
                    }
                }
                else if (requestCode == 3000)
                {
                    if (resultCode == Result.Ok)
                    {
                        Sneaker.With(this)
                            .SetTitle(GetText(Resource.String.Lbl_Video_Success), Android.Resource.Color.White) // Title and title color
                            .SetMessage(GetText(Resource.String.Lbl_Video_Uploaded), Android.Resource.Color.White) // Message and message color
                            .SetDuration(4000)
                            .AutoHide(true)
                            .SetHeight(ViewGroup.LayoutParams.WrapContent)
                            .SneakSuccess();
                    }
                }
                else if (requestCode == 528 && resultCode == Result.Ok)
                {
                    var item = JsonConvert.DeserializeObject<PlayListVideoObject>(data.GetStringExtra("ItemPlaylist"));
                    if (item != null)
                    {
                        if (MyChannelFragment?.PlayListFragment?.MAdapter?.PlayListsList?.Count > 0)
                        {
                            var dataPlayList = MyChannelFragment.PlayListFragment.MAdapter.PlayListsList.FirstOrDefault(q => q.ListId == item.ListId);
                            if (dataPlayList != null)
                            {
                                dataPlayList = item;
                                int index = MyChannelFragment.PlayListFragment.MAdapter.PlayListsList.IndexOf(dataPlayList);
                                MyChannelFragment.PlayListFragment.MAdapter.NotifyItemChanged(index);
                            }
                        }

                        if (LibraryFragment?.PlayListsVideosFragment?.MAdapter?.PlayListsList?.Count > 0)
                        {
                            var dataPlayList = LibraryFragment.PlayListsVideosFragment.MAdapter.PlayListsList.FirstOrDefault(q => q.ListId == item.ListId);
                            if (dataPlayList != null)
                            {
                                dataPlayList = item;
                                int index = LibraryFragment.PlayListsVideosFragment.MAdapter.PlayListsList.IndexOf(dataPlayList);
                                LibraryFragment.PlayListsVideosFragment.MAdapter.NotifyItemChanged(index);
                            }
                        }
                    }
                }
                else if (requestCode == InitPayPalPayment?.PayPalDataRequestCode)
                {
                    switch (resultCode)
                    {
                        case Result.Ok:
                            var confirmObj = data.GetParcelableExtra(PaymentActivity.ExtraResultConfirmation);
                            PaymentConfirmation configuration = Extensions.JavaCast<PaymentConfirmation>(confirmObj);
                            if (configuration != null)
                            {
                                //string createTime = configuration.ProofOfPayment.CreateTime;
                                //string intent = configuration.ProofOfPayment.Intent;
                                //string paymentId = configuration.ProofOfPayment.PaymentId;
                                //string state = configuration.ProofOfPayment.State;
                                //string transactionId = configuration.ProofOfPayment.TransactionId;

                                if (PayType == "purchaseVideo")
                                {
                                    if (Methods.CheckConnectivity())
                                    {
                                        (int apiStatus, var respond) = await RequestsAsync.Global.BuyVideoAsync(PaymentVideoObject.Id);
                                        if (apiStatus == 200)
                                        {
                                            if (respond is MessageObject result)
                                            {
                                                FragmentTransaction ft = SupportFragmentManager.BeginTransaction();

                                                if (RestrictedVideoPlayerFragment != null && RestrictedVideoPlayerFragment.IsAdded)
                                                    ft.Hide(RestrictedVideoPlayerFragment);

                                                if (YouTubeFragment != null)
                                                {
                                                    if (YouTubeFragment.IsAdded)
                                                    {
                                                        if (YoutubePlayer != null && YoutubePlayer.IsPlaying)
                                                            YoutubePlayer?.Pause();

                                                        ft.Hide(YouTubeFragment).AddToBackStack(null).Commit();
                                                        YouTubeFragment.View.Visibility = ViewStates.Gone;

                                                        if (VideoActionsController.SimpleExoPlayerView.Visibility == ViewStates.Gone)
                                                            VideoActionsController.SimpleExoPlayerView.Visibility = ViewStates.Visible;
                                                    }
                                                }

                                                PaymentVideoObject.IsPurchased = "1";
                                                StartPlayVideo(PaymentVideoObject);

                                                Toast.MakeText(this, GetText(Resource.String.Lbl_Done), ToastLength.Long).Show();
                                                StopService(new Intent(this, typeof(PayPalService)));
                                            }
                                        }
                                        else Methods.DisplayReportResult(this, respond);
                                    }
                                    else
                                        Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long).Show();
                                }
                                else if (PayType == "Subscriber")
                                {
                                    if (Methods.CheckConnectivity())
                                    {
                                        UserChannelFragment?.SetSubscribeChannelWithPaid();
                                    }
                                    else
                                        Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long).Show();
                                }
                                else if (PayType == "SubscriberVideo")
                                {
                                    if (Methods.CheckConnectivity())
                                    {
                                        VideoDataWithEventsLoader.SetSubscribeChannelWithPaid();
                                    }
                                    else
                                        Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long).Show();
                                }
                                else if (PayType == "RentVideo")
                                {
                                    if (Methods.CheckConnectivity())
                                    {
                                        (int apiStatus, var respond) = await RequestsAsync.Video.RentVideo_Http(PaymentVideoObject.Id).ConfigureAwait(false);
                                        if (apiStatus == 200)
                                        {
                                            RunOnUiThread(() =>
                                            {
                                                Toast.MakeText(this, GetText(Resource.String.Lbl_VideoSuccessfullyPaid), ToastLength.Long).Show();
                                            });
                                        }
                                        else Methods.DisplayReportResult(this, respond);
                                    }
                                    else
                                        Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long).Show();
                                }
                            }
                            break;
                        case Result.Canceled:
                            Toast.MakeText(this, GetText(Resource.String.Lbl_Canceled), ToastLength.Long).Show();
                            break;
                    }
                }
                else if (requestCode == PaymentActivity.ResultExtrasInvalid)
                {
                    Toast.MakeText(this, GetText(Resource.String.Lbl_Invalid), ToastLength.Long).Show();
                }
                else if (requestCode == 252 && resultCode == Result.Ok)
                {
                    //File fileCover = new File(UserDetails.Cover);
                    //var photoUriCover = FileProvider.GetUriForFile(this, this.PackageName + ".fileprovider", fileCover);
                    //Glide.With(this).Load(photoUriCover).Apply(new RequestOptions()).Into(MyChannelFragment.ImageCoverChannel);

                    //File fileAvatar = new File(UserDetails.Avatar);
                    //var photoUriAvatar = FileProvider.GetUriForFile(this, this.PackageName + ".fileprovider", fileAvatar);
                    //Glide.With(this).Load(photoUriAvatar).Apply(new RequestOptions().CircleCrop()).Into(MyChannelFragment.ImageChannel);

                    MyChannelFragment?.GetDataChannelApi();
                }
                //If its from Camera or Gallery
                else if (requestCode == CropImage.CropImageActivityRequestCode)
                {
                    if (resultCode == Result.Ok)
                    {
                        var result = CropImage.GetActivityResult(data);
                        if (result.IsSuccessful)
                        {
                            var resultUri = result.Uri;

                            if (!string.IsNullOrEmpty(resultUri.Path))
                            {
                                EditVideoFragment.GetInstance().PathImage = resultUri.Path ?? "";
                                File file2 = new File(resultUri.Path);
                                var photoUri = FileProvider.GetUriForFile(this, PackageName + ".fileprovider", file2);
                                Glide.With(this).Load(photoUri).Apply(new RequestOptions()).Into(EditVideoFragment.GetInstance()?.Image);
                            }
                            else
                            {
                                Toast.MakeText(this, GetText(Resource.String.Lbl_something_went_wrong), ToastLength.Long).Show();
                            }
                        }
                    }
                }
                else if (requestCode == 1001 && resultCode == Result.Ok)
                {
                    if (Methods.CheckConnectivity())
                    {
                        (int apiStatus, var respond) = await RequestsAsync.Video.RentVideo_Http(PaymentVideoObject.Id).ConfigureAwait(false);
                        if (apiStatus == 200)
                        {
                            RunOnUiThread(() =>
                            {
                                Toast.MakeText(this, GetText(Resource.String.Lbl_VideoSuccessfullyPaid), ToastLength.Long).Show();
                            });
                        }
                        else Methods.DisplayReportResult(this, respond);
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long).Show();
                    }
                }
                else if (requestCode == 8520 && AppTools.CheckPictureInPictureAllowed(this))
                {
                    UserDetails.PipIsChecked = true;
                    MainSettings.SharedData.Edit().PutBoolean("picture_in_picture_key", UserDetails.PipIsChecked).Commit();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            try
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

                if (requestCode == 110)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        Window.AddFlags(WindowManagerFlags.KeepScreenOn);
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denied), ToastLength.Long).Show();
                    }
                }
                else if (requestCode == 108)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        EditVideoFragment.GetInstance()?.OpenDialogGallery();
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denied), ToastLength.Long).Show();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region PictureInPictur

        public void OnOffsetChanged(AppBarLayout appBarLayout, int verticalOffset)
        {
            try
            {
                var percentage = ((float)Math.Abs(verticalOffset) / appBarLayout.TotalScrollRange);
                Console.WriteLine(percentage);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override void OnPictureInPictureModeChanged(bool isInPictureInPictureMode, Configuration newConfig)
        {
            try
            {
                CoordinatorLayoutView.Visibility = isInPictureInPictureMode ? ViewStates.Gone : ViewStates.Visible;
                MoreMultiButtons.Visibility = isInPictureInPictureMode || !FragmentBottomNavigator.ShowMoreMenuButton() ? ViewStates.Gone : ViewStates.Visible;
                NavigationTabBar.Visibility = isInPictureInPictureMode ? ViewStates.Gone : ViewStates.Visible;

                VideoActionsController.HideControls(isInPictureInPictureMode);

                if (VideoActionsController?.ControlView != null)
                    VideoActionsController.ControlView.Visibility = isInPictureInPictureMode ? ViewStates.Gone : ViewStates.Visible;

                base.OnPictureInPictureModeChanged(isInPictureInPictureMode, newConfig);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected override void OnUserLeaveHint()
        {
            try
            {
                var isNotShowingVideo = !VideoActionsController.SimpleExoPlayerView.IsShown;
                if (isNotShowingVideo)
                {
                    return;
                }

                switch (VideoDataWithEventsLoader.VideoType)
                {
                    case VideoEnumTypes.Normal:
                        if (Build.VERSION.SdkInt >= BuildVersionCodes.O && PackageManager.HasSystemFeature(PackageManager.FeaturePictureInPicture))
                        {
                            Rational rational = new Rational(16, 9);
                            PictureInPictureParams.Builder builder = new PictureInPictureParams.Builder();
                            builder.SetAspectRatio(rational);
                            EnterPictureInPictureMode(builder.Build());
                        }
                        base.OnUserLeaveHint();
                        break;
                    case VideoEnumTypes.Youtube:
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region YouTube Player


        public void OnInitializationFailure(IYouTubePlayerProvider p0, YouTubeInitializationResult p1)
        {
            if (AppSettings.DisableYouTubeInitializationFailureMessages)
                return;

            if (p1.IsUserRecoverableError)
                p1.GetErrorDialog(this, 1).Show();
            else
                Toast.MakeText(this, p1.ToString(), ToastLength.Short).Show();
        }

        public void OnInitializationSuccess(IYouTubePlayerProvider p0, IYouTubePlayer player, bool wasRestored)
        {
            try
            {
                if (YoutubePlayer == null)
                {
                    YoutubePlayer = player;

                    YoutubePlayer.SetPlayerStateChangeListener(new YouTubePlayerEvents());
                    YoutubePlayer.SetOnFullscreenListener(this);
                }

                if (!wasRestored)
                {
                    if (string.IsNullOrEmpty(VideoIdYoutube))
                        return;

                    YoutubePlayer.LoadVideo(VideoIdYoutube);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        #endregion

        #region WakeLock System

        private void AddFlagsWakeLock()
        {
            try
            {
                if ((int)Build.VERSION.SdkInt < 23)
                {
                    Window.AddFlags(WindowManagerFlags.KeepScreenOn);
                }
                else
                {
                    if (CheckSelfPermission(Manifest.Permission.WakeLock) == Permission.Granted)
                    {
                        Window.AddFlags(WindowManagerFlags.KeepScreenOn);
                    }
                    else
                    {
                        //request Code 110
                        new PermissionsController(this).RequestPermission(110);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void ClearFlagsWakeLock()
        {
            try
            {
                Window.ClearFlags(WindowManagerFlags.KeepScreenOn);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        public void SetOnWakeLock()
        {
            try
            {
                if (Wl == null)
                {
                    PowerManager pm = (PowerManager)GetSystemService(PowerService);
                    Wl = pm.NewWakeLock(WakeLockFlags.ScreenBright, "My Tag");
                    Wl.Acquire();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void SetOffWakeLock()
        {
            try
            {
                // ..screen will stay on during this section..
                Wl?.Release();
                Wl = null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion


        #region Service Chat

        public void SetService(bool run = true)
        {
            try
            {
                if (run)
                {
                    try
                    {
                        Receiver = new ServiceResultReceiver(new Handler());
                        Receiver.SetReceiver(this);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }

                    var intent = new Intent(this, typeof(ScheduledApiService));
                    intent.PutExtra("receiverTag", Receiver);
                    StartService(intent);
                }
                else
                {
                    var intentService = new Intent(this, typeof(ScheduledApiService));
                    StopService(intentService);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnReceiveResult(int resultCode, Bundle resultData)
        {
            try
            {
                var result = JsonConvert.DeserializeObject<GetChatsObject>(resultData.GetString("Json"));
                if (result != null)
                {
                    LastChatActivity.GetInstance()?.LoadDataJsonLastChat(result);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Payment & Dialog

        public string Price, PayType;
        public VideoObject PaymentVideoObject;
        private UserDataObject PaymentUserData;
        public void OpenDialog(UserDataObject userData)
        {
            try
            {
                PaymentUserData = userData;

                DialogType = "ChannelIsPaid";
                //This channel is paid, You must pay to subscribe
                var dialog = new MaterialDialog.Builder(this).Theme(AppSettings.SetTabDarkTheme ? AFollestad.MaterialDialogs.Theme.Dark : AFollestad.MaterialDialogs.Theme.Light);

                dialog.Title(Resource.String.Lbl_Warning);
                dialog.Content(GetText(Resource.String.Lbl_ChannelIsPaid));
                dialog.PositiveText(GetText(Resource.String.Lbl_Ok)).OnPositive(this);
                dialog.NegativeText(GetText(Resource.String.Lbl_Cancel)).OnNegative(this);
                dialog.AlwaysCallSingleChoiceCallback();
                dialog.Build().Show();
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
                if (DialogType == "ChannelIsPaid")
                {
                    if (p1 == DialogAction.Positive)
                    {
                        DialogType = "Payment";

                        var arrayAdapter = new List<string>();
                        var dialogList = new MaterialDialog.Builder(this).Theme(AppSettings.SetTabDarkTheme ? AFollestad.MaterialDialogs.Theme.Dark : AFollestad.MaterialDialogs.Theme.Light);

                        if (AppSettings.ShowPaypal)
                            arrayAdapter.Add(GetString(Resource.String.Btn_Paypal));

                        if (AppSettings.ShowCreditCard)
                            arrayAdapter.Add(GetString(Resource.String.Lbl_CreditCard));

                        dialogList.Items(arrayAdapter);
                        dialogList.NegativeText(GetString(Resource.String.Lbl_Close)).OnNegative(this);
                        dialogList.AlwaysCallSingleChoiceCallback();
                        dialogList.ItemsCallback(this).Build().Show();
                    }
                    else if (p1 == DialogAction.Negative)
                    {
                        p0.Dismiss();
                    }
                }
                else
                {
                    if (p1 == DialogAction.Positive)
                    {

                    }
                    else if (p1 == DialogAction.Negative)
                    {
                        p0.Dismiss();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnSelection(MaterialDialog p0, View p1, int itemId, ICharSequence itemString)
        {
            try
            {
                string text = itemString.ToString();
                if (text == GetString(Resource.String.Btn_Paypal))
                {
                    Price = PaymentUserData.SubscriberPrice;
                    PayType = "SubscriberVideo";

                    InitPayPalPayment?.BtnPaypalOnClick(PaymentUserData.SubscriberPrice, "SubscriberVideo");
                }
                else if (text == GetString(Resource.String.Lbl_CreditCard))
                {
                    OpenIntentCreditCard();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void OpenIntentCreditCard()
        {
            try
            {
                Intent intent = new Intent(this, typeof(PaymentCardDetailsActivity));
                intent.PutExtra("Price", PaymentUserData.SubscriberPrice);
                intent.PutExtra("payType", "SubscriberVideo");
                StartActivity(intent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        private void GetGeneralAppData()
        {
            try
            {
                var sqlEntity = new SqLiteDatabase();

                var data = ListUtils.DataUserLoginList.FirstOrDefault();
                if (data != null && UserDetails.IsLogin && data.Status != "Active")
                {
                    data.Status = "Active";
                    UserDetails.Status = "Active";
                    sqlEntity.InsertOrUpdateLogin_Credentials(data);
                }

                var dataUser = sqlEntity.GetDataMyChannel();
                if (dataUser != null)
                {
                    Glide.With(this).Load(UserDetails.Avatar).Apply(new RequestOptions().SetDiskCacheStrategy(DiskCacheStrategy.All).CircleCrop()).Preload();

                    GlideImageLoader.LoadImage(this, UserDetails.Avatar, FragmentBottomNavigator.ProfileImage, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);
                }

                if (UserDetails.IsLogin)
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { GetNotifications, () => ApiRequest.GetChannelData(this, UserDetails.UserId), () => ApiRequest.PlayListsVideosApi(this), () => ApiRequest.WatchLaterVideosApi(this) });

                sqlEntity.Dispose();

                LoadConfigSettings();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void LoadConfigSettings()
        {
            try
            {
                var dbDatabase = new SqLiteDatabase();
                var settingsData = dbDatabase.Get_Settings();
                if (settingsData != null)
                    ListUtils.MySettingsList = settingsData;

                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => ApiRequest.GetSettings_Api(this) });

                if (UserDetails.IsLogin)
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => ApiRequest.AdsVideosApi(this) });

                dbDatabase.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void GetDataOneSignal()
        {
            try
            {
                string type = OneSignalNotification.NotificationInfo?.TypeText ?? "Don't have type";
                if (!string.IsNullOrEmpty(type) && type != "Don't have type")
                {
                    if (type.Contains("added") || type.Contains("disliked") || type.Contains("liked") || type.Contains("commented"))
                    {
                        var channel = OneSignalNotification.VideoData;
                        VideoObject video = new VideoObject()
                        {
                            Id = channel.Id,
                            VideoId = channel.VideoId,
                            UserId = channel.UserId,
                            VideoLocation = channel.VideoLocation,
                            Youtube = channel.Youtube,
                            Vimeo = channel.Vimeo,
                            Daily = channel.Daily,
                            Facebook = channel.Facebook,
                            Ok = channel.Ok,
                            Twitch = channel.Twitch,
                            TwitchType = channel.TwitchType,
                            Thumbnail = channel.Thumbnail,
                            IsOwner = channel.IsOwner,
                            AgeRestriction = channel.AgeRestriction,
                            GeoBlocking = channel.GeoBlocking,
                        };
                        StartPlayVideo(video);
                    }
                    else if (type.Contains("unsubscribed") || type.Contains("subscribed"))
                    {
                        ShowUserChannelFragment(new UserDataObject() { Id = OneSignalNotification.Userid }, OneSignalNotification.Userid);
                    }
                    else
                    {
                        ShowUserChannelFragment(new UserDataObject() { Id = OneSignalNotification.Userid }, OneSignalNotification.Userid);
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnFullscreen(bool isFullScreen)
        {
            IsYoutubeOnFullScreen = isFullScreen;
        }
    }
}