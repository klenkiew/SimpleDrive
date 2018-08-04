using System;
using System.Collections.Generic;
using FileService.Model;
using FileService.Tests.Helpers;

namespace FileService.Tests.Fakes
{
    public class FakeGenericRepository<T> : IRepository<T> where T : class, IEntity
    {
        protected virtual string EntityName { get; } = "entity";

        protected readonly Dictionary<string, T> filesById = new Dictionary<string, T>();

        public T GetById(string id)
        {
            return filesById.GetValueOrDefault(id, null);
        }

        public void Save(T entity)
        {
            // simulate generating id from a database / autogenerating by orm
            if (entity.Id == null)
                EntityHelper.SetId(entity, Guid.NewGuid().ToString());
                
            if (filesById.ContainsKey(entity.Id))
                throw new InvalidOperationException($"A {EntityName} with same id already exists.");

            filesById.Add(entity.Id, entity);
        }

        public virtual void Save(T entity, string id)
        {
            if (filesById.ContainsKey(id))
                throw new InvalidOperationException($"A {EntityName} with same id already exists.");

            filesById.Add(id, entity);
        }

        public void Update(T entity)
        {
            if (!filesById.ContainsKey(entity.Id))
                throw new InvalidOperationException($"The {EntityName} to update doesn't exist.");

            filesById[entity.Id] = entity;
        }

        public void Delete(T entity)
        {
            if (!filesById.Remove(entity.Id))
                throw new InvalidOperationException($"The {EntityName} to delete doesn't exist");
        }
    }
}