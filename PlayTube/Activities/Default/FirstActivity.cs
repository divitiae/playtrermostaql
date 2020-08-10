using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using PlayTube.Activities.Tabbes;
using PlayTube.Helpers.Controller;
using PlayTube.Helpers.Models;
using PlayTube.Helpers.Utils;
using PlayTube.OneSignal;
using PlayTube.SQLite;
using PlayTubeClient;

namespace PlayTube.Activities.Default
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class FirstActivity : AppCompatActivity
    {
        private VideoView VideoViewer;
        private Button LoginButton, RegisterButton, SkipButton;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                Methods.App.FullScreenApp(this);

                Client a = new Client(AppSettings.TripleDesAppServiceProvider);

                SetContentView(Resource.Layout.First_Activity_Layout);

                LoginButton = FindViewById<Button>(Resource.Id.LoginButton);
                RegisterButton = FindViewById<Button>(Resource.Id.RegisterButton);
                SkipButton = FindViewById<Button>(Resource.Id.SkipButton);
                VideoViewer = FindViewById<VideoView>(Resource.Id.videoView);
                
                Android.Net.Uri uri = Android.Net.Uri.Parse("android.resource://" + PackageName + "/" + Resource.Raw.MainVideo);
                VideoViewer.SetVideoURI(uri);
                VideoViewer.Start();

                if (!AppSettings.ShowSkipButton)
                    SkipButton.Visibility = ViewStates.Gone;

                //OneSignal Notification  
                //====================================== 
                if (string.IsNullOrEmpty(UserDetails.DeviceId))
                    OneSignalNotification.RegisterNotificationDevice();

                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => ApiRequest.GetSettings_Api(this) });
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
        private void SkipButton_Click(object sender, EventArgs e)
        {
            try
            {
                UserDetails.Username = "";
                UserDetails.FullName = "";
                UserDetails.Password = "";
                UserDetails.AccessToken = "";
                UserDetails.UserId = Client.UserId = "0";
                UserDetails.Status = "Pending";
                UserDetails.Cookie = "";
                UserDetails.Email = "";

                DataTables.LoginTb login = new DataTables.LoginTb
                {
                    Username = "",
                    Password = "",
                    AccessToken = "",
                    UserId = "0",
                    Status = "Pending",
                    Cookie = "",
                    Email = "",
                    Lang = ""
                };

                var sqlEntity = new SqLiteDatabase();
                sqlEntity.InsertOrUpdateLogin_Credentials(login);
                sqlEntity.Dispose();

                UserDetails.IsLogin = false;
                StartActivity(new Intent(this, typeof(TabbedMainActivity)));
                Finish(); 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void RegisterButton_Click(object sender, EventArgs e)
        {
            try
            {
                StartActivity(new Intent(this, typeof(RegisterActivity)));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            try
            {
                StartActivity(new Intent(this, typeof(LoginActivity)));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void VideoViewer_Completion(object sender, EventArgs e)
        {
            try
            {
                VideoViewer.Start();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        protected override void OnStop()
        {
            try
            {
                base.OnStop();
                VideoViewer.StopPlayback();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
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

        private void AddOrRemoveEvent(bool addEvent)
        {
            try
            {
                // true +=  // false -=
                if (addEvent)
                {
                   
                    RegisterButton.Click += RegisterButton_Click;
                    LoginButton.Click += LoginButton_Click;
                    SkipButton.Click += SkipButton_Click;
                    VideoViewer.Completion += VideoViewer_Completion;
                }
                else
                {
                    
                    RegisterButton.Click -= RegisterButton_Click;
                    LoginButton.Click -= LoginButton_Click;
                    SkipButton.Click -= SkipButton_Click;
                    VideoViewer.Completion -= VideoViewer_Completion;
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

                if (!VideoViewer.IsPlaying)
                {
                    VideoViewer.Start();
                }
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