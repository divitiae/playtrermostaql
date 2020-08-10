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
using PlayTube.Helpers.Utils;
using Java.Util;
using PlayTubeClient.Classes.Video;
using IList = System.Collections.IList;

namespace PlayTube.Adapters
{
    public class PopChannelRowAdapter : RecyclerView.Adapter, ListPreloader.IPreloadModelProvider
    {
        public event EventHandler<PopChannelRowAdapterClickEventArgs> ItemClick;
        public event EventHandler<PopChannelRowAdapterClickEventArgs> ItemLongClick;
        private readonly Activity ActivityContext;

        public ObservableCollection<GetPopularChannelsObject.Channel> ChannelList = new ObservableCollection<GetPopularChannelsObject.Channel>();

        public PopChannelRowAdapter(Activity context)
        {
            HasStableIds = true;
            ActivityContext = context;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> ChannelSubscribed_View
                View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Style_PopularChannels_view, parent, false); 
                var vh = new PopChannelRowAdapterViewHolder(itemView, OnClick, OnLongClick); 
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
                if (viewHolder is PopChannelRowAdapterViewHolder holder)
                {
                    var item = ChannelList[position];
                    if (item != null)
                    { 
                        GlideImageLoader.LoadImage(ActivityContext, item.UserData.Avatar, holder.ImgChannel, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);

                        holder.TxtNamechannal.Text = Methods.FunString.SubStringCutOf(AppTools.GetNameFinal(item.UserData), 16);
                         
                        holder.TxtViews.Text = item.Views + " " + ActivityContext.GetText(Resource.String.Lbl_Views);
                        holder.TxtSubscribers.Text = item.SubscribersCount + " " + ActivityContext.GetText(Resource.String.Lbl_Subscribers); 
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
         public override int ItemCount => ChannelList?.Count ?? 0;
 

        public GetPopularChannelsObject.Channel GetItem(int position)
        {
            return ChannelList[position];
        }

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
        void OnClick(PopChannelRowAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(PopChannelRowAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

        public IList GetPreloadItems(int p0)
        {
            try
            {
                var d = new List<string>();
                var item = ChannelList[p0];

                if (item == null)
                    return Collections.SingletonList(p0);

                if (item.UserData?.Avatar != "")
                {
                    d.Add(item.UserData?.Avatar);
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

    public class PopChannelRowAdapterViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get; private set; }

        public RelativeLayout RelativeLayoutMain { get; private set; }
        public ImageView ImgChannel { get; private set; }
        public TextView TxtNamechannal { get; private set; }
        public TextView TxtViews { get; private set; }
        public TextView TxtSubscribers { get; private set; }

        #endregion

        public PopChannelRowAdapterViewHolder(View itemView, Action<PopChannelRowAdapterClickEventArgs> clickListener, Action<PopChannelRowAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                RelativeLayoutMain = (RelativeLayout)MainView.FindViewById(Resource.Id.main);

                ImgChannel = MainView.FindViewById<ImageView>(Resource.Id.image);
                TxtNamechannal = MainView.FindViewById<TextView>(Resource.Id.name);
                TxtViews = MainView.FindViewById<TextView>(Resource.Id.views);
                TxtSubscribers = MainView.FindViewById<TextView>(Resource.Id.subscribers);

                //Event
                itemView.Click += (sender, e) => clickListener(new PopChannelRowAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
                itemView.LongClick += (sender, e) => longClickListener(new PopChannelRowAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }

    public class PopChannelRowAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}