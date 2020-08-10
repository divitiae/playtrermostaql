//###############################################################
// Author >> Elin Doughouz 
// Copyright (c) PlayTube 12/07/2018 All Right Reserved
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// Follow me on facebook >> https://www.facebook.com/Elindoughous
//=========================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AutoMapper;
using Newtonsoft.Json;
using PlayTube.Activities.Chat;
using PlayTube.Helpers.Controller;
using PlayTube.Helpers.Models;
using PlayTube.Helpers.Utils;
using PlayTube.OneSignal;
using PlayTubeClient;
using PlayTubeClient.Classes.Global;
using PlayTubeClient.Classes.Messages;
using SQLite;

namespace PlayTube.SQLite
{
    public class SqLiteDatabase : IDisposable
    {
        //############# DON'T MODIFY HERE #############

        private static readonly string Folder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        public static readonly string PathCombine = System.IO.Path.Combine(Folder, "PlayTube.db");

        private static SQLiteConnection Connection;

        //############# CONNECTION #############

        #region DataBaseFunctions

        private SQLiteConnection OpenConnection()
        {
            try
            {
                Connection = new SQLiteConnection(PathCombine);
                return Connection;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public void Connect()
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    Connection?.CreateTable<DataTables.MySettingsTb>();
                    Connection?.CreateTable<DataTables.LoginTb>();
                    Connection?.CreateTable<DataTables.ChannelTb>();
                    Connection?.CreateTable<DataTables.WatchOfflineVideosTb>();
                    Connection?.CreateTable<DataTables.SubscriptionsChannelTb>();
                    Connection?.CreateTable<DataTables.LibraryItemTb>();
                    Connection?.CreateTable<DataTables.SharedVideosTb>();
                    Connection?.CreateTable<DataTables.NotInterestedTb>();
                    Connection?.CreateTable<DataTables.LastChatTb>();
                    Connection?.CreateTable<DataTables.MessageTb>();

                    Connection?.Dispose();
                    Connection?.Close();
                }
            }
            catch (SQLiteException exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Close Connection in Database
        public void Close()
        {
            try
            {
                Connection.Close();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void ClearAll()
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    Connection.DeleteAll<DataTables.MySettingsTb>();
                    Connection.DeleteAll<DataTables.LoginTb>();
                    Connection.DeleteAll<DataTables.ChannelTb>();
                    Connection.DeleteAll<DataTables.WatchOfflineVideosTb>();
                    Connection.DeleteAll<DataTables.SubscriptionsChannelTb>();
                    Connection.DeleteAll<DataTables.LibraryItemTb>();
                    Connection.DeleteAll<DataTables.SharedVideosTb>();
                    Connection.DeleteAll<DataTables.NotInterestedTb>();
                    Connection.DeleteAll<DataTables.LastChatTb>();
                    Connection.DeleteAll<DataTables.MessageTb>();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Delete table 
        public void DropAll()
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    Connection.DropTable<DataTables.MySettingsTb>();
                    Connection.DropTable<DataTables.LoginTb>();
                    Connection.DropTable<DataTables.ChannelTb>();
                    Connection.DropTable<DataTables.WatchOfflineVideosTb>();
                    Connection.DropTable<DataTables.SubscriptionsChannelTb>();
                    Connection.DropTable<DataTables.LibraryItemTb>();
                    Connection.DropTable<DataTables.SharedVideosTb>();
                    Connection.DropTable<DataTables.NotInterestedTb>();
                    Connection.DropTable<DataTables.LastChatTb>();
                    Connection.DropTable<DataTables.MessageTb>();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }


        public void Dispose()
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    Connection?.Dispose();
                    Connection?.Close();
                    GC.SuppressFinalize(this);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        //############# CONNECTION #############

        #region Settings

        //Insert data Settings
        public void InsertOrUpdate_Settings(GetSettingsObject.SiteSettings data)
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    var resultChannelTb = Connection.Table<DataTables.MySettingsTb>().FirstOrDefault();
                    if (resultChannelTb == null)
                    {
                        resultChannelTb = Mapper.Map<DataTables.MySettingsTb>(data);
                        if (resultChannelTb != null)
                        {
                            resultChannelTb.CurrencyArray = JsonConvert.SerializeObject(data.CurrencyArray);
                            resultChannelTb.CurrencySymbolArray = JsonConvert.SerializeObject(data.CurrencySymbolArray);
                            resultChannelTb.Continents = JsonConvert.SerializeObject(data.Continents);
                            resultChannelTb.Categories = JsonConvert.SerializeObject(data.Categories);
                            if (data.SubCategories != null)
                                resultChannelTb.SubCategories = JsonConvert.SerializeObject(data.SubCategories?.SubCategoriessList);
                            resultChannelTb.MoviesCategories = JsonConvert.SerializeObject(data.MoviesCategories);
                            resultChannelTb.ImportSystem = data.ImportSystem;
                            resultChannelTb.UploadSystem = data.UploadSystem;

                            Connection.Insert(resultChannelTb);
                        }
                    }
                    else
                    {
                        var db = Mapper.Map<DataTables.MySettingsTb>(data);
                        if (db != null)
                        {
                            db.CurrencyArray = JsonConvert.SerializeObject(data.CurrencyArray);
                            db.CurrencySymbolArray = JsonConvert.SerializeObject(data.CurrencySymbolArray);
                            db.Continents = JsonConvert.SerializeObject(data.Continents);
                            db.Categories = JsonConvert.SerializeObject(data.Categories);
                            if (data.SubCategories != null)
                                db.SubCategories = JsonConvert.SerializeObject(data.SubCategories?.SubCategoriessList);
                            db.MoviesCategories = JsonConvert.SerializeObject(data.MoviesCategories);
                            db.ImportSystem = data.ImportSystem;
                            db.UploadSystem = data.UploadSystem;

                            Connection.Update(db);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Get data setting 
        public GetSettingsObject.SiteSettings Get_Settings()
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    var data = Connection.Table<DataTables.MySettingsTb>().FirstOrDefault();
                    if (data != null)
                    {
                        var db = Mapper.Map<GetSettingsObject.SiteSettings>(data);
                        if (db != null)
                        {
                            var asd = db;
                            asd.CurrencyArray = new List<string>();
                            asd.CurrencySymbolArray = new GetSettingsObject.CurrencySymbolArray();
                            asd.Continents = new List<string>();
                            asd.Categories = new Dictionary<string, string>();
                            asd.SubCategories = new GetSettingsObject.SubCategoriesUnion();
                            asd.MoviesCategories = new Dictionary<string, string>();

                            if (!string.IsNullOrEmpty(data.CurrencyArray))
                                asd.CurrencyArray = JsonConvert.DeserializeObject<List<string>>(data.CurrencyArray);

                            if (!string.IsNullOrEmpty(data.CurrencySymbolArray))
                                asd.CurrencySymbolArray = JsonConvert.DeserializeObject<GetSettingsObject.CurrencySymbolArray>(data.CurrencySymbolArray);

                            if (!string.IsNullOrEmpty(data.Continents))
                                asd.Continents = JsonConvert.DeserializeObject<List<string>>(data.Continents);

                            if (!string.IsNullOrEmpty(data.Categories))
                                asd.Categories = JsonConvert.DeserializeObject<Dictionary<string, string>>(data.Categories);

                            if (!string.IsNullOrEmpty(data.SubCategories))
                                asd.SubCategories = new GetSettingsObject.SubCategoriesUnion()
                                {
                                    SubCategoriessList = JsonConvert.DeserializeObject<Dictionary<string, List<Dictionary<string, string>>>>(data.SubCategories),
                                };

                            if (!string.IsNullOrEmpty(data.MoviesCategories))
                                asd.MoviesCategories = JsonConvert.DeserializeObject<Dictionary<string, string>>(data.MoviesCategories);

                            //Categories
                            var listCategories = db.Categories.Select(cat => new Classes.Category
                            {
                                Id = cat.Key,
                                Name = Methods.FunString.DecodeString(cat.Value),
                                Color = "#212121",
                                Image = CategoriesController.GetImageCategory(cat.Value),
                                SubList = new List<Classes.Category>()
                            }).ToList();

                            CategoriesController.ListCategories.Clear();
                            CategoriesController.ListCategories = new ObservableCollection<Classes.Category>(listCategories);

                            if (db.SubCategories?.SubCategoriessList?.Count > 0)
                            {
                                //Sub Categories
                                foreach (var sub in db.SubCategories?.SubCategoriessList)
                                {
                                    var subCategories = ListUtils.MySettingsList?.SubCategories?.SubCategoriessList?.FirstOrDefault(a => a.Key == sub.Key).Value;
                                    if (subCategories?.Count > 0)
                                    {
                                        var cat = CategoriesController.ListCategories.FirstOrDefault(a => a.Id == sub.Key);
                                        if (cat != null)
                                        {
                                            foreach (var pairs in subCategories.SelectMany(pairs => pairs))
                                            {
                                                cat.SubList.Add(new Classes.Category()
                                                {
                                                    Id = pairs.Key,
                                                    Name = Methods.FunString.DecodeString(pairs.Value),
                                                    Color = "#212121",
                                                });
                                            }
                                        }
                                    }
                                }
                            }

                            //Movies Categories
                            var listMovies = db.MoviesCategories.Select(cat => new Classes.Category
                            {
                                Id = cat.Key,
                                Name = Methods.FunString.DecodeString(cat.Value),
                                Color = "#212121",
                                SubList = new List<Classes.Category>()
                            }).ToList();

                            CategoriesController.ListCategoriesMovies.Clear();
                            CategoriesController.ListCategoriesMovies = new ObservableCollection<Classes.Category>(listMovies);

                            AppSettings.Lang = data.Language;

                            AppSettings.OneSignalAppId = data.PushId;
                            AppSettings.ShowButtonImport = string.IsNullOrWhiteSpace(data.ImportSystem) ? AppSettings.ShowButtonImport : data.ImportSystem == "on";
                            AppSettings.ShowButtonUpload = string.IsNullOrWhiteSpace(data.UploadSystem) ? AppSettings.ShowButtonUpload : data.UploadSystem == "on";
                            OneSignalNotification.RegisterNotificationDevice();

                            return db;
                        }

                        return null;
                    }

                    return null;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return null;
            }
        }

        #endregion

        #region Login

        //Get data Login
        public DataTables.LoginTb Get_data_Login()
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    var dataUser = Connection.Table<DataTables.LoginTb>().FirstOrDefault();
                    if (dataUser != null)
                    {
                        UserDetails.Username = dataUser.Username;
                        UserDetails.FullName = dataUser.Username;
                        UserDetails.Password = dataUser.Password;
                        UserDetails.UserId = Client.UserId = dataUser.UserId;
                        UserDetails.AccessToken = Current.AccessToken = dataUser.AccessToken;
                        UserDetails.Status = dataUser.Status;
                        UserDetails.Cookie = dataUser.Cookie;
                        UserDetails.Email = dataUser.Email;
                        AppSettings.Lang = dataUser.Lang;

                        ListUtils.DataUserLoginList.Add(dataUser);

                        return dataUser;
                    }

                    return null;
                }

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return null;
            }
        }

        public void InsertOrUpdateLogin_Credentials(DataTables.LoginTb db)
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    var dataUser = Connection.Table<DataTables.LoginTb>().FirstOrDefault();
                    if (dataUser != null)
                    {
                        dataUser.UserId = UserDetails.UserId;
                        dataUser.AccessToken = UserDetails.AccessToken;
                        dataUser.Cookie = UserDetails.Cookie;
                        dataUser.Username = UserDetails.Username;
                        dataUser.Password = UserDetails.Password;
                        dataUser.Status = UserDetails.Status;
                        dataUser.Lang = AppSettings.Lang;
                        dataUser.DeviceId = UserDetails.DeviceId;
                        dataUser.Email = UserDetails.Email;

                        Connection.Update(dataUser);
                    }
                    else
                    {
                        Connection.Insert(db);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region MyChannel

        //Insert Or Update data MyChannel
        public void InsertOrUpdate_DataMyChannel(UserDataObject channel)
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    var resultChannelTb = Connection.Table<DataTables.ChannelTb>().FirstOrDefault();
                    if (resultChannelTb != null)
                    {
                        resultChannelTb = Mapper.Map<DataTables.ChannelTb>(resultChannelTb);
                        if (resultChannelTb != null)
                        {
                            UserDetails.Avatar = resultChannelTb.Avatar;
                            UserDetails.Cover = resultChannelTb.Cover;
                            UserDetails.Username = resultChannelTb.Username;
                            UserDetails.FullName = AppTools.GetNameFinal(resultChannelTb);

                            ListUtils.MyChannelList.Clear();
                            ListUtils.MyChannelList.Add(resultChannelTb);

                            Connection.Update(resultChannelTb);
                        }
                    }
                    else
                    {
                        var db = Mapper.Map<DataTables.ChannelTb>(channel);
                        if (db != null)
                        {
                            Connection.Insert(db);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Get Data My Channel
        public UserDataObject GetDataMyChannel()
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    var channel = Connection.Table<DataTables.ChannelTb>().FirstOrDefault();
                    if (channel != null)
                    {
                        var db = Mapper.Map<UserDataObject>(channel);
                        if (db != null)
                        {
                            UserDetails.Avatar = db.Avatar;
                            UserDetails.Cover = db.Cover;
                            UserDetails.Username = db.Username;
                            UserDetails.FullName = AppTools.GetNameFinal(db);

                            ListUtils.MyChannelList.Clear();
                            ListUtils.MyChannelList.Add(db);

                            return channel;
                        }

                        return null;
                    }

                    return null;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return null;
            }
        }

        #endregion

        #region SubscriptionsChannel Videos

        //Insert SubscriptionsChannel Videos
        public void Insert_One_SubscriptionChannel(UserDataObject channel)
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    if (channel != null)
                    {
                        var select = Connection.Table<DataTables.SubscriptionsChannelTb>().FirstOrDefault(a => a.Id == channel.Id);
                        if (select == null)
                        {
                            var db = Mapper.Map<DataTables.SubscriptionsChannelTb>(channel);
                            if (db != null) Connection.Insert(db);
                        }
                        else
                        {
                            select = Mapper.Map<DataTables.SubscriptionsChannelTb>(channel);
                            if (select != null) Connection.Update(select);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Insert SubscriptionsChannel Videos
        public void InsertAllSubscriptionsChannel(ObservableCollection<UserDataObject> channelsList)
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    var result = Connection.Table<DataTables.SubscriptionsChannelTb>().ToList();

                    var list = new List<DataTables.SubscriptionsChannelTb>();
                    foreach (var info in channelsList)
                    {
                        var db = Mapper.Map<DataTables.SubscriptionsChannelTb>(info);
                        if (db != null) list.Add(db);

                        var update = result.FirstOrDefault(a => a.Id == info.Id);
                        if (update != null)
                        {
                            update = db;
                            Connection.Update(update);
                        }
                    }

                    if (list.Count <= 0) return;

                    Connection.BeginTransaction();
                    //Bring new  
                    var newItemList = list.Where(c => !result.Select(fc => fc.Id).Contains(c.Id)).ToList();
                    if (newItemList.Count > 0)
                        Connection.InsertAll(newItemList);

                    result = Connection.Table<DataTables.SubscriptionsChannelTb>().ToList();
                    var deleteItemList = result.Where(c => !list.Select(fc => fc.Id).Contains(c.Id)).ToList();
                    if (deleteItemList.Count > 0)
                        foreach (var delete in deleteItemList)
                            Connection.Delete(delete);

                    Connection.Commit();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Remove SubscriptionsChannel Videos
        public void RemoveSubscriptionsChannel(string subscriptionsChannelId)
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    if (!string.IsNullOrEmpty(subscriptionsChannelId))
                    {
                        var select = Connection.Table<DataTables.SubscriptionsChannelTb>().FirstOrDefault(a => a.Id.ToString() == subscriptionsChannelId);
                        if (select != null)
                        {
                            Connection.Delete(select);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Get SubscriptionsChannel Videos
        public ObservableCollection<UserDataObject> GetSubscriptionsChannel()
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    var select = Connection.Table<DataTables.SubscriptionsChannelTb>().ToList();
                    if (select.Count > 0)
                    {
                        var list = new ObservableCollection<UserDataObject>();
                        foreach (var item in select)
                        {
                            var db = Mapper.Map<UserDataObject>(item);
                            if (db != null) list.Add(db);
                        }

                        return list;
                    }

                    return new ObservableCollection<UserDataObject>();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return new ObservableCollection<UserDataObject>();
            }
        }

        #endregion

        #region WatchOffline Videos

        //Insert WatchOffline Videos
        public void Insert_WatchOfflineVideos(VideoObject video)
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    if (video != null)
                    {
                        var select = Connection.Table<DataTables.WatchOfflineVideosTb>().FirstOrDefault(a => a.VideoId == video.VideoId);
                        if (select == null)
                        {
                            var db = Mapper.Map<DataTables.WatchOfflineVideosTb>(video);
                            db.Owner = JsonConvert.SerializeObject(video.Owner);
                            Connection.Insert(db);
                        }
                        else
                        {
                            select = Mapper.Map<DataTables.WatchOfflineVideosTb>(video);
                            select.Owner = JsonConvert.SerializeObject(video.Owner);
                            Connection.Update(select);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Remove WatchOffline Videos
        public void Remove_WatchOfflineVideos(string watchOfflineVideosId)
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    if (!string.IsNullOrEmpty(watchOfflineVideosId))
                    {
                        var select = Connection.Table<DataTables.WatchOfflineVideosTb>().FirstOrDefault(a => a.VideoId == watchOfflineVideosId);
                        if (select != null)
                        {
                            Connection.Delete(select);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Get WatchOffline Videos
        public ObservableCollection<VideoObject> Get_WatchOfflineVideos()
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    var select = Connection.Table<DataTables.WatchOfflineVideosTb>().ToList();
                    if (select.Count > 0)
                    {
                        var list = new ObservableCollection<VideoObject>();
                        foreach (var item in select)
                        {
                            var db = new VideoObject()
                            {
                                Id = item.Id,
                                VideoId = item.VideoId,
                                UserId = item.UserId,
                                ShortId = item.ShortId,
                                Title = item.Title,
                                Description = item.Description,
                                Thumbnail = item.Thumbnail,
                                VideoLocation = item.VideoLocation,
                                Youtube = item.Youtube,
                                Vimeo = item.Vimeo,
                                Daily = item.Daily,
                                Facebook = item.Facebook,
                                Time = item.Time,
                                TimeDate = item.TimeDate,
                                Active = item.Active,
                                Tags = item.Tags,
                                Duration = item.Duration,
                                Size = item.Size,
                                Converted = item.Converted,
                                CategoryId = item.CategoryId,
                                Views = item.Views,
                                Featured = item.Featured,
                                Registered = item.Registered,
                                Type = item.Type,
                                Approved = item.Approved,
                                OrgThumbnail = item.OrgThumbnail,
                                VideoType = item.VideoType,
                                Source = item.Source,
                                Url = item.Url,
                                EditDescription = item.EditDescription,
                                MarkupDescription = item.MarkupDescription,
                                IsLiked = item.IsLiked,
                                IsDisliked = item.IsDisliked,
                                IsOwner = item.IsOwner,
                                TimeAlpha = item.TimeAlpha,
                                TimeAgo = item.TimeAgo,
                                CategoryName = item.CategoryName,
                                Likes = item.Likes,
                                Dislikes = item.Dislikes,
                                LikesPercent = item.LikesPercent,
                                DislikesPercent = item.DislikesPercent,
                                AgeRestriction = item.AgeRestriction,
                                Country = item.Country,
                                Demo = item.Demo,
                                GeoBlocking = item.GeoBlocking,
                                Gif = item.Gif,
                                IsMovie = item.IsMovie,
                                IsPurchased = item.IsPurchased,
                                MovieRelease = item.MovieRelease,
                                Ok = item.Ok,
                                PlaylistLink = item.PlaylistLink,
                                Privacy = item.Privacy,
                                Producer = item.Producer,
                                Quality = item.Quality,
                                Rating = item.Rating,
                                RentPrice = item.RentPrice,
                                SellVideo = item.SellVideo,
                                Stars = item.Stars,
                                SubCategory = item.SubCategory,
                                TrId = item.TrId,
                                Twitch = item.Twitch,
                                TwitchType = item.TwitchType,
                                DataVideoId = item.DataVideoId,
                                IsSubscribed = item.IsSubscribed,
                                Monetization = item.Monetization,
                                The1080P = item.The1080P,
                                The2048P = item.The2048P,
                                The240P = item.The240P,
                                The360P = item.The360P,
                                The4096P = item.The4096P,
                                The480P = item.The480P,
                                The720P = item.The720P,
                                Owner = new UserDataObject(),
                            };

                            if (!string.IsNullOrEmpty(item.Owner))
                                db.Owner = JsonConvert.DeserializeObject<UserDataObject>(item.Owner);

                            list.Add(db);
                        }

                        return list;
                    }

                    return new ObservableCollection<VideoObject>();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return new ObservableCollection<VideoObject>();
            }
        }

        public void Update_WatchOfflineVideos(string videoid, string videopath)
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    var select = Connection.Table<DataTables.WatchOfflineVideosTb>().FirstOrDefault(a => a.VideoId == videoid);
                    if (select != null)
                    {
                        //select.VideoName = videoid + ".mp4";
                        select.VideoLocation = videopath;
                        Connection.Update(select);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        #region Library Item

        //Insert data LibraryItem
        public void InsertLibraryItem(Classes.LibraryItem libraryItem)
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    if (libraryItem == null)
                        return;
                    var select = Connection.Table<DataTables.LibraryItemTb>().FirstOrDefault(a => a.SectionId == libraryItem.SectionId);
                    if (select != null)
                    {
                        select.VideoCount = libraryItem.VideoCount;
                        select.BackgroundImage = libraryItem.BackgroundImage;
                        Connection.Update(select);
                    }
                    else
                    {
                        var item = new DataTables.LibraryItemTb
                        {
                            SectionId = libraryItem.SectionId,
                            SectionText = libraryItem.SectionText,
                            VideoCount = libraryItem.VideoCount,
                            BackgroundImage = libraryItem.BackgroundImage
                        };
                        Connection.Insert(item);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Insert data LibraryItem
        public void InsertLibraryItem(ObservableCollection<Classes.LibraryItem> libraryList)
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    if (libraryList?.Count == 0)
                        return;
                    if (libraryList != null)
                    {
                        foreach (var libraryItem in libraryList)
                        {
                            var select = Connection.Table<DataTables.LibraryItemTb>().FirstOrDefault(a => a.SectionId == libraryItem.SectionId);
                            if (select != null)
                            {
                                select.SectionId = libraryItem.SectionId;
                                select.SectionText = libraryItem.SectionText;
                                select.VideoCount = libraryItem.VideoCount;
                                select.BackgroundImage = libraryItem.BackgroundImage;

                                Connection.Update(select);
                            }
                            else
                            {
                                var item = new DataTables.LibraryItemTb
                                {
                                    SectionId = libraryItem.SectionId,
                                    SectionText = libraryItem.SectionText,
                                    VideoCount = libraryItem.VideoCount,
                                    BackgroundImage = libraryItem.BackgroundImage
                                };
                                Connection.Insert(item);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Get data LibraryItem
        public ObservableCollection<DataTables.LibraryItemTb> Get_LibraryItem()
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    var select = Connection.Table<DataTables.LibraryItemTb>().OrderBy(a => a.SectionId).ToList();
                    if (select.Count > 0)
                    {
                        return new ObservableCollection<DataTables.LibraryItemTb>(select);
                    }

                    return new ObservableCollection<DataTables.LibraryItemTb>();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return new ObservableCollection<DataTables.LibraryItemTb>();
            }
        }

        #endregion

        #region Shared Videos

        //Insert Shared Videos
        public void Insert_SharedVideos(VideoObject video)
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    if (video != null)
                    {
                        var select = Connection.Table<DataTables.SharedVideosTb>().FirstOrDefault(a => a.VideoId == video.VideoId);
                        if (select == null)
                        {
                            var db = Mapper.Map<DataTables.SharedVideosTb>(video);
                            db.Owner = JsonConvert.SerializeObject(video.Owner);
                            Connection.Insert(db);
                        }
                        else
                        {
                            select = Mapper.Map<DataTables.SharedVideosTb>(video);
                            select.Owner = JsonConvert.SerializeObject(video.Owner);
                            Connection.Update(select);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Get Shared Videos
        public ObservableCollection<VideoObject> Get_SharedVideos()
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    var select = Connection.Table<DataTables.SharedVideosTb>().ToList();
                    if (select.Count > 0)
                    {
                        var list = new ObservableCollection<VideoObject>();
                        foreach (var item in select)
                        {
                            var db = new VideoObject()
                            {
                                Id = item.Id,
                                VideoId = item.VideoId,
                                UserId = item.UserId,
                                ShortId = item.ShortId,
                                Title = item.Title,
                                Description = item.Description,
                                Thumbnail = item.Thumbnail,
                                VideoLocation = item.VideoLocation,
                                Youtube = item.Youtube,
                                Vimeo = item.Vimeo,
                                Daily = item.Daily,
                                Facebook = item.Facebook,
                                Time = item.Time,
                                TimeDate = item.TimeDate,
                                Active = item.Active,
                                Tags = item.Tags,
                                Duration = item.Duration,
                                Size = item.Size,
                                Converted = item.Converted,
                                CategoryId = item.CategoryId,
                                Views = item.Views,
                                Featured = item.Featured,
                                Registered = item.Registered,
                                Type = item.Type,
                                Approved = item.Approved,
                                OrgThumbnail = item.OrgThumbnail,
                                VideoType = item.VideoType,
                                Source = item.Source,
                                Url = item.Url,
                                EditDescription = item.EditDescription,
                                MarkupDescription = item.MarkupDescription,
                                IsLiked = item.IsLiked,
                                IsDisliked = item.IsDisliked,
                                IsOwner = item.IsOwner,
                                TimeAlpha = item.TimeAlpha,
                                TimeAgo = item.TimeAgo,
                                CategoryName = item.CategoryName,
                                Likes = item.Likes,
                                Dislikes = item.Dislikes,
                                LikesPercent = item.LikesPercent,
                                DislikesPercent = item.DislikesPercent,
                                AgeRestriction = item.AgeRestriction,
                                Country = item.Country,
                                Demo = item.Demo,
                                GeoBlocking = item.GeoBlocking,
                                Gif = item.Gif,
                                IsMovie = item.IsMovie,
                                IsPurchased = item.IsPurchased,
                                MovieRelease = item.MovieRelease,
                                Ok = item.Ok,
                                PlaylistLink = item.PlaylistLink,
                                Privacy = item.Privacy,
                                Producer = item.Producer,
                                Quality = item.Quality,
                                Rating = item.Rating,
                                RentPrice = item.RentPrice,
                                SellVideo = item.SellVideo,
                                Stars = item.Stars,
                                SubCategory = item.SubCategory,
                                TrId = item.TrId,
                                Twitch = item.Twitch,
                                TwitchType = item.TwitchType,
                                DataVideoId = item.DataVideoId,
                                IsSubscribed = item.IsSubscribed,
                                Monetization = item.Monetization,
                                The1080P = item.The1080P,
                                The2048P = item.The2048P,
                                The240P = item.The240P,
                                The360P = item.The360P,
                                The4096P = item.The4096P,
                                The480P = item.The480P,
                                The720P = item.The720P,
                                Owner = new UserDataObject(),
                            };

                            if (!string.IsNullOrEmpty(item.Owner))
                                db.Owner = JsonConvert.DeserializeObject<UserDataObject>(item.Owner);

                            list.Add(db);
                        }

                        return list;
                    }

                    return new ObservableCollection<VideoObject>();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return new ObservableCollection<VideoObject>();
            }
        }

        #endregion

        #region Not Interested Videos

        //Insert NotInterested Videos
        public void Insert_NotInterestedVideos(VideoObject video)
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    if (video != null)
                    {
                        var select = Connection.Table<DataTables.NotInterestedTb>().FirstOrDefault(a => a.VideoId == video.VideoId);
                        if (select == null)
                        {
                            var db = Mapper.Map<DataTables.NotInterestedTb>(video);
                            db.Owner = JsonConvert.SerializeObject(video.Owner);
                            Connection.Insert(db);
                        }
                        else
                        {
                            select = Mapper.Map<DataTables.NotInterestedTb>(video);
                            select.Owner = JsonConvert.SerializeObject(video.Owner);
                            Connection.Update(select);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Get NotInterested Videos
        public ObservableCollection<VideoObject> Get_NotInterestedVideos()
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    var select = Connection.Table<DataTables.NotInterestedTb>().ToList();
                    if (select.Count > 0)
                    {
                        var list = new ObservableCollection<VideoObject>();
                        foreach (var item in select)
                        {
                            var db = new VideoObject()
                            {
                                Id = item.Id,
                                VideoId = item.VideoId,
                                UserId = item.UserId,
                                ShortId = item.ShortId,
                                Title = item.Title,
                                Description = item.Description,
                                Thumbnail = item.Thumbnail,
                                VideoLocation = item.VideoLocation,
                                Youtube = item.Youtube,
                                Vimeo = item.Vimeo,
                                Daily = item.Daily,
                                Facebook = item.Facebook,
                                Time = item.Time,
                                TimeDate = item.TimeDate,
                                Active = item.Active,
                                Tags = item.Tags,
                                Duration = item.Duration,
                                Size = item.Size,
                                Converted = item.Converted,
                                CategoryId = item.CategoryId,
                                Views = item.Views,
                                Featured = item.Featured,
                                Registered = item.Registered,
                                Type = item.Type,
                                Approved = item.Approved,
                                OrgThumbnail = item.OrgThumbnail,
                                VideoType = item.VideoType,
                                Source = item.Source,
                                Url = item.Url,
                                EditDescription = item.EditDescription,
                                MarkupDescription = item.MarkupDescription,
                                IsLiked = item.IsLiked,
                                IsDisliked = item.IsDisliked,
                                IsOwner = item.IsOwner,
                                TimeAlpha = item.TimeAlpha,
                                TimeAgo = item.TimeAgo,
                                CategoryName = item.CategoryName,
                                Likes = item.Likes,
                                Dislikes = item.Dislikes,
                                LikesPercent = item.LikesPercent,
                                DislikesPercent = item.DislikesPercent,
                                AgeRestriction = item.AgeRestriction,
                                Country = item.Country,
                                Demo = item.Demo,
                                GeoBlocking = item.GeoBlocking,
                                Gif = item.Gif,
                                IsMovie = item.IsMovie,
                                IsPurchased = item.IsPurchased,
                                MovieRelease = item.MovieRelease,
                                Ok = item.Ok,
                                PlaylistLink = item.PlaylistLink,
                                Privacy = item.Privacy,
                                Producer = item.Producer,
                                Quality = item.Quality,
                                Rating = item.Rating,
                                RentPrice = item.RentPrice,
                                SellVideo = item.SellVideo,
                                Stars = item.Stars,
                                SubCategory = item.SubCategory,
                                TrId = item.TrId,
                                Twitch = item.Twitch,
                                TwitchType = item.TwitchType,
                                DataVideoId = item.DataVideoId,
                                IsSubscribed = item.IsSubscribed,
                                Monetization = item.Monetization,
                                The1080P = item.The1080P,
                                The2048P = item.The2048P,
                                The240P = item.The240P,
                                The360P = item.The360P,
                                The4096P = item.The4096P,
                                The480P = item.The480P,
                                The720P = item.The720P,
                                Owner = new UserDataObject(),
                            };

                            if (!string.IsNullOrEmpty(item.Owner))
                                db.Owner = JsonConvert.DeserializeObject<UserDataObject>(item.Owner);

                            list.Add(db);
                        }

                        return list;
                    }

                    return new ObservableCollection<VideoObject>();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return new ObservableCollection<VideoObject>();
            }
        }

        #endregion

        #region Last Chat

        //Insert data To Last Chat Table
        public void InsertOrReplaceLastChatTable(ObservableCollection<GetChatsObject.Data> usersContactList)
        {
            try
            {
                using (Connection = OpenConnection())
                {

                    var result = Connection.Table<DataTables.LastChatTb>().ToList();
                    List<DataTables.LastChatTb> list = new List<DataTables.LastChatTb>();
                    foreach (var info in usersContactList)
                    {
                        var user = new DataTables.LastChatTb
                        {
                            Id = info.Id,
                            UserOne = info.UserOne,
                            UserTwo = info.UserTwo,
                            Time = info.Time,
                            TextTime = info.TextTime,
                            GetCountSeen = info.GetCountSeen,
                            UserDataJson = JsonConvert.SerializeObject(info.User),
                            GetLastMessageJson = JsonConvert.SerializeObject(info.GetLastMessage),
                        };

                        list.Add(user);

                        var update = result.FirstOrDefault(a => a.Id == info.Id);
                        if (update != null)
                        {
                            update = user;
                            Connection.Update(update);
                        }
                    }

                    if (list.Count > 0)
                    {
                        Connection.BeginTransaction();
                        //Bring new  
                        var newItemList = list.Where(c => !result.Select(fc => fc.Id).Contains(c.Id)).ToList();
                        if (newItemList.Count > 0)
                        {
                            Connection.InsertAll(newItemList);
                        }

                        var deleteItemList = result.Where(c => !list.Select(fc => fc.Id).Contains(c.Id)).ToList();
                        if (deleteItemList.Count > 0)
                        {
                            foreach (var delete in deleteItemList)
                            {
                                Connection.Delete(delete);
                            }
                        }

                        Connection.Commit();
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Get data To LastChat Table
        public ObservableCollection<GetChatsObject.Data> GetAllLastChat()
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    var select = Connection.Table<DataTables.LastChatTb>().ToList();
                    if (select.Count > 0)
                    {
                        var list = select.Select(user => new GetChatsObject.Data
                        {
                            Id = user.Id,
                            UserOne = user.UserOne,
                            UserTwo = user.UserTwo,
                            Time = user.Time,
                            TextTime = user.TextTime,
                            GetCountSeen = user.GetCountSeen,
                            User = JsonConvert.DeserializeObject<UserDataObject>(user.UserDataJson),
                            GetLastMessage =
                                JsonConvert.DeserializeObject<GetChatsObject.GetLastMessage>(user.GetLastMessageJson),
                        }).ToList();

                        return new ObservableCollection<GetChatsObject.Data>(list);
                    }

                    return new ObservableCollection<GetChatsObject.Data>();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return new ObservableCollection<GetChatsObject.Data>();
            }
        }

        // Get data To LastChat Table By Id >> Load More
        public ObservableCollection<GetChatsObject.Data> GetLastChatById(int id, int nSize)
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    var query = Connection.Table<DataTables.LastChatTb>().Where(w => w.AutoIdLastChat >= id)
                        .OrderBy(q => q.AutoIdLastChat).Take(nSize).ToList();
                    if (query.Count > 0)
                    {
                        var list = query.Select(user => new GetChatsObject.Data
                        {
                            Id = user.Id,
                            UserOne = user.UserOne,
                            UserTwo = user.UserTwo,
                            Time = user.Time,
                            TextTime = user.TextTime,
                            GetCountSeen = user.GetCountSeen,
                            User = JsonConvert.DeserializeObject<UserDataObject>(user.UserDataJson),
                            GetLastMessage =
                                JsonConvert.DeserializeObject<GetChatsObject.GetLastMessage>(user.GetLastMessageJson),
                        }).ToList();

                        if (list.Count > 0)
                            return new ObservableCollection<GetChatsObject.Data>(list);
                        return null;
                    }

                    return new ObservableCollection<GetChatsObject.Data>();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new ObservableCollection<GetChatsObject.Data>();
            }
        }

        //Remove data To LastChat Table
        public void DeleteUserLastChat(string userId)
        {
            try
            {
                using (Connection = OpenConnection())
                {

                    var user = Connection.Table<DataTables.LastChatTb>()
                        .FirstOrDefault(c => c.UserTwo.ToString() == userId);
                    if (user != null)
                    {
                        Connection.Delete(user);
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Clear All data LastChat
        public void ClearLastChat()
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    Connection.DeleteAll<DataTables.LastChatTb>();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        #region Message

        //Insert data To Message Table
        public void InsertOrReplaceMessages(ObservableCollection<GetUserMessagesObject.Message> messageList)
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    var listOfDatabaseForInsert = new List<DataTables.MessageTb>();

                    // get data from database
                    var resultMessage = Connection.Table<DataTables.MessageTb>().ToList();
                    var listAllMessage = resultMessage.Select(messages => new GetUserMessagesObject.Message()
                    {
                        Id = messages.Id,
                        FromId = messages.FromId,
                        ToId = messages.ToId,
                        Text = messages.Text,
                        Seen = messages.Seen,
                        Time = messages.Time,
                        FromDeleted = messages.FromDeleted,
                        ToDeleted = messages.ToDeleted,
                        TextTime = messages.TextTime,
                        Position = messages.Position,
                    }).ToList();

                    foreach (var messages in messageList)
                    {
                        var maTb = new DataTables.MessageTb()
                        {
                            Id = messages.Id,
                            FromId = messages.FromId,
                            ToId = messages.ToId,
                            Text = messages.Text,
                            Seen = messages.Seen,
                            Time = messages.Time,
                            FromDeleted = messages.FromDeleted,
                            ToDeleted = messages.ToDeleted,
                            TextTime = messages.TextTime,
                            Position = messages.Position,
                        };

                        var dataCheck = listAllMessage.FirstOrDefault(a => a.Id == messages.Id);
                        if (dataCheck != null)
                        {
                            var checkForUpdate = resultMessage.FirstOrDefault(a => a.Id == dataCheck.Id);
                            if (checkForUpdate != null)
                            {
                                checkForUpdate.Id = messages.Id;
                                checkForUpdate.FromId = messages.FromId;
                                checkForUpdate.ToId = messages.ToId;
                                checkForUpdate.Text = messages.Text;
                                checkForUpdate.Seen = messages.Seen;
                                checkForUpdate.Time = messages.Time;
                                checkForUpdate.FromDeleted = messages.FromDeleted;
                                checkForUpdate.ToDeleted = messages.ToDeleted;
                                checkForUpdate.TextTime = messages.TextTime;
                                checkForUpdate.Position = messages.Position;

                                Connection.Update(checkForUpdate);
                            }
                            else
                            {
                                listOfDatabaseForInsert.Add(maTb);
                            }
                        }
                        else
                        {
                            listOfDatabaseForInsert.Add(maTb);
                        }
                    }

                    Connection.BeginTransaction();

                    //Bring new  
                    if (listOfDatabaseForInsert.Count > 0)
                    {
                        Connection.InsertAll(listOfDatabaseForInsert);
                    }

                    Connection.Commit();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Update one Messages Table
        public void InsertOrUpdateToOneMessages(GetUserMessagesObject.Message messages)
        {
            try
            {
                using (Connection = OpenConnection())
                {

                    var data = Connection.Table<DataTables.MessageTb>().FirstOrDefault(a => a.Id == messages.Id);
                    if (data != null)
                    {
                        data.Id = messages.Id;
                        data.FromId = messages.FromId;
                        data.ToId = messages.ToId;
                        data.Text = messages.Text;
                        data.Seen = messages.Seen;
                        data.Time = messages.Time;
                        data.FromDeleted = messages.FromDeleted;
                        data.ToDeleted = messages.ToDeleted;
                        data.TextTime = messages.TextTime;
                        data.Position = messages.Position;

                        Connection.Update(data);
                    }
                    else
                    {
                        var mdb = new DataTables.MessageTb
                        {
                            Id = messages.Id,
                            FromId = messages.FromId,
                            ToId = messages.ToId,
                            Text = messages.Text,
                            Seen = messages.Seen,
                            Time = messages.Time,
                            FromDeleted = messages.FromDeleted,
                            ToDeleted = messages.ToDeleted,
                            TextTime = messages.TextTime,
                            Position = messages.Position,
                        };

                        //Insert  one Messages Table
                        Connection.Insert(mdb);
                    }


                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Get data To Messages
        public string GetMessagesList(string fromId, string toId, string beforeMessageId)
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    var beforeQ = "";
                    if (beforeMessageId != "0")
                    {
                        beforeQ = "AND Id < " + beforeMessageId + " AND Id <> " + beforeMessageId + " ";
                    }

                    var query = Connection.Query<DataTables.MessageTb>(
                        "SELECT * FROM MessageTb WHERE ((FromId =" + fromId + " and ToId=" + toId + ") OR (FromId =" +
                        toId + " and ToId=" + fromId + ")) " + beforeQ);
                    var queryList = query
                        .Where(w => w.FromId == fromId && w.ToId == toId || w.ToId == fromId && w.FromId == toId)
                        .OrderBy(q => q.Time).TakeLast(35).ToList();
                    if (queryList.Count > 0)
                    {
                        foreach (var messages in queryList)
                        {
                            var m = new GetUserMessagesObject.Message
                            {
                                Id = messages.Id,
                                FromId = messages.FromId,
                                ToId = messages.ToId,
                                Text = messages.Text,
                                Seen = messages.Seen,
                                Time = messages.Time,
                                FromDeleted = messages.FromDeleted,
                                ToDeleted = messages.ToDeleted,
                                TextTime = messages.TextTime,
                                Position = messages.Position,
                            };

                            if (beforeMessageId == "0")
                            {
                                MessagesBoxActivity.MAdapter?.Add(m);
                            }
                            else
                            {
                                MessagesBoxActivity.MAdapter?.Insert(m, beforeMessageId);
                            }
                        }

                        return "1";
                    }

                    return "0";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "0";
            }
        }

        //Get data To where first Messages >> load more
        public List<DataTables.MessageTb> GetMessageList(string fromId, string toId, string beforeMessageId)
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    var beforeQ = "";
                    if (beforeMessageId != "0")
                    {
                        beforeQ = "AND Id < " + beforeMessageId + " AND Id <> " + beforeMessageId + " ";
                    }

                    var query = Connection.Query<DataTables.MessageTb>(
                        "SELECT * FROM MessageTb WHERE ((FromId =" + fromId + " and ToId=" + toId + ") OR (FromId =" +
                        toId + " and ToId=" + fromId + ")) " + beforeQ);
                    var queryList = query
                        .Where(w => w.FromId == fromId && w.ToId == toId || w.ToId == fromId && w.FromId == toId)
                        .OrderBy(q => q.Time).TakeLast(35).ToList();
                    return queryList;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        //Remove data To Messages Table
        public void Delete_OneMessageUser(string messageId)
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    var user = Connection.Table<DataTables.MessageTb>().FirstOrDefault(c => c.Id == messageId);
                    if (user != null)
                    {
                        Connection.Delete(user);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void DeleteAllMessagesUser(string fromId, string toId)
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    var query = Connection.Query<DataTables.MessageTb>(
                        "Delete FROM MessageTb WHERE ((FromId =" + fromId + " and ToId=" + toId + ") OR (FromId =" +
                        toId + " and ToId=" + fromId + "))");
                    Console.WriteLine(query);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Remove All data To Messages Table
        public void ClearAll_Messages()
        {
            try
            {
                Connection.DeleteAll<DataTables.MessageTb>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion
    }
}