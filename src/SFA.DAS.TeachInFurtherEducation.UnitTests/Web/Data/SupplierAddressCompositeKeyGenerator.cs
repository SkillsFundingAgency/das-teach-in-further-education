// SupplierAddressCompositeKeyGeneratorTests.cs
using System;
using System.Security.Cryptography;
using System.Text;
using SFA.DAS.TeachInFurtherEducation.Web.Data;
using SFA.DAS.TeachInFurtherEducation.Web.Data.Interfaces;
using SFA.DAS.TeachInFurtherEducation.Web.Data.Models;
using Xunit;

namespace SFA.DAS.TeachInFurtherEducation.UnitTests.Web.Tests.Data
{
    public class SupplierAddressCompositeKeyGeneratorTests
    {
        private readonly ICompositeKeyGenerator<SupplierAddressModel> _keyGenerator;

        public SupplierAddressCompositeKeyGeneratorTests()
        {
            // Initialize the key generator
            _keyGenerator = new SupplierAddressCompositeKeyGenerator();
        }

        #region GenerateKey Tests

        [Fact]
        public void GenerateKey_WithAllPropertiesSet_ReturnsCorrectHash()
        {
            // Arrange
            var supplierAddress = new SupplierAddressModel
            {
                Type = "Headquarters",
                OrganisationName = "Acme Corporation",
                ParentOrganisation = "Acme Holdings",
                AddressLine1 = "123 Main Street",
                AddressLine2 = "Suite 100",
                AddressLine3 = "Building A",
                Area = "Downtown",
                City = "Metropolis",
                Postcode = "12345",
                Telephone = "555-1234",
                Website = "https://www.acme.com"
            };

            // Expected key string
            var expectedKeyString = "Headquarters|Acme Corporation|Acme Holdings|123 Main Street|Suite 100|Building A|Downtown|Metropolis|12345|555-1234|https://www.acme.com";

            // Compute expected MD5 hash
            string expectedHash = ComputeMd5Hash(expectedKeyString);

            // Act
            var generatedKey = _keyGenerator.GenerateKey(supplierAddress);

            // Assert
            Assert.Equal(expectedHash, generatedKey);
        }

        [Fact]
        public void GenerateKey_WithSomeEmptyProperties_ReturnsCorrectHash()
        {
            // Arrange
            var supplierAddress = new SupplierAddressModel
            {
                Type = "Branch",
                OrganisationName = "Beta LLC",
                ParentOrganisation = null, // Null
                AddressLine1 = "456 Elm Street",
                AddressLine2 = "", // Empty
                AddressLine3 = "Floor 2",
                Area = "Uptown",
                City = "Gotham",
                Postcode = "67890",
                Telephone = null, // Null
                Website = "" // Empty
            };

            // Expected key string (nulls treated as empty strings)
            var expectedKeyString = "Branch|Beta LLC||456 Elm Street||Floor 2|Uptown|Gotham|67890||";

            // Compute expected MD5 hash
            string expectedHash = ComputeMd5Hash(expectedKeyString);

            // Act
            var generatedKey = _keyGenerator.GenerateKey(supplierAddress);

            // Assert
            Assert.Equal(expectedHash, generatedKey);
        }

        [Fact]
        public void GenerateKey_WithAllPropertiesEmpty_ReturnsCorrectHash()
        {
            // Arrange
            var supplierAddress = new SupplierAddressModel
            {
                Type = "",
                OrganisationName = "",
                ParentOrganisation = "",
                AddressLine1 = "",
                AddressLine2 = "",
                AddressLine3 = "",
                Area = "",
                City = "",
                Postcode = "",
                Telephone = "",
                Website = ""
            };

            // Expected key string (all empty)
            var expectedKeyString = "||||||||||";

            // Compute expected MD5 hash
            string expectedHash = ComputeMd5Hash(expectedKeyString);

            // Act
            var generatedKey = _keyGenerator.GenerateKey(supplierAddress);

            // Assert
            Assert.Equal(expectedHash, generatedKey);
        }

        [Fact]
        public void GenerateKey_WithSpecialCharacters_ReturnsCorrectHash()
        {
            // Arrange
            var supplierAddress = new SupplierAddressModel
            {
                Type = "Regional Office",
                OrganisationName = "Gamma & Sons, Inc.",
                ParentOrganisation = "Gamma Holdings/Partners",
                AddressLine1 = "789 Oak Street, Apt #5",
                AddressLine2 = "Suite @",
                AddressLine3 = "Building *",
                Area = "Suburbia",
                City = "Star City",
                Postcode = "ABCDE",
                Telephone = "+1 (555) 678-9012",
                Website = "https://www.gamma.com?ref=home"
            };

            // Expected key string
            var expectedKeyString = "Regional Office|Gamma & Sons, Inc.|Gamma Holdings/Partners|789 Oak Street, Apt #5|Suite @|Building *|Suburbia|Star City|ABCDE|+1 (555) 678-9012|https://www.gamma.com?ref=home";

            // Compute expected MD5 hash
            string expectedHash = ComputeMd5Hash(expectedKeyString);

            // Act
            var generatedKey = _keyGenerator.GenerateKey(supplierAddress);

            // Assert
            Assert.Equal(expectedHash, generatedKey);
        }

        [Fact]
        public void GenerateKey_WithDifferentOrganisations_ReturnsDifferentHashes()
        {
            // Arrange
            var supplierAddress1 = new SupplierAddressModel
            {
                Type = "Office",
                OrganisationName = "Delta Corp",
                ParentOrganisation = "Delta Holdings",
                AddressLine1 = "321 Pine Street",
                AddressLine2 = "Suite 200",
                AddressLine3 = "",
                Area = "Midtown",
                City = "Central City",
                Postcode = "54321",
                Telephone = "555-4321",
                Website = "https://www.delta.com"
            };

            var supplierAddress2 = new SupplierAddressModel
            {
                Type = "Office",
                OrganisationName = "Epsilon Corp",
                ParentOrganisation = "Epsilon Holdings",
                AddressLine1 = "654 Cedar Street",
                AddressLine2 = "Suite 300",
                AddressLine3 = "",
                Area = "Westside",
                City = "Coast City",
                Postcode = "98765",
                Telephone = "555-5678",
                Website = "https://www.epsilon.com"
            };

            // Act
            var generatedKey1 = _keyGenerator.GenerateKey(supplierAddress1);
            var generatedKey2 = _keyGenerator.GenerateKey(supplierAddress2);

            // Assert
            Assert.NotEqual(generatedKey1, generatedKey2);
        }

        [Fact]
        public void GenerateKey_WithIdenticalInputs_ReturnsSameHash()
        {
            // Arrange
            var supplierAddress1 = new SupplierAddressModel
            {
                Type = "Headquarters",
                OrganisationName = "Zeta Enterprises",
                ParentOrganisation = "Zeta Holdings",
                AddressLine1 = "111 First Street",
                AddressLine2 = "Suite 1",
                AddressLine3 = "Building X",
                Area = "Industrial",
                City = "Blüdhaven",
                Postcode = "11223",
                Telephone = "555-0000",
                Website = "https://www.zeta.com"
            };

            var supplierAddress2 = new SupplierAddressModel
            {
                Type = "Headquarters",
                OrganisationName = "Zeta Enterprises",
                ParentOrganisation = "Zeta Holdings",
                AddressLine1 = "111 First Street",
                AddressLine2 = "Suite 1",
                AddressLine3 = "Building X",
                Area = "Industrial",
                City = "Blüdhaven",
                Postcode = "11223",
                Telephone = "555-0000",
                Website = "https://www.zeta.com"
            };

            // Act
            var generatedKey1 = _keyGenerator.GenerateKey(supplierAddress1);
            var generatedKey2 = _keyGenerator.GenerateKey(supplierAddress2);

            // Assert
            Assert.Equal(generatedKey1, generatedKey2);
        }

        [Fact]
        public void GenerateKey_WithNullSupplierAddress_ThrowsArgumentNullException()
        {
            // Arrange
            SupplierAddressModel supplierAddress = null!;

            // Act
            Action act = () => _keyGenerator.GenerateKey(supplierAddress);

            // Assert
            Assert.Throws<ArgumentNullException>("entity", act);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Computes the MD5 hash of the input string and returns it as a lowercase hexadecimal string.
        /// </summary>
        /// <param name="input">The input string to hash.</param>
        /// <returns>The MD5 hash as a lowercase hexadecimal string.</returns>
        private string ComputeMd5Hash(string input)
        {
            using (var md5 = MD5.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var hashBytes = md5.ComputeHash(bytes);
                var sb = new StringBuilder();
                foreach (var b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }

        #endregion
    }
}
