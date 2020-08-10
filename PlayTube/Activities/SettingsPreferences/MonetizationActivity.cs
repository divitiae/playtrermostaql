using System;
using System.Globalization;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Ads;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using PlayTube.Helpers.Ads;
using PlayTube.Helpers.Controller;
using PlayTube.Helpers.Models;
using PlayTube.Helpers.Utils;
using PlayTubeClient.Classes.Global;
using PlayTubeClient.RestCalls;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace PlayTube.Activities.SettingsPreferences
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MonetizationActivity : AppCompatActivity
    {
        #region Variables Basic

        private AdView MAdView;
        private TextView CountBalnceText, SendText;
        private EditText AmountEditText, PayPalEmailEditText;
        private double CountBalnce;
        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Methods.App.FullScreenApp(this);

                SetTheme(AppSettings.SetTabDarkTheme ? Resource.Style.MyTheme_Dark_Base : Resource.Style.MyTheme_Base);

                // Create your application here
                SetContentView(Resource.Layout.MonetizationLayout);
                //Get Value And Set Toolbar
                InitComponent();
                InitToolbar();  
                Get_Data_User();
            }
            catch (Exception e)
            {
                Console.WriteLine(e); 
            } 
        }
         
        protected override void OnResume()
        {
            try
            {
                base.OnResume();
                AddOrRemoveEvent(true);

                MAdView?.Resume();
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
                AddOrRemoveEvent(false);

                MAdView?.Pause();
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

        protected override void OnDestroy()
        {
            try
            {
                MAdView?.Destroy();
                base.OnDestroy();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
        #region Menu

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        #endregion

        #region Functions

        private void InitComponent()
        {
            try
            {
                 SendText = FindViewById<TextView>(Resource.Id.toolbar_title);
                 CountBalnceText = FindViewById<TextView>(Resource.Id.countBalnceText);
                 AmountEditText = FindViewById<EditText>(Resource.Id.Monetization_Edit);
                 PayPalEmailEditText = FindViewById<EditText>(Resource.Id.PayPalEmail_Edit); 

                 MAdView = FindViewById<AdView>(Resource.Id.adView);
                 AdsGoogle.InitAdView(MAdView, null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void InitToolbar()
        {
            try
            {
                Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (toolbar != null)
                {
                    toolbar.Title = GetText(Resource.String.Lbl_Withdrawals);

                    SetSupportActionBar(toolbar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true);
                } 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
 
        private void AddOrRemoveEvent(bool addEvent)
        {
            try
            {
                // true +=  // false -=
                if (addEvent)
                {
                    SendText.Click += SendTextOnClick; 
                }
                else
                {
                    SendText.Click -= SendTextOnClick;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Events

        private async void SendTextOnClick(object sender, EventArgs e)
        {
            try
            {
                if (CountBalnce < Convert.ToDouble(AmountEditText.Text))
                {
                    Toast.MakeText(this, GetText(Resource.String.Lbl_CantRequestMonetization),ToastLength.Long).Show(); 
                } 
                else if (string.IsNullOrEmpty(PayPalEmailEditText.Text.Replace(" ", "")) || string.IsNullOrEmpty(AmountEditText.Text.Replace(" ", "")))
                {
                    Toast.MakeText(this, GetText(Resource.String.Lbl_Please_check_your_details), ToastLength.Long).Show(); 
                }
                else
                {
                    if (Methods.CheckConnectivity())
                    {
                        //Show a progress
                        AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading));

                        var (apiStatus, respond) = await RequestsAsync.Global.Monetization_Http(AmountEditText.Text, PayPalEmailEditText.Text);
                        if (apiStatus == 200)
                        {
                            if (respond is MessageObject result)
                            {
                                Console.WriteLine(result.Message);
                                Toast.MakeText(this, GetText(Resource.String.Lbl_RequestSentMonetization), ToastLength.Long).Show();
                            } 
                        }
                        else Methods.DisplayReportResult(this, respond);

                        AndHUD.Shared.Dismiss(this);
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long).Show();
                    }
                }
            }
            catch (Exception exception)
            {
                AndHUD.Shared.Dismiss(this);
                Console.WriteLine(exception);
            }
        }

        #endregion
         
        private async void Get_Data_User() 
        {
            try
            {
                if (ListUtils.MyChannelList.Count == 0)
                    await ApiRequest.GetChannelData(this,UserDetails.UserId);

                var local = ListUtils.MyChannelList.FirstOrDefault();
                if (local != null)
                {
                    CountBalnce = Convert.ToDouble(local.Balance);
                    CountBalnceText.Text = "$" + CountBalnce.ToString(CultureInfo.InvariantCulture);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        } 
    }
}