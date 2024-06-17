using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim
{

    [ExcludeFromCodeCoverage]
    public class MenuItem
    {

        public required string MenuItemTitle { get; set; }

        public required string MenuItemText { get; set; }

        public required string MenuItemSource { get; set; }

        public required int MenuItemOrder { get; set; }

    }

}
