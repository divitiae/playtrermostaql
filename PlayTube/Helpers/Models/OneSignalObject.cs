using Newtonsoft.Json;

namespace PlayTube.Helpers.Models
{
    public class OneSignalObject
    {
        public class NotificationInfoObject
        {
            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("notifier_id")]
            public int NotifierId { get; set; }

            [JsonProperty("recipient_id")]
            public int RecipientId { get; set; }

            [JsonProperty("video_id")]
            public int VideoId { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("text")]
            public object Text { get; set; }

            [JsonProperty("url")]
            public string Url { get; set; }

            [JsonProperty("seen")]
            public string Seen { get; set; }

            [JsonProperty("time")]
            public string Time { get; set; }

            [JsonProperty("sent_push")]
            public int SentPush { get; set; }

            [JsonProperty("type_text")]
            public string TypeText { get; set; }
        }

        public class VideoObject
        {

            [JsonProperty("youtube")]
            public string Youtube { get; set; }

            [JsonProperty("thumbnail")]
            public string Thumbnail { get; set; }

            [JsonProperty("twitch")]
            public string Twitch { get; set; }

            [JsonProperty("is_owner")]
            public bool? IsOwner { get; set; }

            [JsonProperty("facebook")]
            public string Facebook { get; set; }

            [JsonProperty("vimeo")]
            public string Vimeo { get; set; }

            [JsonProperty("user_id")]
            public string UserId { get; set; }

            [JsonProperty("twitch_type")]
            public string TwitchType { get; set; }

            [JsonProperty("daily")]
            public string Daily { get; set; }

            [JsonProperty("geo_blocking")]
            public string GeoBlocking { get; set; }

            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("ok")]
            public string Ok { get; set; }

            [JsonProperty("age_restriction")]
            public string AgeRestriction { get; set; }

            [JsonProperty("video_location")]
            public string VideoLocation { get; set; }

            [JsonProperty("video_id")]
            public string VideoId { get; set; }
        }

    }
}