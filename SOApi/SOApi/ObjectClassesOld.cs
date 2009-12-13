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
        public int VoteCount { get; set; }
        /// <summary>
        /// Number of views.
        /// </summary>
        public double ViewCount { get; set; }
        /// <summary>
        /// Title.
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Created date of the post (expressed as a DateTime)
        /// </summary>
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// Last edit date of the post (expressed as a DateTime)
        /// </summary>
        public DateTime LastEditDate { get; set; }
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
        /// Rep change from the post.
        /// </summary>
        public double Rep { get; set; }
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


        public string Tags
        {
            get;
            set;
        }

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
