//###############################################################
// Author >> Elin Doughouz 
// Copyright (c) PlayTube 12/07/2018 All Right Reserved
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// Follow me on facebook >> https://www.facebook.com/Elindoughous
//=========================================================

using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using PlayTube.Activities.Default;
using PlayTube.Helpers.Ads;
using PlayTube.Helpers.Utils;
using PlayTubeClient.Classes.Global;
using PlayTubeClient.RestCalls;


namespace PlayTube.Activities.SettingsPreferences
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class PasswordActivity : AppCompatActivity
    {
        #region Variables Basic

        private  EditText TxtCurrentPassword;
        private  EditText TxtNewPassword;
        private  EditText TxtRepeatPassword;
        private TextView SaveTextView ,LinkTextView;
        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Methods.App.FullScreenApp(this);

                SetTheme(AppSettings.SetTabDarkTheme ? Resource.Style.MyTheme_Dark_Base : Resource.Style.MyTheme_Base);

                // Create your application here
                SetContentView(Resource.Layout.Settings_Password_Layout);
                
                //Set ToolBar
                var toolBar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
                toolBar.Title = GetText(Resource.String.Lbl_Change_Password);

                SetSupportActionBar(toolBar);
                SupportActionBar.SetDisplayShowCustomEnabled(true);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.SetHomeButtonEnabled(true);
                SupportActionBar.SetDisplayShowHomeEnabled(true);

                //Get values
                TxtCurrentPassword = FindViewById<EditText>(Resource.Id.CurrentPassword_Edit);
                TxtNewPassword = FindViewById<EditText>(Resource.Id.NewPassword_Edit);
                TxtRepeatPassword = FindViewById<EditText>(Resource.Id.RepeatPassword_Edit);
                LinkTextView = FindViewById<TextView>(Resource.Id.linkText);
                LinkTextView.Click += LinkTextViewOnClick;

                SaveTextView = FindViewById<TextView>(Resource.Id.toolbar_title);
                SaveTextView.Click += SaveTextViewOnClick;

                AdsGoogle.Ad_AdMobNative(this);

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void SaveTextViewOnClick(object sender, EventArgs e)
        {
            SaveDataButtonOnClick();
        }

        private void LinkTextViewOnClick(object sender, EventArgs e)
        { 
            try
            {
                StartActivity(new Intent(this, typeof(ForgetPasswordActivity)));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private async void SaveDataButtonOnClick()
        {
            try
            {
                if (string.IsNullOrEmpty(TxtCurrentPassword.Text.Replace(" ", "")) || string.IsNullOrEmpty(TxtNewPassword.Text.Replace(" ", "")) || string.IsNullOrEmpty(TxtRepeatPassword.Text.Replace(" ", "")))
                {
                    Toast.MakeText(this, GetText(Resource.String.Lbl_Please_check_your_details), ToastLength.Long).Show();
                    return;
                }

                if (TxtNewPassword.Text != TxtRepeatPassword.Text)
                {
                    Toast.MakeText(this, GetText(Resource.String.Lbl_Your_password_dont_match), ToastLength.Long).Show();
                }
                else
                {
                    if (Methods.CheckConnectivity())
                    {
                        //Show a progress
                        AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading));

                        try
                        {
                            var (apiStatus, respond) = await RequestsAsync.Global.Change_Password_Http(TxtNewPassword.Text, TxtRepeatPassword.Text, TxtCurrentPassword.Text);
                            if (apiStatus == 200)
                            {
                                if (respond is MessageObject result)
                                {
                                    Toast.MakeText(this, result.Message, ToastLength.Short).Show();
                                    AndHUD.Shared.Dismiss(this);
                                }
                            }
                            else
                            {
                                Methods.DisplayReportResult(this, respond);
                                if (respond is ErrorObject error)
                                {
                                    AndHUD.Shared.ShowError(this, error.errors.ErrorText, MaskType.Clear, TimeSpan.FromSeconds(2));
                                }
                            }
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception);
                        }

                        AndHUD.Shared.Dismiss(this);
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_something_went_wrong), ToastLength.Short).Show();
                    }
                }
            }
            catch (Exception exception)
            {
                AndHUD.Shared.Dismiss(this);
                Console.WriteLine(exception);
            }
        }
 
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

        public override void OnTrimMemory(TrimMemory level)
        {
            try
            {
                
                
                
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                base.OnTrimMemory(level);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
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

        public void AddOrRemoveEvent(bool addEvent)
        {
            try
            {
                // true +=  // false -=
                if (addEvent)
                {

                }
                else
                {

                }
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
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}