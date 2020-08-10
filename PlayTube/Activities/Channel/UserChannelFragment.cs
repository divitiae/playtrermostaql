using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AFollestad.MaterialDialogs;
using Android;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using Com.Github.Library.Bubbleview;
using Java.Lang;
using Newtonsoft.Json;
using PlayTube.Activities.Channel.Tab;
using PlayTube.Activities.Chat;
using PlayTube.Activities.Tabbes;
using PlayTube.Adapters;
using PlayTube.Helpers.Ads;
using PlayTube.Helpers.CacheLoaders;
using PlayTube.Helpers.Controller;
using PlayTube.Helpers.Fonts;
using PlayTube.Helpers.Models;
using PlayTube.Helpers.Utils;
using PlayTube.Payment;
using PlayTube.SQLite;
using PlayTubeClient.Classes.Global;
using PlayTubeClient.RestCalls;
using Exception = System.Exception;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Fragment = Android.Support.V4.App.Fragment;
using Methods = PlayTube.Helpers.Utils.Methods;

namespace PlayTube.Activities.Channel
{
    public class UserChannelFragment : Fragment, MaterialDialog.ISingleButtonCallback, MaterialDialog.IListCallback
    {
        #region Variables Basic

        private ChPlayListFragment PlayListFragment;
        private ChVideosFragment VideosFragment;
        public ChActivitiesFragment ActivitiesFragment;
        private ChAboutFragment AboutFragment;
        private AppBarLayout AppBarLayout;
        private TabLayout Tabs;
        private ViewPager ViewPagerView;
        private Toolbar MainToolbar;
        private ImageView ImageChannel, ImageCoverChannel, IconMesseges;
        private CollapsingToolbarLayout CollapsingToolbar;
        private TextView ChannelNameText, ChannelVerifiedText;
        private string  IdChannel = "", DialogType;
        private TabbedMainActivity GlobalContext;
        public Button SubscribeChannelButton;
        private TextView TxtSubscribeCount;
        public UserDataObject UserData;
        private AdsGoogle.AdMobRewardedVideo RewardedVideoAd;
        private BubbleLinearLayout SubscribeCountLayout;
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
                View view = inflater.Inflate(Resource.Layout.UserChannel_Layout, container, false);


                if (!string.IsNullOrEmpty(Arguments.GetString("Object")))
                {
                    UserData = JsonConvert.DeserializeObject<UserDataObject>(Arguments.GetString("Object"));
                    if (UserData != null)
                    {
                        IdChannel = UserData.Id;
                    }
                }

                //Get Value And Set Toolbar
                InitComponent(view);
                InitToolbar(view);
                SetTap();

                SubscribeChannelButton.Click += SubscribeChannelButtonClick;
                IconMesseges.Click += IconMessegesOnClick;

                GetDataUser();

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
            try
            {
                switch (item.ItemId)
                {
                    case Android.Resource.Id.Home:
                        GlobalContext.FragmentNavigatorBack();
                        return true;
                }
                return base.OnOptionsItemSelected(item);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return false;
            }
        }

        #endregion

        #region Functions

        private void InitComponent(View view)
        {
            try
            {
                IconMesseges = view.FindViewById<ImageView>(Resource.Id.Messeges_icon);

                ImageCoverChannel = view.FindViewById<ImageView>(Resource.Id.Imagevideo);
                ImageChannel = view.FindViewById<ImageView>(Resource.Id.ChannelImage);
                CollapsingToolbar = view.FindViewById<CollapsingToolbarLayout>(Resource.Id.collapsingToolbar);
                ChannelNameText = view.FindViewById<TextView>(Resource.Id.ChannelName);
                ChannelVerifiedText = view.FindViewById<TextView>(Resource.Id.ChannelVerifiedText);
                SubscribeChannelButton = view.FindViewById<Button>(Resource.Id.SubcribeButton);
                  
                SubscribeCountLayout = view.FindViewById<BubbleLinearLayout>(Resource.Id.bubble_layout);
                TxtSubscribeCount = view.FindViewById<TextView>(Resource.Id.subcriberscount);
                 
                Tabs = view.FindViewById<TabLayout>(Resource.Id.channeltabs);
                ViewPagerView = view.FindViewById<ViewPager>(Resource.Id.Channelviewpager);
                AppBarLayout = view.FindViewById<AppBarLayout>(Resource.Id.mainAppBarLayout);
                AppBarLayout.SetExpanded(true);

                if (!UserDetails.IsLogin)
                    IconMesseges.Visibility = ViewStates.Gone;
                 
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, ChannelVerifiedText, IonIconsFonts.CheckmarkCircled);

                ChannelVerifiedText.Visibility = ViewStates.Gone;
                SubscribeChannelButton.Tag = "Subscribe"; 
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
                GlobalContext.SetToolBar(MainToolbar, " ");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        private void SetTap()
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

        #region Events

        private void IconMessegesOnClick(object sender, EventArgs e)
        {
            try
            {
                UserDataObject item = UserData;

                if (item != null)
                {
                    Intent intent = new Intent(Activity, typeof(MessagesBoxActivity));
                    intent.PutExtra("UserId", IdChannel);
                    intent.PutExtra("TypeChat", "Owner");
                    intent.PutExtra("UserItem", JsonConvert.SerializeObject(item));

                    // Check if we're running on Android 5.0 or higher
                    if ((int)Build.VERSION.SdkInt < 23)
                    {
                        Activity.StartActivity(intent);
                    }
                    else
                    {
                        //Check to see if any permission in our group is available, if one, then all are
                        if (Activity.CheckSelfPermission(Manifest.Permission.ReadExternalStorage) == Permission.Granted &&
                            Activity.CheckSelfPermission(Manifest.Permission.WriteExternalStorage) == Permission.Granted)
                        {
                            Activity.StartActivity(intent);
                        }
                        else
                            new PermissionsController(Activity).RequestPermission(100);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void SubscribeChannelButtonClick(object sender, EventArgs e)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    if (UserDetails.IsLogin)
                    {
                        if (!string.IsNullOrEmpty(UserData.SubscriberPrice) && UserData.SubscriberPrice != "0")
                        {
                            if (SubscribeChannelButton.Tag.ToString() == "PaidSubscribe")
                            {
                                DialogType = "PaidSubscribe";

                                //This channel is paid, You must pay to subscribe
                                var dialog = new MaterialDialog.Builder(Context).Theme(AppSettings.SetTabDarkTheme ? AFollestad.MaterialDialogs.Theme.Dark : AFollestad.MaterialDialogs.Theme.Light);

                                dialog.Title(Resource.String.Lbl_Warning);
                                dialog.Content(GetText(Resource.String.Lbl_ChannelIsPaid));
                                dialog.PositiveText(GetText(Resource.String.Lbl_Ok)).OnPositive(this);
                                dialog.NegativeText(GetText(Resource.String.Lbl_Cancel)).OnNegative(this);
                                dialog.AlwaysCallSingleChoiceCallback();
                                dialog.Build().Show();
                            }
                            else
                            {
                                SubscribeChannelButton.Tag = "Subscribe";
                                SubscribeChannelButton.Text = GetText(Resource.String.Btn_Subscribe);
                                //Color
                                SubscribeChannelButton.SetTextColor(Color.ParseColor("#efefef"));
                                //icon
                                Drawable icon = Activity.GetDrawable(Resource.Drawable.SubcribeButton);
                                icon.Bounds = new Rect(10, 10, 10, 7);
                                SubscribeChannelButton.SetCompoundDrawablesWithIntrinsicBounds(icon, null, null, null);

                                //Remove The Video to Subscribed Videos Database
                                var sqlEntity = new SqLiteDatabase();
                                sqlEntity.RemoveSubscriptionsChannel(IdChannel);
                                sqlEntity.Dispose();

                                //Send API Request here for UnSubscribed
                                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Global.Add_Subscribe_To_Channel_Http(UserData.Id) });

                                // Toast.MakeText(this, this.GetText(Resource.String.Lbl_Channel_Removed_successfully, ToastLength.Short).Show();
                            }
                        }
                        else
                        {
                            if (SubscribeChannelButton.Tag.ToString() == "Subscribe")
                            {
                                SubscribeChannelButton.Tag = "Subscribed";
                                SubscribeChannelButton.Text = GetText(Resource.String.Btn_Subscribed);

                                //Color
                                SubscribeChannelButton.SetTextColor(Color.ParseColor("#efefef"));
                                //icon
                                Drawable icon = Activity.GetDrawable(Resource.Drawable.SubcribedButton);
                                icon.Bounds = new Rect(10, 10, 10, 7);
                                SubscribeChannelButton.SetCompoundDrawablesWithIntrinsicBounds(icon, null, null, null);

                                //Add The Video to  Subcribed Videos Database
                                Events_Insert_SubscriptionsChannel();

                                //Send API Request here for Subcribe
                                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Global.Add_Subscribe_To_Channel_Http(UserData.Id) });

                                Toast.MakeText(Activity, Activity.GetText(Resource.String.Lbl_Subscribed_successfully), ToastLength.Short).Show();
                            }
                            else
                            {
                                SubscribeChannelButton.Tag = "Subscribe";
                                SubscribeChannelButton.Text = GetText(Resource.String.Btn_Subscribe);
                                //Color
                                SubscribeChannelButton.SetTextColor(Color.ParseColor("#efefef"));
                                //icon
                                Drawable icon = Activity.GetDrawable(Resource.Drawable.SubcribeButton);
                                icon.Bounds = new Rect(10, 10, 10, 7);
                                SubscribeChannelButton.SetCompoundDrawablesWithIntrinsicBounds(icon, null, null, null);

                                //Remove The Video to Subcribed Videos Database
                                var sqlEntity = new SqLiteDatabase();
                                sqlEntity.RemoveSubscriptionsChannel(IdChannel);
                                sqlEntity.Dispose();

                                //Send API Request here for UnSubcribe
                                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Global.Add_Subscribe_To_Channel_Http(UserData.Id) });

                                // Toast.MakeText(this, this.GetText(Resource.String.Lbl_Channel_Removed_successfully, ToastLength.Short).Show();
                            }
                        }
                    }
                    else
                    {
                        PopupDialogController dialog = new PopupDialogController(Activity, null, "Login");
                        dialog.ShowNormalDialog(GetText(Resource.String.Lbl_Warning), GetText(Resource.String.Lbl_Please_sign_in_Subcribed), GetText(Resource.String.Lbl_Yes), GetText(Resource.String.Lbl_No));
                    }
                }
                else
                {
                    Toast.MakeText(Activity, Activity.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
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
                AboutFragment = new ChAboutFragment();
                ActivitiesFragment = new ChActivitiesFragment();

                Bundle bundle = new Bundle();
                bundle.PutString("ChannelId", IdChannel);

                PlayListFragment.Arguments = bundle;
                VideosFragment.Arguments = bundle;
                AboutFragment.Arguments = bundle;
                ActivitiesFragment.Arguments = bundle;

                MainTabAdapter adapter = new MainTabAdapter(ChildFragmentManager);
                adapter.AddFragment(VideosFragment, GetText(Resource.String.Lbl_Videos));
                adapter.AddFragment(PlayListFragment, GetText(Resource.String.Lbl_PlayLists));
                adapter.AddFragment(ActivitiesFragment, GetText(Resource.String.Lbl_Activities));
                adapter.AddFragment(AboutFragment, GetText(Resource.String.Lbl_AboutChannal));

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
         
        #endregion

        private async void GetDataUser()
        {
            try
            {
                SetDataUser();
                 
                var data = await ApiRequest.GetChannelData(Activity, IdChannel);
                if (data == null) return;
                UserData = data;
                SetDataUser();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
        private void SetDataUser()
        {
            try
            {
                if (UserData != null)
                {
                    var name = AppTools.GetNameFinal(UserData);
                    CollapsingToolbar.Title = name;
                    ChannelNameText.Text = name;
                  
                    if (string.IsNullOrEmpty(UserData.SubCount))
                        UserData.SubCount = "0";
                     
                    TxtSubscribeCount.Text = UserData.SubCount;

                    GlideImageLoader.LoadImage(Activity, UserData.Avatar, ImageChannel, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);
                    Glide.With(Activity).Load(UserData.Cover).Apply(new RequestOptions().FitCenter()).Into(ImageCoverChannel);

                    //Verified 
                    ChannelVerifiedText.Visibility = UserData.Verified == "1" ? ViewStates.Visible : ViewStates.Gone;

                    if (!string.IsNullOrEmpty(UserData.SubscriberPrice) && UserData.SubscriberPrice != "0")
                    {
                        if (UserData.IsSubscribedToChannel == "0")
                        {
                            //This channel is paid, You must pay to subscribe
                            SubscribeChannelButton.Tag = "PaidSubscribe";

                            //Color
                            SubscribeChannelButton.SetTextColor(Color.ParseColor("#efefef"));
                            //icon
                            Drawable icon = Activity.GetDrawable(Resource.Drawable.SubcribeButton);
                            icon.Bounds = new Rect(10, 10, 10, 7);
                            SubscribeChannelButton.SetCompoundDrawablesWithIntrinsicBounds(icon, null, null, null);

                            var idCurrency = ListUtils.MySettingsList?.PaymentCurrency;
                            var (currency, currencyIcon) = AppTools.GetCurrency(idCurrency);
                            Console.WriteLine(currency);
                            SubscribeChannelButton.Text = Activity.GetText(Resource.String.Btn_Subscribe) + " " + currencyIcon + UserData.SubscriberPrice;

                            PlayListFragment.MRecycler.Visibility = ViewStates.Gone;
                            SetEmptyPageSubscribeChannelWithPaid(PlayListFragment.EmptyStateLayout, PlayListFragment.Inflated);

                            VideosFragment.MRecycler.Visibility = ViewStates.Gone;
                            SetEmptyPageSubscribeChannelWithPaid(VideosFragment.EmptyStateLayout, VideosFragment.Inflated);
                        }
                        else
                        {
                            SubscribeChannelButton.Tag = "Subscribed";

                            SubscribeChannelButton.Text = Activity.GetText(Resource.String.Btn_Subscribed);

                            //Color
                            SubscribeChannelButton.SetTextColor(Color.ParseColor("#efefef"));
                            //icon
                            Drawable icon = Activity.GetDrawable(Resource.Drawable.SubcribedButton);
                            icon.Bounds = new Rect(10, 10, 10, 7);
                            SubscribeChannelButton.SetCompoundDrawablesWithIntrinsicBounds(icon, null, null, null);
                        }
                    }
                    else
                    {
                        SubscribeChannelButton.Tag = UserData.IsSubscribedToChannel == "0" ? "Subscribe" : "Subscribed";

                        if (SubscribeChannelButton.Tag.ToString() == "Subscribed")
                        {
                            SubscribeChannelButton.Text = Activity.GetText(Resource.String.Btn_Subscribed);

                            //Color
                            SubscribeChannelButton.SetTextColor(Color.ParseColor("#efefef"));
                            //icon
                            Drawable icon = Activity.GetDrawable(Resource.Drawable.SubcribedButton);
                            icon.Bounds = new Rect(10, 10, 10, 7);
                            SubscribeChannelButton.SetCompoundDrawablesWithIntrinsicBounds(icon, null, null, null);
                        }
                        else if (SubscribeChannelButton.Tag.ToString() == "Subscribe")
                        {
                            SubscribeChannelButton.Text = Activity.GetText(Resource.String.Btn_Subscribe);

                            //Color
                            SubscribeChannelButton.SetTextColor(Color.ParseColor("#efefef"));
                            //icon
                            Drawable icon = Activity.GetDrawable(Resource.Drawable.SubcribeButton);
                            icon.Bounds = new Rect(10, 10, 10, 7);
                            SubscribeChannelButton.SetCompoundDrawablesWithIntrinsicBounds(icon, null, null, null);
                        }
                    }
                } 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void Events_Insert_SubscriptionsChannel()
        {
            try
            {
                var sqlEntity = new SqLiteDatabase();
 
                if (UserData != null)
                    sqlEntity.Insert_One_SubscriptionChannel(UserData);
                 
                sqlEntity.Dispose();

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #region MaterialDialog

        public void OnClick(MaterialDialog p0, DialogAction p1)
        {
            try
            {
                if (DialogType == "PaidSubscribe")
                {
                    if (p1 == DialogAction.Positive)
                    { 
                        DialogType = "Payment";

                        var arrayAdapter = new List<string>();
                        var dialogList = new MaterialDialog.Builder(Activity).Theme(AppSettings.SetTabDarkTheme ? Theme.Dark : Theme.Light);

                        if (AppSettings.ShowPaypal)
                            arrayAdapter.Add(Activity.GetString(Resource.String.Btn_Paypal));

                        if (AppSettings.ShowCreditCard)
                            arrayAdapter.Add(Activity.GetString(Resource.String.Lbl_CreditCard));

                        dialogList.Items(arrayAdapter);
                        dialogList.NegativeText(Activity.GetString(Resource.String.Lbl_Close)).OnNegative(this);
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
                if (text == Activity.GetString(Resource.String.Btn_Paypal))
                {
                    GlobalContext.Price = UserData.SubscriberPrice;
                    GlobalContext.PayType = "Subscriber";
                    GlobalContext.InitPayPalPayment.BtnPaypalOnClick(UserData.SubscriberPrice, "Subscriber");
                }
                else if (text == Activity.GetString(Resource.String.Lbl_CreditCard))
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
                Intent intent = new Intent(Activity, typeof(PaymentCardDetailsActivity));
                intent.PutExtra("Price", UserData.SubscriberPrice);
                intent.PutExtra("payType", "Subscriber");
                Activity.StartActivity(intent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        public async void SetSubscribeChannelWithPaid()
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    SubscribeChannelButton.Tag = "Subscribed";
                    SubscribeChannelButton.Text = GetText(Resource.String.Btn_Subscribed);

                    //Color
                    SubscribeChannelButton.SetTextColor(Color.ParseColor("#efefef"));
                    //icon
                    Drawable icon = Activity.GetDrawable(Resource.Drawable.SubcribedButton);
                    icon.Bounds = new Rect(10, 10, 10, 7);
                    SubscribeChannelButton.SetCompoundDrawablesWithIntrinsicBounds(icon, null, null, null);

                    //Add The Video to  Subscribe Videos Database
                    Events_Insert_SubscriptionsChannel();

                    //Send API Request here for Subscribe
                    (int apiStatus, var respond) = await RequestsAsync.Global.Add_Subscribe_To_Channel_Http(IdChannel, "paid");
                    if (apiStatus == 200)
                    {
                        if (respond is MessageObject result)
                        {
                            Console.WriteLine(result.Message);
                            Activity.RunOnUiThread(() =>
                            {
                                Toast.MakeText(Activity,Activity.GetText(Resource.String.Lbl_Subscribed_successfully),ToastLength.Short).Show();

                                PlayListFragment.ShowEmptyPage(); 
                                VideosFragment.ShowEmptyPage(); 
                            });
                        }
                    }
                    else Methods.DisplayReportResult(Activity, respond);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
          
        public void SetEmptyPageSubscribeChannelWithPaid(ViewStub emptyStateLayout , View inflated)
        {
            if (emptyStateLayout == null) return;
            try
            {
                if (inflated == null)
                    inflated = emptyStateLayout.Inflate();

                EmptyStateInflater x = new EmptyStateInflater();
                x.InflateLayout(inflated, EmptyStateInflater.Type.SubscribeChannelWithPaid);

                var idCurrency = ListUtils.MySettingsList?.PaymentCurrency;
                var (currency, currencyIcon) = AppTools.GetCurrency(idCurrency);
                Console.WriteLine(currency);
                x.TitleText.Text = Activity.GetText(Resource.String.Lbl_SubscribeFor) + " "+ currencyIcon + UserData.SubscriberPrice + " " + Activity.GetText(Resource.String.Lbl_AndUnlockAllTheVideos);
                SubscribeChannelButton.Text = Activity.GetText(Resource.String.Btn_Subscribe) + " " + currencyIcon + UserData.SubscriberPrice;

                if (!x.EmptyStateButton.HasOnClickListeners)
                {
                    x.EmptyStateButton.Click += null;
                    x.EmptyStateButton.Click += SubscribeChannelWithPaidButtonOnClick;
                }

                emptyStateLayout.Visibility = ViewStates.Visible;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void SubscribeChannelWithPaidButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                //This channel is paid, You must pay to subscribe
                var dialog = new MaterialDialog.Builder(Context).Theme(AppSettings.SetTabDarkTheme ? Theme.Dark : Theme.Light);

                dialog.Title(Resource.String.Lbl_Warning);
                dialog.Content(Context.GetText(Resource.String.Lbl_ChannelIsPaid));
                dialog.PositiveText(Context.GetText(Resource.String.Lbl_Ok)).OnPositive(this);
                dialog.NegativeText(Context.GetText(Resource.String.Lbl_Cancel)).OnNegative(this);
                dialog.AlwaysCallSingleChoiceCallback();
                dialog.Build().Show();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
    }
}
