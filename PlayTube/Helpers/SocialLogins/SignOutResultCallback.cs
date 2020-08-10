using Android.Gms.Common.Apis;
using Java.Lang;
using PlayTube.Activities.Default;

namespace PlayTube.Helpers.SocialLogins
{
    public class SignOutResultCallback : Object, IResultCallback
    {
        public LoginActivity Activity { get; set; }

        public void OnResult(Object result)
        {
            //Activity.UpdateUI(false);
        }
    }
}