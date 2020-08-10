using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Gms.Ads;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using AT.Markushi.UI;
using Com.Luseen.Autolinklibrary;
using PlayTube.Activities.Tabbes;
using PlayTube.Helpers.Ads;
using PlayTube.Helpers.Controller;
using PlayTube.Helpers.Fonts;
using PlayTube.Helpers.Models;
using PlayTube.Helpers.Utils;
using PlayTubeClient.RestCalls;
using Fragment = Android.Support.V4.App.Fragment;
 
namespace PlayTube.Activities.Channel.Tab
{
    public class ChAboutFragment : Fragment
    {
        #region  Variables Basic

        private string IdChannel = "", Facebook = "",Google = "", Twitter = "";
        private AutoLinkTextView TxtAboutChannel;
        private TextView  IconMonetization, TxtMonetization, TxtGender, TxtEmail;
        private CircleButton BtnFacebook, BtnTwitter, BtnGoogle;
        private TabbedMainActivity GlobalContext;
        private AdView MAdView;
        private LinearLayout LinearMonetization1, LinearMonetization2;
        private LinearLayout LinearBlock2;

        #endregion

        #region General

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your fragment here
            GlobalContext = (TabbedMainActivity)Activity;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                View view = inflater.Inflate(Resource.Layout.ChAboutLayout, container, false);

                IdChannel = Arguments.GetString("ChannelId");

                InitComponent(view);  
                 
                BtnFacebook.Click += BtnFacebookOnClick;
                BtnTwitter.Click += BtnTwitterOnClick;
                BtnGoogle.Click += BtnGoogleOnClick;

                LinearBlock2.Click += LinearBlock2OnClick;


                StartApiService();

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
                
                TxtAboutChannel = (AutoLinkTextView)view.FindViewById(Resource.Id.AboutChannal_text);

                
                TxtGender = (TextView)view.FindViewById(Resource.Id.gender_text);

          
                TxtEmail = (TextView)view.FindViewById(Resource.Id.email_text);

                BtnFacebook = (CircleButton)view.FindViewById(Resource.Id.facebook_button);
                BtnTwitter = (CircleButton)view.FindViewById(Resource.Id.twitter_button);
                BtnGoogle = (CircleButton)view.FindViewById(Resource.Id.google_button);
                 
                
                LinearBlock2 = (LinearLayout)view.FindViewById(Resource.Id.BlockLiner);

               
              
                LinearMonetization1 = (LinearLayout)view.FindViewById(Resource.Id.Monetization_Linear);
                LinearMonetization2 = (LinearLayout)view.FindViewById(Resource.Id.MonetizationLiner);

                IconMonetization = (TextView)view.FindViewById(Resource.Id.Monetization_icon);
                TxtMonetization = (TextView)view.FindViewById(Resource.Id.Monetization_text);

                //FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, IconAboutChannel, IonIconsFonts.IosInformationOutline);
                //IconAboutChannel.SetTextColor(Color.ParseColor(AppSettings.MainColor));

                //FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, IconGender, IonIconsFonts.Male);
                //IconGender.SetTextColor(Color.ParseColor("#c51162"));

                //FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, IconEmail, IonIconsFonts.Email);
                //IconEmail.SetTextColor(Color.ParseColor("#dd2c00"));

                BtnFacebook.Tag = "facebook";
                BtnTwitter.Tag = "twitter";
                BtnGoogle.Tag = "google";

                if (IdChannel == UserDetails.UserId)
                {
                   
                    LinearBlock2.Visibility = ViewStates.Gone;

                    LinearMonetization1.Visibility = ViewStates.Visible;
                    LinearMonetization2.Visibility = ViewStates.Visible;

                    FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, IconMonetization, IonIconsFonts.SocialUsd);
                   
                }
                else
                {
                    LinearMonetization1.Visibility = ViewStates.Gone;
                    LinearMonetization2.Visibility = ViewStates.Gone;

                    
                    LinearBlock2.Visibility = ViewStates.Visible;

                   
                }

                if (!AppSettings.ShowEmailAccount)
                {
                    TxtEmail.Visibility = ViewStates.Gone;
                }

                MAdView = view.FindViewById<AdView>(Resource.Id.adView);
                AdsGoogle.InitAdView(MAdView, null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
          
        #endregion

        #region Events

        private async void LinearBlock2OnClick(object sender, EventArgs e)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    Toast.MakeText(Context, Context.GetText(Resource.String.Lbl_Blocked_successfully), ToastLength.Long).Show();

                     var (apiStatus, respond) = await RequestsAsync.Global.BlockUnBlockUserAsync(IdChannel).ConfigureAwait(false);
                     if (apiStatus != 200)
                         Methods.DisplayReportResult(Activity, respond);

                    GlobalContext.FragmentNavigatorBack();
                }
                else
                {
                    Toast.MakeText(Context, Context.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void BtnGoogleOnClick(object sender, EventArgs e)
        {
            try
            {
                if (Methods.CheckConnectivity())
                    new IntentController(Activity).OpenAppOnGooglePlay(Google);
                else
                    Toast.MakeText(Context, Context.GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show(); 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void BtnTwitterOnClick(object sender, EventArgs e)
        {
            try
            {
                if (Methods.CheckConnectivity())
                    new IntentController(Activity).OpenTwitterIntent(Twitter);
                else
                    Toast.MakeText(Context, Context.GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void BtnFacebookOnClick(object sender, EventArgs e)
        {
            try
            { 
                if (Methods.CheckConnectivity())
                    new IntentController(Activity).OpenFacebookIntent(Context, Facebook);
                else
                    Toast.MakeText(Context, Context.GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
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
                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { LoadDataAsync });
        }

        private async Task LoadDataAsync()
        {
            if (Methods.CheckConnectivity())
            { 
                var channel = await ApiRequest.GetChannelData(Activity, IdChannel);
                if (channel != null)
                {
                    TextSanitizer text = new TextSanitizer(TxtAboutChannel, Activity, GlobalContext);
                    text.Load(AppTools.GetAboutFinal(channel));
                     
                    if (channel.Gender == "male" || channel.Gender == "Male")
                    {
                        TxtGender.Text = GetText(Resource.String.Radio_Male);
                    }
                    else
                    {
                        TxtGender.Text = GetText(Resource.String.Radio_Female);
                    }

                    TxtEmail.Text = channel.Email;

                    Facebook = channel.Facebook;
                    Google = channel.Google;
                    Twitter = channel.Twitter;

                    if (string.IsNullOrEmpty(channel.Facebook))
                    {
                        BtnFacebook.Enabled = false;
                        BtnFacebook.SetColor(Color.ParseColor("#8c8a8a"));
                    }
                    else
                    {
                        BtnFacebook.Enabled = true;
                    }

                    if (string.IsNullOrEmpty(channel.Twitter))
                    {
                        BtnTwitter.Enabled = false;
                        BtnTwitter.SetColor(Color.ParseColor("#8c8a8a"));
                    }
                    else
                    {
                        BtnTwitter.Enabled = true;
                    }

                    if (string.IsNullOrEmpty(channel.Google))
                    {
                        BtnGoogle.Enabled = false;
                        BtnGoogle.SetColor(Color.ParseColor("#8c8a8a"));
                    }
                    else
                    {
                        BtnGoogle.Enabled = true;
                    }
                     
                    if (IdChannel == UserDetails.UserId)
                    {
                        var idCurrency = ListUtils.MySettingsList?.PaymentCurrency; 
                        var (currency, currencyIcon) = AppTools.GetCurrency(idCurrency);
                        Console.WriteLine(currencyIcon);
                        TxtMonetization.Text = channel.Balance + " " + currency;  
                    }
                }  
            }
            else
            { 
                Toast.MakeText(Context, Context.GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
            }
        }
          
        #endregion 
         
    }
}