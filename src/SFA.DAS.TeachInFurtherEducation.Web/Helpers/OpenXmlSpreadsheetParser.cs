using System;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using SFA.DAS.TeachInFurtherEducation.Web.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.TeachInFurtherEducation.Web.Helpers
{
    public class OpenXmlSpreadsheetParser : ISpreadsheetParser
    {
        public Task<List<Dictionary<string, string>>> ParseAsync(byte[] fileData)
        {
            return Task.Run(() =>
            {
                try
                {
                    List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
                    
                    using (var stream = new MemoryStream(fileData))
                    using (var document = SpreadsheetDocument.Open(stream, false))
                    {
                        var workbookPart = document.WorkbookPart;
                        
                        if (workbookPart == null)
                        {
                            return result;
                        }
                        
                        var sheet = workbookPart.Workbook.Sheets?.Elements<Sheet>().FirstOrDefault();
                        
                        if (sheet == null || string.IsNullOrEmpty(sheet.Id))
                        {
                            return result;
                        }
                        
                        // Safe to use sheet.Id here since we just checked it's not null or empty
                        var worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheet.Id!);
                        var rows = worksheetPart.Worksheet.Descendants<Row>().ToList();

                        if (rows.Count > 1)
                        {
                            var headers = rows[0].Elements<Cell>().Select(c => GetCellValue(c, workbookPart)).ToList();

                            for (int i = 1; i < rows.Count; i++)
                            {
                                var rowDict = new Dictionary<string, string>();
                                var row = rows[i].Elements<Cell>().ToList();

                                for (int j = 0; j < headers.Count; j++)
                                {
                                    var cellValue = j < row.Count ? GetCellValue(row[j], workbookPart) : string.Empty;
                                    rowDict[headers[j]] = cellValue;
                                }

                                result.Add(rowDict);
                            }
                        }
                    }
                    
                    return result;
                }
                catch (Exception)
                {
                    // Log the exception if you have logging configured
                    // Logger.LogError(ex, "Error parsing Excel file");
                    
                    // Return empty list instead of throwing
                    return new List<Dictionary<string, string>>();
                }
            });
        }

        private string GetCellValue(Cell cell, WorkbookPart workbookPart)
        {
            var value = cell.CellValue?.Text;

            if (cell.DataType?.Value == CellValues.SharedString)
            {
                return workbookPart!.SharedStringTablePart!.SharedStringTable
                    .Elements<SharedStringItem>().ElementAt(int.Parse(value!)).InnerText;
            }

            return value ?? string.Empty;
        }
    }
}
