using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Android.App;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using Java.Util;
using PlayTube.Activities.Article;
using PlayTube.Helpers.CacheLoaders;
using PlayTube.Helpers.Controller;
using PlayTube.Helpers.Utils;
using PlayTubeClient.Classes.Global;
using IList = System.Collections.IList;

namespace PlayTube.Adapters
{
    public class ArticlesAdapter : RecyclerView.Adapter, ListPreloader.IPreloadModelProvider
    {
        private readonly Activity ActivityContext;
        public ArticlesFragment AFragment;
        public Dictionary<int, string> CategoryColor = new Dictionary<int, string>();
        public ObservableCollection<ArticleObject> ArticlesList = new ObservableCollection<ArticleObject>();

        public ArticlesAdapter(Activity context, ArticlesFragment fragment)
        {
            try
            {
                HasStableIds = true;
                ActivityContext = context;
                AFragment = fragment;
            }
            catch (Exception e)
            {
                Console.WriteLine(e); 
            }
        }

       public override int ItemCount => ArticlesList?.Count ?? 0;

        public event EventHandler<ArticlesAdapterClickEventArgs> ItemClick;
        public event EventHandler<ArticlesAdapterClickEventArgs> ItemLongClick;

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_Article_View
                var itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Style_Article_View, parent, false);
                var vh = new ArticlesAdapterViewHolder(itemView, OnClick, OnLongClick);
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

                if (viewHolder is ArticlesAdapterViewHolder holder)
                {
                    var item = ArticlesList[position];
                    if (item != null) Initialize(holder, item);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void Initialize(ArticlesAdapterViewHolder holder, ArticleObject item)
        {
            try
            {
                GlideImageLoader.LoadImage(ActivityContext, !string.IsNullOrEmpty(item.Image) ? item.Image : "blackdefault", holder.Image, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);

                GlideImageLoader.LoadImage(ActivityContext, !string.IsNullOrEmpty(item.UserData?.Avatar) ? item.UserData.Avatar : "no_profile_image_circle", holder.UserImageProfile, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);

                var color = Methods.FunString.RandomColor();
                holder.Category.SetBackgroundColor(Color.ParseColor(color));
                CategoryColor.Add(item.Id, color);
                 
                string id = item.Url.Split('/').Last();

                string name = Methods.FunString.DecodeString(CategoriesController.ListCategories?.FirstOrDefault(a => a.Id == (item.Category))?.Name);
                if (string.IsNullOrEmpty(name))
                    name = ActivityContext.GetString(Resource.String.Lbl_Unknown);

                holder.Category.Text = name;  

                holder.Description.Text = Methods.FunString.DecodeString(item.Description);
                holder.Title.Text = Methods.FunString.DecodeString(item.Title);
                holder.Username.Text = Methods.FunString.DecodeString(item.UserData?.Name);
                holder.ViewMore.Text = ActivityContext.GetText(Resource.String.Lbl_ReadMore) + " >"; //READ MORE &gt; 
                holder.Time.Text = Methods.FunString.DecodeString(item.TextTime);

                if (!holder.UserItem.HasOnClickListeners)
                    holder.UserItem.Click += (sender, args) =>
                    {
                        try
                        {
                            AFragment.OpenChannel(item);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

 
        public ArticleObject GetItem(int position)
        {
            return ArticlesList[position];
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

        private void OnClick(ArticlesAdapterClickEventArgs args)
        {
            ItemClick?.Invoke(this, args);
        }

        private void OnLongClick(ArticlesAdapterClickEventArgs args)
        {
            ItemLongClick?.Invoke(this, args);
        }
        public IList GetPreloadItems(int p0)
        {
            try
            {
                var d = new List<string>();
                var item = ArticlesList[p0];

                if (item == null)
                    return Collections.SingletonList(p0);

                var image = !string.IsNullOrEmpty(item.Image) ? item.Image : "blackdefault";
                var imageAvatar = !string.IsNullOrEmpty(item.UserData?.Avatar) ? item.UserData.Avatar : "no_profile_image_circle";
                
                if (!string.IsNullOrEmpty(image))
                {
                    d.Add(image); 
                    d.Add(imageAvatar); 

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
            return Glide.With(ActivityContext).Load(p0.ToString()).Apply(new RequestOptions().CenterCrop());
        }
    }

    public class ArticlesAdapterViewHolder : RecyclerView.ViewHolder
    {
        public ArticlesAdapterViewHolder(View itemView, Action<ArticlesAdapterClickEventArgs> clickListener,Action<ArticlesAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                UserItem = MainView.FindViewById<RelativeLayout>(Resource.Id.UserItem_Layout);
                Image = MainView.FindViewById<ImageView>(Resource.Id.Image);
                Category = MainView.FindViewById<TextView>(Resource.Id.Category);
                Title = MainView.FindViewById<TextView>(Resource.Id.Title);
                Description = MainView.FindViewById<TextView>(Resource.Id.description);
                UserImageProfile = MainView.FindViewById<ImageView>(Resource.Id.UserImageProfile);
                Username = MainView.FindViewById<TextView>(Resource.Id.Username);
                Time = MainView.FindViewById<TextView>(Resource.Id.card_dist);
                ViewMore = MainView.FindViewById<TextView>(Resource.Id.View_more);

                //Event
                itemView.Click += (sender, e) => clickListener(new ArticlesAdapterClickEventArgs{ View = itemView, Position = AdapterPosition });
                itemView.LongClick += (sender, e) => longClickListener(new ArticlesAdapterClickEventArgs{ View = itemView, Position = AdapterPosition });
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #region Variables Basic

        public View MainView { get; }

        public ImageView Image { get; private set; }
        public TextView Title { get; private set; }
        public TextView Description { get; private set; }
        public ImageView UserImageProfile { get; private set; }
        public TextView Category { get; private set; }
        public TextView Username { get; private set; }
        public TextView Time { get; private set; }
        public TextView ViewMore { get; private set; }
        public RelativeLayout UserItem { get; private set; }

        #endregion
    }

    public class ArticlesAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
} 