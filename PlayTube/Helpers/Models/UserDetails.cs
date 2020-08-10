using System;
using Android.App;
using Android.Provider;

namespace PlayTube.Helpers.Models
{
    public static class UserDetails
    {
        //############# DONT'T MODIFY HERE #############
        //Auto Session bindable objects 
        //*********************************************************
        public static string AccessToken = "";

        public static string UserId = "0";
        public static string Username = "";
        public static string FullName = "";
        public static string Password = "";
        public static string Email = "";
        public static string Cookie = "";
        public static string Status = "";
        public static string Avatar = "no_profile_image";
        public static string Cover = "";
        public static string DeviceId = "";
        public static string LangName = "";

        public static bool AutoNext = false;
        public static bool PipIsChecked = false;

        public static bool NotificationPopup { get; set; } = true;

        public static Int32 UnixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        public static string Time = UnixTimestamp.ToString();

        public static string AndroidId = Settings.Secure.GetString(Application.Context.ContentResolver, Settings.Secure.AndroidId);

        public static bool IsLogin = false;
         
        public static string FilterTime;
        public static string FilterTypeSortBy;

        public static void ClearAllValueUserDetails()
        {
            try
            {
                AccessToken = string.Empty;
                UserId = string.Empty;
                Username = string.Empty;
                FullName = string.Empty;
                Password = string.Empty;
                Email = string.Empty;
                Cookie = string.Empty;
                Status = string.Empty;
                Avatar = string.Empty;
                Cover = string.Empty;
                DeviceId = string.Empty;
                LangName = string.Empty;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}