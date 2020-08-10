using System;
using System.Collections.ObjectModel;
using Android.App;
using Android.Views;
using Android.Support.V7.Widget;
using Android.Widget;
using PlayTube.Helpers.CacheLoaders;
using PlayTube.Helpers.Controller;
using PlayTube.Helpers.Models;
 
namespace PlayTube.Adapters
{
    public class CategoryAdapter : RecyclerView.Adapter
    {
        public event EventHandler<CategoryAdapterClickEventArgs> ItemClick;
        public event EventHandler<CategoryAdapterClickEventArgs> ItemLongClick;
        private int Position;
        private readonly Activity ActivityContext;
        public ObservableCollection<Classes.Category> CategoryList = new ObservableCollection<Classes.Category>();

        public CategoryAdapter(Activity context)
        {
            HasStableIds = true;
            ActivityContext = context;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                View itemView;

                if (AppSettings.CategoriesVideoStyleText)
                {
                    //Setup your layout here >> CategoryViewStyle2
                    itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.CategoryViewStyle2, parent, false);
                }
                else 
                {
                    //Setup your layout here >> CategoryViewStyle1
                    itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.CategoryViewStyle1, parent, false);
                }

                var vh = new CategoryAdapterViewHolder(itemView, OnClick, OnLongClick);
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
                Position = position;

                if (viewHolder is CategoryAdapterViewHolder holder)
                {
                    var item = CategoryList[Position];
                    if (item != null)
                    {
                        if (AppSettings.CategoriesVideoStyleText)
                        {
                            holder.TxtCategoryName.Text = item.Name;
                        }
                        else
                        {  
                            GlideImageLoader.LoadImage(ActivityContext, item.Image, holder.CategoryImage,ImageStyle.CenterCrop,ImagePlaceholders.Drawable);
                            holder.TxtCategoryName.Text = item.Name;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

       public override int ItemCount => CategoryList?.Count ?? 0;
        public Classes.Category GetItem(int position)
        {
            return CategoryList[position];
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
        
        void OnClick(CategoryAdapterClickEventArgs args) => ItemClick?.Invoke(ActivityContext, args);
        void OnLongClick(CategoryAdapterClickEventArgs args) => ItemLongClick?.Invoke(ActivityContext, args);

    }

    public class CategoryAdapterViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get; private set; }
        public ImageView CategoryImage { get; private set; }
        public TextView TxtCategoryName { get; private set; }

        #endregion
        
        public CategoryAdapterViewHolder(View itemView, Action<CategoryAdapterClickEventArgs> clickListener,Action<CategoryAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                //Get values
                if (AppSettings.CategoriesVideoStyleText)
                {
                    TxtCategoryName = MainView.FindViewById<TextView>(Resource.Id.Category_name);
                }
                else
                {
                    CategoryImage = (ImageView)MainView.FindViewById(Resource.Id.Imagevideo);
                    TxtCategoryName = MainView.FindViewById<TextView>(Resource.Id.Category_name);
                }

                //Create an Event
                itemView.Click += (sender, e) => clickListener(new CategoryAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
                itemView.LongClick += (sender, e) => longClickListener(new CategoryAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }

    public class CategoryAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}