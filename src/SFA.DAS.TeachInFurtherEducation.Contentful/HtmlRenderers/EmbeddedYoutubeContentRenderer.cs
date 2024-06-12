using Contentful.Core.Models;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Text.RegularExpressions;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.HtmlRenderers
{
    /// <summary>
    /// A renderer for an embedded YouTube video.
    /// </summary>
    /// <remarks>
    /// Aspect ratio support inspired by bootstrap's ratio
    /// https://getbootstrap.com/docs/5.1/helpers/ratio/#about
    /// Other useful links
    /// https://stackoverflow.com/questions/14436155/how-to-create-inline-style-with-before-and-after
    /// </remarks>
    public class EmbeddedYoutubeContentRenderer : IContentRenderer
    {
        /// <summary>
        /// The order of this renderer in the collection.
        /// </summary>
        public int Order { get; set; } = 30;

        /// <summary>
        /// Whether or not this renderer supports the provided content.
        /// </summary>
        /// <param name="content">The content to evaluate.</param>
        /// <returns>Returns true if the content is a paragraph, contains only an iframe and refers to a youtube embedded url, otherwise false.</returns>
        public bool SupportsContent(IContent content)
        {
            if (!(content is Paragraph paragraph))
                return false;

            if (paragraph.Content.Count != 1 || !(paragraph.Content[0] is Text))
                return false;

            string text = ((Text)paragraph.Content[0]).Value.Trim();

            return text.StartsWith("<iframe") && text.EndsWith("</iframe>") &&
                   (text.Contains("youtube.com/embed/") || text.Contains("youtube-nocookie.com/embed/"));
        }

        //todo: this doesn't work

        /// <summary>
        /// Renders the content raw inside an html p-tag
        /// </summary>
        /// <param name="content">The content to render.</param>
        /// <returns>The p-tag as a string.</returns>
        public Task<string> RenderAsync(IContent content)
        {
            string? iframe = ((Text)((Paragraph)content).Content[0]).Value;

            string aspectRatio = AspectRatio(iframe);

            var sb = new StringBuilder();
            sb.Append($"<p class=\"govuk-body\"><div class=\"app-video-container\" style=\"--aspect-ratio: {aspectRatio}%\">");

            sb.Append(iframe.Replace("youtube.com/embed/", "youtube-nocookie.com/embed/", StringComparison.InvariantCultureIgnoreCase).Trim());

            sb.Append("</div></p>");
            return Task.FromResult(sb.ToString());
        }

        static string AspectRatio(string iframe)
        {
            const string defaultRatio = "56.25";

            var match = Regex.Match(iframe, "height\\s*=\\s*\"([\\d]+)");
            if (!match.Success)
                return defaultRatio;

            int height = int.Parse(match.Groups[1].Value);

            match = Regex.Match(iframe, "width\\s*=\\s*\"([\\d]+)");
            if (!match.Success)
                return defaultRatio;

            int width = int.Parse(match.Groups[1].Value);

            float aspectRatio = (height*100) / (float)width;
            return aspectRatio.ToString("0.00");
        }
    }
}