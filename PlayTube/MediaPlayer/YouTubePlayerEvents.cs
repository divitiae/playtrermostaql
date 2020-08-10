using System.Linq;
using Com.Google.Android.Youtube.Player;
using PlayTube.Activities.Tabbes;
using PlayTube.Helpers.Models;
using PlayTube.Helpers.Utils;
using Object = Java.Lang.Object;

namespace PlayTube.MediaPlayer
{
    public class YouTubePlayerEvents : Object, IYouTubePlayerPlayerStateChangeListener
    {
        public void OnAdStarted()
        {
        }

        public void OnError(YouTubePlayerErrorReason p0)
        {
        }

        public void OnLoaded(string p0)
        {
        }

        public void OnLoading()
        {
        }

        public void OnVideoEnded()
        {
            if (ListUtils.ArrayListPlay.Count > 0 && UserDetails.AutoNext)
            {
                var data = ListUtils.ArrayListPlay.FirstOrDefault();
                if (data != null)
                {
                    TabbedMainActivity.GetInstance()?.StartPlayVideo(data);
                }
            }
        }

        public void OnVideoStarted()
        {
        }
    }
}