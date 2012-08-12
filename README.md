**Note:** this library is heavily outdated: it was last updated in December 2009. Since then, the Stack Exchange API has moved from pre-alpha status to a mature version 2. This library works solely for the pre-alpha API. In August 2012, I moved it wholesale from its initial home at Google Code to Github for posterity.

This library utilises the continuously-advancing API functions of the Stack Overflow site trilogy. You can find out more about Stack Overflow at http://stackoverflow.com/.

This library was created by Maxim Zaslavsky and is licensed under the BSD License. Please contribute!

## Links ##

- [Archived version of project announcement on Meta Stack Overflow site](http://web.archive.org/web/20100429111205/http://meta.stackoverflow.com/questions/31200/soapidotnet-net-library-for-stack-overflow-api)
- [Initial home of the project](http://code.google.com/p/soapidotnet)
- [Official Stack Exchange API](http://api.stackexchange.com/) and [newer libraries](http://stackapps.com)

## Documentation ##

These are the main methods in the `StackOverflow` class:

- *GetQuestionActivity* - Utilises question activity feeds to extract information about activity (posts) inside questions.
- *GetRecentActivity* - Utilises recent user activity feeds to find information regarding activity of a user.
- *GetRecentQuestions* - Utilises recent question feeds to obtain recently updated questions on a certain site.
- *GetTagQuestions* - Utilises tag feeds to find questions in a particular tag.
- *GetUserAnswers* - Obtains a user's answers.
- *GetUserFavorites* - Obtains a user's favorites.
- *GetUserFlair* - Obtains a user's flair info.
- *GetUserIdsFromUsername* - Provides all userids of users that have a certain display name.
- *GetUserQuestions* - Obtains a user's questions.
- *UserReputationGraph* - Obtains reputation changes for a user.

### Sample Code ###

Below is some sample code:

```c#

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
```

For further information, see the files in the `docs` directory.