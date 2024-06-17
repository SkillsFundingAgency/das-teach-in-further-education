using Microsoft.AspNetCore.Html;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.TeachInFurtherEducation.Web.Models
{
    public class PreviewModel
    {
        public static PreviewModel NotPreviewModel { get; } = new PreviewModel();

        public bool IsPreview { get; set; }
        public IEnumerable<HtmlString> PreviewErrors { get; set; }

        private PreviewModel()
        {
            IsPreview = false;
            PreviewErrors = Enumerable.Empty<HtmlString>();
        }

        public PreviewModel(IEnumerable<HtmlString> errors)
        {
            IsPreview = true;
            PreviewErrors = errors;
        }
    }
}
