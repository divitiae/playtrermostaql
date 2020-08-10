using System;
using System.Collections.ObjectModel;
using Android.App;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using Bumptech.Glide;
using Java.Util;
using PlayTube.Helpers.Controller;
using PlayTube.Helpers.Fonts;
using PlayTube.Helpers.Models;
using IList = System.Collections.IList;

namespace PlayTube.Adapters
{
    public class CategoryIconAdapter : RecyclerView.Adapter, ListPreloader.IPreloadModelProvider
    {
        public event EventHandler<CategoryIconAdapterClickEventArgs> ItemClick;
        public event EventHandler<CategoryIconAdapterClickEventArgs> ItemLongClick;
        private int Position;
        private readonly Activity ActivityContext;
        public ObservableCollection<Classes.Category> CategoryList = new ObservableCollection<Classes.Category>();

        public CategoryIconAdapter(Activity context)
        {
            HasStableIds = true;
            ActivityContext = context;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Style_CategoriesIconView, parent, false); 
                var vh = new CategoryIconAdapterViewHolder(itemView, OnClick, OnLongClick);
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

                if (viewHolder is CategoryIconAdapterViewHolder holder)
                {
                    var item = CategoryList[Position];
                    if (item != null)
                    {
                        holder.TxtCategoryName.Text = item.Name;

                        FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeSolid, holder.CategoryIcon, CategoriesController.GetIconCategory(item.Name));
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

        void OnClick(CategoryIconAdapterClickEventArgs args) => ItemClick?.Invoke(ActivityContext, args);
        void OnLongClick(CategoryIconAdapterClickEventArgs args) => ItemLongClick?.Invoke(ActivityContext, args);

        public IList GetPreloadItems(int p0)
        {
            try
            {
                return Collections.SingletonList(p0);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Collections.SingletonList(p0);
            }
        }

        public RequestBuilder GetPreloadRequestBuilder(Java.Lang.Object p0)
        {
            return Glide.With(ActivityContext).Load(p0.ToString());
        }
    }

    public class CategoryIconAdapterViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get; private set; }
        public TextView CategoryIcon { get; private set; }
        public TextView TxtCategoryName { get; private set; }

        #endregion

        public CategoryIconAdapterViewHolder(View itemView, Action<CategoryIconAdapterClickEventArgs> clickListener, Action<CategoryIconAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                //Get values
                CategoryIcon = (TextView)MainView.FindViewById(Resource.Id.Icon);
                TxtCategoryName = MainView.FindViewById<TextView>(Resource.Id.name);

                //Create an Event
                itemView.Click += (sender, e) => clickListener(new CategoryIconAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
                itemView.LongClick += (sender, e) => longClickListener(new CategoryIconAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }

    public class CategoryIconAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}