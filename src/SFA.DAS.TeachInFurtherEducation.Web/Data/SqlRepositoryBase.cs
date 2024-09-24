using Microsoft.EntityFrameworkCore;
using SFA.DAS.TeachInFurtherEducation.Web.Data.Interfaces;
using System.Collections.Generic;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.TeachInFurtherEducation.Web.Data.Models;

namespace SFA.DAS.TeachInFurtherEducation.Web.Data
{
    /// <summary>
    /// Provides a base implementation for repository operations using SQL Server and Entity Framework Core.
    /// </summary>
    /// <typeparam name="T">The type of the entity being managed by the repository.</typeparam>
    public abstract class SqlRepositoryBase<T> : IRepository<T> where T : ModelBase
    {
        protected readonly SqlDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlRepositoryBase{T}"/> class.
        /// </summary>
        /// <param name="context">The Entity Framework DbContext instance used to interact with SQL Server.</param>
        protected SqlRepositoryBase(SqlDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public virtual async Task<T?> GetById(string id, string? partitionKey = null)
        {
            return await _context!.Set<T>().FindAsync(id);
        }

        /// <inheritdoc />
        public virtual async Task AddOrUpdate(T item, string? partitionKey = null)
        {
            try
            {
                // Check if the entity already exists in the database
                var entityInDb = await _context.Set<T>().FindAsync(item.Id);
                if (entityInDb == null)
                {
                    await _context.Set<T>().AddAsync(item);
                }
                else
                {
                    _context.Entry(entityInDb).CurrentValues.SetValues(item); // Update existing entity
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <inheritdoc />
        public virtual async Task DeleteById(string id, string? partitionKey = null)
        {
            var entity = await GetById(id);
            if (entity != null)
            {
                _context.Set<T>().Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
