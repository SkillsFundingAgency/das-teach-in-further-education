using SFA.DAS.TeachInFurtherEducation.Web.Data.Models;

namespace SFA.DAS.TeachInFurtherEducation.Web.Data.Interfaces
{
    /// <summary>
    /// Interface for generating composite keys for entities.
    /// </summary>
    /// <typeparam name="T">The type of the entity for which the composite key is generated.</typeparam>
    public interface ICompositeKeyGenerator<in T> where T : ModelBase
    {
        /// <summary>
        /// Generates a composite key for the given entity.
        /// </summary>
        /// <param name="entity">The entity for which the composite key is to be generated.</param>
        /// <returns>The generated composite key.</returns>
        string GenerateKey(T entity);
    }
}
