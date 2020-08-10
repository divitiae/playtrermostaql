using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Widget;
using PlayTube.Activities.Tabbes;
using PlayTube.Helpers.Controller;
using PlayTube.Helpers.Models;
using PlayTube.Helpers.Utils;
using PlayTube.SQLite;
using PlayTubeClient;
using PlayTubeClient.Classes.Global;
using PlayTubeClient.Classes.Playlist;
using Plugin.Share;
using Plugin.Share.Abstractions;

namespace PlayTube.Activities.Models
{
    public class LibrarySynchronizer
    {
        private readonly Activity Activity;
        private readonly TabbedMainActivity ActivityContext;

        public LibrarySynchronizer(Activity activityContext)
        {
            try
            {
                Activity = activityContext;
                ActivityContext = TabbedMainActivity.GetInstance();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void AddToSubscriptions(VideoObject video, int count = 0)
        {
            try
            {
                if (UserDetails.IsLogin)
                {
                    var item = ActivityContext?.LibraryFragment?.MAdapter?.LibraryList?.FirstOrDefault(a => a.SectionId == "1");
                    if (item == null) return;

                    if (count != 0)
                    {
                        item.VideoCount = count;
                    }
                    else
                    {
                        item.VideoCount = item.VideoCount + 1;
                    }

                    if (video != null)
                    {
                        item.BackgroundImage = video.Thumbnail;
                    }
                    else
                    {
                        item.BackgroundImage = "blackdefault";
                        item.VideoCount = 0;
                    }
                    ActivityContext?.LibraryFragment?.MAdapter?.NotifyItemChanged(0);

                    var sqlEntity = new SqLiteDatabase();
                    sqlEntity.InsertLibraryItem(item);
                    sqlEntity.Dispose();
                }
                else
                {
                    PopupDialogController dialog = new PopupDialogController(Activity, video, "Login");
                    dialog.ShowNormalDialog(Activity.GetText(Resource.String.Lbl_Warning), Activity.GetText(Resource.String.Lbl_Please_sign_in_Subcribed), Activity.GetText(Resource.String.Lbl_Yes), Activity.GetText(Resource.String.Lbl_No));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void AddToWatchLater(VideoObject video, int count = 0)
        {
            try
            {
                if (UserDetails.IsLogin)
                {
                    var item = ActivityContext?.LibraryFragment?.MAdapter?.LibraryList?.FirstOrDefault(a => a.SectionId == "2");
                    if (item == null) return;
                    item.VideoCount = count != 0 ? count : item.VideoCount + 1;
                    if (video != null)
                    {
                        item.BackgroundImage = video.Thumbnail;
                    }
                    else
                    {
                        item.BackgroundImage = "blackdefault";
                        item.VideoCount = 0;
                    }
                    ActivityContext?.LibraryFragment?.MAdapter?.NotifyItemChanged(1);
                }
                else
                {
                    PopupDialogController dialog = new PopupDialogController(Activity, video, "Login");
                    dialog.ShowNormalDialog(Activity.GetText(Resource.String.Lbl_Warning), Activity.GetText(Resource.String.Lbl_Please_sign_in_WatchLater), Activity.GetText(Resource.String.Lbl_Yes), Activity.GetText(Resource.String.Lbl_No));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        public void RemovedFromWatchLater(VideoObject video, int count = 0)
        {
            try
            {
                if (UserDetails.IsLogin)
                {
                    var item = ActivityContext?.LibraryFragment?.MAdapter?.LibraryList?.FirstOrDefault(a => a.SectionId == "2");
                    if (item == null) return;
                    item.VideoCount = count != 0 ? count : item.VideoCount - 1;
                    item.BackgroundImage = "blackdefault";
                    ActivityContext?.LibraryFragment?.MAdapter?.NotifyItemChanged(1);
                }
                else
                {
                    PopupDialogController dialog = new PopupDialogController(Activity, video, "Login");
                    dialog.ShowNormalDialog(Activity.GetText(Resource.String.Lbl_Warning), Activity.GetText(Resource.String.Lbl_Please_sign_in_WatchLater), Activity.GetText(Resource.String.Lbl_Yes), Activity.GetText(Resource.String.Lbl_No));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void AddToRecentlyWatched(VideoObject video, int count = 0)
        {
            try
            {
                var item = ActivityContext?.LibraryFragment?.MAdapter?.LibraryList?.FirstOrDefault(a => a.SectionId == "3");
                if (item == null) return;
                item.VideoCount = count != 0 ? count : item.VideoCount + 1;
                if (video != null)
                {
                    item.BackgroundImage = video.Thumbnail;
                }
                else
                {
                    item.BackgroundImage = "blackdefault";
                    item.VideoCount = 0;
                }

                ActivityContext?.LibraryFragment?.MAdapter?.NotifyItemChanged(2);

                var sqlEntity = new SqLiteDatabase();
                sqlEntity.InsertLibraryItem(item);
                sqlEntity.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void RemoveRecentlyWatched()
        {
            try
            {
                var item = ActivityContext?.LibraryFragment?.MAdapter?.LibraryList?.FirstOrDefault(a => a.SectionId == "3");
                if (item == null) return;
                item.VideoCount = 0;
                item.BackgroundImage = "blackdefault";
                ActivityContext?.LibraryFragment?.MAdapter?.NotifyItemChanged(2);

                var sqlEntity = new SqLiteDatabase();
                sqlEntity.InsertLibraryItem(item);
                sqlEntity.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        public void AddToWatchOffline(VideoObject video, int count = 0)
        {
            try
            {
                var item = ActivityContext?.LibraryFragment?.MAdapter?.LibraryList?.FirstOrDefault(a => a.SectionId == "4");
                if (item == null) return;
                item.VideoCount = count != 0 ? count : item.VideoCount + 1;

                if (video != null)
                {
                    item.BackgroundImage = video.Thumbnail;
                }
                else
                {
                    item.BackgroundImage = "blackdefault";
                    item.VideoCount = 0;
                }
                ActivityContext?.LibraryFragment?.MAdapter?.NotifyItemChanged(3);

                var sqlEntity = new SqLiteDatabase();
                sqlEntity.InsertLibraryItem(item);
                sqlEntity.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        public void AddToLiked(VideoObject video, int count = 0)
        {
            try
            {
                if (UserDetails.IsLogin)
                {
                    var item = ActivityContext?.LibraryFragment?.MAdapter?.LibraryList?.FirstOrDefault(a => a.SectionId == "6");
                    if (item == null) return;
                    if (count != 0)
                    {
                        item.VideoCount = count;
                    }
                    else
                    {
                        item.VideoCount = item.VideoCount + 1;
                    }
                    if (video != null)
                    {
                        item.BackgroundImage = video.Thumbnail;
                    }
                    else
                    {
                        item.BackgroundImage = "blackdefault";
                        item.VideoCount = 0;
                    }
                    ActivityContext?.LibraryFragment?.MAdapter?.NotifyItemChanged(5);

                    var sqlEntity = new SqLiteDatabase();
                    sqlEntity.InsertLibraryItem(item);
                    sqlEntity.Dispose();
                }
                else
                {
                    PopupDialogController dialog = new PopupDialogController(Activity, video, "Login");
                    dialog.ShowNormalDialog(Activity.GetText(Resource.String.Lbl_Warning), Activity.GetText(Resource.String.Lbl_Please_sign_in_Like), Activity.GetText(Resource.String.Lbl_Yes), Activity.GetText(Resource.String.Lbl_No));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }
      
        public void AddToPaid(VideoObject video, int count = 0)
        {
            try
            {
                if (UserDetails.IsLogin)
                {
                    var item = ActivityContext?.LibraryFragment?.MAdapter?.LibraryList?.FirstOrDefault(a => a.SectionId == "8");
                    if (item == null) return;
                    item.VideoCount = count != 0 ? count : item.VideoCount + 1;
                    if (video != null)
                    {
                        item.BackgroundImage = video.Thumbnail;
                    }
                    else
                    {
                        item.BackgroundImage = "blackdefault";
                        item.VideoCount = 0;
                    }
                    ActivityContext?.LibraryFragment?.MAdapter?.NotifyItemChanged(5);

                    var sqlEntity = new SqLiteDatabase();
                    sqlEntity.InsertLibraryItem(item);
                    sqlEntity.Dispose();
                }
                else
                {
                    PopupDialogController dialog = new PopupDialogController(Activity, video, "Login");
                    dialog.ShowNormalDialog(Activity.GetText(Resource.String.Lbl_Warning), Activity.GetText(Resource.String.Lbl_Please_sign_in_Paid), Activity.GetText(Resource.String.Lbl_Yes), Activity.GetText(Resource.String.Lbl_No));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }
         
        public void AddToPlaylist(VideoObject video)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    if (UserDetails.IsLogin)
                    {
                        PopupDialogController dialog = new PopupDialogController(ActivityContext, video, "PlayList");
                        dialog.ShowPlayListDialog();
                    }
                    else
                    {
                        PopupDialogController dialog = new PopupDialogController(ActivityContext, video, "Login");
                        dialog.ShowNormalDialog(Activity.GetText(Resource.String.Lbl_Warning), Activity.GetText(Resource.String.Lbl_Please_sign_in_playlist), Activity.GetText(Resource.String.Lbl_Yes), Activity.GetText(Resource.String.Lbl_No));
                    }
                }
                else
                {
                    Toast.MakeText(Activity, Activity.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void AddToPlaylistVideo(VideoObject video, int count = 0)
        {
            try
            {
                var item = ActivityContext?.LibraryFragment?.MAdapter?.LibraryList?.FirstOrDefault(a => a.SectionId == "5");
                if (item == null) return;
                item.VideoCount = count != 0 ? count : item.VideoCount + 1;
                if (video != null)
                {
                    item.BackgroundImage = video.Thumbnail;
                }
                else
                {
                    item.BackgroundImage = "blackdefault";
                    item.VideoCount = 0;
                }
                ActivityContext?.LibraryFragment?.MAdapter?.NotifyItemChanged(4);

                var sqlEntity = new SqLiteDatabase();
                sqlEntity.InsertLibraryItem(item);
                sqlEntity.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void AddReportVideo(VideoObject video)
        {
            try
            {
                if (UserDetails.IsLogin)
                {
                    PopupDialogController dialog = new PopupDialogController(Activity, video, "Report");
                    dialog.ShowEditTextDialog(Activity.GetText(Resource.String.Lbl_Report_This_Video), Activity.GetText(Resource.String.Lbl_Report_This_Video_Text), Activity.GetText(Resource.String.Lbl_Submit), Activity.GetText(Resource.String.Lbl_Cancel));
                }
                else
                {
                    PopupDialogController dialog = new PopupDialogController(Activity, video, "Login");
                    dialog.ShowNormalDialog(Activity.GetText(Resource.String.Lbl_Warning), Activity.GetText(Resource.String.Lbl_Please_sign_in_Report), Activity.GetText(Resource.String.Lbl_Yes), Activity.GetText(Resource.String.Lbl_No));
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void AddToShareVideo(VideoObject video, int count = 0)
        {
            try
            {
                var item = ActivityContext?.LibraryFragment?.MAdapter?.LibraryList?.FirstOrDefault(a => a.SectionId == "7");
                if (item == null) return;
                item.VideoCount = count != 0 ? count : item.VideoCount + 1;
                if (video != null)
                {
                    item.BackgroundImage = video.Thumbnail;
                }
                else
                {
                    item.BackgroundImage = "blackdefault";
                    item.VideoCount = 0;
                }
                ActivityContext?.LibraryFragment?.MAdapter?.NotifyItemChanged(6);

                var sqlEntity = new SqLiteDatabase();
                sqlEntity.InsertLibraryItem(item);
                sqlEntity.Dispose();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public async void ShareVideo(VideoObject video)
        {
            try
            {
                //Share Plugin same as video
                if (!CrossShare.IsSupported)
                {
                    return;
                }

                await CrossShare.Current.Share(new ShareMessage
                {
                    Title = video.Title,
                    Text = video.Description,
                    Url = video.Url
                });

                var sqlEntity = new SqLiteDatabase();
                sqlEntity.Insert_SharedVideos(video);
                sqlEntity.Dispose();

                AddToShareVideo(video);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void EditPlaylist(PlayListVideoObject video)
        {
            try
            {
                ActivityContext.EditPlaylistOnClick(video);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Event Menu >> Help 
        public void OnMenuHelpClick(VideoObject videoObject)
        {
            try
            {
                var intent = new Intent(Activity, typeof(LocalWebViewActivity));
                intent.PutExtra("URL", Client.WebsiteUrl + "/contact-us");
                intent.PutExtra("Type", Activity.GetText(Resource.String.Lbl_Help));
                Activity.StartActivity(intent);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}