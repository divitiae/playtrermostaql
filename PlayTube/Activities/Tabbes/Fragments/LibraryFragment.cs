using System;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Sothree.Slidinguppanel;
using PlayTube.Activities.Library;
using PlayTube.Adapters;
using PlayTube.Helpers.Fonts;
using Fragment = Android.Support.V4.App.Fragment;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace PlayTube.Activities.Tabbes.Fragments
{
    public class LibraryFragment : Fragment
    {
        #region Variables Basic

        public LibraryAdapter MAdapter;
        private RecyclerView MRecycler;
        private LinearLayoutManager LayoutManager;
        private TabbedMainActivity GlobalContext;
        public TextView NotificationButton;
        public SubscriptionsFragment SubscriptionsFragment;
        private WatchLaterVideosFragment WatchLaterVideosFragment;
        private RecentlyWatchedVideosFragment RecentlyWatchedVideosFragment;
        private WatchOfflineVideosFragment WatchOfflineVideosFragment;
        public PlayListsVideosFragment PlayListsVideosFragment;
        private LikedVideosFragment LikedVideosFragment;
        private SharedVideosFragment SharedVideosFragment;
        private PaidVideosFragment PaidVideosFragment;

        #endregion

        #region General

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            GlobalContext = (TabbedMainActivity)Activity;
            HasOptionsMenu = true;

            if (MAdapter == null)
                MAdapter = new LibraryAdapter(Activity);

        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                // Use this to return your custom view for this Fragment
                View view = inflater.Inflate(Resource.Layout.Library_Layout, container, false);

                //Get Value And Set Toolbar
                InitComponent(view);
                InitToolbar(view);
                SetRecyclerViewAdapters();

                MAdapter.ItemClick += MAdapterOnItemClick;
                NotificationButton.Click += NotificationButton_Click;
                  
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

                NotificationButton = (TextView)view.FindViewById(Resource.Id.toolbar_title);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, NotificationButton, IonIconsFonts.AndroidNotifications);
                NotificationButton.SetTextColor(Color.White);
                NotificationButton.SetTextSize(ComplexUnitType.Sp, 20f);
                NotificationButton.Visibility = ViewStates.Visible;
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
                LayoutManager = new LinearLayoutManager(Context);
                MRecycler.SetLayoutManager(LayoutManager); 
                MRecycler.SetAdapter(MAdapter);
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
                    string title = Context.GetString(Resource.String.Lbl_Library);
                    GlobalContext.SetToolBar(toolbar, title,false);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Events

          
        private void MAdapterOnItemClick(object sender, LibraryAdapterClickEventArgs e)
        {
            try
            {
                if (e.Position <= -1) return;

                var item = MAdapter.GetItem(e.Position);
                if (item != null)
                {
                    if (item.SectionId == "1") // Subscriptions
                    {
                        SubscriptionsFragment = new SubscriptionsFragment();
                        GlobalContext.FragmentBottomNavigator.DisplayFragment(SubscriptionsFragment);
                    }
                    else if (item.SectionId == "2") // Watch Later
                    {
                        WatchLaterVideosFragment = new WatchLaterVideosFragment();
                        GlobalContext.FragmentBottomNavigator.DisplayFragment(WatchLaterVideosFragment);

                    }
                    else if (item.SectionId == "3") // Recently Watched 
                    {
                        RecentlyWatchedVideosFragment = new RecentlyWatchedVideosFragment();
                        GlobalContext.FragmentBottomNavigator.DisplayFragment(RecentlyWatchedVideosFragment);
                    }
                    else if (item.SectionId == "4") // Watch Offline 
                    {
                        WatchOfflineVideosFragment = new WatchOfflineVideosFragment();
                        GlobalContext.FragmentBottomNavigator.DisplayFragment(WatchOfflineVideosFragment);

                    }
                    else if (item.SectionId == "5") // PlayLists
                    {
                        PlayListsVideosFragment = new PlayListsVideosFragment();
                        GlobalContext.FragmentBottomNavigator.DisplayFragment(PlayListsVideosFragment);
                    }
                    else if (item.SectionId == "6") // Liked
                    {
                        LikedVideosFragment = new LikedVideosFragment();
                        GlobalContext.FragmentBottomNavigator.DisplayFragment(LikedVideosFragment);
                    }
                    else if (item.SectionId == "7") // Shared
                    {
                        SharedVideosFragment = new SharedVideosFragment();
                        GlobalContext.FragmentBottomNavigator.DisplayFragment(SharedVideosFragment);
                    }
                    else if (item.SectionId == "8") // Paid
                    {
                        PaidVideosFragment = new PaidVideosFragment();
                        GlobalContext.FragmentBottomNavigator.DisplayFragment(PaidVideosFragment);
                    }

                    if (GlobalContext.SlidingUpPanel.GetPanelState() == SlidingUpPanelLayout.PanelState.Expanded)
                        GlobalContext.SlidingUpPanel.SetPanelState(SlidingUpPanelLayout.PanelState.Collapsed); 
                } 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void NotificationButton_Click(object sender, EventArgs e)
        {
            try
            {
                NotificationFragment notificationFragment = new NotificationFragment();
                GlobalContext.FragmentBottomNavigator.DisplayFragment(notificationFragment);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }


        #endregion

    }
}    