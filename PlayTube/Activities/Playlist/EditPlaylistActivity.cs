using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Ads;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using Newtonsoft.Json;
using PlayTube.Helpers.Ads;
using PlayTube.Helpers.Utils;
using PlayTubeClient.Classes.Global;
using PlayTubeClient.Classes.Playlist;
using PlayTubeClient.RestCalls;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace PlayTube.Activities.Playlist
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class EditPlaylistActivity : AppCompatActivity
    {
        #region Variables Basic

        public EditText TxtNewplaylist, TxtDescription;
        public RadioButton RbPrivate, RbPublic;
        public TextView SaveTextView;
        public string Status = "", PlaylistId = "";
        public AdView MAdView;
        public PlayListVideoObject PlayListVideoObject;
        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Methods.App.FullScreenApp(this);

                SetTheme(AppSettings.SetTabDarkTheme ? Resource.Style.MyTheme_Dark_Base : Resource.Style.MyTheme_Base);

                // Create your application here
                SetContentView(Resource.Layout.CreatNewPlaylist_Layout);
                 
                PlaylistId = Intent.GetStringExtra("IdList");

                //Get Value And Set Toolbar
                InitComponent();
                InitToolbar(); 
                GetDataPlayList();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        protected override void OnResume()
        {
            try
            {
                base.OnResume();
                AddOrRemoveEvent(true);

                MAdView?.Resume();
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

                MAdView?.Pause();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            try
            {
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                base.OnTrimMemory(level);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override void OnLowMemory()
        {
            try
            {
                GC.Collect(GC.MaxGeneration);
                base.OnLowMemory();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected override void OnDestroy()
        {
            try
            {
                MAdView?.Destroy();
                base.OnDestroy();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void GetDataPlayList()
        {
            try
            {
                PlayListVideoObject = JsonConvert.DeserializeObject<PlayListVideoObject>(Intent.GetStringExtra("Item"));
                if (PlayListVideoObject != null)
                {
                    TxtNewplaylist.Text = PlayListVideoObject.Name;
                    TxtDescription.Text = PlayListVideoObject.Description;

                    if (PlayListVideoObject.Privacy == 1)
                    {
                        RbPublic.Checked = true;
                        RbPrivate.Checked = false;
                        Status = "1";
                    } 
                    else
                    {
                        RbPublic.Checked = false;
                        RbPrivate.Checked = true;
                        Status = "0";
                    } 
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
          
        #region Menu

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        #endregion

        #region Functions

        private void InitComponent()
        {
            try
            {
                //Get values
                TxtNewplaylist = FindViewById<EditText>(Resource.Id.nameplaylist_Edit);
                TxtDescription = FindViewById<EditText>(Resource.Id.description_Edit);
                RbPrivate = (RadioButton)FindViewById(Resource.Id.radioPrivate);
                RbPublic = (RadioButton)FindViewById(Resource.Id.radioPublic);

                SaveTextView = FindViewById<TextView>(Resource.Id.toolbar_title);
                SaveTextView.Click += SaveTextViewOnClick;

                MAdView = FindViewById<AdView>(Resource.Id.adView);
                AdsGoogle.InitAdView(MAdView, null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void InitToolbar()
        {
            try
            {
                //Set ToolBar
                var toolBar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (toolBar != null)
                {
                    toolBar.Title = GetText(Resource.String.Lbl_Creat_New_Playlist);

                    SetSupportActionBar(toolBar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
 
        public void AddOrRemoveEvent(bool addEvent)
        {
            try
            {
                // true +=  // false -=
                if (addEvent)
                {
                    //Event  
                    RbPrivate.CheckedChange += RbPrivateOnCheckedChange;
                    RbPublic.CheckedChange += RbPublicOnCheckedChange;
                }
                else
                {
                    //Event  
                    RbPrivate.CheckedChange -= RbPrivateOnCheckedChange;
                    RbPublic.CheckedChange -= RbPublicOnCheckedChange;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Events

        private void SaveTextViewOnClick(object sender, EventArgs e)
        {
            SaveDataButtonOnClick();
        }
         
        //Public
        private void RbPublicOnCheckedChange(object sender, CompoundButton.CheckedChangeEventArgs checkedChangeEventArgs)
        {
            try
            {
                bool isChecked = RbPublic.Checked;
                if (isChecked)
                {
                    RbPrivate.Checked = false;
                    Status = "1";
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void RbPrivateOnCheckedChange(object sender, CompoundButton.CheckedChangeEventArgs checkedChangeEventArgs)
        {
            try
            {
                bool isChecked = RbPrivate.Checked;
                if (isChecked)
                {
                    RbPublic.Checked = false;
                    Status = "0";
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private async void SaveDataButtonOnClick()
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    //Show a progress
                    AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading));

                   var (apiResult, respond) = await RequestsAsync.Playlist.EditPlaylist_Http(PlaylistId, TxtNewplaylist.Text, TxtDescription.Text, Status); 
                    if (apiResult == 200)
                    {
                        if (respond is MessageObject result)
                        {
                            Console.WriteLine(result.Message);
                            PlayListVideoObject playLists = new PlayListVideoObject
                            {
                                Id = PlayListVideoObject.Id,
                                ListId = PlaylistId,
                                UserId = PlayListVideoObject.UserId,
                                Name = TxtNewplaylist.Text,
                                Description = TxtDescription.Text,
                                Privacy = Convert.ToInt32(Status),
                                Views = PlayListVideoObject.Views,
                                Icon = PlayListVideoObject.Icon,
                                Time = PlayListVideoObject.Time,
                            };

                            var dataPlayList = ListUtils.PlayListVideoObjectList?.FirstOrDefault(q => q.ListId == PlaylistId);
                            if (dataPlayList != null)
                                dataPlayList = playLists;

                            Console.WriteLine(dataPlayList);
                            AndHUD.Shared.Dismiss(this);
                            Toast.MakeText(this, GetText(Resource.String.Lbl_Created_successfully_playlist), ToastLength.Short).Show();

                            Intent intent = new Intent();
                            intent.PutExtra("ItemPlaylist", JsonConvert.SerializeObject(playLists));
                            SetResult(Result.Ok, intent);
                            Finish();
                        }
                    }
                    else Methods.DisplayReportResult(this, respond);
                }
                else
                {
                    AndHUD.Shared.Dismiss(this);
                    Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                }
            }
            catch (Exception exception)
            {
                AndHUD.Shared.Dismiss(this);
                Console.WriteLine(exception);
            }
        }

        #endregion 
    }
}