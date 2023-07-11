using DAL.DataAccess;
using Microsoft.EntityFrameworkCore;
using System;

namespace LIB.Infrastructure
{

    public interface IDbFactory : IDisposable
    {
        DbContext Context { get; }
    }

    public class DbFactory : Disposable, IDbFactory
    {
        private DbContext _context;

        public DbContext Context
        {
            get
            {
                return this._context != null ? this._context : (this._context = new ACSDbContext());
            }
        }
    }
}