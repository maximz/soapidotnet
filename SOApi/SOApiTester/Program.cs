using System;
using System.Collections.Generic;
using System.Text;
using SOApiDotNet;

namespace SOApiDotNetTester
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Testing SOApiDotNet.");
            Console.WriteLine("Getting favorites for user 130164 on Stack Overflow...");
            List<SOFavorite> fav = StackOverflow.GetUserFavorites(130164, 0, 10, SortRule.recent, TrilogySite.SO);
            foreach (SOFavorite sofav in fav)
            {
                Console.WriteLine("Favorite: Id: {0}; FavCount: {1}; Title: {2}; Tags: {3};", sofav.Id, sofav.FavCount, sofav.Title, sofav.Tags);
                Console.WriteLine();

            }
            Console.WriteLine(); Console.WriteLine(new string('-',99)); Console.WriteLine();

            Console.WriteLine("Getting questions for user 130164 on Stack Overflow...");

            UserQuestions questions = StackOverflow.GetUserQuestions(130164, 0, 10, SortRule.recent, TrilogySite.SO);
            foreach (Question soquest in questions.Posts)
            {
                Console.WriteLine("Question: Id: {0}; FavCount: {1}; Title: {2}; Tags: {3};", soquest.Id, soquest.FavCount, soquest.Title, soquest.Tags);
                Console.WriteLine();

            }
            Console.WriteLine(); Console.WriteLine(new string('-', 99)); Console.WriteLine();

            Console.WriteLine("Rep graph");

            List<RepChange> rep = StackOverflow.UserReputationGraph(130164, DateTime.Now.AddDays(-90), DateTime.Now, TrilogySite.SO);
            foreach (RepChange change in rep)
            {
                Console.WriteLine("Change: Post: {0}; Title: {1}; Positive rep: {2}; Negative rep: {3};", change.PostUrl, change.PostTitle, change.RepPositive, change.RepNegative);
                Console.WriteLine();
            }

            Console.WriteLine(); Console.WriteLine(new string('-', 99)); Console.WriteLine();

            Console.WriteLine("User flair");

            UserFlair flair = StackOverflow.GetUserFlair(130164, TrilogySite.SO);
            Console.WriteLine("Rep: {0}; Display name: {1};", flair.reputation, flair.displayName);
            Console.WriteLine(); Console.WriteLine(new string('-', 99)); Console.WriteLine();


            Console.ReadLine();
        }
    }
}
