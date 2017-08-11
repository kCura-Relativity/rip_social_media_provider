using System;

namespace SocialMedia.Twitter
{
    public class Constants
    {
        public const String MULTIPLE_CHOICE_DELIMITER = ";";
        public const Int32 MAXIMUM_TWEET_DOWNLOAD = 50;
        public const String CONTENT_TYPE = "application/x-www-form-urlencoded;charset=UTF-8";
        public const String MEDIA_URL_FORMAT = @"<img src=""{0}"" height=""200"" />";
        public class AppConfigKeys
        {
            public const String TWITTER_CONSUMER_KEY = "TwitterConsumerKey";
            public const String TWITTER_CONSUMER_SECRET = "TwitterConsumerSecret";
        }
        public class AuthorizationTypes
        {
            public const String BASIC = "Basic";
            public const String BEARER = "Bearer";
        }
        public class URLs
        {
            public const String REQUEST_BEARER_TOKEN = "https://api.twitter.com/oauth2/token";
            public const String DOWNLOAD_FEED = "https://api.twitter.com/1.1/statuses/user_timeline.json?screen_name={0}&count={1}";
        }
        public class FieldNames
        {
            public const String ID = "ID";
            public const String TWITTER_URL = "TwitterURL";
            public const String TWITTER_HANDLE = "TwitterHandle";
            public const String TEXT = "Text";
            public const String DATE_CREATED = "DateCreated";
            public const String HASH_TAGS = "HashTags";
            public const String HAS_MEDIA = "HasMedia";
            public const String MEDIA_TYPE = "MediaType";
            public const String MEDIA_URL = "MediaURL";
            public const String IS_REPLY_TO = "IsReplyTo";
            public const String IS_RETWEET = "IsReTweet";
            public const String RETWEET_COUNT = "ReTweetCount";
            public const String LIKE_COUNT = "LikeCount";
        }

        public class TwiiterFeedFieldNames
        {
            public const String ID = "id";
            public const String TEXT = "Text";
            public const String CREATED_AT = "created_at";
            public const String ENTITIES = "entities";
            public const String MEDIA = "media";
            public const String IS_REPLY_TO = "in_reply_to_status_id";
            public const String RETWEET_STATUS = "retweeted_status";
            public const String RETWEET_COUNT = "retweet_count";
            public const String FAVORITE_COUNT = "favorite_count";
            public const String MEDIA_URL = "media_url";
            public const String TYPE = "type";
            public const String HASH_TAGS = "hashtags";
        }
    }
}
