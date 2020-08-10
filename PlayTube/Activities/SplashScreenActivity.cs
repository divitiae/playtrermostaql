//###############################################################
// Author >> Elin Doughouz 
// Copyright (c) PlayTube 12/07/2018 All Right Reserved
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// Follow me on facebook >> https://www.facebook.com/Elindoughous
//=========================================================

using System;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Ads;
using Android.OS;
using Android.Support.V7.App;
using PlayTube.Activities.Default;
using PlayTube.Activities.Tabbes;
using PlayTube.SQLite;
using Android.Widget;
using Java.Lang;
using PlayTube.Helpers.Controller;
using PlayTube.Helpers.Models;
using PlayTube.Helpers.Utils;
using PlayTubeClient;
using Exception = System.Exception;

namespace PlayTube.Activities
{
    [Activity(MainLauncher = true, Icon = "@mipmap/icon", Theme = "@style/SplashScreenTheme", NoHistory = true, ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class SplashScreenActivity : AppCompatActivity
    {
        private SqLiteDatabase DbDatabase; 

        protected override void OnResume()
        {
            try
            {
                base.OnResume();
                 
                DbDatabase = new SqLiteDatabase();
                DbDatabase.Connect();

                // Check if we're running on Android 5.0 or higher
                if ((int)Build.VERSION.SdkInt < 23)
                {
                    new Handler(Looper.MainLooper).Post(new Runnable(SimulateStartup));
                }
                else
                {
                    if (CheckSelfPermission(Manifest.Permission.ReadExternalStorage) == Permission.Granted && CheckSelfPermission(Manifest.Permission.WriteExternalStorage) == Permission.Granted)
                    {
                        new Handler(Looper.MainLooper).Post(new Runnable(SimulateStartup));
                    }
                    else
                    {
                        RequestPermissions(new[]
                        {
                            Manifest.Permission.ReadExternalStorage,
                            Manifest.Permission.WriteExternalStorage,
                            Manifest.Permission.AccessMediaLocation,
                        }, 100);
                    }
                } 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void SimulateStartup()
        {
            try
            {
                DbDatabase = new SqLiteDatabase();
                DbDatabase.Connect();

                if (!string.IsNullOrEmpty(AppSettings.Lang))
                {
                    LangController.SetApplicationLang(this, AppSettings.Lang);
                }
                else
                {
                    UserDetails.LangName = Resources.Configuration.Locale.Language.ToLower();
                    LangController.SetApplicationLang(this, UserDetails.LangName);
                }

                Methods.Path.Chack_MyFolder();

                var result = DbDatabase.Get_data_Login();
                if (result != null)
                { 
                    UserDetails.UserId = Client.UserId = result.UserId;
                    UserDetails.AccessToken = Current.AccessToken = result.AccessToken;
                    UserDetails.Cookie = result.Cookie;
                     
                    switch (result.Status)
                    {
                        case "Active":
                            UserDetails.IsLogin = true; 
                            StartActivity(new Intent(this, typeof(TabbedMainActivity)));
                            break;
                        case "Pending":
                            StartActivity(new Intent(this, typeof(TabbedMainActivity)));
                            break;
                        default:
                            StartActivity(new Intent(this, typeof(FirstActivity)));
                            break;
                    }
                }
                else
                { 
                    StartActivity(new Intent(this, typeof(FirstActivity)));
                }
                DbDatabase.Dispose();

                if (AppSettings.ShowAdMobBanner || AppSettings.ShowAdMobInterstitial || AppSettings.ShowAdMobRewardVideo || AppSettings.ShowAdMobNative)
                    MobileAds.Initialize(this, GetString(Resource.String.admob_app_id)); 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            try
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

                if (requestCode != 100) return;
                if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                {
                    new Handler(Looper.MainLooper).Post(new Runnable(SimulateStartup)); 
                }
                else  
                {
                    Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denied), ToastLength.Long).Show();
                    Finish();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e); 
            } 
        }
    }
}