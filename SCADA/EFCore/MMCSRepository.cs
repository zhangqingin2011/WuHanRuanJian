using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SCADA
{
    public class MMCSRepository
    {
        public MMCSRepository(MMCSDbContext context = null)
        {
            Context = context ?? new MMCSDbContext();
            Context.Database.EnsureCreated();
        }

        public MMCSDbContext Context { get; }
        public Guid? CurrentUserId { get; set; }

        private object locker = new object();

        public bool Save()
        {
            int res = 0;
            try
            {
                res = Context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                res = -1;
            }
            if (res > 0)
                return true;
            else
                return false;
        }

        public async Task<bool> SaveAsync(CancellationToken cancellationToken = default) => await Context.SaveChangesAsync(cancellationToken) > 0;

        private TEntity SetCreateValue<TEntity>(TEntity entity) where TEntity : Entity
        {
            entity.CreatorUserId = CurrentUserId;
            entity.CreationTime = DateTime.Now;
            entity.LastModifierUserId = null;
            entity.LastModificationTime = null;
            entity.IsDeleted = false;
            entity.DeleterUserId = null;
            entity.DeletionTime = null;
            return entity;
        }

        public bool Insert<TEntity>(TEntity entity) where TEntity : Entity
        {
            bool res = false;
            lock (locker)
            {
                try
                {
                    Context.Set<TEntity>().Add(SetCreateValue(entity));
                    res = Save();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            return res;
        }

        public bool InsertRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : Entity
        {
            bool res = false;
            lock (locker)
            {
                try
                {
                    var list = entities.Select(e => SetCreateValue(e));
                    Context.Set<TEntity>().AddRange(list);
                    res = Save();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            return res;
        }

        public bool InsertParams<TEntity>(params TEntity[] entities) where TEntity : Entity => InsertRange(entities.AsEnumerable());

        public async Task<bool> InsertAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : Entity
        {
            await Context.Set<TEntity>().AddAsync(SetCreateValue(entity), cancellationToken);
            return await SaveAsync(cancellationToken);
        }

        public async Task<bool> InsertRangeAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) where TEntity : Entity
        {
            var list = entities.Select(entity => SetCreateValue(entity));
            await Context.Set<TEntity>().AddRangeAsync(list, cancellationToken);
            return await SaveAsync(cancellationToken);
        }

        public async Task<bool> InsertParamsAsync<TEntity>(params TEntity[] entities) where TEntity : Entity => await InsertRangeAsync(entities.AsEnumerable());

        private TEntity SetModifyValue<TEntity>(TEntity entity) where TEntity : Entity
        {
            entity.LastModifierUserId = CurrentUserId;
            entity.LastModificationTime = DateTime.Now;
            return entity;
        }

        public bool Update<TEntity>(TEntity entity) where TEntity : Entity
        {
            bool res = false;
            lock (locker)
            {
                try
                {
                    Context.Set<TEntity>().Update(SetModifyValue(entity));
                    res = Save();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            return res;
        }

        public bool UpdateRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : Entity
        {
            bool res = false;
            lock (locker)
            {
                try
                {
                    Context.Set<TEntity>().UpdateRange(entities.Select(e => SetModifyValue(e)));
                    res = Save();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            return res;
        }

        public bool UpdateParams<TEntity>(params TEntity[] entities) where TEntity : Entity => UpdateRange(entities.AsEnumerable());

        private TEntity SetDeleteValue<TEntity>(TEntity entity) where TEntity : Entity
        {
            entity.IsDeleted = true;
            entity.DeleterUserId = CurrentUserId;
            entity.DeletionTime = DateTime.Now;
            return entity;
        }

        public bool Delete<TEntity>(TEntity entity) where TEntity : Entity
        {
            bool res = false;
            lock (locker)
            {
                try
                {
                    Context.Set<TEntity>().Update(SetDeleteValue(entity));
                    res = Save();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            return res;
        }

        public bool RealDelete<TEntity>(TEntity entity) where TEntity : Entity
        {
            bool res = false;
            lock (locker)
            {
                try
                {
                    Context.Set<TEntity>().Remove(entity);
                    res = Save();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            return res;
        }

        public bool DeleteRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : Entity
        {
            bool res = false;
            lock (locker)
            {
                try
                {
                    Context.Set<TEntity>().UpdateRange(entities.Select(e => SetDeleteValue(e)));
                    res = Save();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            return res;
        }

        public bool DeleteParams<TEntity>(params TEntity[] entities) where TEntity : Entity => DeleteRange(entities.AsEnumerable());

        public bool Delete<TEntity>(Guid id) where TEntity : Entity => Delete<TEntity>(p => p.Id == id);

        public bool Delete<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : Entity
        {
            bool res = false;
            lock (locker)
            {
                try
                {
                    if (where != null)
                    {
                        Context.Set<TEntity>().UpdateRange(Context.Set<TEntity>().Where(where).Select(e => SetDeleteValue(e)));
                    }
                    res = Save();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            return res;
        }

        public bool EmptyData<TEntity>() where TEntity : Entity
        {
            bool res = false;
            lock (locker)
            {
                try
                {
                    Context.Set<TEntity>().RemoveRange(Context.Set<TEntity>());
                    res = Save();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            return res;
        }

        public int Count<TEntity>(string where = null) where TEntity : Entity => Get<TEntity>(where).Count;

        public int Count<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : Entity => Get(where).Count;

        public long LongCount<TEntity>(string where = null) where TEntity : Entity => Get<TEntity>(where).LongCount();

        public long LongCount<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : Entity => Get(where).LongCount();

        public bool Exist<TEntity>(Guid id) where TEntity : Entity => Exist<TEntity>(p => p.Id == id);

        public bool Exist<TEntity>(string where = null) where TEntity : Entity => LongCount<TEntity>(where) > 0;

        public bool Exist<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : Entity => LongCount(where) > 0;

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="where">条件中不可以使用导航属性</param>
        /// <param name="orderBy"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public ICollection<TEntity> Get<TEntity>(
            string where = null,
            string orderBy = null,
            int pageSize = 0,
            int pageIndex = 1) where TEntity : Entity
        {
            lock (locker)
            {
                try
                {
                    IQueryable<TEntity> set = Context.Set<TEntity>().Where(p => !p.IsDeleted);
                    if (!string.IsNullOrWhiteSpace(where))
                    {
                        set = set.Where(where);
                    }
                    if (!string.IsNullOrWhiteSpace(orderBy))
                    {
                        set = set.OrderBy(orderBy);
                    }
                    if (pageSize != 0)
                    {
                        set = set.Skip((pageIndex - 1) * pageSize).Take(pageSize);
                    }
                    return set.ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return null;
        }
       
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="where">条件中不可以使用导航属性</param>
        /// <param name="orderBy"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public ICollection<TEntity> GetDelet<TEntity>(
            string where = null,
            string orderBy = null,
            int pageSize = 0,
            int pageIndex = 1) where TEntity : Entity
        {
            lock (locker)
            {
                try
                {
                    IQueryable<TEntity> set = Context.Set<TEntity>();
                    if (!string.IsNullOrWhiteSpace(where))
                    {
                        set = set.Where(where);
                    }
                    if (!string.IsNullOrWhiteSpace(orderBy))
                    {
                        set = set.OrderBy(orderBy);
                    }
                    if (pageSize != 0)
                    {
                        set = set.Skip((pageIndex - 1) * pageSize).Take(pageSize);
                    }
                    return set.ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return null;
        }
        public ICollection<TEntity> GetOrderByASC<TEntity>(
            string orderBy = "OrderNO",
            int pageSize = 0,
            int pageIndex = 1) where TEntity : Entity
        {
            lock (locker)
            {
                try
                {
                    IQueryable<TEntity> set = Context.Set<TEntity>().Where(p => !p.IsDeleted);

                    set = set.OrderBy(orderBy);
                    if (pageSize != 0)
                    {
                        set = set.Skip((pageIndex - 1) * pageSize).Take(pageSize);
                    }
                    return set.ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return null;
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="where">条件中不可以使用导航属性</param>
        /// <param name="orderBy"></param>
        /// <param name="isAsc"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public ICollection<TEntity> Get<TEntity>(
            Expression<Func<TEntity, bool>> where,
            string orderBy = null,
            int pageSize = 0,
            int pageIndex = 1) where TEntity : Entity
        {
            lock (locker)
            {
                try
                {
                    IQueryable<TEntity> set = Context.Set<TEntity>().Where(p => !p.IsDeleted);
                    if (where != null)
                    {
                        set = set.Where(where);
                    }
                    if (!string.IsNullOrWhiteSpace(orderBy))
                    {
                        set = set.OrderBy(orderBy);
                    }
                    if (pageSize != 0)
                    {
                        set = set.Skip((pageIndex - 1) * pageSize).Take(pageSize);
                    }
                    return set.ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return null;
        }
        // <summary>
        /// 获取数据
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="where">条件中不可以使用导航属性</param>
        /// <param name="orderBy"></param>
        /// <param name="isAsc"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public ICollection<TEntity> GetDelet<TEntity>(
            Expression<Func<TEntity, bool>> where,
            string orderBy = null,
            int pageSize = 0,
            int pageIndex = 1) where TEntity : Entity
        {
            lock (locker)
            {
                try
                {
                    IQueryable<TEntity> set = Context.Set<TEntity>();
                    if (where != null)
                    {
                        set = set.Where(where);
                    }
                    if (!string.IsNullOrWhiteSpace(orderBy))
                    {
                        set = set.OrderBy(orderBy);
                    }
                    if (pageSize != 0)
                    {
                        set = set.Skip((pageIndex - 1) * pageSize).Take(pageSize);
                    }
                    return set.ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return null;
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="where">条件中不可以使用导航属性</param>
        /// <param name="orderBy"></param>
        /// <param name="isAsc"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public ICollection<TEntity> Get<TEntity, TKey>(
            Expression<Func<TEntity, bool>> where = null,
            Expression<Func<TEntity, TKey>> orderBy = null,
            bool isAsc = true,
            int pageSize = 0,
            int pageIndex = 1) where TEntity : Entity
        {
            IQueryable<TEntity> set = Context.Set<TEntity>().Where(p => !p.IsDeleted);
            if (where != null)
            {
                set = set.Where(where);
            }
            if (orderBy != null)
            {
                set = isAsc ? set.OrderBy(orderBy) : set.OrderByDescending(orderBy);
            }
            if (pageSize != 0)
            {
                set = set.Skip((pageIndex - 1) * pageSize).Take(pageSize);
            }
            return set.ToList();
        }

        /// <summary>
        /// 获取一条数据
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public TEntity GetSingle<TEntity>(Guid id) where TEntity : Entity => GetSingle<TEntity>(p => p.Id == id);

        /// <summary>
        /// 获取一条数据
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="predicate">条件中可以使用导航属性</param>
        /// <returns></returns>
        public TEntity GetSingle<TEntity>(Func<TEntity, bool> predicate = null) where TEntity : Entity => predicate == null ? Get<TEntity>().FirstOrDefault() : Get<TEntity>().FirstOrDefault(predicate);

        /// <summary>
        /// 获取一条数据
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="predicate">条件中可以使用导航属性</param>
        /// <returns></returns>
        public TEntity GetSingleDelet<TEntity>(Func<TEntity, bool> predicate = null) where TEntity : Entity => predicate == null ? GetDelet<TEntity>().FirstOrDefault() : GetDelet<TEntity>().FirstOrDefault(predicate);

        #region SQL

        public int ExecuteSqlCommand(FormattableString sql) => Context.Database.ExecuteSqlCommand(sql);

        public int ExecuteSqlCommand(RawSqlString sql, IEnumerable<object> parameters) => Context.Database.ExecuteSqlCommand(sql, parameters);

        public int ExecuteSqlCommand(RawSqlString sql, params object[] parameters) => Context.Database.ExecuteSqlCommand(sql, parameters);

        public async Task<int> ExecuteSqlCommandAsync(FormattableString sql, CancellationToken cancellationToken = default) => await Context.Database.ExecuteSqlCommandAsync(sql, cancellationToken);

        public async Task<int> ExecuteSqlCommandAsync(RawSqlString sql, IEnumerable<object> parameters, CancellationToken cancellationToken = default) => await Context.Database.ExecuteSqlCommandAsync(sql, parameters, cancellationToken);

        public async Task<int> ExecuteSqlCommandAsync(RawSqlString sql, params object[] parameters) => await Context.Database.ExecuteSqlCommandAsync(sql, parameters);

        public IQueryable<TEntity> FromSql<TEntity>(FormattableString sql) where TEntity : Entity => Context.Set<TEntity>().AsNoTracking().FromSql(sql);

        public IQueryable<TEntity> FromSql<TEntity>(RawSqlString sql, params object[] parameters) where TEntity : Entity => Context.Set<TEntity>().AsNoTracking().FromSql(sql, parameters);

        #endregion
    }
}
