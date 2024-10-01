// AddressHelperTests.cs
using System;
using Xunit;
using SFA.DAS.TeachInFurtherEducation.Web.Helpers;

namespace SFA.DAS.TeachInFurtherEducation.UnitTests.Web.Tests.Helpers
{
    public class AddressHelperTests
    {
        #region FormatAddress Tests

        [Fact]
        public void FormatAddress_AllInputsNull_ReturnsEmptyString()
        {
            // Arrange
            string addressLine1 = null;
            string addressLine2 = null;
            string addressLine3 = null;
            string city = null;
            string county = null;
            string postcode = null;

            // Act
            var result = AddressHelper.FormatAddress(addressLine1, addressLine2, addressLine3, city, county, postcode);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void FormatAddress_AllInputsEmpty_ReturnsEmptyString()
        {
            // Arrange
            string addressLine1 = "";
            string addressLine2 = "";
            string addressLine3 = "";
            string city = "";
            string county = "";
            string postcode = "";

            // Act
            var result = AddressHelper.FormatAddress(addressLine1, addressLine2, addressLine3, city, county, postcode);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void FormatAddress_AllInputsProvidedWithoutCommas_ReturnsJoinedString()
        {
            // Arrange
            string addressLine1 = "123 Main Street";
            string addressLine2 = "Apt 4B";
            string addressLine3 = "Building 5";
            string city = "Springfield";
            string county = "Greene";
            string postcode = "65802";

            string expected = "123 Main Street\nApt 4B\nBuilding 5\nSpringfield\nGreene\n65802";

            // Act
            var result = AddressHelper.FormatAddress(addressLine1, addressLine2, addressLine3, city, county, postcode);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void FormatAddress_AddressLinesContainCommas_ReturnsSplitAndJoinedString()
        {
            // Arrange
            string addressLine1 = "123 Main Street, Suite 100";
            string addressLine2 = "Building A, Floor 2";
            string addressLine3 = "Wing B, Room 3";
            string city = "Springfield";
            string county = "Greene";
            string postcode = "65802";

            string expected = "123 Main Street\nSuite 100\nBuilding A\nFloor 2\nWing B\nRoom 3\nSpringfield\nGreene\n65802";

            // Act
            var result = AddressHelper.FormatAddress(addressLine1, addressLine2, addressLine3, city, county, postcode);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void FormatAddress_SomeInputsNullOrEmpty_ReturnsOnlyNonEmptyComponents()
        {
            // Arrange
            string addressLine1 = "123 Main Street";
            string addressLine2 = null;
            string addressLine3 = "";
            string city = "Springfield";
            string county = null;
            string postcode = "65802";

            string expected = "123 Main Street\nSpringfield\n65802";

            // Act
            var result = AddressHelper.FormatAddress(addressLine1, addressLine2, addressLine3, city, county, postcode);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void FormatAddress_InputsContainOnlyWhitespace_ReturnsEmptyString()
        {
            // Arrange
            string addressLine1 = "   ";
            string addressLine2 = "\t";
            string addressLine3 = "\n";
            string city = "  ";
            string county = " ";
            string postcode = "   ";

            // Act
            var result = AddressHelper.FormatAddress(addressLine1, addressLine2, addressLine3, city, county, postcode);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void FormatAddress_MixedInputsWithAndWithoutCommas_ReturnsProperlyFormattedString()
        {
            // Arrange
            string addressLine1 = "123 Main Street, Suite 100";
            string addressLine2 = "Building A";
            string addressLine3 = null;
            string city = "Springfield";
            string county = "Greene";
            string postcode = "65802";

            string expected = "123 Main Street\nSuite 100\nBuilding A\nSpringfield\nGreene\n65802";

            // Act
            var result = AddressHelper.FormatAddress(addressLine1, addressLine2, addressLine3, city, county, postcode);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void FormatAddress_AddressLinesWithMultipleCommas_ReturnsAllComponentsSplitAndTrimmed()
        {
            // Arrange
            string addressLine1 = "123 Main Street, Suite 100, Floor 2";
            string addressLine2 = "Building A, Wing B";
            string addressLine3 = "Room 301, Desk 5";
            string city = "Springfield";
            string county = "Greene";
            string postcode = "65802";

            string expected = "123 Main Street\nSuite 100\nFloor 2\nBuilding A\nWing B\nRoom 301\nDesk 5\nSpringfield\nGreene\n65802";

            // Act
            var result = AddressHelper.FormatAddress(addressLine1, addressLine2, addressLine3, city, county, postcode);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void FormatAddress_SingleComponentProvided_ReturnsThatComponent()
        {
            // Arrange
            string addressLine1 = "123 Main Street";
            string addressLine2 = null;
            string addressLine3 = null;
            string city = null;
            string county = null;
            string postcode = null;

            string expected = "123 Main Street";

            // Act
            var result = AddressHelper.FormatAddress(addressLine1, addressLine2, addressLine3, city, county, postcode);

            // Assert
            Assert.Equal(expected, result);
        }

        #endregion
    }
}
