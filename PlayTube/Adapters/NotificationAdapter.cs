using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Android.App;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using PlayTube.Helpers.CacheLoaders;
using PlayTube.Helpers.Fonts;
using PlayTube.Helpers.Utils;
using Java.Util;
using PlayTubeClient.Classes.Global;
using IList = System.Collections.IList;

namespace PlayTube.Adapters
{
    public class NotificationAdapter : RecyclerView.Adapter, ListPreloader.IPreloadModelProvider
    {
        public event EventHandler<NotificationAdapterClickEventArgs> ItemClick;
        public event EventHandler<NotificationAdapterClickEventArgs> ItemLongClick;
        private readonly Activity ActivityContext;
        public ObservableCollection<NotificationObject> NotificationList = new ObservableCollection<NotificationObject>();

        public NotificationAdapter(Activity context)
        {
            HasStableIds = true;
            ActivityContext = context;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Notifcation_view, parent, false);
                var vh = new NotificationAdapterViewHolder(itemView, OnClick, OnLongClick);
                return vh;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return null;
            }
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            try
            {
                if (viewHolder is NotificationAdapterViewHolder holder)
                {
                    var item = NotificationList[position];
                    if (item != null)
                    {
                        GlideImageLoader.LoadImage(ActivityContext, item.Userdata.Avatar, holder.NotificationImage, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);

                        if (item.Title.Contains("added"))
                        {
                            FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, holder.NotificationIcon, IonIconsFonts.Videocamera);
                            holder.NotifcationEvent.Text = item.Title == "added a new video" ? ActivityContext.GetText(Resource.String.Lbl_Notif_Added) : item.Title;
                        }
                        else if (item.Title.Contains("unsubscribed"))
                        {
                            FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, holder.NotificationIcon, IonIconsFonts.PersonAdd);
                            holder.NotifcationEvent.Text = item.Title == "unsubscribed from your channel" ? ActivityContext.GetText(Resource.String.Lbl_Notif_Unsubscribed) : item.Title;
                        }
                        else if (item.Title.Contains("subscribed"))
                        {
                            FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, holder.NotificationIcon, IonIconsFonts.PersonAdd);
                            holder.NotifcationEvent.Text = item.Title == "subscribed to your channel" ? ActivityContext.GetText(Resource.String.Lbl_Notif_Subscribed) : item.Title;
                        }
                        else if (item.Title.Contains("disliked"))
                        {
                            FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, holder.NotificationIcon, IonIconsFonts.Thumbsdown);
                            holder.NotifcationEvent.Text = item.Title == "disliked your video" ? ActivityContext.GetText(Resource.String.Lbl_Notif_Disliked) : item.Title;
                        }
                        else if (item.Title.Contains("liked"))
                        {
                            FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, holder.NotificationIcon, IonIconsFonts.Thumbsup);
                            holder.NotifcationEvent.Text = item.Title == "liked your video" ? ActivityContext.GetText(Resource.String.Lbl_Notif_Liked) : item.Title;
                        }
                        else if (item.Title.Contains("commented"))
                        {
                            FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, holder.NotificationIcon, IonIconsFonts.Chatboxes);
                            holder.NotifcationEvent.Text = item.Title == "commented on your video" ? ActivityContext.GetText(Resource.String.Lbl_Notif_Commented) : item.Title;
                        }
                        else
                        {
                            FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, holder.NotificationIcon, IonIconsFonts.AndroidNotifications);
                            holder.NotifcationEvent.Text = item.Title;
                        }
 
                        holder.Username.Text = AppTools.GetNameFinal(item.Userdata);
                        holder.NotificationTime.Text = item.Time;
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

         
        public NotificationObject GetItem(int position)
        {
            return NotificationList[position];
        }

        public override int ItemCount => NotificationList?.Count ?? 0;
 
        public override long GetItemId(int position)
        {
            try
            {
                return position;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return 0;
            }
        }

        public override int GetItemViewType(int position)
        {
            try
            {
                return position;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return 0;
            }
        }

        void OnClick(NotificationAdapterClickEventArgs args) => ItemClick?.Invoke(ActivityContext, args);
        void OnLongClick(NotificationAdapterClickEventArgs args) => ItemLongClick?.Invoke(ActivityContext, args);
        public IList GetPreloadItems(int p0)
        {
            try
            {
                var d = new List<string>();
                var item = NotificationList[p0];

                if (item == null)
                    return Collections.SingletonList(p0);

                if (item.Userdata?.Avatar != "")
                {
                    d.Add(item.Userdata?.Avatar);
                    return d;
                }

                return d;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Collections.SingletonList(p0);
            }
        }

        public RequestBuilder GetPreloadRequestBuilder(Java.Lang.Object p0)
        {
            return Glide.With(ActivityContext).Load(p0.ToString())
                .Apply(new RequestOptions().CenterCrop());
        }

    }

    public class NotificationAdapterViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get; set; }

        public ImageView NotificationImage { get; private set; }
        public TextView Username { get; private set; }
        public TextView NotifcationEvent { get; private set; }
        public TextView NotificationIcon { get; private set; }
        public TextView NotificationTime { get; private set; }

        #endregion


        public NotificationAdapterViewHolder(View itemView, Action<NotificationAdapterClickEventArgs> clickListener,Action<NotificationAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                NotificationImage = (ImageView)MainView.FindViewById(Resource.Id.UserImage);
                Username = MainView.FindViewById<TextView>(Resource.Id.NotificationUserName);
                NotifcationEvent = MainView.FindViewById<TextView>(Resource.Id.NotificationText);
                NotificationIcon = MainView.FindViewById<TextView>(Resource.Id.NotificationIcon);
                NotificationTime = MainView.FindViewById<TextView>(Resource.Id.NotificationTime);

                //Create an Event
                itemView.Click += (sender, e) => clickListener(new NotificationAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
                itemView.LongClick += (sender, e) => longClickListener(new NotificationAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }

    public class NotificationAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}