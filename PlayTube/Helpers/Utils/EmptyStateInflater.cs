using System;
using Android.App;
using Android.Views;
using Android.Widget;

namespace PlayTube.Helpers.Utils
{
    public class EmptyStateInflater
    {
        public Button EmptyStateButton;
        public ImageView EmptyStateIcon;
        public TextView DescriptionText;
        public TextView TitleText;

        public enum Type
        {
            NoConnection,
            NoSearchResult,
            SomThingWentWrong,
            NoComments,
            NoReplies,
            NoNotification,
            NoMessage,
            NoVideo,
            NoVideoByCat,
            NoArticle,
            NoRecentlyWatched,
            NoLiked,
            NoPlayLists,
            NoTrending,
            NoWatchLater,
            NoWatchOfflineVideos,
            NoChannels,
            NoPaid,
            NoBlock,
            SubscribeChannelWithPaid,
            Login,
            NoSessions,
            NoActivities,
        }

        public void InflateLayout(View inflated, Type type)
        {
            try
            {
                EmptyStateIcon = (ImageView)inflated.FindViewById(Resource.Id.emtyicon);
                TitleText = (TextView)inflated.FindViewById(Resource.Id.headText);
                DescriptionText = (TextView)inflated.FindViewById(Resource.Id.seconderyText);
                EmptyStateButton = (Button)inflated.FindViewById(Resource.Id.button);

                if (type == Type.NoConnection)
                {
                    //FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, EmptyStateIcon, IonIconsFonts.IosThunderstormOutline);
                    EmptyStateIcon.SetImageResource(Resource.Drawable.img_no_internet);
                    TitleText.Text = Application.Context.GetText(Resource.String.Lbl_NoConnection_TitleText);
                    DescriptionText.Text = Application.Context.GetText(Resource.String.Lbl_NoConnection_DescriptionText);
                    EmptyStateButton.Text = Application.Context.GetText(Resource.String.Lbl_NoConnection_Button);

                }
                else if (type == Type.NoSearchResult)
                {
                    //FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, EmptyStateIcon, IonIconsFonts.Search);
                    EmptyStateIcon.SetImageResource(Resource.Drawable.icon_search_vector);
                    TitleText.Text = Application.Context.GetText(Resource.String.Lbl_Empty_search);
                    DescriptionText.Text = Application.Context.GetText(Resource.String.Lbl_Start_search);
                    EmptyStateButton.Visibility = ViewStates.Gone;
                }
                else if (type == Type.SomThingWentWrong)
                {
                    //FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, EmptyStateIcon, IonIconsFonts.Close);
                    EmptyStateIcon.SetImageResource(Resource.Drawable.ic_warning_po);
                    TitleText.Text = Application.Context.GetText(Resource.String.Lbl_SomThingWentWrong_TitleText);
                    DescriptionText.Text = Application.Context.GetText(Resource.String.Lbl_SomThingWentWrong_DescriptionText);
                    EmptyStateButton.Text = Application.Context.GetText(Resource.String.Lbl_SomThingWentWrong_Button);
                } 
                else if (type == Type.NoComments)
                {
                    //FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, EmptyStateIcon, IonIconsFonts.Chatbubble);
                    EmptyStateIcon.SetImageResource(Resource.Drawable.icon_comment_post_vector);
                    TitleText.Text = Application.Context.GetText(Resource.String.Lbl_NoComments_TitleText);
                    DescriptionText.Text = Application.Context.GetText(Resource.String.Lbl_NoComments_DescriptionText);
                    EmptyStateButton.Visibility = ViewStates.Gone;
                }
                else if (type == Type.NoReplies)
                {
                    EmptyStateIcon.Visibility = ViewStates.Gone;
                    TitleText.Text = Application.Context.GetText(Resource.String.Lbl_NoReplies); 
                    DescriptionText.Text = " ";
                    EmptyStateButton.Visibility = ViewStates.Gone;
                }
                else if (type == Type.NoNotification)
                {
                    //FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, EmptyStateIcon, IonIconsFonts.AndroidNotifications);
                    EmptyStateIcon.SetImageResource(Resource.Drawable.icon_notification_vector);
                    TitleText.Text = Application.Context.GetText(Resource.String.Lbl_Empty_notifications);
                    DescriptionText.Text = Application.Context.GetText(Resource.String.Lbl_Start_notifications);
                    EmptyStateButton.Visibility = ViewStates.Gone;
                } 
                else if (type == Type.NoMessage)
                {
                    //FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, EmptyStateIcon, IonIconsFonts.Chatbox);
                    EmptyStateIcon.SetImageResource(Resource.Drawable.icon_message_vector);
                    TitleText.Text = Application.Context.GetText(Resource.String.Lbl_Empty_Lastmessages);
                    DescriptionText.Text = Application.Context.GetText(Resource.String.Lbl_Start_Lastmessages);
                    EmptyStateButton.Visibility = ViewStates.Gone;
                }
                else if (type == Type.NoArticle)
                {
                    //FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeSolid, EmptyStateIcon, "\uf15c");
                    EmptyStateIcon.SetImageResource(Resource.Drawable.icon_blog_vector);
                    TitleText.Text = Application.Context.GetText(Resource.String.Lbl_Empty_Article);
                    DescriptionText.Text = Application.Context.GetText(Resource.String.Lbl_Start_Article);
                    EmptyStateButton.Visibility = ViewStates.Gone;
                }
                else if (type == Type.NoVideo)
                {
                    //FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, EmptyStateIcon, IonIconsFonts.AndroidCamera);
                    EmptyStateIcon.SetImageResource(Resource.Drawable.ic_no_video_vector);
                    TitleText.Text = Application.Context.GetText(Resource.String.Lbl_Emptyvideos);
                    DescriptionText.Text = Application.Context.GetText(Resource.String.Lbl_No_videos_found_for_now);
                    EmptyStateButton.Visibility = ViewStates.Gone;
                }
                else if (type == Type.NoVideoByCat)
                {
                    //FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, EmptyStateIcon, IonIconsFonts.AndroidCamera);
                    EmptyStateIcon.SetImageResource(Resource.Drawable.ic_no_video_vector);

                    TitleText.Text = Application.Context.GetText(Resource.String.Lbl_Emptyvideos);
                    DescriptionText.Text = Application.Context.GetText(Resource.String.Lbl_Start_Category);
                    EmptyStateButton.Visibility = ViewStates.Gone;
                }
                else if (type == Type.NoRecentlyWatched)
                {
                    //FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, EmptyStateIcon, IonIconsFonts.AndroidCamera);
                    EmptyStateIcon.SetImageResource(Resource.Drawable.ic_no_video_vector);

                    TitleText.Text = Application.Context.GetText(Resource.String.Lbl_Dont_have_any_videos);
                    DescriptionText.Text = Application.Context.GetText(Resource.String.Lbl_Start_Historyvideo);
                    EmptyStateButton.Visibility = ViewStates.Gone;
                }
                else if (type == Type.NoLiked)
                {
                    //FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, EmptyStateIcon, IonIconsFonts.AndroidCamera);
                    EmptyStateIcon.SetImageResource(Resource.Drawable.ic_no_video_vector);

                    TitleText.Text = Application.Context.GetText(Resource.String.Lbl_Dont_have_any_videos);
                    DescriptionText.Text = Application.Context.GetText(Resource.String.Lbl_Start_likedvideo);
                    EmptyStateButton.Visibility = ViewStates.Gone;
                }
                else if (type == Type.NoPlayLists)
                {
                    //FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, EmptyStateIcon, IonIconsFonts.AndroidCamera);
                    EmptyStateIcon.SetImageResource(Resource.Drawable.ic_no_playlist_vector);

                    TitleText.Text = Application.Context.GetText(Resource.String.Lbl_Empty_PlayLists);
                    DescriptionText.Text = Application.Context.GetText(Resource.String.Lbl_No_videos_found_for_now);
                    EmptyStateButton.Visibility = ViewStates.Gone;
                }
                else if (type == Type.NoTrending)
                {
                    //FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, EmptyStateIcon, IonIconsFonts.AndroidCamera);
                    EmptyStateIcon.SetImageResource(Resource.Drawable.icon_fire_vector);

                    TitleText.Text = Application.Context.GetText(Resource.String.Lbl_Emptyvideos);
                    DescriptionText.Text = Application.Context.GetText(Resource.String.Lbl_Start_Trending);
                    EmptyStateButton.Visibility = ViewStates.Gone;
                }
                else if (type == Type.NoWatchLater)
                {
                    //FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, EmptyStateIcon, IonIconsFonts.AndroidCamera);
                    EmptyStateIcon.SetImageResource(Resource.Drawable.ic_no_video_vector);

                    TitleText.Text = Application.Context.GetText(Resource.String.Lbl_Dont_have_any_videos);
                    DescriptionText.Text = Application.Context.GetText(Resource.String.Lbl_Start_watchlater);
                    EmptyStateButton.Visibility = ViewStates.Gone;
                }
                else if (type == Type.NoWatchOfflineVideos)
                {
                    //FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, EmptyStateIcon, IonIconsFonts.AndroidCamera);
                    EmptyStateIcon.SetImageResource(Resource.Drawable.ic_no_video_vector);

                    TitleText.Text = Application.Context.GetText(Resource.String.Lbl_Dont_have_any_videos);
                    DescriptionText.Text = Application.Context.GetText(Resource.String.Lbl_Start_watchoffline);
                    EmptyStateButton.Visibility = ViewStates.Gone;
                }
                else if (type == Type.NoChannels)
                {
                    //FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, EmptyStateIcon, IonIconsFonts.AndroidCamera);
                    EmptyStateIcon.SetImageResource(Resource.Drawable.icon_username_vector);

                    TitleText.Text = Application.Context.GetText(Resource.String.Lbl_NoChannelsFound);
                    DescriptionText.Text = "";
                    EmptyStateButton.Visibility = ViewStates.Gone;
                }
                else if (type == Type.NoPaid)
                {
                    //FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, EmptyStateIcon, IonIconsFonts.AndroidCamera);
                    EmptyStateIcon.SetImageResource(Resource.Drawable.ic_no_video_vector);

                    TitleText.Text = Application.Context.GetText(Resource.String.Lbl_NoPaidVideosFound);
                    DescriptionText.Text = "";
                    EmptyStateButton.Visibility = ViewStates.Gone;
                }
                else if (type == Type.NoBlock)
                {
                    //FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, EmptyStateIcon, IonIconsFonts.Person);
                    EmptyStateIcon.SetImageResource(Resource.Drawable.icon_block_vector); 
                    TitleText.Text = Application.Context.GetText(Resource.String.Lbl_NoBlockUsers);
                    DescriptionText.Text = " ";
                    EmptyStateButton.Visibility = ViewStates.Gone;
                }
                else if (type == Type.SubscribeChannelWithPaid)
                {
                    //FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, EmptyStateIcon, IonIconsFonts.SocialUsd);
                    EmptyStateIcon.SetImageResource(Resource.Drawable.ic_hand_usd_vector);

                    TitleText.Text = "";
                    DescriptionText.Text = " ";
                    EmptyStateButton.Visibility = ViewStates.Visible;
                }
                else if (type == Type.Login)
                {
                    //FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, EmptyStateIcon, IonIconsFonts.AndroidBulb);
                    EmptyStateIcon.SetImageResource(Resource.Drawable.icon_username_vector);

                    TitleText.Text = Application.Context.GetText(Resource.String.Lbl_Please_sign_in_view_comment);
                    DescriptionText.Text = " ";
                    EmptyStateButton.Text = Application.Context.GetText(Resource.String.Lbl_Login);
                }
                else if (type == Type.NoSessions)
                {
                    //FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, EmptyStateIcon, FontAwesomeIcon.Fingerprint);
                    EmptyStateIcon.SetImageResource(Resource.Drawable.ic_hand_usd_vector);

                    TitleText.Text = Application.Context.GetText(Resource.String.Lbl_Empty_Sessions);
                    DescriptionText.Text = "";
                    EmptyStateButton.Visibility = ViewStates.Gone;
                }
                else if (type == Type.NoActivities)
                {
                    //FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, EmptyStateIcon, IonIconsFonts.AndroidCamera);
                    EmptyStateIcon.SetImageResource(Resource.Drawable.icon_blog_vector);
                    TitleText.Text = Application.Context.GetText(Resource.String.Lbl_Empty_Activities);
                    DescriptionText.Text = "";
                    EmptyStateButton.Visibility = ViewStates.Gone;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}