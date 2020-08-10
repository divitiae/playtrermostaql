using Android.App;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Com.Airbnb.Lottie;
using PlayTube.Activities.Tabbes;
using System;
using System.Collections.Generic;
using PlayTube.Helpers.Models;
using Fragment = Android.Support.V4.App.Fragment;
using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;
using FragmentManager = Android.Support.V4.App.FragmentManager;

namespace PlayTube.Activities.Models
{
    public class CustomNavigationController : Java.Lang.Object, View.IOnClickListener
    {
        private readonly Activity MainContext;

        private FrameLayout NotificationButton;
        private LinearLayout HomeButton, ProfileButton, TrendButton, BlogButton;
        private ImageView HomeImage, NotificationImage, TrendImage, BlogImage;
        public ImageView ProfileImage;
        private int PageNumber;


        public readonly List<Fragment> FragmentListTab0 = new List<Fragment>();
        public readonly List<Fragment> FragmentListTab1 = new List<Fragment>();
        public readonly List<Fragment> FragmentListTab2 = new List<Fragment>();
        public readonly List<Fragment> FragmentListTab3 = new List<Fragment>();
        public readonly List<Fragment> FragmentListTab4 = new List<Fragment>();

        private readonly TabbedMainActivity Context;
        private LinearLayout MainLayout;

        public CustomNavigationController(Activity activity)
        {
            MainContext = activity;

            if(activity is TabbedMainActivity cont)
                Context = cont;

            Initialize();
        }


        public void Initialize()
        {
            try
            {
                MainLayout = MainContext.FindViewById<LinearLayout>(Resource.Id.llMain);
                HomeButton = MainContext.FindViewById<LinearLayout>(Resource.Id.llHome);
                TrendButton = MainContext.FindViewById<LinearLayout>(Resource.Id.llTrend);
                BlogButton = MainContext.FindViewById<LinearLayout>(Resource.Id.llBlog);
                NotificationButton = MainContext.FindViewById<FrameLayout>(Resource.Id.llNotification);
                ProfileButton = MainContext.FindViewById<LinearLayout>(Resource.Id.llProfile);
               
                if (!UserDetails.IsLogin)
                {
                    BlogButton.Visibility = ViewStates.Gone;
                    NotificationButton.Visibility = ViewStates.Gone;
                    ProfileButton.Visibility = ViewStates.Gone;

                    MainLayout.WeightSum = 2; 
                }
                else
                {
                    if (!AppSettings.ShowArticle)
                    {
                        BlogButton.Visibility = ViewStates.Gone;
                        MainLayout.WeightSum = 4;
                    } 
                }
                 
                HomeImage = MainContext.FindViewById<ImageView>(Resource.Id.ivHome);
                NotificationImage = MainContext.FindViewById<ImageView>(Resource.Id.ivNotification);
                ProfileImage = MainContext.FindViewById<ImageView>(Resource.Id.ivProfile);
                TrendImage = MainContext.FindViewById<ImageView>(Resource.Id.ivTrend);
                BlogImage = MainContext.FindViewById<ImageView>(Resource.Id.ivBlog);

                HomeButton.SetOnClickListener(this);
                TrendButton.SetOnClickListener(this);

                if (UserDetails.IsLogin)
                {
                    if (AppSettings.ShowArticle)
                        BlogButton.SetOnClickListener(this);

                    NotificationButton.SetOnClickListener(this);
                    ProfileButton.SetOnClickListener(this);
                } 
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.llHome:
                    EnableNavigationButton(HomeImage);
                    PageNumber = 0;
                    ShowFragment0();
                    break;

                case Resource.Id.llTrend:
                    EnableNavigationButton(TrendImage);
                    PageNumber = 1;
                    ShowFragment1();
                    break;
                case Resource.Id.llBlog:
                    EnableNavigationButton(BlogImage);
                    PageNumber = 2;
                    ShowFragment2();
                    break;
                case Resource.Id.llNotification:
                    EnableNavigationButton(NotificationImage);
                    PageNumber = 3;
                    ShowFragment3();
                   
                    break;
                case Resource.Id.llProfile:
                    EnableNavigationButton(ProfileImage);
                    PageNumber = 4;
                    ShowFragment4();
                    
                    break;
            }
        }

        public void EnableNavigationButton(ImageView image)
        {
            DisableAllNavigationButton();
            image.Background = MainContext.GetDrawable(Resource.Drawable.shape_bg_bottom_navigation);

            if (image.Id == ProfileImage.Id)
                return;

            image.SetColorFilter(Color.ParseColor(AppSettings.MainColor));

        }

        public void DisableAllNavigationButton()
        {
            HomeImage.Background = null;
            HomeImage.SetColorFilter(AppSettings.SetTabDarkTheme ? Color.White : Color.Black);

            NotificationImage.Background = null;
            NotificationImage.SetColorFilter(AppSettings.SetTabDarkTheme ? Color.White : Color.Black);

            ProfileImage.Background = null;
            //ProfileImage.SetColorFilter(AppSettings.SetTabDarkTheme ? Color.White : Color.Black);

            TrendImage.Background = null;
            TrendImage.SetColorFilter(AppSettings.SetTabDarkTheme ? Color.White : Color.Black);

            BlogImage.Background = null;
            BlogImage.SetColorFilter(AppSettings.SetTabDarkTheme ? Color.White : Color.Black);

        }

        public void ShowNotificationBadge(bool showBadge)
        {
            LottieAnimationView animationView2 = MainContext.FindViewById<LottieAnimationView>(Resource.Id.animation_view2);

            if (showBadge)
            {
                NotificationImage.SetImageDrawable(null);

                animationView2.SetAnimation("NotificationLotti.json");
                animationView2.PlayAnimation();
            }
            else
            {
                animationView2.Progress = 0;
                animationView2.CancelAnimation();
                NotificationImage.SetImageResource(Resource.Drawable.icon_notification_vector);
            }
        }

        public Fragment GetSelectedTabBackStackFragment()
        {
            switch (PageNumber)
            {
                case 0:
                    {
                        var currentFragment = FragmentListTab0[FragmentListTab0.Count - 2];
                        if (currentFragment != null)
                            return currentFragment;
                        break;
                    }
                case 1:
                    {
                        var currentFragment = FragmentListTab1[FragmentListTab1.Count - 2];
                        if (currentFragment != null)
                            return currentFragment;
                        break;
                    }
                case 2:
                    {
                        var currentFragment = FragmentListTab2[FragmentListTab2.Count - 2];
                        if (currentFragment != null)
                            return currentFragment;
                        break;
                    }
                case 3:
                    {
                        var currentFragment = FragmentListTab3[FragmentListTab3.Count - 2];
                        if (currentFragment != null)
                            return currentFragment;
                        break;
                    }
                case 4:
                    {
                        var currentFragment = FragmentListTab4[FragmentListTab4.Count - 2];
                        if (currentFragment != null)
                            return currentFragment;
                        break;
                    }

                default:
                    return null;

            }

            return null;
        }

        public int GetCountFragment()
        {
            try
            {
                switch (PageNumber)
                {
                    case 0:
                        return FragmentListTab0.Count > 1 ? FragmentListTab0.Count : 0;
                    case 1:
                        return FragmentListTab1.Count > 1 ? FragmentListTab1.Count : 0;
                    case 2:
                        return FragmentListTab2.Count > 1 ? FragmentListTab2.Count : 0;
                    case 3:
                        return FragmentListTab3.Count > 1 ? FragmentListTab4.Count : 0;
                    case 4:
                        return FragmentListTab4.Count > 1 ? FragmentListTab4.Count : 0;
                    default:
                        return 0;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 0;
            }
        }

        public static void HideFragmentFromList(List<Fragment> fragmentList, FragmentTransaction ft)
        {
            try
            {
                if (fragmentList.Count < 0) 
                    return;

                foreach (var fra in fragmentList)
                {
                    if (fra.IsVisible)
                        ft.Hide(fra);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void DisplayFragment(Fragment newFragment)
        {
            try
            {
                FragmentTransaction ft = Context.SupportFragmentManager.BeginTransaction();

                HideFragmentFromList(FragmentListTab0, ft);
                HideFragmentFromList(FragmentListTab1, ft);
                HideFragmentFromList(FragmentListTab2, ft);
                HideFragmentFromList(FragmentListTab3, ft);
                HideFragmentFromList(FragmentListTab4, ft);

                if (PageNumber == 0)
                    if (!FragmentListTab0.Contains(newFragment))
                        FragmentListTab0.Add(newFragment);

                if (PageNumber == 1)
                    if (!FragmentListTab1.Contains(newFragment))
                        FragmentListTab1.Add(newFragment);

                if (PageNumber == 2)
                    if (!FragmentListTab2.Contains(newFragment))
                        FragmentListTab2.Add(newFragment);

                if (PageNumber == 3)
                    if (!FragmentListTab3.Contains(newFragment))
                        FragmentListTab3.Add(newFragment);

                if (PageNumber == 4)
                    if (!FragmentListTab4.Contains(newFragment))
                        FragmentListTab4.Add(newFragment);

                if (!newFragment.IsAdded)
                    ft.Add(Resource.Id.mainFragmentHolder, newFragment, newFragment.Id.ToString());

                ft.Show(newFragment).Commit();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void RemoveFragment(Fragment oldFragment)
        {
            try
            {
                FragmentTransaction ft = Context.SupportFragmentManager.BeginTransaction();

                if (PageNumber == 0)
                    if (FragmentListTab0.Contains(oldFragment))
                        FragmentListTab0.Remove(oldFragment);

                if (PageNumber == 1)
                    if (FragmentListTab1.Contains(oldFragment))
                        FragmentListTab1.Remove(oldFragment);

                if (PageNumber == 2)
                    if (FragmentListTab2.Contains(oldFragment))
                        FragmentListTab2.Remove(oldFragment);

                if (PageNumber == 3)
                    if (FragmentListTab3.Contains(oldFragment))
                        FragmentListTab3.Remove(oldFragment);

                if (PageNumber == 4)
                    if (FragmentListTab4.Contains(oldFragment))
                        FragmentListTab4.Remove(oldFragment);


                HideFragmentFromList(FragmentListTab0, ft);
                HideFragmentFromList(FragmentListTab1, ft);
                HideFragmentFromList(FragmentListTab2, ft);
                HideFragmentFromList(FragmentListTab3, ft);
                HideFragmentFromList(FragmentListTab4, ft);

                if (oldFragment.IsAdded)
                    ft.Remove(oldFragment);

                switch (PageNumber)
                {
                    case 0:
                        {
                            var currentFragment = FragmentListTab0[FragmentListTab0.Count - 1];
                            ft.Show(currentFragment).Commit();
                            break;
                        }
                    case 1:
                        {
                            var currentFragment = FragmentListTab1[FragmentListTab1.Count - 1];
                            ft.Show(currentFragment).Commit();
                            break;
                        }
                    case 2:
                        {
                            var currentFragment = FragmentListTab2[FragmentListTab2.Count - 1];
                            ft.Show(currentFragment).Commit();
                            break;
                        }
                    case 3:
                        {
                            var currentFragment = FragmentListTab3[FragmentListTab3.Count - 1];
                            ft.Show(currentFragment).Commit();
                            break;
                        }
                    case 4:
                        {
                            var currentFragment = FragmentListTab4[FragmentListTab4.Count - 1];
                            ft.Show(currentFragment).Commit();
                            break;
                        }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnBackStackClickFragment()
        {
            try
            {
                if (PageNumber == 0)
                {
                    if (FragmentListTab0.Count > 1)
                    {
                        var currentFragment = FragmentListTab0[FragmentListTab0.Count - 1];
                        if (currentFragment != null)
                            RemoveFragment(currentFragment);
                    }
                    else
                    {
                        Context.Finish();
                    }
                }
                else if (PageNumber == 1)
                {
                    if (FragmentListTab1.Count > 1)
                    {
                        var currentFragment = FragmentListTab1[FragmentListTab1.Count - 1];
                        if (currentFragment != null)
                            RemoveFragment(currentFragment);
                    }
                    else
                    {
                        Context.Finish();
                    }

                }
                else if (PageNumber == 2)
                {
                    if (FragmentListTab2.Count > 1)
                    {
                        var currentFragment = FragmentListTab2[FragmentListTab2.Count - 1];
                        if (currentFragment != null)
                            RemoveFragment(currentFragment);
                    }
                    else
                    {
                        Context.Finish();
                    }
                }
                else if (PageNumber == 3)
                {
                    if (FragmentListTab3.Count > 1)
                    {
                        var currentFragment = FragmentListTab3[FragmentListTab3.Count - 1];
                        if (currentFragment != null)
                            RemoveFragment(currentFragment);
                    }
                    else
                    {
                        Context.Finish();
                    }
                }
                else if (PageNumber == 4)
                {
                    if (FragmentListTab4.Count > 1)
                    {
                        var currentFragment = FragmentListTab4[FragmentListTab4.Count - 1];
                        if (currentFragment != null)
                            RemoveFragment(currentFragment);
                    }
                    else
                    {
                        Context.Finish();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void ShowFragment0()
        {
            try
            {
                if (FragmentListTab0.Count <= 0) 
                    return;
                var currentFragment = FragmentListTab0[FragmentListTab0.Count - 1];
                if (currentFragment != null)
                    DisplayFragment(currentFragment);

                ToggleFloatingButtonVisibility();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void ShowFragment1()
        {
            try
            {
                if (FragmentListTab1.Count <= 0) return;
                var currentFragment = FragmentListTab1[FragmentListTab1.Count - 1];
                if (currentFragment != null)
                    DisplayFragment(currentFragment);

                ToggleFloatingButtonVisibility();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void ShowFragment2()
        {
            try
            {
                if (FragmentListTab2.Count <= 0) return;
                var currentFragment = FragmentListTab2[FragmentListTab2.Count - 1];
                if (currentFragment != null)
                    DisplayFragment(currentFragment);

                ToggleFloatingButtonVisibility();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void ShowFragment3()
        {
            try
            {
                if (FragmentListTab3.Count <= 0) return;
                var currentFragment = FragmentListTab3[FragmentListTab3.Count - 1];
                if (currentFragment != null)
                    DisplayFragment(currentFragment);

                ToggleFloatingButtonVisibility();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void ShowFragment4()
        {
            try
            {
                if (FragmentListTab4.Count <= 0) return;
                var currentFragment = FragmentListTab4[FragmentListTab4.Count - 1];
                if (currentFragment != null)
                    DisplayFragment(currentFragment);

                ToggleFloatingButtonVisibility();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void ToggleFloatingButtonVisibility()
        {
            Context.MoreMultiButtons.Visibility = ShowMoreMenuButton() ? ViewStates.Visible : ViewStates.Gone;
        }

        public bool ShowMoreMenuButton()
        {
            return PageNumber == 4;
        }

        public static bool BringFragmentToTop(Fragment Tobeshown, FragmentManager fragmentManager, List<Fragment> videoFrameLayoutFragments)
        {

            

            if (Tobeshown != null)
            {
                FragmentTransaction fragmentTransaction = fragmentManager.BeginTransaction();


                foreach (var f in fragmentManager.Fragments)
                {
                    if (videoFrameLayoutFragments.Contains(f))
                    {
                        if (f == Tobeshown)
                            fragmentTransaction.Show(f);
                        else
                            fragmentTransaction.Hide(f);
                    }
                    
                }

                fragmentTransaction.Commit();

                return true;
            }
            else
            {
                FragmentTransaction fragmentTransaction = fragmentManager.BeginTransaction();

                foreach (var f in videoFrameLayoutFragments)
                {
                  fragmentTransaction.Hide(f);
                }

                fragmentTransaction.Commit();
            }

            return false;
        }
    }
}