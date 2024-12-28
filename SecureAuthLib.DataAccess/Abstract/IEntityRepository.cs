using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SecureAuthLib.DataAccess.Abstract
{
    public interface IEntityRepository<T> where T : class, new()
    {
        // Adds a new entity to the repository
        void Add(T entity);

        // Updates an existing entity in the repository
        void Update(T entity);

        // Deletes an entity from the repository
        void Delete(T entity);

        // Retrieves an entity by a given filter
        T Get(Expression<Func<T, bool>> filter);

        // Retrieves all entities matching a given filter
        List<T> GetAll(Expression<Func<T, bool>> filter = null);
    }
}
