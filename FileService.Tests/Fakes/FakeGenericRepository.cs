using System;
using System.Collections.Generic;
using FileService.Model;
using FileService.Tests.Helpers;

namespace FileService.Tests.Fakes
{
    public class FakeGenericRepository<T> : IRepository<T> where T : class, IEntity
    {
        protected virtual string EntityName { get; } = "entity";

        protected readonly Dictionary<string, T> entitiesById = new Dictionary<string, T>();

        public T GetById(string id)
        {
            return entitiesById.GetValueOrDefault(id, null);
        }

        public void Save(T entity)
        {
            // simulate generating id from a database / autogenerating by orm
            if (entity.Id == null)
                EntityHelper.SetId(entity, Guid.NewGuid().ToString());
                
            if (entitiesById.ContainsKey(entity.Id))
                throw new InvalidOperationException($"A {EntityName} with the same id already exists.");

            entitiesById.Add(entity.Id, entity);
        }

        public void Update(T entity)
        {
            if (!entitiesById.ContainsKey(entity.Id))
                throw new InvalidOperationException($"The {EntityName} to update doesn't exist.");

            entitiesById[entity.Id] = entity;
        }

        public void Delete(T entity)
        {
            if (!entitiesById.Remove(entity.Id))
                throw new InvalidOperationException($"The {EntityName} to delete doesn't exist");
        }
    }
}