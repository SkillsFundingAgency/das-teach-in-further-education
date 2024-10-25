using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SFA.DAS.TeachInFurtherEducation.Web.Models
{
    public class RedirectConfig
    {
        public bool AppendReferrerOnQueryString { get; set; } = false;
        public List<Trigger> Triggers { get; set; } = new List<Trigger>();
    }

    public class Trigger
    {
        public required int Seq { get; set; }
        public required string Exp { get; set; }
        public Regex? CompiledExp { get; set; }
        public List<Rule> Rules { get; set; } = new List<Rule>();
    }

    public class Rule
    {
        public required int Seq { get; set; }
        public required string Exp { get; set; }
        public Regex? CompiledExp { get; set; }

        public required string SendTo { get; set; }
    }
}
