using System;
using System.Collections.Generic;
using AFollestad.MaterialDialogs;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using Java.Lang;
using PlayTube.Activities.Tabbes;
using PlayTube.Helpers.Fonts;
using PlayTube.Helpers.Models;
using Exception = System.Exception;

namespace PlayTube.Activities.Channel.ChannelPopular
{
    public class ChannelPopularFilterBottomDialogFragment : BottomSheetDialogFragment, MaterialDialog.IListCallback, MaterialDialog.ISingleButtonCallback
    {
        #region Variables Basic

        private TextView IconBack, IconSortBy, IconTime, BtnReset;
        private TextView TxtTitlePage, TxtSortBy, TxtTime;
        private RelativeLayout SortByLayout, TimeLayout;
        private Button ButtonApply;
        private string TypeDialog , TimeChecked = "", SortByChecked = "";

        #endregion

        #region General
         
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                // create ContextThemeWrapper from the original Activity Context with the custom theme
                Context contextThemeWrapper = AppSettings.SetTabDarkTheme ? new ContextThemeWrapper(Activity, Resource.Style.MyTheme_Dark_Base) : new ContextThemeWrapper(Activity, Resource.Style.MyTheme_Base);

                // clone the inflater using the ContextThemeWrapper
                LayoutInflater localInflater = inflater.CloneInContext(contextThemeWrapper);

                View view = localInflater.Inflate(Resource.Layout.ButtomSheetChannelPopularFilter, container, false);

                //Get Value And Set Toolbar
                InitComponent(view);
                 
                return view;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
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

        #endregion

        #region Functions

        private void InitComponent(View view)
        {
            try
            {
                IconBack = view.FindViewById<TextView>(Resource.Id.IconBack);
                IconSortBy = view.FindViewById<TextView>(Resource.Id.IconSortBy);
                IconTime = view.FindViewById<TextView>(Resource.Id.IconTime);

                TxtTitlePage = view.FindViewById<TextView>(Resource.Id.titlepage);
                TxtSortBy = view.FindViewById<TextView>(Resource.Id.SortByPlace);
                TxtTime = view.FindViewById<TextView>(Resource.Id.TimePlace);

                SortByLayout = view.FindViewById<RelativeLayout>(Resource.Id.LayoutSortBy);
                TimeLayout = view.FindViewById<RelativeLayout>(Resource.Id.LayoutTime);

                BtnReset = view.FindViewById<TextView>(Resource.Id.Resetbutton);
                ButtonApply = view.FindViewById<Button>(Resource.Id.ApplyButton);

                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, IconBack, IonIconsFonts.ChevronLeft);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, IconSortBy, IonIconsFonts.Cash);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, IconTime, IonIconsFonts.SocialBuffer);

                FontUtils.SetFont(TxtTitlePage, Fonts.SfMedium);
                FontUtils.SetFont(TxtTime, Fonts.SfRegular);
                FontUtils.SetFont(TxtSortBy, Fonts.SfRegular);

                TxtSortBy.Text = "";
                TxtTime.Text = "";

                IconBack.Click += IconBackOnClick;
                SortByLayout.Click += SortByLayoutOnClick;
                TimeLayout.Click += TimeLayoutOnClick;
                ButtonApply.Click += ButtonApplyOnClick;
                BtnReset.Click += BtnResetOnClick; 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Events

        //Choose Time
        private void TimeLayoutOnClick(object sender, EventArgs e)
        {
            try
            {
                TypeDialog = "Time";
                 
                List<string> arrayAdapter = new List<string>();
                MaterialDialog.Builder dialogList = new MaterialDialog.Builder(Context).Theme(AppSettings.SetTabDarkTheme ? AFollestad.MaterialDialogs.Theme.Dark : AFollestad.MaterialDialogs.Theme.Light);

                arrayAdapter.Add(GetText(Resource.String.Lbl_Today));
                arrayAdapter.Add(GetText(Resource.String.Lbl_ThisWeek));
                arrayAdapter.Add(GetText(Resource.String.Lbl_ThisMonth));
                arrayAdapter.Add(GetText(Resource.String.Lbl_ThisYear));
                arrayAdapter.Add(GetText(Resource.String.Lbl_AllTime));

                dialogList.Title(GetText(Resource.String.Lbl_TimeBy));
                dialogList.Items(arrayAdapter);
                dialogList.PositiveText(GetText(Resource.String.Lbl_Cancel)).OnPositive(this);
                dialogList.AlwaysCallSingleChoiceCallback();
                dialogList.ItemsCallback(this).Build().Show(); 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Choose SortBy
        private void SortByLayoutOnClick(object sender, EventArgs e)
        {
            try
            {
                TypeDialog = "SortBy";

                List<string> arrayAdapter = new List<string>();
                MaterialDialog.Builder dialogList = new MaterialDialog.Builder(Context).Theme(AppSettings.SetTabDarkTheme ? AFollestad.MaterialDialogs.Theme.Dark : AFollestad.MaterialDialogs.Theme.Light);

                arrayAdapter.Add(Context.GetText(Resource.String.Lbl_Views));
                arrayAdapter.Add(Context.GetText(Resource.String.Lbl_Subscribers));
                arrayAdapter.Add(Context.GetText(Resource.String.Lbl_MostActive));

                dialogList.Title(GetText(Resource.String.Lbl_SortBy));
                dialogList.Items(arrayAdapter);
                dialogList.PositiveText(GetText(Resource.String.Lbl_Cancel)).OnPositive(this);
                dialogList.AlwaysCallSingleChoiceCallback();
                dialogList.ItemsCallback(this).Build().Show(); 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Save and sent data and set new search 
        private void ButtonApplyOnClick(object sender, EventArgs e)
        {
            try
            {
                if (SortByChecked == Context.GetText(Resource.String.Lbl_Views))
                {
                    UserDetails.FilterTypeSortBy = "views";
                }
                else if (SortByChecked == Context.GetText(Resource.String.Lbl_Subscribers))
                {
                    UserDetails.FilterTypeSortBy = "subscribers";
                }
                else if (SortByChecked == Context.GetText(Resource.String.Lbl_MostActive))
                {
                    UserDetails.FilterTypeSortBy = "most_active";
                }
                else
                {
                    UserDetails.FilterTypeSortBy = "most_active";
                }

                if (TimeChecked == Context.GetText(Resource.String.Lbl_Today))
                {
                    UserDetails.FilterTime = "today";
                }
                else if (TimeChecked == Context.GetText(Resource.String.Lbl_ThisWeek))
                {
                    UserDetails.FilterTime = "this_week";
                }
                else if (TimeChecked == Context.GetText(Resource.String.Lbl_ThisMonth))
                {
                    UserDetails.FilterTime = "this_month";
                }
                else if (TimeChecked == Context.GetText(Resource.String.Lbl_ThisYear))
                {
                    UserDetails.FilterTime = "this_year";
                }
                else if (TimeChecked == Context.GetText(Resource.String.Lbl_AllTime))
                {
                    UserDetails.FilterTime = "all_time";
                }
                else
                {
                    UserDetails.FilterTime = "this_month";
                }

                var channelPopularFragment = TabbedMainActivity.GetInstance().TrendingFragment.AllChannelPopularFragment;
                if (channelPopularFragment != null)
                {
                    if (channelPopularFragment.MAdapter != null)
                    {
                        channelPopularFragment.SwipeRefreshLayout.Refreshing = true;
                        channelPopularFragment.SwipeRefreshLayout.Enabled = true;
            

                        channelPopularFragment.EmptyStateLayout.Visibility = ViewStates.Gone;
                        channelPopularFragment.MRecycler.Visibility = ViewStates.Visible;

                        channelPopularFragment.MAdapter.ChannelList.Clear();
                        channelPopularFragment.MAdapter.NotifyDataSetChanged();
                    }

                    channelPopularFragment.StartApiService();
                }
                 
                Dismiss();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Back
        private void IconBackOnClick(object sender, EventArgs e)
        {
            try
            {
                Dismiss();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Reset data 
        private void BtnResetOnClick(object sender, EventArgs e)
        {
            try
            {
                TimeChecked = "this_month";
                SortByChecked = "most_active";

                UserDetails.FilterTime = "this_month";
                UserDetails.FilterTypeSortBy = "most_active";

                TxtSortBy.Text = Context.GetText(Resource.String.Lbl_MostActive);
                TxtTime.Text = Context.GetText(Resource.String.Lbl_ThisMonth); 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        #region MaterialDialog

        public void OnSelection(MaterialDialog p0, View p1, int itemId, ICharSequence itemstring)
        {
            try
            {
                if (TypeDialog == "SortBy")
                {
                    TxtSortBy.Text = itemstring.ToString(); 
                    SortByChecked = itemstring.ToString();
                }
                else
                { 
                    TxtTime.Text = itemstring.ToString();
                    TimeChecked = itemstring.ToString();
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
                if (p1 == DialogAction.Positive)
                {
                }
                else if (p1 == DialogAction.Negative)
                {
                    p0.Dismiss();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion
         
    }
}