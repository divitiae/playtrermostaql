//###############################################################
// Author >> Elin Doughouz 
// Copyright (c) PlayTube 12/07/2018 All Right Reserved
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// Follow me on facebook >> https://www.facebook.com/Elindoughous
//=========================================================

using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Content;
using Com.Luseen.Autolinklibrary;
using PlayTube.Activities;
using PlayTube.Activities.Tabbes;
using PlayTube.Activities.Videos;
using PlayTube.Helpers.Utils;

namespace PlayTube.Helpers.Controller
{
    public class TextSanitizer 
    {
        private readonly TabbedMainActivity Context;
        private readonly AutoLinkTextView AutoLinkTextView;
        private readonly Activity Activity;

        public TextSanitizer(AutoLinkTextView linkTextView , Activity activity , TabbedMainActivity contextTabbed = null)
        {
            try
            {
                AutoLinkTextView = linkTextView;
                Activity = activity;
                Context = contextTabbed;          
                
                AutoLinkTextView.AutoLinkOnClick += AutoLinkTextViewOnAutoLinkOnClick; 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
        public void Load(string text, string position = "Left")
        {
            try
            {
                AutoLinkTextView.AddAutoLinkMode(AutoLinkMode.ModePhone, AutoLinkMode.ModeEmail, AutoLinkMode.ModeHashtag, AutoLinkMode.ModeUrl, AutoLinkMode.ModeMention, AutoLinkMode.ModeCustom);
                if (position == "Right" || position == "right")
                {
                    AutoLinkTextView.SetPhoneModeColor(ContextCompat.GetColor(Activity, Resource.Color.right_ModePhone_color));
                    AutoLinkTextView.SetEmailModeColor(ContextCompat.GetColor(Activity, Resource.Color.right_ModeEmail_color));
                    AutoLinkTextView.SetHashtagModeColor(ContextCompat.GetColor(Activity, Resource.Color.right_ModeHashtag_color));
                    AutoLinkTextView.SetUrlModeColor(ContextCompat.GetColor(Activity, Resource.Color.right_ModeUrl_color));
                    AutoLinkTextView.SetMentionModeColor(ContextCompat.GetColor(Activity, Resource.Color.right_ModeMention_color));
                    AutoLinkTextView.SetCustomModeColor(ContextCompat.GetColor(Activity, Resource.Color.right_ModeUrl_color));
                    AutoLinkTextView.SetSelectedStateColor(ContextCompat.GetColor(Activity, Resource.Color.accent));
                }
                else
                {
                    AutoLinkTextView.SetPhoneModeColor(ContextCompat.GetColor(Activity, Resource.Color.Left_ModePhone_color));
                    AutoLinkTextView.SetEmailModeColor(ContextCompat.GetColor(Activity, Resource.Color.Left_ModeEmail_color));
                    AutoLinkTextView.SetHashtagModeColor(ContextCompat.GetColor(Activity, Resource.Color.Left_ModeHashtag_color));
                    AutoLinkTextView.SetUrlModeColor(ContextCompat.GetColor(Activity, Resource.Color.Left_ModeUrl_color));
                    AutoLinkTextView.SetMentionModeColor(ContextCompat.GetColor(Activity, Resource.Color.Left_ModeMention_color));
                    AutoLinkTextView.SetCustomModeColor(ContextCompat.GetColor(Activity, Resource.Color.Left_ModeUrl_color));
                    AutoLinkTextView.SetSelectedStateColor(ContextCompat.GetColor(Activity, Resource.Color.accent));
                }
                var textt = text.Split('/');
                if (textt.Count() > 1)
                {
                    AutoLinkTextView.SetCustomModeColor(ContextCompat.GetColor(Activity, Resource.Color.Left_ModeUrl_color));
                    AutoLinkTextView.SetCustomRegex(@"\b(" + textt.LastOrDefault() + @")\b");
                }

                string laststring = text.Replace(" /", " ");
                if (!string.IsNullOrEmpty(laststring))
                    AutoLinkTextView.SetAutoLinkText(laststring);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
      
        public void AutoLinkTextViewOnAutoLinkOnClick(object sender, AutoLinkOnClickEventArgs autoLinkOnClickEventArgs)
        {
            try
            {
                AutoLinkMode matchedText = autoLinkOnClickEventArgs.P0;
                var typetext = Methods.FunString.Check_Regex(autoLinkOnClickEventArgs.P1);
                if (typetext == "Email" || matchedText == AutoLinkMode.ModeEmail)
                {
                    Methods.App.SendEmail(Activity, autoLinkOnClickEventArgs.P1);
                }
                else if (typetext == "Website" || matchedText == AutoLinkMode.ModeUrl)
                {
                    string url = autoLinkOnClickEventArgs.P1;
                    if (!autoLinkOnClickEventArgs.P1.Contains("http"))
                    {
                        url = "http://" + autoLinkOnClickEventArgs.P1;
                    }
                   
                    var intent = new Intent(Activity, typeof(LocalWebViewActivity));
                    intent.PutExtra("URL", url);
                    intent.PutExtra("Type", url);
                    Activity.StartActivity(intent);
                }
                else if (typetext == "Hashtag" || matchedText == AutoLinkMode.ModeHashtag)
                {
                    Bundle bundle = new Bundle();
                    bundle.PutString("Key", autoLinkOnClickEventArgs.P1.Replace("#", ""));
                    VideosByKeyFragment videoViewerFragment = new VideosByKeyFragment()
                    {
                        Arguments = bundle
                    };
                    Context?.FragmentBottomNavigator?.DisplayFragment(videoViewerFragment); 
                }
                else if (typetext == "Mention" || matchedText == AutoLinkMode.ModeMention)
                {
                    Bundle bundle = new Bundle();
                    bundle.PutString("Key", autoLinkOnClickEventArgs.P1.Replace("@", ""));
                    VideosByKeyFragment videoViewerFragment = new VideosByKeyFragment()
                    {
                        Arguments = bundle
                    };
                    Context?.FragmentBottomNavigator?.DisplayFragment(videoViewerFragment);
                }
                else if (typetext == "Number" || matchedText == AutoLinkMode.ModePhone)
                {
                    Methods.App.SaveContacts(Activity, autoLinkOnClickEventArgs.P1, "", "2");
                } 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        } 
    }
}