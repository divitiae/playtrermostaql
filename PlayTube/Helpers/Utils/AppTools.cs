using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using PlayTube.Helpers.Controller;
using PlayTube.Helpers.Fonts;
using PlayTubeClient.Classes.Global;
using Task = System.Threading.Tasks.Task;

namespace PlayTube.Helpers.Utils
{
    public static class AppTools
    {
        public static string GetNameFinal(UserDataObject dataUser)
        {
            try
            {
                if (AppSettings.ShowUserPlayListVideoObject)
                    return Methods.FunString.DecodeString(dataUser.Username);

                string name = !string.IsNullOrEmpty(dataUser.FirstName) && !string.IsNullOrEmpty(dataUser.LastName) ? dataUser.FirstName + " " + dataUser.LastName : dataUser.Username;
                return Methods.FunString.DecodeString(name);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Methods.FunString.DecodeString(dataUser?.Username);
            }
        }

        public static string GetAboutFinal(UserDataObject dataUser)
        {
            try
            {
                if (!string.IsNullOrEmpty(dataUser.About) && !string.IsNullOrWhiteSpace(dataUser.About))
                    return Methods.FunString.DecodeString(dataUser.About);

                return Application.Context.Resources.GetString(Resource.String.Lbl_HasNotAnyInfo);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Application.Context.Resources.GetString(Resource.String.Lbl_HasNotAnyInfo);
            }
        }

        public static (string, string) GetCurrency(string idCurrency)
        {
            try
            {
                if (AppSettings.CurrencyStatic) return (AppSettings.CurrencyCodeStatic, AppSettings.CurrencyIconStatic);

                var list = ListUtils.MySettingsList;
                if (list == null)
                    return (AppSettings.CurrencyCodeStatic, AppSettings.CurrencyIconStatic);

                string currency;
                bool success = int.TryParse(idCurrency, out var number);
                if (success)
                {
                    Console.WriteLine("Converted '{0}' to {1}.", idCurrency, number);
                    currency = list.CurrencyArray[number] ?? AppSettings.CurrencyCodeStatic;
                }
                else
                {
                    Console.WriteLine("Attempted conversion of '{0}' failed.", idCurrency ?? "<null>");
                    currency = idCurrency;
                }

                if (list.CurrencySymbolArray != null)
                {
                    string currencyIcon;
                    switch (currency)
                    {
                        case "USD":
                            currencyIcon = !string.IsNullOrEmpty(list.CurrencySymbolArray.Usd) ? list.CurrencySymbolArray.Usd ?? "$" : "$";
                            break;
                        case "Jpy":
                            currencyIcon = !string.IsNullOrEmpty(list.CurrencySymbolArray.Jpy) ? list.CurrencySymbolArray.Jpy ?? "¥" : "¥";
                            break;
                        case "EUR":
                            currencyIcon = !string.IsNullOrEmpty(list.CurrencySymbolArray.Eur) ? list.CurrencySymbolArray.Eur ?? "€" : "€";
                            break;
                        case "TRY":
                            currencyIcon = !string.IsNullOrEmpty(list.CurrencySymbolArray.Try) ? list.CurrencySymbolArray.Try ?? "₺" : "₺";
                            break;
                        case "GBP":
                            currencyIcon = !string.IsNullOrEmpty(list.CurrencySymbolArray.Gbp) ? list.CurrencySymbolArray.Gbp ?? "£" : "£";
                            break;
                        case "RUB":
                            currencyIcon = !string.IsNullOrEmpty(list.CurrencySymbolArray.Rub) ? list.CurrencySymbolArray.Rub ?? "$" : "$";
                            break;
                        case "PLN":
                            currencyIcon = !string.IsNullOrEmpty(list.CurrencySymbolArray.Pln) ? list.CurrencySymbolArray.Pln ?? "zł" : "zł";
                            break;
                        case "ILS":
                            currencyIcon = !string.IsNullOrEmpty(list.CurrencySymbolArray.Ils) ? list.CurrencySymbolArray.Ils ?? "₪" : "₪";
                            break;
                        case "BRL":
                            currencyIcon = !string.IsNullOrEmpty(list.CurrencySymbolArray.Brl) ? list.CurrencySymbolArray.Brl ?? "R$" : "R$";
                            break;
                        case "INR":
                            currencyIcon = !string.IsNullOrEmpty(list.CurrencySymbolArray.Inr) ? list.CurrencySymbolArray.Inr ?? "₹" : "₹";
                            break;
                        default:
                            currencyIcon = !string.IsNullOrEmpty(list.CurrencySymbolArray.Usd) ? list.CurrencySymbolArray.Usd ?? "$" : "$";
                            break;
                    }

                    return (currency, currencyIcon);
                }

                return (AppSettings.CurrencyCodeStatic, AppSettings.CurrencyIconStatic);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return (AppSettings.CurrencyCodeStatic, AppSettings.CurrencyIconStatic);
            }
        }

        public static Dictionary<string, string> GetPrivacyList(Activity activity)
        {
            try
            {
                var arrayAdapter = new Dictionary<string, string>
                {
                    {"0", activity.GetString(Resource.String.Radio_public)},
                    {"1", activity.GetString(Resource.String.Radio_private)},
                    {"2", activity.GetString(Resource.String.Lbl_Unlisted)},
                };

                return arrayAdapter;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new Dictionary<string, string>();
            }
        }

        public static List<VideoObject> ListFilter(List<VideoObject> list, bool removeNotInterested = true)
        {
            try
            {
                const string sDuration = "0:0";
                const string mDuration = "00:00";
                const string hDuration = "00:00:00";

                list.RemoveAll(a => a.Privacy == "1" || a.Privacy == "2" || a.Duration == sDuration || a.Duration == mDuration || a.Duration == hDuration || string.IsNullOrEmpty(a.VideoId.ToString()));

                if (!removeNotInterested) return list;
                List<VideoObject> result = list.Except(ListUtils.GlobalNotInterestedList).ToList();

                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        foreach (var videoObject in result)
                        {
                            SetLinkWithQuality(videoObject);

                            if (string.IsNullOrEmpty(videoObject.CategoryName))
                                videoObject.CategoryName = CategoriesController.GetCategoryName(videoObject);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                });

                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new List<VideoObject>();
            }
        }


        public static void ShowGlobalBadgeSystem(TextView videoTypeIcon, VideoObject item)
        {
            try
            {
                if (!string.IsNullOrEmpty(item.Twitch) && AppSettings.SetTwichTypeBadgeIcon)
                {
                    videoTypeIcon.Visibility = ViewStates.Visible;
                    FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, videoTypeIcon, IonIconsFonts.SocialTwitch);
                    videoTypeIcon.BackgroundTintList = ColorStateList.ValueOf(Color.ParseColor("#6441A4"));
                }
                else if (!string.IsNullOrEmpty(item.Vimeo) && AppSettings.SetVimeoTypeBadgeIcon)
                {
                    FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, videoTypeIcon, IonIconsFonts.SocialVimeo);
                    videoTypeIcon.Visibility = ViewStates.Visible;
                    videoTypeIcon.BackgroundTintList = ColorStateList.ValueOf(Color.ParseColor("#0D94CD"));
                }
                else if (!string.IsNullOrEmpty(item.Youtube) && AppSettings.SetYoutubeTypeBadgeIcon)
                {
                    FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, videoTypeIcon, IonIconsFonts.SocialYoutube);
                    videoTypeIcon.Visibility = ViewStates.Visible;
                    videoTypeIcon.BackgroundTintList = ColorStateList.ValueOf(Color.ParseColor("#FF0000"));
                }
                else if (!string.IsNullOrEmpty(item.Ok) && AppSettings.SetOkTypeBadgeIcon)
                {
                    videoTypeIcon.Text = "Ok";
                    videoTypeIcon.Visibility = ViewStates.Visible;
                    videoTypeIcon.BackgroundTintList = ColorStateList.ValueOf(Color.ParseColor("#F0932A"));
                }
                else if (!string.IsNullOrEmpty(item.Daily) && AppSettings.SetDailyMotionTypeBadgeIcon)
                {
                    videoTypeIcon.Text = "d";
                    videoTypeIcon.Visibility = ViewStates.Visible;
                    videoTypeIcon.BackgroundTintList = ColorStateList.ValueOf(Color.ParseColor("#00d2f3"));
                }
                else if (!string.IsNullOrEmpty(item.Facebook) && AppSettings.SetFacebookTypeBadgeIcon)
                {
                    videoTypeIcon.Text = "FB";
                    videoTypeIcon.Visibility = ViewStates.Visible;
                    videoTypeIcon.BackgroundTintList = ColorStateList.ValueOf(Color.ParseColor("#3b5999"));
                }
                else
                {
                    videoTypeIcon.Visibility = ViewStates.Gone;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void SetLinkWithQuality(VideoObject videoData)
        {
            try
            {
                if (videoData == null
                    || !string.IsNullOrEmpty(videoData.Twitch) || !string.IsNullOrEmpty(videoData.Vimeo) || !string.IsNullOrEmpty(videoData.Youtube)
                    || !string.IsNullOrEmpty(videoData.Ok) || !string.IsNullOrEmpty(videoData.Daily) || !string.IsNullOrEmpty(videoData.Facebook)
                    || videoData.VideoType == "VideoObject/youtube" || videoData.VideoLocation.Contains("Youtube") || videoData.VideoLocation.Contains("youtu"))
                    return;

                var explodeVideo = videoData.VideoLocation.Split("_video");
                if (explodeVideo.Length <= 0) return;

                videoData.VideoAuto = explodeVideo[1].Replace("video", "").Replace("converted.mp4", "").Replace("_", "");

                if (videoData.The240P == "1" && string.IsNullOrEmpty(videoData.Video240P))
                {
                    videoData.Video240P = explodeVideo[0] + "_video_240p_converted.mp4";
                }
                if (videoData.The360P == "1" && string.IsNullOrEmpty(videoData.Video360P))
                {
                    videoData.Video360P = explodeVideo[0] + "_video_360p_converted.mp4";
                }
                if (videoData.The480P == "1" && string.IsNullOrEmpty(videoData.Video480P))
                {
                    videoData.Video480P = explodeVideo[0] + "_video_480p_converted.mp4";
                }
                if (videoData.The720P == "1" && string.IsNullOrEmpty(videoData.Video720P))
                {
                    videoData.Video720P = explodeVideo[0] + "_video_720p_converted.mp4";
                }
                if (videoData.The1080P == "1" && string.IsNullOrEmpty(videoData.Video1080P))
                {
                    videoData.Video1080P = explodeVideo[0] + "_video_1080p_converted.mp4";
                }
                if (videoData.The4096P == "1" && string.IsNullOrEmpty(videoData.Video4096P))
                {
                    videoData.Video4096P = explodeVideo[0] + "_video_4096p_converted.mp4";
                }
                if (videoData.The2048P == "1" && string.IsNullOrEmpty(videoData.Video2048P))
                {
                    videoData.Video2048P = explodeVideo[0] + "_video_2048p_converted.mp4";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static string GetLinkWithQuality(VideoObject videoData, string quality)
        {
            try
            {
                if (videoData == null
                    || !string.IsNullOrEmpty(videoData.Twitch) || !string.IsNullOrEmpty(videoData.Vimeo) || !string.IsNullOrEmpty(videoData.Youtube)
                    || !string.IsNullOrEmpty(videoData.Ok) || !string.IsNullOrEmpty(videoData.Daily) || !string.IsNullOrEmpty(videoData.Facebook)
                    || videoData.VideoType == "VideoObject/youtube" || videoData.VideoLocation.Contains("Youtube") || videoData.VideoLocation.Contains("youtu"))
                    return videoData?.VideoLocation ?? "";

                if (quality.Contains("Auto") && !string.IsNullOrEmpty(videoData.VideoAuto))
                {
                    return videoData.VideoAuto;
                }
                if (quality.Contains("240p") && videoData.The240P == "1" && !string.IsNullOrEmpty(videoData.Video240P))
                {
                    return videoData.Video240P;
                }
                if (quality.Contains("360p") && videoData.The360P == "1" && !string.IsNullOrEmpty(videoData.Video360P))
                {
                    return videoData.Video360P;
                }
                if (quality.Contains("480p") && videoData.The480P == "1" && !string.IsNullOrEmpty(videoData.Video480P))
                {
                    return videoData.Video480P;
                }
                if (quality.Contains("720p") && videoData.The720P == "1" && !string.IsNullOrEmpty(videoData.Video720P))
                {
                    return videoData.Video720P;
                }
                if (quality.Contains("1080p") && videoData.The1080P == "1" && !string.IsNullOrEmpty(videoData.Video1080P))
                {
                    return videoData.Video1080P;
                }
                if (quality.Contains("4096p") && videoData.The4096P == "1" && !string.IsNullOrEmpty(videoData.Video4096P))
                {
                    return videoData.Video4096P;
                }
                if (quality.Contains("2048p") && videoData.The2048P == "1" && !string.IsNullOrEmpty(videoData.Video2048P))
                {
                    return videoData.Video2048P;
                }

                return videoData.VideoLocation ?? "";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return videoData?.VideoLocation ?? "";
            }
        }

        private static Dictionary<string, float> GetListPlaybackSpeedData()
        {
            var playbackSpeeds = new Dictionary<string, float>
            {
                { "0.25x", 0.25f },
                { "0.50x", 0.50f },
                { "0.75x", 0.75f },
                { "Normal", 1f },
                { "1.25x", 1.25f },
                { "1.50x", 1.50f },
                { "1.75x", 1.75f },
                { "2.0x", 2.0f }
            };

            return playbackSpeeds;
        }

        public static List<string> GetListPlaybackSpeed()
        {
            var playbackSpeeds = GetListPlaybackSpeedData();

            return playbackSpeeds.Select(data => data.Key).ToList();
        }

        public static float GetSpeed(string selectedSpeed)
        {
            var playbackSpeeds = GetListPlaybackSpeedData();
            playbackSpeeds.TryGetValue(selectedSpeed, out float speed);

            return speed == 0f ? 1.0f : speed;
        }

        public static string GetSpeedText(float selectedSpeed)
        {

            var playbackSpeeds = GetListPlaybackSpeedData();
            var speedText = playbackSpeeds.Where(pair => pair.Value == selectedSpeed)
                   .Select(pair => pair.Key);

            return speedText?.FirstOrDefault();
        }

        public static List<string> GetListQualityVideo(VideoObject videoData)
        {
            try
            {
                var arrayAdapter = new List<string>();
                if (!string.IsNullOrEmpty(videoData.VideoAuto))
                {
                    arrayAdapter.Add("Auto (" + videoData.VideoAuto + ")");
                }
                if (videoData.The240P == "1")
                {
                    arrayAdapter.Add("240p");
                }
                if (videoData.The360P == "1")
                {
                    arrayAdapter.Add("360p");
                }
                if (videoData.The480P == "1")
                {
                    arrayAdapter.Add("480p");
                }
                if (videoData.The720P == "1")
                {
                    arrayAdapter.Add("720p - HD");
                }
                if (videoData.The1080P == "1")
                {
                    arrayAdapter.Add("1080p - HD");
                }
                if (videoData.The4096P == "1")
                {
                    arrayAdapter.Add("4096p - 4K");
                }
                if (videoData.The2048P == "1")
                {
                    arrayAdapter.Add("2048p - 4K");
                }

                return arrayAdapter;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new List<string>();
            }
        }

        public static bool CheckPictureInPictureAllowed(Activity activityContext)
        {
            try
            {
                bool opPictureInPictureAllowed = false;

                if ((int)Build.VERSION.SdkInt > 23)
                {
                    var appOpsManager = (AppOpsManager)activityContext.GetSystemService(Context.AppOpsService);
                    if (appOpsManager != null)
                        opPictureInPictureAllowed = AppOpsManagerMode.Allowed == appOpsManager.CheckOpNoThrow(AppOpsManager.OpstrPictureInPicture, Process.MyUid(), activityContext.PackageName);
                }

                if (activityContext.PackageManager.HasSystemFeature(PackageManager.FeaturePictureInPicture) && opPictureInPictureAllowed)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

    }
}