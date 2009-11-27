using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.Net;
using System.Web;
using System.IO;
using System.Xml;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Utilities;
using Web20Tools.Feed.Rss;

namespace SOApiDotNet
{
    /// <summary>
    /// An enumeration representing different Stack Overflow sites.
    /// </summary>
    public enum TrilogySite
    {
        /// <summary>
        /// stackoverflow.com
        /// </summary>
        SO,
        /// <summary>
        /// serverfault.com
        /// </summary>
        SF,
        /// <summary>
        /// superuesr.com
        /// </summary>
        SU,
        /// <summary>
        /// meta.stackoverflow.com
        /// </summary>
        Meta
    }

    /// <summary>
    /// An enumeration representing sort rules of pages.
    /// </summary>
    public enum SortRule
    {
        /// <summary>
        /// Sorting by recent changes/updates
        /// </summary>
        recent,
        /// <summary>
        /// Sorting by view count
        /// </summary>
        views,
        /// <summary>
        /// Sorting by time
        /// </summary>
        newest,
        /// <summary>
        /// Sorting by vote count.
        /// </summary>
        votes
    }

    /// <summary>
    /// The main class that handles the Stack Overflow API.
    /// </summary>
    public static class StackOverflow
    {
        /// <summary>
        /// Provides all userids of users that have a certain display name.
        /// </summary>
        /// <param name="username">The display name in question.</param>
        /// <param name="site">Trilogy site in question.</param>
        /// <returns>A list of all the userids, in Long format. Next, you should obtain user flair to find the right user.</returns>
        public static List<long> GetUserIdsFromUsername(string username, TrilogySite site)
        {
            List<long> userids = new List<long>();
            if (string.IsNullOrEmpty(username))
            {
                throw new ApplicationException("Please enter a username to find userids for it.");
                return null;
            }
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://"+GetSiteUrl(site)+".com/users/filter/" + Uri.EscapeUriString(username));
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            int currentindex = 0;
            string html = new StreamReader(response.GetResponseStream()).ReadToEnd();
            currentindex = html.IndexOf("<div class=\"user-details\">");

            if(currentindex == -1)
            {
                return userids;
            }

            string intermediary1 = html.Substring(html.IndexOf("<a", currentindex), html.IndexOf("</a>", currentindex) - html.IndexOf("<a", currentindex) + 1);
            currentindex = intermediary1.IndexOf("href=\"/users/");
            string strId = intermediary1.Substring(currentindex + 13, intermediary1.IndexOf("/", currentindex + 13) - currentindex + 13);
            long lId = long.Parse(strId);
            userids.Add(lId);
            while (html.IndexOf("<div class=\"user-details\">", currentindex) != -1)
            {
                intermediary1 = html.Substring(html.IndexOf("<a", html.IndexOf("<div class=\"user-details\">", currentindex), html.IndexOf("</a>", html.IndexOf("<div class=\"user-details\">", currentindex) - html.IndexOf("<a", html.IndexOf("<div class=\"user-details\">", currentindex) + 1))));
                currentindex = intermediary1.IndexOf("href=\"/users/");
                strId = intermediary1.Substring(currentindex + 13, intermediary1.IndexOf("/", currentindex + 13) - currentindex + 13);
                lId = long.Parse(strId);
                userids.Add(lId);
            }
            return userids;
        }

        /// <summary>
        /// Obtains a user's favorites.
        /// </summary>
        /// <param name="userid">Userid of the user in question.</param>
        /// <param name="page">Page #.</param>
        /// <param name="pagesize">Page size.</param>
        /// <param name="sort">SortRule of the page.</param>
        /// <param name="site">Trilogy site in question.</param>
        /// <returns>A list of SOFavorite's for the user in question.</returns>
        public static List<SOFavorite> GetUserFavorites(long userid, int page, long pagesize, SortRule sort, TrilogySite site)
        {
            //List<SOFavorite> result = new List<SOFavorite>();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format("http://{4}.com/api/userfavorites.json?userid={0}&page={1}&pagesize={2}&sort={3}",userid, page, pagesize, sort, GetSiteUrl(site)));
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            List<SOFavorite> fav = (List<SOFavorite>)new JsonSerializer().Deserialize(new JsonReader((TextReader)new StreamReader(response.GetResponseStream())));
            //Do the JSON magic!

            return fav;
        }

        /// <summary>
        /// Obtains a user's favorites with default parameters (i.e. 100000 per page)
        /// </summary>
        /// <param name="userid">Id of the user.</param>
        /// <param name="site">Trilogy site in question.</param>
        /// <returns></returns>
        public static List<SOFavorite> GetUserFavorites(long userid, TrilogySite site) //how to short-redirect method call? --> ALIAS
        {
            return GetUserFavorites(userid, 0, 100000, SortRule.recent, site);
        }

        /// <summary>
        /// Obtains reputation changes for a user.
        /// </summary>
        /// <param name="userid">Id of the user.</param>
        /// <param name="fromtime">Starting time, expressed as a DateTime.</param>
        /// <param name="totime">Ending time, expressed as a DateTime.</param>
        /// <param name="site">Trilogy site in question.</param>
        /// <returns>A list of RepChange's for the user.</returns>
        public static List<RepChange> UserReputationGraph(long userid, DateTime fromtime, DateTime totime, TrilogySite site) { return UserReputationGraph(userid, ConvertToUnixTimestamp(fromtime), ConvertToUnixTimestamp(totime), site); }

        /// <summary>
        /// Obtains reputation changes for a user.
        /// </summary>
        /// <param name="userid">Id of the user.</param>
        /// <param name="fromtime">Starting time, expressed as a double (Unix timestamp).</param>
        /// <param name="totime">Ending time, expressed as a double (Unix timestamp).</param>
        /// <param name="site">Trilogy site in question.</param>
        /// <returns>A list of RepChange's for the user.</returns>
        public static List<RepChange> UserReputationGraph (long userid, double fromtime, double totime, TrilogySite site)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format("http://{3}.com/users/rep-graph/{0}/{1}/{2}", userid, fromtime, totime, GetSiteUrl(site)));
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            List<RepChange> forret = (List<RepChange>)new JsonSerializer().Deserialize(new JsonReader((TextReader)new StreamReader(response.GetResponseStream())));
            //Do the JSON magic!

            return forret;
        }
        
        /// <summary>
        /// Obtains a user's flair info.
        /// </summary>
        /// <param name="userid">Id of the user.</param>
        /// <param name="site">Trilogy site in question.</param>
        /// <returns>Flair details, expressed through an object of type UserFlair.</returns>
        public static UserFlair GetUserFlair(long userid, TrilogySite site)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format("http://{1}.com/users/flair/{0}.json", userid, GetSiteUrl(site)));
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            UserFlair forret = (UserFlair)new JsonSerializer().Deserialize(new JsonReader((TextReader)new StreamReader(response.GetResponseStream())));
            //Do the JSON magic!

            return forret;
        }
        
        /// <summary>
        /// Obtains a user's questions.
        /// </summary>
        /// <param name="userid">Id of the user.</param>
        /// <param name="page">Page#</param>
        /// <param name="pagesize">Size of each page.</param>
        /// <param name="sort">SortRule.</param>
        /// <param name="site">Trilogy site in question.</param>
        /// <returns>An object of type UserQuestions which represents the questions of the user in question.</returns>
        public static UserQuestions GetUserQuestions(long userid, int page, int pagesize, SortRule sort, TrilogySite site)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format("http://{4}.com/api/userquestions.json?userid={0}&page={1}&pagesize={2}&sort={3}", userid, page, pagesize, sort.ToString(), GetSiteUrl(site)));
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            UserQuestions forret = (UserQuestions)new JsonSerializer().Deserialize(new JsonReader((TextReader)new StreamReader(response.GetResponseStream())));
            //Do the JSON magic!

            return forret;
        }

        /// <summary>
        /// Obtains a user's answers.
        /// </summary>
        /// <param name="userid">Id of the user.</param>
        /// <param name="page">Page #</param>
        /// <param name="pagesize">Size of each page.</param>
        /// <param name="sort">SortRule.</param>
        /// <param name="site">Trilogy site in question.</param>
        /// <returns>An object of type UserAnswers which represents the answers of the user in question.</returns>
        public static UserAnswers GetUserAnswers (long userid, int page, int pagesize, SortRule sort, TrilogySite site)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format("http://{4}.com/api/useranswers.json?userid={0}&page={1}&pagesize={2}&sort={3}", userid, page, pagesize, sort.ToString(), GetSiteUrl(site)));
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            UserAnswers forret = (UserAnswers)new JsonSerializer().Deserialize(new JsonReader((TextReader)new StreamReader(response.GetResponseStream())));
            //Do the JSON magic!

            return forret;
        }

        #region Feeds

        /// <summary>
        /// Utilises recent question feeds to obtain recently updated questions on a certain site.
        /// </summary>
        /// <param name="site">Trilogy site in question.</param>
        /// <returns>A list of objects of type Question, which represents the recent questions on a trilogy site.</returns>
        public static List<Question> GetRecentQuestions(TrilogySite site)
        {
            List<Question> RecentQuestions = new List<Question>();
            RssFeed feed = RssFeed.Load(string.Format("http://{0}.com/feeds",GetSiteUrl(site)));
            RssChannel channel = (RssChannel)feed.Channels[0];
            foreach (RssItem item in channel.Items)
            {
                Question toadd = new Question();
                foreach(RssCategory cat in item.Categories)
                {
                    toadd.Categories.Add(cat.Name);
                }
                toadd.Author = item.Author;
                toadd.CreatedDate = ConvertToUnixTimestamp(item.PubDate).ToString();
                toadd.Id = item.Link.Url.ToString();
                toadd.Link = item.Link.Url.ToString();
                toadd.Summary = item.Description;
                
                //TODO: OTHER PROPERTIES
                RecentQuestions.Add(toadd);
            }
            return RecentQuestions;
        }

        /// <summary>
        /// Utilises recent user activity feeds to find information regarding activity of a user.
        /// </summary>
        /// <param name="userid">Userid of the user in question</param>
        /// <param name="site">Trilogy site in question</param>
        /// <returns>The user's recent activity, expressed in a list of Posts.</returns>
        public static List<Post> GetRecentActivity(long userid, TrilogySite site)
        {
            List<Post> Activity = new List<Post>();
            RssFeed feed = RssFeed.Load(string.Format("http://{0}.com/feeds/user/{1}",GetSiteUrl(site),userid));
            RssChannel channel = (RssChannel)feed.Channels[0];
            foreach (RssItem item in channel.Items)
            {
                Post toadd = new Post();
                foreach (RssCategory cat in item.Categories)
                {
                    toadd.Categories.Add(cat.Name);
                }
                toadd.Author = item.Author;
                toadd.CreatedDate = ConvertToUnixTimestamp(item.PubDate).ToString();
                toadd.Id = item.Link.Url.ToString();
                toadd.Link = item.Link.Url.ToString();
                toadd.Summary = item.Description;

                //TODO: OTHER PROPERTIES
                Activity.Add(toadd);
            }
            return Activity;


        }

        /// <summary>
        /// Utilises question activity feeds to extract information about activity (posts) inside questions.
        /// </summary>
        /// <param name="numQuestion">Question ID</param>
        /// <param name="site">Trilogy site in question</param>
        /// <returns>A list of posts that are included in the activity corresponding to the question site and ID provided.</returns>
        public static List<Post> GetQuestionActivity(long numQuestion, TrilogySite site)
        {
            List<Post> Activity = new List<Post>();
            RssFeed feed = RssFeed.Load(string.Format("http://{0}.com/feeds/question/{1}", GetSiteUrl(site), numQuestion));
            RssChannel channel = (RssChannel)feed.Channels[0];
            foreach (RssItem item in channel.Items)
            {
                Post toadd = new Post();
                foreach (RssCategory cat in item.Categories)
                {
                    toadd.Categories.Add(cat.Name);
                }
                toadd.Author = item.Author;
                toadd.CreatedDate = ConvertToUnixTimestamp(item.PubDate).ToString();
                toadd.Id = item.Link.Url.ToString();
                toadd.Link = item.Link.Url.ToString();
                toadd.Summary = item.Description;

                //TODO: OTHER PROPERTIES
                Activity.Add(toadd);
            }
            return Activity;


        }

        /// <summary>
        /// Utilises tag feeds to find questions in a particular tag.
        /// </summary>
        /// <param name="tagname">Name of the tag in question.</param>
        /// <param name="site">Trilogy site in question.</param>
        /// <returns>Recent questions in that tag for the site provided.</returns>
        public static List<Question> GetTagQuestions(string tagname, TrilogySite site)
        {
            List<Question> questions = new List<Question>();
            RssFeed feed = RssFeed.Load(string.Format("http://{0}.com/feeds/tag/{1}", GetSiteUrl(site), Uri.EscapeUriString(tagname.Trim())));
            RssChannel channel = (RssChannel)feed.Channels[0];
            foreach (RssItem item in channel.Items)
            {
                Question toadd = new Question();
                foreach (RssCategory cat in item.Categories)
                {
                    toadd.Categories.Add(cat.Name);
                }
                toadd.Author = item.Author;
                toadd.CreatedDate = ConvertToUnixTimestamp(item.PubDate).ToString();
                toadd.Id = item.Link.Url.ToString();
                toadd.Link = item.Link.Url.ToString();
                toadd.Summary = item.Description;

                //TODO: OTHER PROPERTIES
                questions.Add(toadd);
            }



            return questions;

        }


        #endregion Feeds





        #region Helper Methods

        /// <summary>
        /// Converts a System.DateTime into a Unix timestamp
        /// </summary>
        /// <param name="value">The DateTime to convert</param>
        /// <returns>Unix timestamp obtained through conversion</returns>
        public static double ConvertToUnixTimestamp(DateTime value)
        {
            //create Timespan by subtracting the value provided from
            //the Unix Epoch
            TimeSpan span = (value - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime());

            //return the total seconds (which is a UNIX timestamp)
            return (double)span.TotalSeconds;
        }

        /// <summary>
        /// Converts a Unix timestamp into a System.DateTime
        /// </summary>
        /// <param name="timestamp">The Unix timestamp to convert, as a double</param>
        /// <returns>DateTime obtained through conversion</returns>
        public static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }

        /// <summary>
        /// Extracts a Unix timestamp from a Json time string. I'm not sure whether the JSON library currently used does this automatically or not. Both options are covered. Next, output should be sent to ConvertFromUnixTimestamp(...).
        /// </summary>
        /// <param name="json">The original JSON date string.</param>
        /// <returns>A Unix timestamp, extracted from the JSON date string, expressed as a double.</returns>
        public static double ExtractTimestampFromJsonTime(string json)
        {
            int FirstParenthesis = json.IndexOf("(");
            int LastParenthesis = json.IndexOf(")");
            if (FirstParenthesis == -1 || LastParenthesis == -1)
            {
                //timestamp has already been extracted by json library
                return double.Parse(json);

            }

            string time = json.Substring(FirstParenthesis+1,LastParenthesis-FirstParenthesis);
            return double.Parse(time);
            
        }


        /// <summary>
        /// Executes an HTTP GET command and retrives the information.		
        /// </summary>
        /// <param name="url">The URL to perform the GET operation</param>
        /// <param name="userName">The username to use with the request</param>
        /// <param name="password">The password to use with the request</param>
        /// <returns>The response of the request, or null if we got 404 or nothing.</returns>
        static string ExecuteGetCommand(string url, string userName, string password)
        {
            using (WebClient client = new WebClient())
            {
                if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
                {
                    client.Credentials = new NetworkCredential(userName, password);
                }

                try
                {
                    using (Stream stream = client.OpenRead(url))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
                catch (WebException ex)
                {
                    //
                    // Handle HTTP 404 errors gracefully and return a null string to indicate there is no content.
                    //
                    if (ex.Response is HttpWebResponse)
                    {
                        if ((ex.Response as HttpWebResponse).StatusCode == HttpStatusCode.NotFound)
                        {
                            return null;
                        }
                    }

                    throw ex;
                }
            }

            return null;
        }
        /// <summary>
        /// Executes an HTTP POST command and retrives the information.		
        /// This function will automatically include a "source" parameter if the "Source" property is set.
        /// </summary>
        /// <param name="url">The URL to perform the POST operation</param>
        /// <param name="userName">The username to use with the request</param>
        /// <param name="password">The password to use with the request</param>
        /// <param name="data">The data to post</param> 
        /// <returns>The response of the request, or null if we got 404 or nothing.</returns>
        static string ExecutePostCommand(string url, string userName, string password, string data)
        {
            WebRequest request = WebRequest.Create(url);
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
            {
                request.Credentials = new NetworkCredential(userName, password);
                request.ContentType = "application/x-www-form-urlencoded";
                request.Method = "POST";

                //Add headers w/ url-encode

                byte[] bytes = Encoding.UTF8.GetBytes(data);

                request.ContentLength = bytes.Length;
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);

                    using (WebResponse response = request.GetResponse())
                    {
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Uses the TrilogySite enumeration to obtain a url for each possibility.
        /// </summary>
        /// <param name="site">Trilogy site in question.</param>
        /// <returns>The url of the trilogy site, in string format.</returns>
        static string GetSiteUrl(TrilogySite site)
        {
            switch (site)
            {
                case TrilogySite.SO:
                    return "stackoverflow";
                case TrilogySite.SF:
                    return "serverfault";
                case TrilogySite.SU:
                    return "superuser";
                case TrilogySite.Meta:
                    return "meta.stackoverflow";
                default:
                    return "stackoverflow";

            }

        }

        #endregion

    }
}
