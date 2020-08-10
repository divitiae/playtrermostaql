using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using PlayTube.Helpers.Controller;
using PlayTubeClient.Classes.Global;

namespace PlayTube.Activities.Videos
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", LaunchMode = LaunchMode.SingleInstance, ConfigurationChanges = ConfigChanges.Keyboard | ConfigChanges.Orientation | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenLayout | ConfigChanges.ScreenSize | ConfigChanges.SmallestScreenSize | ConfigChanges.UiMode)]
    public class FullScreenVideoActivity : AppCompatActivity
    {
        public VideoController VideoActionsController;
        private static VideoObject VideoData;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                View mContentView = Window.DecorView;
                var uiOptions = (int)mContentView.SystemUiVisibility;
                var newUiOptions = (int)uiOptions;

                newUiOptions |= (int)SystemUiFlags.LowProfile;
                newUiOptions |= (int)SystemUiFlags.Fullscreen;
                newUiOptions |= (int)SystemUiFlags.HideNavigation;
                newUiOptions |= (int)SystemUiFlags.Immersive;
                mContentView.SystemUiVisibility = (StatusBarVisibility)newUiOptions;

                //ScreenOrientation.Portrait >>  Make to run your application only in portrait mode
                //ScreenOrientation.Landscape >> Make to run your application only in LANDSCAPE mode 
                RequestedOrientation = ScreenOrientation.Landscape;

                SetContentView(Resource.Layout.FullScreenDialog_Layout);

                VideoActionsController = new VideoController(this, "FullScreen"); 
                VideoActionsController.PlayFullScreen(VideoData);
                if (Intent.GetStringExtra("Downloaded") == "Downloaded")
                    VideoActionsController.DownloadIcon.SetImageDrawable(GetDrawable(Resource.Drawable.ic_checked_red));
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
        
        public override void OnBackPressed()
        {
            VideoActionsController.InitFullscreenDialog("Close");
            base.OnBackPressed();
        }

        public static void SetVideoData(VideoObject videoObject)
        {
            VideoData = videoObject;
        }
    }
}