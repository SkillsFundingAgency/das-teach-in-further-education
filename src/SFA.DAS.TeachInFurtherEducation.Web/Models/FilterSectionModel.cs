using System.Collections.Generic;

namespace SFA.DAS.TeachInFurtherEducation.Web.Models
{

    public class FilterSectionModel
    {

        public required string FilterSectionModelID { get; set; }

        public required string FilterSectionModelName { get; set; }

        public List<FilterSectionAspectModel> FilterSectionModelAspects { get; set; } = [];

    }

}
