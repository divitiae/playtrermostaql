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
using PlayTubeClient.Classes.Global;
using Java.Util;
using IList = System.Collections.IList;


namespace PlayTube.Adapters
{
    public class ChannelAdapter : RecyclerView.Adapter, ListPreloader.IPreloadModelProvider
    {
        public event EventHandler<ChannelAdapterClickEventArgs> ItemClick;
        public event EventHandler<ChannelAdapterClickEventArgs> ItemLongClick;
        private readonly Activity ActivityContext;

        public ObservableCollection<UserDataObject> ChannelList = new ObservableCollection<UserDataObject>();

        public ChannelAdapter(Activity context)
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
                View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ChannelSubscribed_View, parent, false);

                var vh = new ChannelAdapterViewHolder(itemView, OnClick, OnLongClick);

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
                if (viewHolder is ChannelAdapterViewHolder holder)
                {
                    var item = ChannelList[position];
                    if (item != null)
                    {
                        if (AppSettings.FlowDirectionRightToLeft)
                        {
                            holder.RelativeLayoutMain.LayoutDirection = LayoutDirection.Rtl;
                            holder.TxtNamechannal.TextDirection = TextDirection.Rtl;
                        }

                        GlideImageLoader.LoadImage(ActivityContext, item.Avatar, holder.ImgChannel,ImageStyle.CircleCrop,ImagePlaceholders.Drawable);

                        holder.TxtNamechannal.Text = Methods.FunString.SubStringCutOf(AppTools.GetNameFinal(item), 16);

                        //Verified 
                        if (item.Verified == "1")
                        {
                            holder.IconVerified.Visibility = ViewStates.Visible;
                            FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, holder.IconVerified, IonIconsFonts.CheckmarkCircled);
                        }
                        else
                        {
                            holder.IconVerified.Visibility = ViewStates.Gone;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

       
        public override int ItemCount => ChannelList?.Count ?? 0;
 
        public UserDataObject GetItem(int position)
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
        void OnClick(ChannelAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(ChannelAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

        public IList GetPreloadItems(int p0)
        {
            try
            {
                var d = new List<string>();
                var item = ChannelList[p0];

                if (item == null)
                    return Collections.SingletonList(p0);

                if (item.Avatar != "")
                {
                    d.Add(item.Avatar);
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

    public class ChannelAdapterViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get; private set; }

        public RelativeLayout RelativeLayoutMain { get; private set; }
        public ImageView ImgChannel { get; private set; }
        public TextView TxtNamechannal { get; private set; }
        public TextView IconVerified { get; private set; }

        #endregion
        
        public ChannelAdapterViewHolder(View itemView, Action<ChannelAdapterClickEventArgs> clickListener,Action<ChannelAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                RelativeLayoutMain = (RelativeLayout)MainView.FindViewById(Resource.Id.main);

                ImgChannel = MainView.FindViewById<ImageView>(Resource.Id.Channel_Image);
                TxtNamechannal = MainView.FindViewById<TextView>(Resource.Id.ChannelName);
                IconVerified = MainView.FindViewById<TextView>(Resource.Id.IconVerified);

                //Event
                itemView.Click += (sender, e) => clickListener(new ChannelAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
                itemView.LongClick += (sender, e) => longClickListener(new ChannelAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }

    public class ChannelAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}