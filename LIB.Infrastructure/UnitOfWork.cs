using System;
using Microsoft.EntityFrameworkCore;

namespace LIB.Infrastructure
{
    public interface IUnitOfWork : IDisposable
    {
        void Commit();

        void ExecuteNonQuery(string query, object[] arrParam);
    }

    public class UnitOfWork : Disposable, IUnitOfWork
    {
        private readonly IDbFactory _factory;
        private DbContext _context;

        public UnitOfWork(IDbFactory factory)
        {
            this._factory = factory;
        }

        protected DbContext Context
        {
            get
            {
                return this._context != null ? this._context : (this._context = this._factory.Context);
            }
        }

        public void Commit()
        {
            Context.SaveChanges();
        }

        public void ExecuteNonQuery(string query, object[] arrParam)
        {
            Context.Database.ExecuteSqlRaw(query, arrParam);
        }
    }
}