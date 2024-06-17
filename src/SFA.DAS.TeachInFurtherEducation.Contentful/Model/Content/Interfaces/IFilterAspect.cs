
namespace SFA.DAS.TeachInFurtherEducation.Contentful.Model.Content.Interfaces
{
    public interface IFilterAspect
    {
        public string Id { get; }
        public string Description { get; }
        //todo: doesn't belong here. create new models in web that compose of content filter and selected flag
        public bool Selected { get; }
    }
}
