using System;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Preferences;
using PlayTube.Helpers.Models;

namespace PlayTube.Activities.SettingsPreferences
{
    public static class MainSettings
    {
        #region Variables Basic

        public static ISharedPreferences SharedData, AutoNext;
        public static readonly string PrefKeyAutoNext = "auto_next_key";

        public static readonly string LightMode = "light";
        public static readonly string DarkMode = "dark";
        public static readonly string DefaultMode = "default";


        #endregion

        public static void Init()
        {
            try
            { 
                SharedData = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
                AutoNext = Application.Context.GetSharedPreferences("auto_next_perf", FileCreationMode.Private);
                 
                UserDetails.AutoNext = AutoNext.GetBoolean(PrefKeyAutoNext, false);

                UserDetails.PipIsChecked = SharedData.GetBoolean("picture_in_picture_key", false);

                string getValue = SharedData.GetString("Night_Mode_key", string.Empty);
                ApplyTheme(getValue);  
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private static void ApplyTheme(string themePref)
        {
            try
            {
                if (themePref == LightMode)
                {
                    AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightNo;
                    AppSettings.SetTabDarkTheme = false;
                }
                else if (themePref == DarkMode)
                {
                    AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightYes;
                    AppSettings.SetTabDarkTheme = true;
                }
                else if (themePref == DefaultMode)
                {
                    if ((int)Build.VERSION.SdkInt >= 29)
                    {
                        AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightFollowSystem;
                    }
                    else
                    {
                        AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightAuto;
                    }

                    var currentNightMode = Application.Context.Resources.Configuration.UiMode & UiMode.NightMask;
                    switch (currentNightMode)
                    {
                        case UiMode.NightNo:
                            // Night mode is not active, we're using the light theme
                            AppSettings.SetTabDarkTheme = false;
                            break;
                        case UiMode.NightYes:
                            // Night mode is active, we're using dark theme
                            AppSettings.SetTabDarkTheme = true;
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        } 
    }
}