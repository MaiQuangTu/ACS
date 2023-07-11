
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace LIB.Infrastructure
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> Get();

        IQueryable<T> Get(string value);

        IQueryable<T> Get(string value, string order);

        IQueryable<T> Get(string value, string order, out int total);

        IQueryable<T> GetByCodition(Expression<Func<T, bool>> expression);

        IQueryable<T> GetByQuery(string query, object[] arrParam);

        T FindById(object id);

        T FindByCodition(Expression<Func<T, bool>> expression);

        T FindByQuery(string query, object[] arrParam);

        T Add(T entity);

        T Update(T entity);

        T Remove(T entity);

        T Remove(object id);

        void AddRange(IEnumerable<T> entities);

        void RemoveRange(IEnumerable<T> entities);

        void RemoveRange(Expression<Func<T, bool>> expression);
    }

    public abstract class Repository<T> : IRepository<T> where T : class
    {
        private readonly DbSet<T> _dbSet;
        private readonly IDbFactory _factory;
        private DbContext _context;

        protected Repository(IDbFactory factory)
        {
            this._factory = factory;
            this._dbSet = Context.Set<T>();
        }

        protected DbContext Context
        {
            get
            {
                return this._context != null ? this._context : (this._context = this._factory.Context);
            }
        }

        public virtual IQueryable<T> Get()
        {
            return this._dbSet;
        }

        public virtual IQueryable<T> Get(string value)
        {
            if (string.IsNullOrEmpty(value))
                return this._dbSet;

            IEnumerable<string> searchFields = GetSearchField();
            if (!searchFields.Any())
                return this._dbSet;

            string whereCodition = string.Join(" or ", searchFields.Select(x => $"{x}.Contains(@0)"));

            return this._dbSet.Where(whereCodition, value);
        }

        public IQueryable<T> Get(string value, string order)
        {
            IEnumerable<string> fields = GetField();
            string sortExpression = string.IsNullOrEmpty(order) ? fields.First() : order.Replace("_desc", " desc");

            if (string.IsNullOrEmpty(value))
                return this._dbSet.OrderBy(sortExpression);

            IEnumerable<string> searchFields = GetSearchField();
            if (!searchFields.Any())
                return this._dbSet.OrderBy(sortExpression);

            string whereCodition = string.Join(" or ", searchFields.Select(x => $"{x}.Contains(@0)"));

            return this._dbSet.Where(whereCodition, value).OrderBy(sortExpression);
        }

        public IQueryable<T> Get(string value, string order, out int total)
        {
            IEnumerable<string> fields = GetField();
            string sortExpression = string.IsNullOrEmpty(order) ? fields.First() : order.Replace("_desc", " desc");

            if (string.IsNullOrEmpty(value))
            {
                total = this._dbSet.Count();

                return this._dbSet.OrderBy(sortExpression);
            }

            IEnumerable<string> searchFields = GetSearchField();
            if (!searchFields.Any())
            {
                total = this._dbSet.Count();

                return this._dbSet.OrderBy(sortExpression);
            }

            string whereCodition = string.Join(" or ", searchFields.Select(x => $"{x}.Contains(@0)"));

            total = this._dbSet.Where(whereCodition, value).Count();

            return this._dbSet.Where(whereCodition, value).OrderBy(sortExpression);
        }

        public virtual IQueryable<T> GetByCodition(Expression<Func<T, bool>> expression)
        {
            return this._dbSet.Where(expression);
        }

        public virtual IQueryable<T> GetByQuery(string query, object[] arrParam)
        {
            return this._dbSet.FromSqlRaw(query, arrParam);
        }

        public virtual T FindById(object id)
        {
            return this._dbSet.Find(id);
        }

        public virtual T FindByCodition(Expression<Func<T, bool>> expression)
        {
            return this._dbSet.Where(expression).FirstOrDefault();
        }

        public virtual T FindByQuery(string query, object[] arrParam)
        {
            return this._dbSet.FromSqlRaw(query, arrParam).FirstOrDefault();
        }

        public virtual T Add(T entity)
        {
            return this._dbSet.Add(entity).Entity;
        }

        public virtual T Update(T entity)
        {
            return this._dbSet.Update(entity).Entity;
        }

        public virtual T Remove(T entity)
        {
            return this._dbSet.Remove(entity).Entity;
        }

        public virtual T Remove(object id)
        {
            return this._dbSet.Remove(this._dbSet.Find(id)).Entity;
        }

        public virtual void AddRange(IEnumerable<T> entities)
        {
            this._dbSet.AddRange(entities);
        }

        public virtual void RemoveRange(IEnumerable<T> entities)
        {
            this._dbSet.RemoveRange(entities);
        }

        public virtual void RemoveRange(Expression<Func<T, bool>> expression)
        {
            this._dbSet.RemoveRange(this._dbSet.Where(expression));
        }

        private static IEnumerable<string> GetField()
        {
            return typeof(T).GetProperties().Select(x => x.Name.Trim().ToLower()).ToList();
        }

        private static IEnumerable<string> GetSearchField()
        {
            return typeof(T).GetProperties().Where(x => x.PropertyType == typeof(string)).Select(x => x.Name.Trim().ToLower()).ToList();
        }
    }
}