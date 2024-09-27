using SFA.DAS.TeachInFurtherEducation.Web.Data.Interfaces;
using SFA.DAS.TeachInFurtherEducation.Web.Data.Models;
using System;
using System.Security.Cryptography;
using System.Text;

namespace SFA.DAS.TeachInFurtherEducation.Web.Data
{
    /// <summary>
    /// A generic implementation of <see cref="ICompositeKeyGenerator{T}"/> for generating composite keys.
    /// </summary>
    /// <typeparam name="T">The type of the entity for which the composite key is generated.</typeparam>
    public class SupplierAddressCompositeKeyGenerator : ICompositeKeyGenerator<SupplierAddressModel>
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeKeyGenerator{T}"/> class.
        /// </summary>
        /// <param name="keySelector">A function that selects the key components from the entity.</param>
        public SupplierAddressCompositeKeyGenerator()
        {
        }

        /// <summary>
        /// Generates a composite key for the given entity.
        /// </summary>
        /// <param name="entity">The entity for which the composite key is to be generated.</param>
        /// <returns>The generated composite key.</returns>
        public string GenerateKey(SupplierAddressModel entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");

            var keyString = $"{entity.Type}|{entity.OrganisationName}|{entity.ParentOrganisation}|{entity.AddressLine1}|{entity.AddressLine2}|{entity.AddressLine3}|{entity.Area}|{entity.City}|{entity.Postcode}|{entity.Telephone}|{entity.Website}";
            
#pragma warning disable S4790

            using (var md5 = MD5.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(keyString);
                var hashBytes = md5.ComputeHash(bytes);
                var hashBuilder = new StringBuilder();
                foreach (var b in hashBytes)
                {
                    hashBuilder.Append(b.ToString("x2"));
                }

                return hashBuilder.ToString();
            }

#pragma warning restore S4790
        }
    }
}
