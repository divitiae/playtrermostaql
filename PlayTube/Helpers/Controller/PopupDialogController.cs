//###############################################################
// Author >> Elin Doughouz 
// Copyright (c) PlayTube 12/07/2018 All Right Reserved
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// Follow me on facebook >> https://www.facebook.com/Elindoughous
//=========================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Text;
using Android.Views;
using Android.Widget;
using Java.Lang;
using PlayTube.Activities.Default;
using PlayTube.Activities.Models;
using PlayTube.Activities.Playlist;
using PlayTube.Activities.SettingsPreferences;
using PlayTube.Activities.Tabbes;
using PlayTube.Helpers.Utils;
using PlayTubeClient.Classes.Global;
using PlayTubeClient.RestCalls;
using Exception = System.Exception;

namespace PlayTube.Helpers.Controller
{
   public class PopupDialogController : Java.Lang.Object, MaterialDialog.IListCallback, MaterialDialog.ISingleButtonCallback, MaterialDialog.IInputCallback
    {
        private readonly Activity ActivityContext;
        private VideoObject Videodata;
        private string TypeDialog;

        public PopupDialogController(Activity activity, VideoObject videoobje,string typeDialog)
        {
            ActivityContext = activity;
            Videodata = videoobje;
            TypeDialog = typeDialog;
        }

       public async void ShowPlayListDialog()
       {
           try
           { 
               MaterialDialog.Builder progressDialog = new MaterialDialog.Builder(ActivityContext).Theme(AppSettings.SetTabDarkTheme ? Theme.Dark : Theme.Light);
               progressDialog.Title(ActivityContext.GetText(Resource.String.Lbl_Checking_PlayLists_info));
               progressDialog.Content(ActivityContext.GetText(Resource.String.Lbl_Please_wait));
               progressDialog.Progress(true, 0);
               progressDialog.ProgressIndeterminateStyle(true);

               MaterialDialog dialog = progressDialog.Build();
               dialog.Show();

               if (ListUtils.PlayListVideoObjectList.Count == 0)
               {
                   await ApiRequest.PlayListsVideosApi(ActivityContext);
               }

               dialog.Dismiss();

               List<string> arrayAdapter = new List<string>();
               MaterialDialog.Builder dialogList = new MaterialDialog.Builder(ActivityContext).Theme(AppSettings.SetTabDarkTheme ? Theme.Dark : Theme.Light);

                var count = ListUtils.PlayListVideoObjectList.Count;
                if (count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        if (!string.IsNullOrEmpty(ListUtils.PlayListVideoObjectList[i].Name))
                            arrayAdapter.Add(ListUtils.PlayListVideoObjectList[i].Name);
                    }
                }

               dialogList.Title(ActivityContext.GetText(Resource.String.Lbl_Select_One_Name));
               dialogList.Items(arrayAdapter);
               dialogList.PositiveText(ActivityContext.GetText(Resource.String.Lbl_Creat_new)).OnPositive(this);
               dialogList.NegativeText(ActivityContext.GetText(Resource.String.Lbl_Cancel)).OnNegative(this);
               dialogList.ItemsCallback(this).Build().Show();
           }
           catch (Exception exception)
           {
               Console.WriteLine(exception);
           }
       }

        public void ShowNormalDialog(string title, string content =null, string positiveText =null, string negativeText = null)
        {
            try
            {
                MaterialDialog.Builder dialogList = new MaterialDialog.Builder(ActivityContext).Theme(AppSettings.SetTabDarkTheme ? Theme.Dark : Theme.Light);
               
                if (!string.IsNullOrEmpty(title))
                    dialogList.Title(title);

                if (!string.IsNullOrEmpty(content))
                    dialogList.Content(content);

                if (!string.IsNullOrEmpty(negativeText))
                {
                    dialogList.NegativeText(negativeText);
                    dialogList.OnNegative(this);
                }

                if (!string.IsNullOrEmpty(positiveText))
                {
                    dialogList.PositiveText(positiveText);
                    dialogList.OnPositive(this);
                }

                dialogList.Build().Show();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void ShowEditTextDialog(string title, string content = null, string positiveText = null, string negativeText = null)
        {
            try
            {
                MaterialDialog.Builder dialogList = new MaterialDialog.Builder(ActivityContext).Theme(AppSettings.SetTabDarkTheme ? Theme.Dark : Theme.Light);

                if (!string.IsNullOrEmpty(title))
                    dialogList.Title(title);

                if (!string.IsNullOrEmpty(content))
                    dialogList.Content(content);

                if (!string.IsNullOrEmpty(negativeText))
                {
                    dialogList.NegativeText(negativeText);
                    dialogList.OnNegative(this);
                }

                if (!string.IsNullOrEmpty(positiveText))
                {
                    dialogList.PositiveText(positiveText);
                    dialogList.OnPositive(this);
                }
                
                dialogList.InputType(InputTypes.ClassText | InputTypes.TextFlagMultiLine);
                dialogList.Input("", "", this);
                dialogList.Build().Show();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
        public  void OnSelection(MaterialDialog p0, View p1, int p2, ICharSequence selectedPlayListName)
        {
            try
            {
                if (TypeDialog == "PlayList")
                {
                    var dataPlaylist = ListUtils.PlayListVideoObjectList.FirstOrDefault(a => a.Name == selectedPlayListName.ToString());
                    if (dataPlaylist != null)
                    { 
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Playlist.Add_To_List_Http(Videodata.Id, dataPlaylist.ListId) });

                        new LibrarySynchronizer(ActivityContext).AddToPlaylistVideo(Videodata);
                        Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_Video_added), ToastLength.Short).Show();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        public void OnClick(MaterialDialog p0, DialogAction p1)
        {
            try
            {
                if (TypeDialog == "PlayList")
                {
                    if (p1 == DialogAction.Positive)
                    {
                        ActivityContext.StartActivity(new Intent(ActivityContext, typeof(CreatNewPlaylistActivity)));
                    }
                    else if (p1 == DialogAction.Negative)
                    {
                        p0.Dismiss();
                    }
                }
                else if (TypeDialog == "Login")
                {
                    if (p1 == DialogAction.Positive)
                    {
                        TabbedMainActivity.GetInstance()?.StopFragmentVideo();
                        ActivityContext.StartActivity(new Intent(ActivityContext, typeof(LoginActivity)));
                    }
                    else if (p1 == DialogAction.Negative)
                    {
                        p0.Dismiss();
                    }
                }
                else if (TypeDialog == "DeleteAcount")
                {
                    if (p1 == DialogAction.Positive)
                    {
                        ActivityContext.StartActivity(new Intent(ActivityContext, typeof(DeleteAcountActivity)));
                    }
                    else if (p1 == DialogAction.Negative)
                    {
                        p0.Dismiss();
                    }
                }
                else if (TypeDialog == "Logout")
                {
                    if (p1 == DialogAction.Positive)
                    {
                         TabbedMainActivity.GetInstance()?.VideoActionsController.ReleaseVideo();

                         ApiRequest.Logout(ActivityContext); 
                    }
                    else if (p1 == DialogAction.Negative)
                    {
                        p0.Dismiss();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e); 
            } 
        }
         
        public void OnInput(MaterialDialog p0, ICharSequence p1)
        {
            try
            {
                if (TypeDialog == "Report")
                {
                    if (p1.Length() > 0)
                    {
                        if (Methods.CheckConnectivity())
                        {
                            PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Video.Report_Video_Http(Videodata.Id, p1.ToString()) });
                             
                            Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_received_your_report), ToastLength.Short).Show();
                        }
                        else
                        {
                            Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                        }
                    }
                    else 
                    {
                        Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_The_name_can_not_be_blank), ToastLength.Short).Show();
                    }
                } 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}