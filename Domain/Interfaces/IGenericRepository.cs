using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IGenericRepository
    {
        public IQueryable<T> GetAll<T>() where T : class, IDataEntity;
        public T GetById<T>(int id) where T : class, IDataEntity;
        public T Create<T>(T item) where T : class, IDataEntity;

        public List<T> CreateRangeDataEntity<T>(List<T> entities) where T : class, IDataEntity;

        public List<T> UpdateDataEntity<T>(List<T> entities) where T : class, IDataEntity;

        public T Update<T>(T item) where T : class, IDataEntity;
        public void Delete<T>(int id) where T : class, IDataEntity;
        public bool IsExist<T>(int id) where T : class, IDataEntity;
        public void BeginTransaction();
        public void CommitTransaction();
        public void RollbackTransaction();
    }
}