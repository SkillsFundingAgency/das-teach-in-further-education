using Contentful.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim
{
    [ExcludeFromCodeCoverage]
    public class Video : BaseModel, IContent
    {
        public string? Title { get; set; }

        public string? Heading { get; set; }

        public Document? VideoDescription { get; set; }

        public string? VideoSource { get; set; }

        public Asset? VideoFile { get; set; }

        public Document? VideoTranscript { get; set; }
    }
}
