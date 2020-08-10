using System;
using AutoMapper;
using AutoMapper.Configuration;
using PlayTube.SQLite;
using PlayTubeClient.Classes.Global;

namespace PlayTube.Helpers.Utils
{
    public static class ClassMapper
    {
        public static void SetMappers()
        {
            try
            {
                var cfg = new MapperConfigurationExpression
                {
                    AllowNullCollections = true
                };

                cfg.CreateMap<GetSettingsObject.SiteSettings, DataTables.MySettingsTb>().ForMember(x => x.AutoIdMySettings, opt => opt.Ignore());
                cfg.CreateMap<VideoObject, DataTables.WatchOfflineVideosTb>().ForMember(x => x.AutoIdaWatchOffline, opt => opt.Ignore());
                cfg.CreateMap<VideoObject, DataTables.SharedVideosTb>().ForMember(x => x.AutoIdSharedVideos, opt => opt.Ignore());
                cfg.CreateMap<VideoObject, DataTables.NotInterestedTb>().ForMember(x => x.AutoIdNotInterested, opt => opt.Ignore());
                cfg.CreateMap<UserDataObject, DataTables.ChannelTb>().ForMember(x => x.AutoIdChannel, opt => opt.Ignore());
                cfg.CreateMap<UserDataObject, DataTables.SubscriptionsChannelTb>().ForMember(x => x.AutoIdSubscriptions, opt => opt.Ignore());

                Mapper.Initialize(cfg);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}