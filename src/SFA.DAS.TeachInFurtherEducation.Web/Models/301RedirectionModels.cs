using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SFA.DAS.TeachInFurtherEducation.Web.Models
{
    public class Http301RedirectConfig
    {
        public bool AppendReferrerOnQueryString { get; set; } = false;
        public List<Http301RedirectTrigger> Triggers { get; set; } = new List<Http301RedirectTrigger>();
    }

    public class Http301RedirectTrigger
    {
        public required int Seq { get; set; }
        public required string Exp { get; set; }
        public Regex? CompiledExp { get; set; }
        public List<Http301RedirectRule> Rules { get; set; } = new List<Http301RedirectRule>();
    }

    public class Http301RedirectRule
    {
        public required int Seq { get; set; }
        public required string Exp { get; set; }
        public Regex? CompiledExp { get; set; }

        public required string SendTo { get; set; }
    }
}
