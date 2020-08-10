using Android.Graphics;
using Android.Support.V4.Widget;

namespace PlayTube.ViewExtensions
{
    public static class ViewExtensions
    {
        public static void SetDefaultStyle(this SwipeRefreshLayout swipeRefreshLayout)
        {
            swipeRefreshLayout.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight, Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight, Android.Resource.Color.HoloRedLight);
            swipeRefreshLayout.Refreshing = true;
            swipeRefreshLayout.Enabled = true;
            swipeRefreshLayout.SetProgressBackgroundColorSchemeColor(AppSettings.SetTabDarkTheme ? Color.ParseColor("#424242") : Color.ParseColor("#f7f7f7"));
        }
    }
}