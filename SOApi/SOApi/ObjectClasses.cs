/*  Copyright 2009 Maxim Zaslavsky.
 * 
 *  This file is part of SOApiDotNet.

    SOApiDotNet is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    SOApiDotNet is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with SOApiDotNet.  If not, see <http://www.gnu.org/licenses/>.
 * 
 *  For more information about SOApiDotNet, please visit <http://code.google.com/p/soapidotnet/>.
 * 
 * 
 * 
 * 
 * */

using System;
using System.Collections.Generic;
using System.Text;

namespace SOApiDotNet
{
    /// <summary>
    /// Represents a post on Stack Overflow (question, answer, or comment).
    /// </summary>
    public class Post
    {
        /// <summary>
        /// Id (link)
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Number of votes.
        /// </summary>
        public double VoteCount { get; set; }
        /// <summary>
        /// Number of views.
        /// </summary>
        public double ViewCount { get; set; }
        /// <summary>
        /// Title.
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Created date of the post (expressed as a Unix timestamp)
        /// </summary>
        public string CreatedDate
        {

            get
            {
                return CreatedDate;
            }
            set
            {
                CreatedDate = value;
                dtCreatedDate = StackOverflow.ConvertFromUnixTimestamp(StackOverflow.ExtractTimestampFromJsonTime(value));

            }

        }
        /// <summary>
        /// Created date of the post (expressed as a DateTime)
        /// </summary>
        public DateTime dtCreatedDate { get; set; }
        /// <summary>
        /// Last edit date of the post (expressed as a Unix timestamp)
        /// </summary>
        public string LastEditDate
        {

            get
            {
                return LastEditDate;
            }
            set
            {
                LastEditDate = value;
                dtLastEditDate = StackOverflow.ConvertFromUnixTimestamp(StackOverflow.ExtractTimestampFromJsonTime(value));

            }

        }
        /// <summary>
        /// Last edit date of the post (expressed as a DateTime)
        /// </summary>
        public DateTime dtLastEditDate { get; set; }
        /// <summary>
        /// Author of the post.
        /// </summary>
        public string Author { get; set; }
        /// <summary>
        /// HTML of the post.
        /// </summary>
        public string Summary { get; set; }
        /// <summary>
        /// URL of the post.
        /// </summary>
        public string Link { get; set; }
        /// <summary>
        /// RSS Categories (or tags) of the post.
        /// </summary>
        public List<string> Categories { get; set; }

    }
    /// <summary>
    /// Represents a favorite question.
    /// </summary>
    public class SOFavorite : Question
    {

    }

    /// <summary>
    /// Represents a change in reputation for a user.
    /// </summary>
    public class RepChange
    {
        /// <summary>
        /// Url of the post.
        /// </summary>
        public string PostUrl { get; set; }
        /// <summary>
        /// Title of the post.
        /// </summary>
        public string PostTitle { get; set; }
        /// <summary>
        /// Rep gained through the post.
        /// </summary>
        public double RepPositive { get; set; }
        /// <summary>
        /// Rep lost through the post.
        /// </summary>
        public double RepNegative { get; set; }
    }
    /// <summary>
    /// Represents a user's flair (stats).
    /// </summary>
    public class UserFlair
    {
        /// <summary>
        /// User id
        /// </summary>
        public long id { get; set; }
        /// <summary>
        /// Gravatar image html.
        /// </summary>
        public string gravatarHtml { get; set; }
        /// <summary>
        /// Url to the user's profile
        /// </summary>
        public string profileUrl { get; set; }
        /// <summary>
        /// User's display name.
        /// </summary>
        public string displayName { get; set; }
        /// <summary>
        /// User's rep.
        /// </summary>
        public double reputation { get; set; }
        /// <summary>
        /// User's badges (html).
        /// </summary>
        public string badgeHtml { get; set; }
    }
    /// <summary>
    /// Represents a user's questions (on one page).
    /// </summary>
    public class UserQuestions
    {
        /// <summary>
        /// Page #.
        /// </summary>
        public int Page { get; set; }
        /// <summary>
        /// Total #.
        /// </summary>
        public double Total { get; set; }
        /// <summary>
        /// List of questions on this page.
        /// </summary>
        public List<Question> Posts { get; set; }



    }
    /// <summary>
    /// Represents a question.
    /// </summary>
    public class Question : Post //TODO: Have Question and Answer derive from Post
    {
 
        /// <summary>
        /// # of favorites.
        /// </summary>
        public double FavCount { get; set; }
 
        /// <summary>
        /// # of answers.
        /// </summary>
        public double AnswerCount { get; set; }
        
        /// <summary>
        /// Tags.
        /// </summary>
        public string Tags { get; set; }

    }
    /// <summary>
    /// Represents a user's answers (on one page).
    /// </summary>
    public class UserAnswers
    {
        /// <summary>
        /// Page #.
        /// </summary>
        public int Page { get; set; }
        /// <summary>
        /// Total #.
        /// </summary>
        public double Total { get; set; }
        /// <summary>
        /// List of answers on this page.
        /// </summary>
        public List<Answer> Posts { get; set; }



    }
    /// <summary>
    /// Represents an answer.
    /// </summary>
    public class Answer : Post
    {
        /// <summary>
        /// Whether this answer is the accepted one for this question or not.
        /// </summary>
        public bool AcceptedAnswer { get; set; }

    }
}
