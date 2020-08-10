using System;
using System.Collections.Generic;
using Android.Support.V4.App;
using Android.Views;
using SupportFragment = Android.Support.V4.App.Fragment;
using SupportFragmentManager = Android.Support.V4.App.FragmentManager;
using Java.Lang;
using Exception = Java.Lang.Exception;
using Object = Java.Lang.Object;

namespace PlayTube.Adapters
{
    public class MainTabAdapter : FragmentStatePagerAdapter
    {
        #region Variables

        public List<SupportFragment> Fragments { get; set; }
        public List<string> FragmentNames { get; set; }


        #endregion  

        public MainTabAdapter(SupportFragmentManager sfm) : base(sfm)
        {
            try
            {
                Fragments = new List<SupportFragment>();
                FragmentNames = new List<string>();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void AddFragment(SupportFragment fragment, string name)
        {
            try
            {
                Fragments.Add(fragment);
                FragmentNames.Add(name);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void ClaerFragment()
        {
            try
            {
                Fragments.Clear();
                FragmentNames.Clear();
                NotifyDataSetChanged();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void RemoveFragment(SupportFragment fragment, string name)
        {
            try
            {
                Fragments.Remove(fragment);
                FragmentNames.Remove(name);
                NotifyDataSetChanged();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void InsertFragment(int index ,SupportFragment fragment, string name)
        {
            try
            {
                Fragments.Insert(index, fragment);
                FragmentNames.Insert(index, name);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public override int Count
        {
            get { return Fragments.Count; }
        }

        public override SupportFragment GetItem(int position)
        {
            try
            {
                if (Fragments[position] != null)
                {
                    return Fragments[position];
                }
                else
                {
                    return Fragments[0];
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return null;
            }
        }

        public override ICharSequence GetPageTitleFormatted(int position)
        {
            return new Java.Lang.String(FragmentNames[position]);
        }

        public override Object InstantiateItem(ViewGroup container, int position)
        {
            try
            {
                return base.InstantiateItem(container, position);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return null;
            }
        }
    }
}