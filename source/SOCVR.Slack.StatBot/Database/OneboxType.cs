using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOCVR.Slack.StatBot.Database
{
    enum OneboxType
    {
        //[CssClassValue("ob-post")]
        Post, //ob-post

        //[CssClassValue("ob-message")]
        ChatMesssage, //ob-message

        //[CssClassValue("ob-user")]
        User, //ob-user

        //[CssClassValue("ob-image")]
        Image, //ob-image

        //[CssClassValue("ob-wikipedia")]
        Wikipedia, //ob-wikipedia

        //[CssClassValue("ob-amazon")]
        Amazon, //ob-amazon

        //[CssClassValue("ob-blog")]
        Blog, //ob-blog

        //[CssClassValue("ob-youtube")]
        Youtube, //ob-youtube

        //[CssClassValue("ob-tweet")]
        Tweet, //ob-tweet

        //[CssClassValue("ob-job")]
        JobPosting, //ob-job

        //[CssClassValue("ob-lpadbug")]
        LaunchpadBug, //ob-lpadbug

        //[CssClassValue("ob-manpage")]
        ManualPage, //ob-manpage

        //[CssClassValue("ob-xkcd")]
        XKCD, //ob-xkcd

        /* Note, there are some types of "one-boxes" that are not included here:
        * - comments from Stack Overflow / other sites
        * - identi.ca dents, whatever that is
        * - Trello, because I don't have a good example to test with
        * - Twitpic, because it seems to be dead
        * - Chat rooms and chat bookmarks, they are technicly not one-boxes
        */
    }
}
