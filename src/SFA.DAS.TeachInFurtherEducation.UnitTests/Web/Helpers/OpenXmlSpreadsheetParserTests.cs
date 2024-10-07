// OpenXmlSpreadsheetParserTests.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using NetTopologySuite.Geometries;
using SFA.DAS.TeachInFurtherEducation.Web.Helpers;
using Xunit;

namespace SFA.DAS.TeachInFurtherEducation.UnitTests.Web.Helpers
{
    public class OpenXmlSpreadsheetParserTests
    {
        private readonly OpenXmlSpreadsheetParser _parser;

        public OpenXmlSpreadsheetParserTests()
        {
            _parser = new OpenXmlSpreadsheetParser();
        }

        [Fact]
        public async Task ParseAsync_ShouldReturnListOfDictionaries_WhenSpreadsheetIsValid()
        {
            // Arrange
            var spreadsheetBytes = CreateSpreadsheet(new List<List<string>>
        {
            new List<string> { "Name", "Age", "City" },
            new List<string> { "Alice", "30", "New York" },
            new List<string> { "Bob", "25", "Los Angeles" }
        });

            // Act
            var result = await _parser.ParseAsync(spreadsheetBytes);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("Alice", result[0]["Name"]);
            Assert.Equal("30", result[0]["Age"]);
            Assert.Equal("New York", result[0]["City"]);

            Assert.Equal("Bob", result[1]["Name"]);
            Assert.Equal("25", result[1]["Age"]);
            Assert.Equal("Los Angeles", result[1]["City"]);
        }

        [Fact]
        public async Task ParseAsync_ShouldReturnEmptyList_WhenOnlyHeadersExist()
        {
            // Arrange
            var spreadsheetBytes = CreateSpreadsheet(new List<List<string>>
        {
            new List<string> { "Name", "Age", "City" }
        });

            // Act
            var result = await _parser.ParseAsync(spreadsheetBytes);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task ParseAsync_ShouldHandleMissingCells_AsEmptyStrings()
        {
            // Arrange
            var spreadsheetBytes = CreateSpreadsheet(new List<List<string>>
        {
            new List<string> { "Name", "Age", "City" },
            new List<string> { "Charlie", "28" }, // Missing "City"
            new List<string> { "Diana", "", "Chicago" } // Empty "Age"
        });

            // Act
            var result = await _parser.ParseAsync(spreadsheetBytes);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("Charlie", result[0]["Name"]);
            Assert.Equal("28", result[0]["Age"]);
            Assert.Equal(string.Empty, result[0]["City"]); // Should be empty string

            Assert.Equal("Diana", result[1]["Name"]);
            Assert.Equal(string.Empty, result[1]["Age"]); // Empty string
            Assert.Equal("Chicago", result[1]["City"]);
        }

        [Fact]
        public async Task ParseAsync_ShouldThrowException_WhenFileDataIsInvalid()
        {
            // Arrange
            var invalidBytes = new byte[] { 0x00, 0x01, 0x02 };

            // Act & Assert
            await Assert.ThrowsAnyAsync<Exception>(() => _parser.ParseAsync(invalidBytes));
        }

        [Fact]
        public async Task ParseAsync_ShouldReturnEmptyList_WhenSpreadsheetHasNoRows()
        {
            // Arrange
            var spreadsheetBytes = CreateEmptySpreadsheet();

            // Act
            var result = await _parser.ParseAsync(spreadsheetBytes);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task ParseAsync_ShouldHandleSharedStringCells_Correctly()
        {
            // Arrange
            var spreadsheetBytes = CreateSpreadsheetWithSharedStrings(new List<List<string>>
        {
            new List<string> { "Product", "Price" },
            new List<string> { "Laptop", "1200" },
            new List<string> { "Phone", "800" }
        });

            // Act
            var result = await _parser.ParseAsync(spreadsheetBytes);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("Laptop", result[0]["Product"]);
            Assert.Equal("1200", result[0]["Price"]);

            Assert.Equal("Phone", result[1]["Product"]);
            Assert.Equal("800", result[1]["Price"]);
        }

        // Helper methods to create in-memory spreadsheets
        private byte[] CreateSpreadsheet(List<List<string>> rows)
        {
            using (var mem = new MemoryStream())
            {
                using (var document = SpreadsheetDocument.Create(mem, SpreadsheetDocumentType.Workbook))
                {
                    var workbookPart = document.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    var sheets = workbookPart.Workbook.AppendChild(new Sheets());

                    var sheet = new Sheet()
                    {
                        Id = workbookPart.GetIdOfPart(worksheetPart),
                        SheetId = 1,
                        Name = "Sheet1"
                    };
                    sheets.Append(sheet);

                    var sheetData = new SheetData();
                    worksheetPart.Worksheet = new Worksheet(sheetData);

                    foreach (var rowData in rows)
                    {
                        var row = new Row();
                        sheetData.Append(row);

                        foreach (var cellValue in rowData)
                        {
                            var cell = new Cell()
                            {
                                CellValue = new CellValue(cellValue),
                                DataType = CellValues.String
                            };
                            row.Append(cell);
                        }
                    }

                    workbookPart.Workbook.Save();
                }

                return mem.ToArray();
            }
        }

        private byte[] CreateSpreadsheetWithSharedStrings(List<List<string>> rows)
        {
            using (var mem = new MemoryStream())
            {
                using (var document = SpreadsheetDocument.Create(mem, SpreadsheetDocumentType.Workbook))
                {
                    var workbookPart = document.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    var sharedStringPart = workbookPart.AddNewPart<SharedStringTablePart>();
                    var sharedStringTable = new SharedStringTable();
                    workbookPart.SharedStringTablePart.SharedStringTable = sharedStringTable;

                    var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    var sheets = workbookPart.Workbook.AppendChild(new Sheets());

                    var sheet = new Sheet()
                    {
                        Id = workbookPart.GetIdOfPart(worksheetPart),
                        SheetId = 1,
                        Name = "Sheet1"
                    };
                    sheets.Append(sheet);

                    var sheetData = new SheetData();
                    worksheetPart.Worksheet = new Worksheet(sheetData);

                    foreach (var rowData in rows)
                    {
                        var row = new Row();
                        sheetData.Append(row);

                        foreach (var cellValue in rowData)
                        {
                            int sharedStringIndex = 0;
                            if (!string.IsNullOrEmpty(cellValue))
                            {
                                var sharedStringItem = new SharedStringItem(new Text(cellValue));
                                sharedStringTable.Append(sharedStringItem);
                                sharedStringIndex = sharedStringTable.Count() - 1;
                            }

                            var cell = new Cell()
                            {
                                CellValue = new CellValue(sharedStringIndex.ToString()),
                                DataType = CellValues.SharedString
                            };
                            row.Append(cell);
                        }
                    }

                    sharedStringTable.Save();
                    workbookPart.Workbook.Save();
                }

                return mem.ToArray();
            }
        }

        private byte[] CreateEmptySpreadsheet()
        {
            using (var mem = new MemoryStream())
            {
                using (var document = SpreadsheetDocument.Create(mem, SpreadsheetDocumentType.Workbook))
                {
                    var workbookPart = document.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    var sheets = workbookPart.Workbook.AppendChild(new Sheets());

                    var sheet = new Sheet()
                    {
                        Id = workbookPart.GetIdOfPart(worksheetPart),
                        SheetId = 1,
                        Name = "Sheet1"
                    };
                    sheets.Append(sheet);

                    var sheetData = new SheetData();
                    worksheetPart.Worksheet = new Worksheet(sheetData);

                    workbookPart.Workbook.Save();
                }

                return mem.ToArray();
            }
        }
    }
}