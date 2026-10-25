using Domain.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Repository
{
    public class GenericRepository<T> : IGenericRepository where T : DbContext
    {
        private readonly T context;
        public GenericRepository(T context)
        {
            this.context = context;
        }
        public virtual IQueryable<T> GetAll<T>() where T : class, IDataEntity
        {
            return this.context.Set<T>().AsQueryable();
        }
        public T GetById<T>(int id) where T : class, IDataEntity
        {
            return this.context.Set<T>().AsNoTracking()
                     .FirstOrDefault(entity => entity.Id == id);
        }
        public T Create<T>(T item) where T : class, IDataEntity
        {
            try
            {
                var add = this.context.Set<T>().Add(item);
                this.context.SaveChanges();
                return item;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public virtual List<T> CreateRangeDataEntity<T>(List<T> entities)
          where T : class, IDataEntity
        {
            this.context.Set<T>().AddRange(entities);
            this.context.SaveChanges();
            return entities;
        }

        public virtual List<T> UpdateDataEntity<T>(List<T> entities)
            where T : class, IDataEntity
        {
            this.context.Set<T>().UpdateRange(entities);
            this.context.SaveChanges();
            return entities;
        }
        public T Update<T>(T item) where T : class, IDataEntity
        {
            try
            {
                this.context.Set<T>().Update(item);
                this.context.SaveChanges();
                return item;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void Delete<T>(int id) where T : class, IDataEntity
        {
            try
            {
                var item = context.Set<T>().FirstOrDefault(entity => entity.Id == id);
                this.context.Set<T>().Remove(item);
                this.context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool IsExist<T>(int id) where T : class, IDataEntity
        {
            return this.context.Set<T>().Any(entity => entity.Id == id);
        }
        public void BeginTransaction()
        {
            if (this.context.Database.CurrentTransaction == null)
            {
                this.context.Database.BeginTransaction();
            }
        }

        public void CommitTransaction()
        {
            if (this.context.Database.CurrentTransaction != null)
            {
                this.context.Database.CommitTransaction();
            }
        }

        public void RollbackTransaction()
        {
            if (this.context.Database.CurrentTransaction != null)
            {
                this.context.Database.RollbackTransaction();
            }
        }
    }
}
