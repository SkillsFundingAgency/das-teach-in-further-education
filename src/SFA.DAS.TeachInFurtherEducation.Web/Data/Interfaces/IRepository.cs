using System.Threading.Tasks;

namespace SFA.DAS.TeachInFurtherEducation.Web.Data.Interfaces
{
    /// <summary>
    /// Defines basic CRUD operations for a repository.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    public interface IRepository<T>
    {
        /// <summary>
        /// Retrieves an item by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the item.</param>
        /// <returns>The item if found; otherwise, null.</returns>
        Task<T?> GetById(string id, string? partitionKey = null);

        /// <summary>
        /// Adds a new item or updates an existing item in the repository.
        /// </summary>
        /// <param name="item">The item to add or update.</param>
        /// <param name="partitionKey">The key used to partition the documents in the container</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AddOrUpdate(T item, string? partitionKey = null);

        /// <summary>
        /// Deletes an item by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the item.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DeleteById(string id, string? partitionKey = null);
    }
}
