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
using SOApiDotNet;
using System.Threading;

namespace SOApiDotNetTester
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Testing SOApiDotNet.");
            Console.WriteLine("JSON");
            Console.WriteLine("Getting favorites for user 130164 on Stack Overflow...");
            List<SOFavorite> fav = StackOverflow.GetUserFavorites(130164, 0, 10, SortRule.recent, TrilogySite.SO);
            foreach (SOFavorite sofav in fav)
            {
                Console.WriteLine("Favorite: Id: {0}; FavCount: {1}; Title: {2}; Tags: {3};", sofav.Id, sofav.FavCount, sofav.Title, sofav.Tags);
                Console.WriteLine();

            }
            Console.WriteLine(); Console.WriteLine(new string('-',99)); Console.WriteLine();

            Console.WriteLine("Getting questions for user 130164 on Stack Overflow...");

            SOUserQuestions questions = StackOverflow.GetUserQuestions(130164, 0, 10, SortRule.recent, TrilogySite.SO);
            foreach (SOQuestion soquest in questions.Posts)
            {
                Console.WriteLine("Question: Id: {0}; FavCount: {1}; Title: {2}; Tags: {3};", soquest.Id, soquest.FavCount, soquest.Title, soquest.Tags);
                Console.WriteLine();

            }
            Console.WriteLine(); Console.WriteLine(new string('-', 99)); Console.WriteLine();

            Console.WriteLine("Rep graph");

            List<SORepChange> rep = StackOverflow.UserReputationGraph(130164, DateTime.Now.AddDays(-89), DateTime.Now, TrilogySite.SO);
            foreach (SORepChange change in rep)
            {
                Console.WriteLine("Change: Post: {0}; Title: {1}; Rep: {2};", change.PostUrl, change.PostTitle, change.Rep);
                Console.WriteLine();
            }

            Console.WriteLine(); Console.WriteLine(new string('-', 99)); Console.WriteLine();

            Console.WriteLine("User flair");

            SOUserFlair flair = StackOverflow.GetUserFlair(130164, TrilogySite.SO);
            Console.WriteLine("Rep: {0}; Display name: {1};", flair.reputation, flair.displayName);
            Console.WriteLine(); Console.WriteLine(new string('-', 99)); Console.WriteLine();

            Console.WriteLine("End JSON.");

            Console.WriteLine(); Console.WriteLine(new string('-', 99)); Console.WriteLine();

            Thread.Sleep(30000);
            Console.WriteLine("Userids from display name: Maxim");

            foreach (long i in StackOverflow.GetUserIdsFromUsername("maxim z", TrilogySite.SO))
            {
                Thread.Sleep(5000);
                SOUserFlair flairzzz = StackOverflow.GetUserFlair(i, TrilogySite.SO);
                Console.WriteLine("One maxim: userid: {0}; flair: rep: {1}; display name: {2}; badge html: {3}; id: {4};", i, flairzzz.reputation, flairzzz.displayName, flairzzz.badgeHtml, flairzzz.id);
            }

            Console.WriteLine(); Console.WriteLine(new string('-', 99)); Console.WriteLine();
            Console.WriteLine("RSS");

            Console.WriteLine("Getting recent questions...");
            List<SOQuestion> quests = StackOverflow.GetRecentQuestions(TrilogySite.SO);
            foreach (SOQuestion i in questions.Posts)
            {
                Console.WriteLine("Question id: {0}; question votes: {1}; first tag: {2}; second tag: {3};", i.Id, i.VoteCount, i.Tags.Split(' ')[0], i.Tags.Split(' ')[1]);
            }

            Console.WriteLine(); Console.WriteLine(new string('-', 99)); Console.WriteLine();

            Thread.Sleep(5000);
            Console.WriteLine("Getting recent activity for user 130164 on Stack Overflow...");
            List<SOPost> soposts = StackOverflow.GetRecentActivity(130164, TrilogySite.SO);
            foreach (SOPost i in soposts)
            {
                Console.WriteLine("Post: author: {0}; title: {1}; created date: {2}; id: {3};", i.Author, i.Title, i.CreatedDate.ToLocalTime().ToShortDateString(), i.Id);
            }


            Console.WriteLine(); Console.WriteLine(new string('-', 99)); Console.WriteLine();
            Thread.Sleep(5000);
            Console.WriteLine("Getting question activity for user 130164 on Stack Overflow...");
            List<SOPost> questionactivity = StackOverflow.GetQuestionActivity(1895552, TrilogySite.SO);
            foreach (SOPost i in questionactivity)
            {
                Console.WriteLine("Post: author: {0}; id: {1}; title: {2}; votes: {3};",i.Author, i.Id, i.Title, i.VoteCount);
            }

            Console.WriteLine(); Console.WriteLine(new string('-', 99)); Console.WriteLine();
            Thread.Sleep(5000);
            Console.WriteLine("Getting tag questions for tag 'C#' on Stack Overflow...");
            List<SOQuestion> tagquestions = StackOverflow.GetTagQuestions("c#", TrilogySite.SO);
            foreach (SOQuestion i in tagquestions)
            {
               Console.WriteLine("Question: Author: {0}; Title: {1}; Created date: {2}; Answer count: {3}; Favorite count: {4}; Id: {5}",i.Author, i.Title, i.CreatedDate.ToShortDateString(), i.AnswerCount, i.FavCount, i.Id);
            }

            Console.WriteLine(); Console.WriteLine(new string('-', 99)); Console.WriteLine();

            Console.WriteLine("END RSS.");

            Console.WriteLine(); Console.WriteLine(new string('-', 99)); Console.WriteLine(new string('-', 99)); Console.WriteLine();


            Console.WriteLine("Tests completed.");

            Console.ReadLine();
        }
    }
}
