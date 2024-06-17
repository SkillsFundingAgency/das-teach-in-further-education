using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Web.BackgroundServices
{
    [ExcludeFromCodeCoverage]
    public class ContentUpdateServiceOptions
    {
        /// <summary>
        /// Whether automatic, timed updates are enabled
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// How often the site's content gets updated.
        /// 
        /// debugging, e.g. : "* * * * *"          once a minute
        /// at,test,test2   : "*/5 7-18 * * *"     every five minutes from 7:00 to 18:55
        /// pp,prod         : "0,30 6-23 * * *"    every half hour from 6:00 to 23:30
        /// </summary>
        /// <remarks>
        /// Warning: Contentful currently has rate throttling set at 13 hits/sec.
        /// With 2 instances per environment, at 0,30, there will be 10 hits/sec.
        /// If the throttling rate is reduced, or the number of environments/instances increases,
        /// we'd have to stagger updates across environments (which we couldn't initially do,
        /// due to the release pipeline requiring config to be the same across environments).
        /// </remarks>
        public string? CronSchedule { get; set; }
    }
}
