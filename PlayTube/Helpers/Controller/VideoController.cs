//###############################################################
// Author >> Elin Doughouz 
// Copyright (c) PlayTube 12/07/2018 All Right Reserved
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// Follow me on facebook >> https://www.facebook.com/Elindoughous
//=========================================================

using System;
using System.Globalization;
using System.Linq;
using System.Timers;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Google.Ads.Interactivemedia.V3.Api;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Ext.Ima;
using Com.Google.Android.Exoplayer2.Source;
using Com.Google.Android.Exoplayer2.Source.Ads;
using Com.Google.Android.Exoplayer2.Source.Dash;
using Com.Google.Android.Exoplayer2.Source.Hls;
using Com.Google.Android.Exoplayer2.Source.Smoothstreaming;
using Com.Google.Android.Exoplayer2.Trackselection;
using Com.Google.Android.Exoplayer2.UI;
using Com.Google.Android.Exoplayer2.Upstream;
using Com.Google.Android.Exoplayer2.Upstream.Cache;
using Com.Google.Android.Exoplayer2.Util;
using Com.Google.Android.Exoplayer2.Video;
using Java.Lang;
using Java.Net;
using PlayTube.Activities.Models;
using PlayTube.Activities.PlayersView;
using PlayTube.Activities.Tabbes;
using PlayTube.Activities.Videos;
using PlayTube.Helpers.Fonts;
using PlayTube.Helpers.Models;
using PlayTube.Helpers.Utils;
using PlayTube.MediaPlayer;
using PlayTubeClient;
using PlayTubeClient.Classes.Global;
using PlayTubeClient.Classes.Video;
using AlertDialog = Android.App.AlertDialog;
using Exception = System.Exception;
using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;
using Random = System.Random;
using Timer = System.Timers.Timer;
using Uri = Android.Net.Uri;

namespace PlayTube.Helpers.Controller
{
    public class VideoController : Java.Lang.Object, View.IOnClickListener, IAdsLoaderEventListener, IAdsLoaderAdViewProvider, IVideoListener
    {
        #region Variables Basic
        public Activity ActivityContext { get; private set; }
        private string ActivityName { get; set; }
        //Expo Player Factory
        private IDataSourceFactory DefaultDataMediaFactory;
        public static SimpleExoPlayer Player { get; private set; }
        private ImaAdsLoader ImaAdsLoader;
        private PlayerEvents PlayerListener;
        private static PlayerView FullScreenPlayerView;
        public PlayerView SimpleExoPlayerView;
        public FrameLayout MainVideoFrameLayout, MainRoot;
        public ImageView DownloadIcon;
        public ImageView ExoBackButton;
        public LinearLayout ExoTopLayout, ExoEventButton, ExoTopAds;
        public PlayerControlView ControlView;
        private ProgressBar LoadingProgressBar;
        private ImageView MFullScreenIcon;
        private FrameLayout MFullScreenButton;
        private ImageView ShareIcon;
        private FrameLayout MenueButton;
        //Dragble VideoObject player info
        private IMediaSource VideoSource;
        private readonly DefaultBandwidthMeter BandwidthMeter = new DefaultBandwidthMeter();
        private int ResumeWindow = 0;
        private long ResumePosition = 0;
        private VideoDownloadAsyncController VideoControllers;
        public VideoObject VideoData;
        private readonly LibrarySynchronizer LibrarySynchronizer;
        public bool ShowRestrictedVideo;
        public TextView BtnSkipIntro;
        private VideoAdDataObject DataAdsVideo;
        private Timer TimerAds;

        private static VideoController Instance;

        #endregion

        public void HideControls(bool isInPictureInPictureMode)
        {
            ExoTopLayout.Visibility = isInPictureInPictureMode ? ViewStates.Gone : ViewStates.Visible;
            ExoBackButton.Visibility = isInPictureInPictureMode ? ViewStates.Gone : ViewStates.Visible;
            DownloadIcon.Visibility = isInPictureInPictureMode ? ViewStates.Gone : ViewStates.Visible;
            MFullScreenIcon.Visibility = isInPictureInPictureMode ? ViewStates.Gone : ViewStates.Visible;
            MFullScreenButton.Visibility = isInPictureInPictureMode ? ViewStates.Gone : ViewStates.Visible;
            ShareIcon.Visibility = isInPictureInPictureMode ? ViewStates.Gone : ViewStates.Visible;
            MenueButton.Visibility = isInPictureInPictureMode ? ViewStates.Gone : ViewStates.Visible;
        }

        public VideoController(Activity activity, string activityName)
        {
            try
            {
                var defaultCookieManager = new CookieManager();
                defaultCookieManager.SetCookiePolicy(CookiePolicy.AcceptOriginalServer);

                ActivityName = activityName;
                ActivityContext = activity;

                Instance = this;

                LibrarySynchronizer = new LibrarySynchronizer(activity);
                Initialize();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public static VideoController GetInstance()
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

        private void Initialize()
        {
            try
            {
                if (ActivityName != "FullScreen")
                {
                    SimpleExoPlayerView = ActivityContext.FindViewById<PlayerView>(Resource.Id.player_view);
                    ControlView = SimpleExoPlayerView.FindViewById<PlayerControlView>(Resource.Id.exo_controller);
                    PlayerListener = new PlayerEvents(this, ActivityContext, ControlView);
                    SimpleExoPlayerView.SetControllerVisibilityListener(PlayerListener);
                    SimpleExoPlayerView.RequestFocus();
                    //Player initialize 
                    ExoTopLayout = ControlView.FindViewById<LinearLayout>(Resource.Id.topLayout);
                    ExoBackButton = ControlView.FindViewById<ImageView>(Resource.Id.BackIcon);
                    DownloadIcon = ControlView.FindViewById<ImageView>(Resource.Id.Download_icon);
                    MFullScreenIcon = ControlView.FindViewById<ImageView>(Resource.Id.exo_fullscreen_icon);
                    MFullScreenButton = ControlView.FindViewById<FrameLayout>(Resource.Id.exo_fullscreen_button);
                    ShareIcon = ControlView.FindViewById<ImageView>(Resource.Id.share_icon);
                    MenueButton = ControlView.FindViewById<FrameLayout>(Resource.Id.exo_menue_button);
                    MainVideoFrameLayout = ActivityContext.FindViewById<FrameLayout>(Resource.Id.root);
                    MainRoot = ActivityContext.FindViewById<FrameLayout>(Resource.Id.Mainroot);
                    ExoTopAds = ControlView.FindViewById<LinearLayout>(Resource.Id.exo_top_ads);
                    ExoEventButton = ControlView.FindViewById<LinearLayout>(Resource.Id.exo_event_buttons);
                    BtnSkipIntro = ControlView.FindViewById<TextView>(Resource.Id.exo_skipIntro);

                    BtnSkipIntro.Visibility = ViewStates.Gone;
                    ExoTopAds.Visibility = ViewStates.Gone;

                    //SimpleExoPlayerView.HideController();
                    //SimpleExoPlayerView.RequestDisallowInterceptTouchEvent(true);
                    //SimpleExoPlayerView.ControllerHideOnTouch = true;

                    MainVideoFrameLayout.SetOnClickListener(this);
                    LoadingProgressBar = ActivityContext.FindViewById<ProgressBar>(Resource.Id.progress_bar);

                    if (!MFullScreenButton.HasOnClickListeners)
                        MFullScreenButton.SetOnClickListener(this);

                    if (!ExoBackButton.HasOnClickListeners)
                    {
                        //ExoBackButton.Click += BackIcon_Click;
                        DownloadIcon.Click += Download_icon_Click;
                        ShareIcon.Click += ShareIcon_Click;
                        MenueButton.Click += Menu_button_Click;
                        BtnSkipIntro.Click += BtnSkipIntroOnClick;
                        ExoTopAds.Click += ExoTopAdsOnClick;
                    }
                }
                else
                {
                    FullScreenPlayerView = ActivityContext.FindViewById<PlayerView>(Resource.Id.player_view2);
                    ControlView = FullScreenPlayerView.FindViewById<PlayerControlView>(Resource.Id.exo_controller);
                    PlayerListener = new PlayerEvents(this, ActivityContext, ControlView);

                    ExoTopLayout = ControlView.FindViewById<LinearLayout>(Resource.Id.topLayout);
                    ExoBackButton = ControlView.FindViewById<ImageView>(Resource.Id.BackIcon);
                    DownloadIcon = ControlView.FindViewById<ImageView>(Resource.Id.Download_icon);
                    MFullScreenIcon = ControlView.FindViewById<ImageView>(Resource.Id.exo_fullscreen_icon);
                    MFullScreenButton = ControlView.FindViewById<FrameLayout>(Resource.Id.exo_fullscreen_button);
                    ShareIcon = ControlView.FindViewById<ImageView>(Resource.Id.share_icon);
                    MenueButton = ControlView.FindViewById<FrameLayout>(Resource.Id.exo_menue_button);
                    ExoTopAds = ControlView.FindViewById<LinearLayout>(Resource.Id.exo_top_ads);
                    ExoEventButton = ControlView.FindViewById<LinearLayout>(Resource.Id.exo_event_buttons);
                    BtnSkipIntro = ControlView.FindViewById<TextView>(Resource.Id.exo_skipIntro);

                    BtnSkipIntro.Visibility = ViewStates.Gone;
                    ExoTopAds.Visibility = ViewStates.Gone;

                    if (!MFullScreenButton.HasOnClickListeners)
                        MFullScreenButton.SetOnClickListener(this);

                    if (!ExoBackButton.HasOnClickListeners)
                    {
                        ExoBackButton.Click += BackIcon_Click;
                        DownloadIcon.Click += Download_icon_Click;
                        ShareIcon.Click += ShareIcon_Click;
                        MenueButton.Click += Menu_button_Click;
                        BtnSkipIntro.Click += BtnSkipIntroOnClick;
                        ExoTopAds.Click += ExoTopAdsOnClick;
                    }
                }

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void PlayVideo(string videoUrL, VideoObject videoObject, RestrictedVideoFragment restrictedVideoPlayerFragment, Activity activity)
        {
            try
            {
                //RestrictedVideoPlayerFragment = restrictedVideoPlayerFragment;
                //ActivityFragment = activity;

                if (videoObject != null)
                {
                    VideoData = videoObject;
                    ReleaseVideo();

                    bool vidMonit = ListUtils.MySettingsList?.UsrVMon == "on" && VideoData.Monetization == "1" && VideoData.Owner.VideoMon == "1";

                    if (ListUtils.ArrayListPlay.Count > 0)
                        ListUtils.ArrayListPlay.Remove(VideoData);

                    var isPro = ListUtils.MyChannelList.FirstOrDefault()?.IsPro ?? "0";
                    if (!AppSettings.AllowOfflineDownload || AppSettings.AllowDownloadProUser && isPro == "0")
                        DownloadIcon.Visibility = ViewStates.Gone;

                    MFullScreenIcon.SetImageDrawable(ActivityContext.GetDrawable(Resource.Drawable.ic_action_ic_fullscreen_expand));

                    LoadingProgressBar.Visibility = ViewStates.Visible;

                    Uri url;
                    //Rent Or Sell
                    if (!string.IsNullOrEmpty(VideoData.SellVideo) && VideoData.SellVideo != "0" || !string.IsNullOrEmpty(VideoData.RentPrice) && VideoData.RentPrice != "0" && AppSettings.RentVideosSystem)
                    {
                        if (!string.IsNullOrEmpty(VideoData.Demo) && VideoData.IsPurchased == "0")
                        {
                            if (!VideoData.Demo.Contains(Client.WebsiteUrl))
                                VideoData.Demo = Client.WebsiteUrl + "/" + VideoData.Demo;

                            url = Uri.Parse(VideoData.Demo);
                            ShowRestrictedVideo = true;
                        }
                        else if (VideoData.IsPurchased != "0")
                        {
                            url = Uri.Parse(!string.IsNullOrEmpty(videoUrL) ? videoUrL : VideoData.VideoLocation);
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(VideoData.SellVideo) && VideoData.SellVideo != "0")
                                ShowRestrictedVideoFragment(restrictedVideoPlayerFragment, activity, "purchaseVideo");
                            else if (!string.IsNullOrEmpty(VideoData.RentPrice) && VideoData.RentPrice != "0" && AppSettings.RentVideosSystem)
                                ShowRestrictedVideoFragment(restrictedVideoPlayerFragment, activity, "RentVideo");
                            return;
                        }
                    }
                    else
                    {
                        url = Uri.Parse(!string.IsNullOrEmpty(videoUrL) ? videoUrL : VideoData.VideoLocation);
                    }

                    AdaptiveTrackSelection.Factory trackSelectionFactory = new AdaptiveTrackSelection.Factory();
                    var trackSelector = new DefaultTrackSelector(trackSelectionFactory);

                    var newParameters = new DefaultTrackSelector.ParametersBuilder()
                        .SetMaxVideoSizeSd()
                        .SetPreferredAudioLanguage("deu")
                        .Build();

                    trackSelector.SetParameters(newParameters);

                    Player = ExoPlayerFactory.NewSimpleInstance(ActivityContext, trackSelector);

                    FullWidthSetting();

                    DefaultDataMediaFactory = new DefaultDataSourceFactory(ActivityContext, Util.GetUserAgent(ActivityContext, AppSettings.ApplicationName), BandwidthMeter);

                    VideoSource = null;

                    // Produces DataSource instances through which media data is loaded.
                    VideoSource = GetMediaSourceFromUrl(url, "normal");

                    if (SimpleExoPlayerView == null)
                        Initialize();

                    //Set Cache Media Load
                    if (PlayerSettings.EnableOfflineMode)
                    {
                        VideoSource = CreateCacheMediaSource(VideoSource, url);

                        if (VideoSource != null)
                        {
                            DownloadIcon.SetImageResource(Resource.Drawable.ic_checked_red);
                            DownloadIcon.Tag = "Downloaded";

                            RunVideoWithAds(VideoSource, vidMonit);
                            return;
                        }
                    }

                    //Set Interactive Media Ads 
                    if (isPro == "0" && PlayerSettings.ShowInteractiveMediaAds && vidMonit)
                        VideoSource = CreateMediaSourceWithAds(VideoSource, PlayerSettings.ImAdsUri);

                    if (VideoSource == null)
                    {
                        VideoSource = GetMediaSourceFromUrl(url, "normal");

                        RunVideoWithAds(VideoSource, vidMonit);
                    }
                    else
                    {
                        RunVideoWithAds(VideoSource, vidMonit);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void FullWidthSetting()
        {
            if (AppSettings.ShowVideoWithDynamicHeight)
            {
                SimpleExoPlayerView.ResizeMode = AspectRatioFrameLayout.ResizeModeFill;
                Player.VideoScalingMode = C.VideoScalingModeScaleToFitWithCropping;
            }
        }

        public void ChangePlaybackSpeed(PlaybackParameters playbackParameters)
        {
            if (Player != null)
            {
                Player.PlaybackParameters = playbackParameters;
            }
        }

        public void PlayVideo(string videoUrL, VideoObject videoObject, long resumePosition)
        {
            try
            {
                if (Player != null)
                {
                    SetStopvideo();

                    Player?.Release();
                    Player = null;

                    //GC Collecter
                    GC.Collect();
                }

                AdaptiveTrackSelection.Factory trackSelectionFactory = new AdaptiveTrackSelection.Factory();
                var trackSelector = new DefaultTrackSelector(trackSelectionFactory);
                var newParameters = new DefaultTrackSelector.ParametersBuilder()
                    .SetMaxVideoSizeSd()
                    .SetPreferredAudioLanguage("deu")
                    .Build();
                trackSelector.SetParameters(newParameters);

                Player = ExoPlayerFactory.NewSimpleInstance(ActivityContext, trackSelector);
                FullWidthSetting();

                DefaultDataMediaFactory = new DefaultDataSourceFactory(ActivityContext, Util.GetUserAgent(ActivityContext, AppSettings.ApplicationName), BandwidthMeter);

                VideoSource = null;
                VideoSource = GetMediaSourceFromUrl(Uri.Parse(videoUrL), "normal");

                SimpleExoPlayerView.Player = Player;
                Player.Prepare(VideoSource);
                Player.AddListener(PlayerListener);
                Player.PlayWhenReady = true;
                Player.AddVideoListener(this);

                bool haveResumePosition = ResumeWindow != C.IndexUnset;
                if (haveResumePosition)
                    Player.SeekTo(ResumeWindow, resumePosition);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void ReleaseVideo()
        {
            try
            {
                if (Player != null)
                {
                    SetStopvideo();

                    Player?.Release();
                    Player = null;

                    //GC Collecter
                    GC.Collect();
                }

                if (TimerAds != null)
                {
                    TimerAds.Enabled = false;
                    TimerAds.Stop();
                    TimerAds = null;
                }

                ReleaseAdsLoader();

                if (DownloadIcon.Tag.ToString() != "false")
                {
                    DownloadIcon.SetImageResource(Resource.Drawable.ic_action_download);
                    DownloadIcon.Tag = "false";
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void SetStopvideo()
        {
            try
            {
                if (SimpleExoPlayerView.Player != null)
                {
                    if (SimpleExoPlayerView.Player.PlaybackState == Com.Google.Android.Exoplayer2.Player.StateReady)
                    {
                        SimpleExoPlayerView.Player.PlayWhenReady = false;
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #region VideoObject player

        private IMediaSource CreateCacheMediaSource(IMediaSource videoSource, Uri videoUrL)
        {
            try
            {
                if (PlayerSettings.EnableOfflineMode)
                {
                    //Set the VideoObject for offline mode 
                    if (!string.IsNullOrEmpty(VideoData.VideoId))
                    {
                        var file = VideoDownloadAsyncController.GetDownloadedDiskVideoUri(VideoData.VideoId);

                        SimpleCache cache = new SimpleCache(ActivityContext.CacheDir, new LeastRecentlyUsedCacheEvictor(1024 * 1024 * 10));
                        CacheDataSourceFactory cacheDataSource = new CacheDataSourceFactory(cache, DefaultDataMediaFactory);

                        if (!string.IsNullOrEmpty(file))
                        {
                            videoUrL = Uri.Parse(file);

                            videoSource = GetMediaSourceFromUrl(videoUrL, "normal");
                            return videoSource;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return null;
            }
        }

        private IMediaSource CreateMediaSourceWithAds(IMediaSource videoSource, Uri imAdsUri)
        {
            try
            {
                // Player = ExoPlayerFactory.NewSimpleInstance(ActivityContext);
                SimpleExoPlayerView.Player = Player;

                if (ImaAdsLoader == null)
                {
                    //ImaAdsLoader = new ImaAdsLoader(ActivityContext, imAdsUri);
                    var imaSdkSettings = ImaSdkFactory.Instance.CreateImaSdkSettings();
                    imaSdkSettings.AutoPlayAdBreaks = true;
                    imaSdkSettings.DebugMode = true;

                    IAdDisplayContainer a = ImaSdkFactory.Instance.CreateAdDisplayContainer();
                    a.AdContainer = MainVideoFrameLayout;
                    ImaAdsLoader = new ImaAdsLoader(ActivityContext, imAdsUri);
                    ImaAdsLoader.SetPlayer(Player);

                    AdMediaSourceFactory adMediaSourceFactory = new AdMediaSourceFactory(this);

                    IMediaSource mediaSourceWithAds = new AdsMediaSource(
                        videoSource,
                        adMediaSourceFactory,
                        ImaAdsLoader,
                        SimpleExoPlayerView);

                    //Player.Prepare(mediaSourceWithAds);
                    //Player.AddListener(PlayerListener);
                    //Player.PlayWhenReady = true;

                    return mediaSourceWithAds;
                }

                return new AdsMediaSource(videoSource, new AdMediaSourceFactory(this), ImaAdsLoader, SimpleExoPlayerView);
            }
            catch (ClassNotFoundException e)
            {
                Console.WriteLine(e.Message);
                // IMA extension not loaded.
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        private class AdMediaSourceFactory : Java.Lang.Object, AdsMediaSource.IMediaSourceFactory
        {
            private readonly VideoController Activity;

            public AdMediaSourceFactory(VideoController activity)
            {
                Activity = activity;
            }

            public IMediaSource CreateMediaSource(Uri uri)
            {
                int type = Util.InferContentType(uri);
                var dataSourceFactory = new DefaultDataSourceFactory(Activity.ActivityContext, Util.GetUserAgent(Activity.ActivityContext, AppSettings.ApplicationName));
                switch (type)
                {
                    case C.TypeDash:
                        return new DashMediaSource.Factory(dataSourceFactory).SetTag("Ads").CreateMediaSource(uri);
                    case C.TypeSs:
                        return new SsMediaSource.Factory(dataSourceFactory).SetTag("Ads").CreateMediaSource(uri);
                    case C.TypeHls:
                        return new HlsMediaSource.Factory(dataSourceFactory).SetTag("Ads").CreateMediaSource(uri);
                    case C.TypeOther:
                        return new ExtractorMediaSource.Factory(dataSourceFactory).SetTag("Ads").CreateMediaSource(uri);
                    default:
                        return new ExtractorMediaSource.Factory(dataSourceFactory).SetTag("Ads").CreateMediaSource(uri);
                }
            }


            public int[] GetSupportedTypes()
            {
                return new[] { C.TypeDash, C.TypeSs, C.TypeHls, C.TypeOther };
            }
        }

        private IMediaSource GetMediaSourceFromUrl(Uri uri, string tag)
        {
            try
            {
                var mBandwidthMeter = new DefaultBandwidthMeter();
                DefaultDataSourceFactory dataSourceFactory = new DefaultDataSourceFactory(ActivityContext, Util.GetUserAgent(ActivityContext, AppSettings.ApplicationName), mBandwidthMeter);
                var buildHttpDataSourceFactory = new DefaultDataSourceFactory(ActivityContext, mBandwidthMeter, new DefaultHttpDataSourceFactory(Util.GetUserAgent(ActivityContext, AppSettings.ApplicationName), new DefaultBandwidthMeter()));
                var buildHttpDataSourceFactoryNull = new DefaultDataSourceFactory(ActivityContext, mBandwidthMeter, new DefaultHttpDataSourceFactory(Util.GetUserAgent(ActivityContext, AppSettings.ApplicationName), null));
                int type = Util.InferContentType(uri, null);
                var src = type switch
                {
                    C.TypeSs => new SsMediaSource.Factory(new DefaultSsChunkSource.Factory(buildHttpDataSourceFactory), buildHttpDataSourceFactoryNull).SetTag(tag).CreateMediaSource(uri),
                    C.TypeDash => new DashMediaSource.Factory(new DefaultDashChunkSource.Factory(buildHttpDataSourceFactory), buildHttpDataSourceFactoryNull).SetTag(tag).CreateMediaSource(uri),
                    C.TypeHls => new HlsMediaSource.Factory(buildHttpDataSourceFactory).SetTag(tag).CreateMediaSource(uri),
                    C.TypeOther => new ExtractorMediaSource.Factory(buildHttpDataSourceFactory).SetTag(tag).CreateMediaSource(uri),
                    _ => new ExtractorMediaSource.Factory(buildHttpDataSourceFactory).SetTag(tag).CreateMediaSource(uri)
                };
                return src;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        #endregion

        public void OnClick(View v)
        {
            try
            {
                if (v.Id == MFullScreenIcon.Id || v.Id == MFullScreenButton.Id)
                {
                    InitFullscreenDialog();
                }

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void ReleaseAdsLoader()
        {
            try
            {
                if (ImaAdsLoader == null) return;
                ImaAdsLoader.Release();
                ImaAdsLoader = null;
                SimpleExoPlayerView?.OverlayFrameLayout.RemoveAllViews();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void RestartPlayAfterShrinkScreen()
        {
            try
            {
                SimpleExoPlayerView.Player = null;
                if (FullScreenPlayerView != null)
                {
                    Player?.AddListener(PlayerListener);
                    Player.AddVideoListener(this);
                    SimpleExoPlayerView.Player = FullScreenPlayerView.Player;
                    SimpleExoPlayerView.Player.PlayWhenReady = true;
                    SimpleExoPlayerView.RequestFocus();
                    MFullScreenIcon.SetImageDrawable(ActivityContext.GetDrawable(Resource.Drawable.ic_action_ic_fullscreen_expand));
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void PlayFullScreen(VideoObject videoObject)
        {
            try
            {
                VideoData = videoObject;
                if (FullScreenPlayerView != null)
                {
                    Player.AddListener(PlayerListener);
                    Player.AddVideoListener(this);
                    FullScreenPlayerView.Player = Player;
                    if (FullScreenPlayerView.Player != null) FullScreenPlayerView.Player.PlayWhenReady = true;
                    MFullScreenIcon.SetImageDrawable(ActivityContext.GetDrawable(Resource.Drawable.ic_action_ic_fullscreen_skrink));
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #region Event 

        //Skip Ads
        private void BtnSkipIntroOnClick(object sender, EventArgs e)
        {
            try
            {
                if (Player != null)
                {
                    Player.Next();

                    ExoTopLayout.Visibility = ViewStates.Visible;
                    ExoEventButton.Visibility = ViewStates.Visible;
                    BtnSkipIntro.Visibility = ViewStates.Gone;
                    ExoTopAds.Visibility = ViewStates.Gone;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Open Link Ads
        private void ExoTopAdsOnClick(object sender, EventArgs e)
        {
            try
            {
                if (DataAdsVideo != null)
                {
                    string url = DataAdsVideo.Url;
                    Methods.App.OpenbrowserUrl(ActivityContext, url);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Full Screen
        public void InitFullscreenDialog(string action = "Open")
        {
            try
            {
                if (ActivityName == "FullScreen" || action != "Open")
                {
                    Intent intent = new Intent();
                    ActivityContext.SetResult(Result.Ok, intent);
                    ActivityContext.Finish();
                }
                else
                {
                    Intent intent = new Intent(ActivityContext, typeof(FullScreenVideoActivity));
                    FullScreenVideoActivity.SetVideoData(VideoData);
                    intent.PutExtra("Downloaded", DownloadIcon.Tag.ToString());
                    ActivityContext.StartActivityForResult(intent, 2000);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Menu VideoObject >> Report ,  Quality , Help , Make VideoObject offline
        private void Menu_button_Click(object sender, EventArgs e)
        {
            try
            {
                var activity = (AppCompatActivity)ActivityContext;
                var dialogFragment = new MoreMenuVideoDialogFragment();
                dialogFragment.Show(activity.SupportFragmentManager, dialogFragment.Tag);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Share
        private void ShareIcon_Click(object sender, EventArgs e)
        {
            try
            {
                LibrarySynchronizer.ShareVideo(VideoData);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Download
        public void Download_icon_Click(object sender, EventArgs e)
        {
            try
            {
                if (DownloadIcon.Tag.ToString() == "false")
                {
                    DownloadIcon.SetImageResource(Resource.Drawable.ic_action_download_stop);
                    DownloadIcon.Tag = "true";

                    if (VideoData.VideoLocation.Contains("youtube") || VideoData.VideoType.Contains("Youtube") || VideoData.VideoType.Contains("youtu"))
                    {
                        var urlVideo = VideoInfoRetriever.VideoDownloadstring;
                        if (!string.IsNullOrEmpty(urlVideo))
                        {
                            VideoControllers = new VideoDownloadAsyncController(urlVideo, VideoData.VideoId, ActivityContext, ActivityName);
                            if (!VideoControllers.CheckDownloadLinkIfExits())
                                VideoControllers.StartDownloadManager(VideoData.Title, VideoData, ActivityName);
                        }
                        else
                        {
                            Methods.DialogPopup.InvokeAndShowDialog(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_Error), ActivityContext.GetText(Resource.String.Lbl_You_can_not_Download_video), ActivityContext.GetText(Resource.String.Lbl_Ok));
                        }
                    }
                    else
                    {
                        VideoControllers = new VideoDownloadAsyncController(VideoData.VideoLocation, VideoData.VideoId, ActivityContext, ActivityName);
                        if (!VideoControllers.CheckDownloadLinkIfExits())
                            VideoControllers.StartDownloadManager(VideoData.Title, VideoData, ActivityName);
                    }
                }
                else if (DownloadIcon.Tag.ToString() == "Downloaded")
                {
                    try
                    {
                        AlertDialog.Builder builder = new AlertDialog.Builder(ActivityContext);
                        builder.SetTitle(ActivityContext.GetText(Resource.String.Lbl_Delete_video));
                        builder.SetMessage(ActivityContext.GetText(Resource.String.Lbl_Do_You_want_to_remove_video));

                        builder.SetPositiveButton(ActivityContext.GetText(Resource.String.Lbl_Yes), delegate
                        {
                            try
                            {
                                VideoDownloadAsyncController.RemoveDiskVideoFile(VideoData.VideoId + ".mp4");
                                DownloadIcon.SetImageResource(Resource.Drawable.ic_action_download);
                                DownloadIcon.Tag = "false";
                            }
                            catch (Exception exception)
                            {
                                Console.WriteLine(exception);
                            }
                        });

                        builder.SetNegativeButton(ActivityContext.GetText(Resource.String.Lbl_No), delegate { });

                        var alert = builder.Create();
                        alert.Show();
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                    }
                }
                else
                {
                    DownloadIcon.SetImageResource(Resource.Drawable.ic_action_download);
                    DownloadIcon.Tag = "false";
                    VideoControllers.StopDownloadManager();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Back
        private void BackIcon_Click(object sender, EventArgs e)
        {
            try
            {
                if (ActivityName == "FullScreen")
                {
                    Intent intent = new Intent();
                    ActivityContext.SetResult(Result.Ok, intent);
                    ActivityContext.Finish();
                }
                else if (ActivityName == "Main")
                {
                    ReleaseVideo();
                    ActivityContext.Finish();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        public void ShowRestrictedVideoFragment(RestrictedVideoFragment restrictedVideoPlayerFragment, Activity activity, string type)
        {
            try
            {
                FragmentTransaction ft = null;
                switch (activity)
                {
                    case GlobalPlayerActivity act:
                        ft = act.SupportFragmentManager.BeginTransaction();
                        break;
                    case TabbedMainActivity act2:
                        ft = act2.SupportFragmentManager.BeginTransaction();
                        break;
                }

                SimpleExoPlayerView.Visibility = ViewStates.Gone;
                ReleaseVideo();

                if (restrictedVideoPlayerFragment == null)
                {
                    restrictedVideoPlayerFragment = new RestrictedVideoFragment();
                }

                if (restrictedVideoPlayerFragment.IsAdded)
                {
                    ft?.Show(restrictedVideoPlayerFragment).Commit();
                    restrictedVideoPlayerFragment.LoadRestriction(type, VideoData.Thumbnail, VideoData);
                }
                else
                {
                    ft?.Add(Resource.Id.root, restrictedVideoPlayerFragment, DateTime.Now.ToString(CultureInfo.InvariantCulture)).Commit();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void RunVideoWithAds(IMediaSource videoSource, bool showAds)
        {
            try
            {
                var isPro = ListUtils.MyChannelList.FirstOrDefault()?.IsPro ?? "0";
                if (isPro == "0" && ListUtils.AdsVideoList.Count > 0 && Methods.CheckConnectivity() && showAds)
                {
                    Random rand = new Random();

                    var playPos = rand.Next(ListUtils.AdsVideoList.Count - 1 + 1);
                    DataAdsVideo = ListUtils.AdsVideoList[playPos];

                    var type = Methods.AttachmentFiles.Check_FileExtension(DataAdsVideo?.Media);
                    if (type == "Video" && DataAdsVideo != null)
                    {
                        //AppSettings.ShowButtonSkip = DataAdsVideo
                        var adVideoSource = GetMediaSourceFromUrl(Uri.Parse(DataAdsVideo.Media), "Ads");
                        if (adVideoSource != null)
                        {
                            ListUtils.AdsVideoList.Remove(DataAdsVideo);

                            // Plays the first video, then the second video.
                            ConcatenatingMediaSource concatenatedSource = new ConcatenatingMediaSource(adVideoSource, videoSource);
                            SimpleExoPlayerView.Player = Player;
                            Player.Prepare(concatenatedSource);
                            Player.AddListener(PlayerListener);
                            Player.PlayWhenReady = true;
                            Player.AddVideoListener(this);

                            ExoTopLayout.Visibility = ViewStates.Gone;
                            ExoEventButton.Visibility = ViewStates.Invisible;
                            BtnSkipIntro.Visibility = ViewStates.Visible;
                            ExoTopAds.Visibility = ViewStates.Visible;

                            BtnSkipIntro.Text = AppSettings.ShowButtonSkip.ToString();
                            BtnSkipIntro.Enabled = false;

                            RunTimer();
                        }
                        else
                        {
                            SimpleExoPlayerView.Player = Player;
                            Player.Prepare(videoSource);
                            Player.AddListener(PlayerListener);
                            Player.AddVideoListener(this);
                            Player.PlayWhenReady = true;


                            ExoTopLayout.Visibility = ViewStates.Visible;
                            ExoEventButton.Visibility = ViewStates.Visible;
                            BtnSkipIntro.Visibility = ViewStates.Gone;
                            ExoTopAds.Visibility = ViewStates.Gone;
                        }
                    }
                    else
                    {

                        SimpleExoPlayerView.Player = Player;

                        Player.Prepare(videoSource);
                        Player.AddListener(PlayerListener);
                        Player.PlayWhenReady = true;
                        Player.AddVideoListener(this);

                        ExoTopLayout.Visibility = ViewStates.Visible;
                        ExoEventButton.Visibility = ViewStates.Visible;
                        BtnSkipIntro.Visibility = ViewStates.Gone;
                        ExoTopAds.Visibility = ViewStates.Gone;
                    }
                }
                else
                {

                    SimpleExoPlayerView.Player = Player;

                    Player.Prepare(videoSource);
                    Player.AddListener(PlayerListener);
                    Player.AddVideoListener(this);
                    Player.PlayWhenReady = true;


                    ExoTopLayout.Visibility = ViewStates.Visible;
                    ExoEventButton.Visibility = ViewStates.Visible;
                    BtnSkipIntro.Visibility = ViewStates.Gone;
                    ExoTopAds.Visibility = ViewStates.Gone;
                }

                bool haveResumePosition = ResumeWindow != C.IndexUnset;
                if (haveResumePosition)
                    Player.SeekTo(ResumeWindow, ResumePosition);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void RunTimer()
        {
            try
            {
                TimerAds = new Timer { Interval = 1000 };
                TimerAds.Elapsed += TimerAdsOnElapsed;
                TimerAds.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private int CountShow = AppSettings.ShowButtonSkip;
        private void TimerAdsOnElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                ActivityContext.RunOnUiThread(() =>
                {
                    if (CountShow == 0)
                    {
                        SetTextSkipIntro();

                        BtnSkipIntro.Enabled = true;

                        TimerAds.Enabled = false;
                        TimerAds.Stop();
                        TimerAds = null;
                    }
                    else if (CountShow > 0)
                    {
                        CountShow--;
                        BtnSkipIntro.Text = CountShow.ToString();
                    }
                    else
                    {
                        SetTextSkipIntro();
                        BtnSkipIntro.Enabled = true;

                        TimerAds.Enabled = false;
                        TimerAds.Stop();
                        TimerAds = null;
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void SetTextSkipIntro()
        {
            try
            {
                Typeface font = Typeface.CreateFromAsset(Application.Context.Resources.Assets, "ionicons.ttf");

                BtnSkipIntro.Gravity = GravityFlags.CenterHorizontal;
                BtnSkipIntro.SetTypeface(font, TypefaceStyle.Normal);
                var woTextDecorator = new TextDecorator
                {
                    Content = ActivityContext.GetText(Resource.String.Lbl_SkipAds) + " " + IonIconsFonts.IosArrowForward,
                    DecoratedContent = new Android.Text.SpannableString(ActivityContext.GetText(Resource.String.Lbl_SkipAds) + " " + IonIconsFonts.IosArrowForward)
                };
                woTextDecorator.SetTextColor(IonIconsFonts.AndroidArrowForward, "#ffffff");
                woTextDecorator.Build(BtnSkipIntro, woTextDecorator.DecoratedContent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnAdClicked()
        {

        }

        public void OnAdLoadError(AdsMediaSource.AdLoadException error, DataSpec dataSpec)
        {

        }


        public void OnAdPlaybackState(AdPlaybackState adPlaybackState)
        {

        }

        public void OnAdTapped()
        {

        }

        public View[] GetAdOverlayViews()
        {
            return SimpleExoPlayerView.GetAdOverlayViews();
        }

        public void ToggleExoPlayerKeepScreenOnFeature(bool keepScreenOn)
        {
            if (SimpleExoPlayerView != null)
            {
                SimpleExoPlayerView.KeepScreenOn = keepScreenOn;
            }

            if (FullScreenPlayerView != null)
            {
                FullScreenPlayerView.KeepScreenOn = keepScreenOn;
            }
        }

        public ViewGroup AdViewGroup { get; }


        /*, Com.Google.Android.Exoplayer2.Video.IVideoListener*/
        public void OnRenderedFirstFrame()
        {
            try
            {

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnSurfaceSizeChanged(int width, int height)
        {
            try
            {
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnVideoSizeChanged(int width, int height, int unappliedRotationDegrees, float pixelWidthHeightRatio)
        {
            try
            {
                if (!AppSettings.ShowVideoWithDynamicHeight)
                {
                    return;
                }

                if (height > width)
                {
                    height = GetPortraitHeight(height);
                }
                // Get layout params of view
                // Use MyView.this to refer to the current MyView instance 
                // inside a callback
                var p = MainRoot.LayoutParameters;
                int currWidth = MainRoot.Width;

                // Set new width/height of view
                // height or width must be cast to float as int/int will give 0
                // and distort view, e.g. 9/16 = 0 but 9.0/16 = 0.5625.
                // p.height is int hence the final cast to int. 
                p.Width = currWidth;
                p.Height = (int)((float)height / width * currWidth);

                // Redraw myView
                MainRoot.RequestLayout();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private int GetPortraitHeight(int videoHeight)
        {
            var screenHeight = GetScreenHeight();
            var maxHeightToShow = (3 * screenHeight / 4);
            var minimumHeight = CovertDpToPixel(220);
            if (maxHeightToShow <= minimumHeight)
            {
                maxHeightToShow = minimumHeight;
            }

            return videoHeight > maxHeightToShow ? maxHeightToShow : videoHeight;
        }

        public int GetScreenHeight()
        {
            var displayMetrics = new DisplayMetrics();
            ActivityContext.WindowManager.DefaultDisplay.GetMetrics(displayMetrics);
            return (int)(displayMetrics.HeightPixels / displayMetrics.Density);
        }

        public int CovertDpToPixel(int dp)
        {
            var displayMetrics = new DisplayMetrics();
            ActivityContext.WindowManager.DefaultDisplay.GetMetrics(displayMetrics);
            return (int)(dp * displayMetrics.Density);
        }
    }
}