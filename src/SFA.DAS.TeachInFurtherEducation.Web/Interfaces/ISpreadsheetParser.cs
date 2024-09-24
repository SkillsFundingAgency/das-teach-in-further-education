using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.TeachInFurtherEducation.Web.Interfaces
{
    /// <summary>
    /// Defines methods for parsing spreadsheet data.
    /// </summary>
    public interface ISpreadsheetParser
    {
        /// <summary>
        /// Parses a byte array representing a spreadsheet into a list of dictionaries.
        /// Each dictionary represents a row, with column names as keys.
        /// </summary>
        /// <param name="byteArray">The byte array of the spreadsheet.</param>
        /// <returns>A list of dictionaries representing the rows in the spreadsheet.</returns>
        Task<List<Dictionary<string, string>>> ParseAsync(byte[] fileData);
    }

}
