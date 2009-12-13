/*  Copyright 2009 Maxim Zaslavsky. All Rights Reserved.
 * 
 *  This file is part of SOApiDotNet.

    Copyright (c) 2009, Maxim Zaslavsky
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
Neither the name of SOApiDotNet nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 * 
 *  For more information about SOApiDotNet, please visit <http://code.google.com/p/soapidotnet/>.
 * 
 * This software uses the following libraries:
 * 
 * RSS.NET - Copyright © 2002-2005 ToolButton Inc.. All Rights Reserved.
 * JSON.NET - Copyright (c) 2007 James Newton-King.
 * 
 * */

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
using System.ServiceModel.Syndication;


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
        /// superuser.com
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
            }
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://"+GetSiteUrl(site)+".com/users/filter/" + Uri.EscapeUriString(username.ToLower()));
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
            int iaf = intermediary1.IndexOf("/", currentindex + 14);
            iaf -= currentindex;
            iaf -= 13;
            string strId = intermediary1.Substring(currentindex + 13, iaf);
            long lId = long.Parse(strId);
            userids.Add(lId);
            currentindex = html.IndexOf("<div class=\"user-details\">") + 1;
            while (html.IndexOf("<div class=\"user-details\">", currentindex) != -1)
            {
                int currentindexbackup = currentindex;
                int i1 = html.IndexOf("<a", html.IndexOf("<div class=\"user-details\">", currentindex)); //finds a in user-details of that user
                intermediary1 = html.Substring(i1, html.IndexOf("</a>", html.IndexOf("<div class=\"user-details\">", currentindex)) - i1 + 1);
                currentindex = intermediary1.IndexOf("href=\"/users/");
                iaf = intermediary1.IndexOf("/", currentindex + 14);
                iaf -= currentindex;
                iaf -= 13;
                strId = intermediary1.Substring(currentindex + 13, iaf);
                lId = long.Parse(strId);
                userids.Add(lId);
                currentindex = html.IndexOf("<div class=\"user-details\">", currentindexbackup+1) + 1; // do we need the first +1?
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
            List<SOFavorite> fav = JsonConvert.DeserializeObject<List<SOFavorite>>(new StreamReader(response.GetResponseStream()).ReadToEnd()); //(List<SOFavorite>)new JsonSerializer().Deserialize(new JsonReader((TextReader)new StreamReader(response.GetResponseStream())));

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
        /// <param name="fromtime">Starting time, expressed as a double (Unix timestamp).</param>
        /// <param name="totime">Ending time, expressed as a double (Unix timestamp).</param>
        /// <param name="site">Trilogy site in question.</param>
        /// <returns>A list of RepChange's for the user.</returns>
        public static List<SORepChange> UserReputationGraph(long userid, double fromtime, double totime, TrilogySite site) { return UserReputationGraph(userid, ConvertFromUnixTimestamp(fromtime), ConvertFromUnixTimestamp(totime), site); }

        /// <summary>
        /// Obtains reputation changes for a user.
        /// </summary>
        /// <param name="userid">Id of the user.</param>
        /// <param name="fromtime">Starting time, expressed as a DateTime. Maximum of 90 days between this and ending time is allowed.</param>
        /// <param name="totime">Ending time, expressed as a DateTime. Maximum of 90 days between this and starting time is allowed.</param>
        /// <param name="site">Trilogy site in question.</param>
        /// <returns>A list of RepChange's for the user.</returns>
        public static List<SORepChange> UserReputationGraph (long userid, DateTime fromtime, DateTime totime, TrilogySite site)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format("http://{3}.com/users/rep/{0}/{1}/{2}", userid, fromtime.ToString("yyyy-MM-dd"), totime.ToString("yyyy-MM-dd"), GetSiteUrl(site)));
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            List<SORepChange> forret = JsonConvert.DeserializeObject<List<SORepChange>>(new StreamReader(response.GetResponseStream()).ReadToEnd()); //(List<RepChange>)new JsonSerializer().Deserialize(new JsonReader((TextReader)new StreamReader(response.GetResponseStream())));

            return forret;
        }
        
        /// <summary>
        /// Obtains a user's flair info.
        /// </summary>
        /// <param name="userid">Id of the user.</param>
        /// <param name="site">Trilogy site in question.</param>
        /// <returns>Flair details, expressed through an object of type UserFlair.</returns>
        public static SOUserFlair GetUserFlair(long userid, TrilogySite site)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format("http://{1}.com/users/flair/{0}.json", userid, GetSiteUrl(site)));
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            SOUserFlair forret = JsonConvert.DeserializeObject<SOUserFlair>(new StreamReader(response.GetResponseStream()).ReadToEnd()); //(UserFlair)new JsonSerializer().Deserialize(new JsonReader((TextReader)new StreamReader(response.GetResponseStream())));

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
        public static SOUserQuestions GetUserQuestions(long userid, int page, int pagesize, SortRule sort, TrilogySite site)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format("http://{4}.com/api/userquestions.json?userid={0}&page={1}&pagesize={2}&sort={3}", userid, page, pagesize, sort.ToString(), GetSiteUrl(site)));
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            SOUserQuestions forret = JsonConvert.DeserializeObject<SOUserQuestions>(new StreamReader(response.GetResponseStream()).ReadToEnd());//(UserQuestions)new JsonSerializer().Deserialize(new JsonReader((TextReader)new StreamReader(response.GetResponseStream())));

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
        public static SOUserAnswers GetUserAnswers (long userid, int page, int pagesize, SortRule sort, TrilogySite site)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format("http://{4}.com/api/useranswers.json?userid={0}&page={1}&pagesize={2}&sort={3}", userid, page, pagesize, sort.ToString(), GetSiteUrl(site)));
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            SOUserAnswers forret = JsonConvert.DeserializeObject<SOUserAnswers>(new StreamReader(response.GetResponseStream()).ReadToEnd());//(UserAnswers)new JsonSerializer().Deserialize(new JsonReader((TextReader)new StreamReader(response.GetResponseStream())));

            return forret;
        }

        #region Feeds

        /// <summary>
        /// Utilises recent question feeds to obtain recently updated questions on a certain site.
        /// </summary>
        /// <param name="site">Trilogy site in question.</param>
        /// <returns>A list of objects of type Question, which represents the recent questions on a trilogy site.</returns>
        public static List<SOQuestion> GetRecentQuestions(TrilogySite site)
        {
            List<SOQuestion> RecentQuestions = new List<SOQuestion>();
            // load the raw feed
            using (var xmlr = XmlReader.Create(string.Format("http://{0}.com/feeds", GetSiteUrl(site))))
            {
                // get the items within a feed
                var feedItems = SyndicationFeed
                                    .Load(xmlr)
                                    .GetRss20Formatter()
                                    .Feed
                                    .Items;

                // print out details about each item in the feed
                foreach (var item in feedItems)
                {
                    SOQuestion toaddquestion = new SOQuestion();
                    toaddquestion.Title = item.Title.Text;
                    toaddquestion.Author = item.Authors[0].Name;
                    toaddquestion.Id = item.Id;
                    //toaddquestion.Categories = item.Categories;
                    toaddquestion.CreatedDate = item.PublishDate.UtcDateTime;
                    toaddquestion.LastEditDate = item.LastUpdatedTime.UtcDateTime;
                    toaddquestion.Link = item.Id;
                    toaddquestion.Summary = item.Summary.Text;
                    foreach (SyndicationCategory cat in item.Categories)
                    {
                        toaddquestion.Tags+=cat.Name+",";


                    }
                    toaddquestion.Tags.Remove(toaddquestion.Tags.LastIndexOf(','));
                    
                    
                    // the extensions assume that there can be more than one value, so get
                    // the first or default value (default == 0)
                    int rank = item.ElementExtensions
                                    .ReadElementExtensions<int>("rank", "http://purl.org/atompub/rank/1.0")[0];

                    toaddquestion.VoteCount = rank;


                    RecentQuestions.Add(toaddquestion);
                    toaddquestion = null;
                }
            }
            
            return RecentQuestions;
        }

        /// <summary>
        /// Utilises recent user activity feeds to find information regarding activity of a user.
        /// </summary>
        /// <param name="userid">Userid of the user in question</param>
        /// <param name="site">Trilogy site in question</param>
        /// <returns>The user's recent activity, expressed in a list of Posts.</returns>
        public static List<SOPost> GetRecentActivity(long userid, TrilogySite site)
        {
            List<SOPost> Activity = new List<SOPost>();
            // load the raw feed
            using (var xmlr = XmlReader.Create(string.Format("http://{0}.com/feeds/user/{1}", GetSiteUrl(site), userid)))
            {
                // get the items within a feed
                var feedItems = SyndicationFeed
                                    .Load(xmlr)
                                    .GetRss20Formatter()
                                    .Feed
                                    .Items;

                // print out details about each item in the feed
                foreach (var item in feedItems)
                {
                    SOPost toadd = new SOPost();
                    toadd.Title = item.Title.Text;
                    toadd.Author = item.Authors[0].Name;
                    toadd.Id = item.Id;
                    toadd.CreatedDate = item.PublishDate.UtcDateTime;
                    toadd.LastEditDate = item.LastUpdatedTime.UtcDateTime;
                    toadd.Link = item.Id;
                    toadd.Summary = item.Summary.Text;
                    bool isThisAQuestion = false;
                    SOQuestion toaddquestion = new SOQuestion(toadd);
                    if (item.Categories.Count != 0)
                    {
                        isThisAQuestion = true;
                    }
                    foreach (SyndicationCategory cat in item.Categories)
                    {
                        toaddquestion.Tags += cat.Name + ",";
                        toaddquestion.Tags.Remove(toaddquestion.Tags.LastIndexOf(','));

                    }

                    try
                    {

                        // the extensions assume that there can be more than one value, so get
                        // the first or default value (default == 0)
                        int rank = item.ElementExtensions
                                        .ReadElementExtensions<int>("rank", "http://purl.org/atompub/rank/1.0")[0];

                        toadd.VoteCount = rank;
                    }
                    catch
                    {
                        //It's a comment! No "rank" element.
                    }
                    toaddquestion.VoteCount = toadd.VoteCount;

                    if (isThisAQuestion)
                    {
                        Activity.Add(toaddquestion);
                    }
                    else
                    {
                        Activity.Add(toadd);
                    }
                    toadd = null;
                    toaddquestion = null;
                }
            }
            
            return Activity;


        }

        /// <summary>
        /// Utilises question activity feeds to extract information about activity (posts) inside questions.
        /// </summary>
        /// <param name="numQuestion">Question ID</param>
        /// <param name="site">Trilogy site in question</param>
        /// <returns>A list of posts that are included in the activity corresponding to the question site and ID provided.</returns>
        public static List<SOPost> GetQuestionActivity(long numQuestion, TrilogySite site)
        {
            List<SOPost> Activity = new List<SOPost>();
            // load the raw feed
            using (var xmlr = XmlReader.Create(string.Format("http://{0}.com/feeds/question/{1}", GetSiteUrl(site), numQuestion)))
            {
                // get the items within a feed
                var feedItems = SyndicationFeed
                                    .Load(xmlr)
                                    .GetRss20Formatter()
                                    .Feed
                                    .Items;

                // print out details about each item in the feed
                foreach (var item in feedItems)
                {
                    SOPost toadd = new SOPost();
                    toadd.Title = item.Title.Text;
                    toadd.Author = item.Authors[0].Name;
                    toadd.Id = item.Id;
                    toadd.CreatedDate = item.PublishDate.UtcDateTime;
                    toadd.LastEditDate = item.LastUpdatedTime.UtcDateTime;
                    toadd.Link = item.Id;
                    toadd.Summary = item.Summary.Text;
                    bool isThisAQuestion = false;
                    SOQuestion toaddquestion = new SOQuestion(toadd);
                    if (item.Categories.Count != 0)
                    {
                        isThisAQuestion = true;
                    }
                    foreach (SyndicationCategory cat in item.Categories)
                    {
                        toaddquestion.Tags += cat.Name + ",";
                        toaddquestion.Tags.Remove(toaddquestion.Tags.LastIndexOf(','));

                    }



                    // the extensions assume that there can be more than one value, so get
                    // the first or default value (default == 0)
                    int rank = item.ElementExtensions
                                    .ReadElementExtensions<int>("rank", "http://purl.org/atompub/rank/1.0")[0];

                    toadd.VoteCount = rank;
                    toaddquestion.VoteCount = toadd.VoteCount;

                    if (isThisAQuestion)
                    {
                        Activity.Add(toaddquestion);
                    }
                    else
                    {
                        Activity.Add(toadd);
                    }
                    toadd = null;
                    toaddquestion = null;
                }
            }
            return Activity;


        }

        /// <summary>
        /// Utilises tag feeds to find questions in a particular tag.
        /// </summary>
        /// <param name="tagname">Name of the tag in question.</param>
        /// <param name="site">Trilogy site in question.</param>
        /// <returns>Recent questions in that tag for the site provided.</returns>
        public static List<SOQuestion> GetTagQuestions(string tagname, TrilogySite site)
        {
            List<SOQuestion> questions = new List<SOQuestion>();
            // load the raw feed
            using (var xmlr = XmlReader.Create(string.Format("http://{0}.com/feeds/tag/{1}", GetSiteUrl(site), Uri.EscapeUriString(tagname.Trim()))))
            {
                // get the items within a feed
                var feedItems = SyndicationFeed
                                    .Load(xmlr)
                                    .GetRss20Formatter()
                                    .Feed
                                    .Items;

                // print out details about each item in the feed
                foreach (var item in feedItems)
                {
                    SOQuestion toaddquestion = new SOQuestion();
                    toaddquestion.Title = item.Title.Text;
                    toaddquestion.Author = item.Authors[0].Name;
                    toaddquestion.Id = item.Id;
                    //toaddquestion.Categories = item.Categories;
                    toaddquestion.CreatedDate = item.PublishDate.UtcDateTime;
                    toaddquestion.LastEditDate = item.LastUpdatedTime.UtcDateTime;
                    toaddquestion.Link = item.Id;
                    toaddquestion.Summary = item.Summary.Text;
                    foreach (SyndicationCategory cat in item.Categories)
                    {
                        toaddquestion.Tags += cat.Name + ",";


                    }
                    toaddquestion.Tags.Remove(toaddquestion.Tags.LastIndexOf(','));


                    // the extensions assume that there can be more than one value, so get
                    // the first or default value (default == 0)
                    int rank = item.ElementExtensions
                                    .ReadElementExtensions<int>("rank", "http://purl.org/atompub/rank/1.0")[0];

                    toaddquestion.VoteCount = rank;


                    questions.Add(toaddquestion);
                    toaddquestion = null;
                }
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
                        if ((ex.Response as HttpWebResponse).StatusCode == HttpStatusCode.Forbidden)
                        {
                            throw new ApplicationException("Rate-limited. Please reduce your impact load on the Stack Overflow servers.");
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
