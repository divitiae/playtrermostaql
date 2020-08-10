using System;
using System.Linq;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using PlayTube.Activities.Channel.Tab;
using PlayTube.Activities.SettingsPreferences;
using PlayTube.Activities.Tabbes;
using PlayTube.Adapters;
using PlayTube.Helpers.Ads;
using PlayTube.Helpers.CacheLoaders;
using PlayTube.Helpers.Controller;
using PlayTube.Helpers.Fonts;
using PlayTube.Helpers.Models;
using PlayTube.Helpers.Utils;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Fragment = Android.Support.V4.App.Fragment;

namespace PlayTube.Activities.Channel
{
    public class MyChannelFragment : Fragment
    {
        #region Variables Basic

        public ChPlayListFragment PlayListFragment;
        public ChVideosFragment VideosFragment;
        public ChActivitiesFragment ActivitiesFragment;
        private AppBarLayout AppBarLayout;
        private TabLayout Tabs;
        private ViewPager ViewPagerView;
        private Toolbar MainToolbar;
        private ImageView ImageChannel, ImageCoverChannel, IconSettings;
        private CollapsingToolbarLayout CollapsingToolbar;
        private TextView ChannelNameText, ChannelVerifiedText ;
        private TabbedMainActivity MainContext;
        private Button SubscribeChannelButton;
         
        #endregion

        #region General

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            MainContext = (TabbedMainActivity)Activity;
            HasOptionsMenu = true;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                // Use this to return your custom view for this Fragment
                View view = inflater.Inflate(Resource.Layout.MyChannelFragment_Layout, container, false);

                //Get Value And Set Toolbar
                InitComponent(view);
                InitToolbar(view);
                InitTab();
                GetDataChannelApi();

                SubscribeChannelButton.Click += SubscribeChannelButtonOnClick;
                IconSettings.Click += IconSettingsOnClick;

                AdsGoogle.Ad_Interstitial(MainContext);
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
                ImageCoverChannel = view.FindViewById<ImageView>(Resource.Id.myImagevideo);
                ImageChannel = view.FindViewById<ImageView>(Resource.Id.myChannelImage);
                CollapsingToolbar = view.FindViewById<CollapsingToolbarLayout>(Resource.Id.mycollapsingToolbar);
                ChannelNameText = view.FindViewById<TextView>(Resource.Id.myChannelName);
                ChannelVerifiedText = view.FindViewById<TextView>(Resource.Id.myChannelVerifiedText);
                IconSettings = view.FindViewById<ImageView>(Resource.Id.mySettings_icon);
                SubscribeChannelButton = view.FindViewById<Button>(Resource.Id.mySubcribeChannelButton);

                Tabs = view.FindViewById<TabLayout>(Resource.Id.mychanneltabs);
                ViewPagerView = view.FindViewById<ViewPager>(Resource.Id.myChannelviewpager);
                AppBarLayout = view.FindViewById<AppBarLayout>(Resource.Id.mymainAppBarLayout);
                AppBarLayout.SetExpanded(true);

                SubscribeChannelButton.Text = GetText(Resource.String.Lbl_Edit);

                ChannelVerifiedText.Visibility = ViewStates.Gone;
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, ChannelVerifiedText,IonIconsFonts.CheckmarkCircled);
               
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
                MainToolbar = view.FindViewById<Toolbar>(Resource.Id.toolbar);
                MainContext.SetToolBar(MainToolbar, " ",false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void InitTab()
        {
            try
            {
                ViewPagerView.OffscreenPageLimit = 3;
                SetUpViewPager(ViewPagerView);
                Tabs.SetupWithViewPager(ViewPagerView);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
          
        #endregion

        #region Tab

        private void SetUpViewPager(ViewPager viewPager)
        {
            try
            {
                PlayListFragment = new ChPlayListFragment();
                VideosFragment = new ChVideosFragment();
                ChAboutFragment aboutFragment = new ChAboutFragment();
                ActivitiesFragment = new ChActivitiesFragment();
                 
                Bundle bundle = new Bundle(); 
                bundle.PutString("ChannelId", UserDetails.UserId);

                PlayListFragment.Arguments = bundle;
                VideosFragment.Arguments = bundle;
                ActivitiesFragment.Arguments = bundle;
                aboutFragment.Arguments = bundle;

                MainTabAdapter adapter = new MainTabAdapter(Activity.SupportFragmentManager);
                adapter.AddFragment(VideosFragment, GetText(Resource.String.Lbl_Videos));
                adapter.AddFragment(PlayListFragment, GetText(Resource.String.Lbl_PlayLists));
                adapter.AddFragment(ActivitiesFragment, GetText(Resource.String.Lbl_Activities));
                adapter.AddFragment(aboutFragment, GetText(Resource.String.Lbl_AboutChannal));

                viewPager.PageSelected += ViewPagerOnPageSelected; 
                viewPager.Adapter = adapter;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void ViewPagerOnPageSelected(object sender, ViewPager.PageSelectedEventArgs page)
        {
            try
            {
                var p = page.Position;
                if (p == 0)
                {

                }
                else if (p == 1)
                {
                    PlayListFragment.StartApiService();
                }
                else if (p == 2)
                {

                }
                else if (p == 3)
                {

                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
        #endregion
         
        #region Get Data Channel

        public async void GetDataChannelApi()
        {
            try
            {
                if (ListUtils.MyChannelList.Count == 0)
                    await ApiRequest.GetChannelData(Activity, UserDetails.UserId);

                var dataChannel = ListUtils.MyChannelList.FirstOrDefault();
                if (dataChannel != null)
                {
                    var name = AppTools.GetNameFinal(dataChannel);

                    CollapsingToolbar.Title = name;
                    ChannelNameText.Text = name;
                     
                    GlideImageLoader.LoadImage(Activity, dataChannel.Avatar, ImageChannel, ImageStyle.CircleCrop, ImagePlaceholders.Drawable); 
                    Glide.With(this).Load(dataChannel.Cover).Apply(new RequestOptions().FitCenter()).Into(ImageCoverChannel);

                    if (dataChannel.Verified == "1")
                        ChannelVerifiedText.Visibility = ViewStates.Visible;

                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion
         
        #region Events

        private void SubscribeChannelButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                var intent = new Intent(Activity, typeof(EditMyChannelActivity));
                Activity.StartActivityForResult(intent,252);

                MainContext.VideoActionsController.SetStopvideo();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void IconSettingsOnClick(object sender, EventArgs e)
        {
            try
            {
                var intent = new Intent(Activity, typeof(SettingsActivity));
                Activity.StartActivity(intent);
                MainContext.VideoActionsController.SetStopvideo();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
        #endregion

    }
}