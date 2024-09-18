// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using StarStocks.Core.Models;
using StarStocks.Core.Interfaces;
using Dapper;

namespace StarStocks.Core.Repositories
{
    public class AbstractRepository<TEntity> : IRepository<TEntity> where TEntity : BaseModel
    {
        protected IUnitOfWork _unit { get; }

        protected IDbConnection _conn { get; }

        public AbstractRepository(IUnitOfWork unit)
        {
            _unit = unit;

            _conn = unit.Connection;
        }

        #region Interface Implement
        public IEnumerable<TEntity> GetAll()
        {
            return _conn.GetList<TEntity>();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _conn.GetListAsync<TEntity>();
        }

        public TEntity FindById(int id)
        {
            return _conn.Get<TEntity>(id);
        }

        public async Task<TEntity> FindByIdAsync(int id)
        {
            return await _conn.GetAsync<TEntity>(id);
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            var allEntities = GetAll();

            return allEntities.Where(predicate.Compile()).ToList();
        }

        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var allEntities = await GetAllAsync();

            return allEntities.Where(predicate.Compile()).ToList();
        }

        public IEnumerable<TEntity> QueryBySql(string sql, DynamicParameters dps = null)
        {
            if (dps == null)
            {
                return _conn.Query<TEntity>(sql);
            }

            return _conn.Query<TEntity>(sql, dps);
        }

        public async Task<IEnumerable<TEntity>> QueryBySqlAsync(string sql, DynamicParameters dps = null)
        {
            if (dps == null)
            {
                return await _conn.QueryAsync<TEntity>(sql);
            }

            return await _conn.QueryAsync<TEntity>(sql, dps);
        }

        public TEntity QuerySingle(string sql, DynamicParameters dps = null)
        {
            if (dps == null)
            {
                return _conn.QuerySingleOrDefault<TEntity>(sql);
            }

            return _conn.QuerySingleOrDefault<TEntity>(sql, dps);
        }

        public async Task<TEntity> QuerySingleAsync(string sql, DynamicParameters dps = null)
        {
            if (dps == null)
            {
                return await _conn.QuerySingleOrDefaultAsync<TEntity>(sql);
            }

            return await _conn.QuerySingleOrDefaultAsync<TEntity>(sql, dps);
        }

        public object QuerySingleValue(string sql, DynamicParameters dps = null)
        {
            if (dps == null)
            {
                return _conn.ExecuteScalar(sql);
            }

            return _conn.ExecuteScalar(sql, dps);
        }

        public async Task<object> QuerySingleValueAsync(string sql, DynamicParameters dps = null)
        {
            if (dps == null)
            {
                return await _conn.ExecuteScalarAsync(sql);
            }

            return await _conn.ExecuteScalarAsync(sql, dps);
        }

        public IEnumerable<object> QueryListValue(string sql, DynamicParameters dps = null)
        {
            if (dps == null)
            {
                return _conn.Query<object>(sql);
            }

            return _conn.Query<object>(sql, dps);
        }

        public async Task<IEnumerable<object>> QueryListValueAsync(string sql, DynamicParameters dps = null)
        {
            if (dps == null)
            {
                return await _conn.QueryAsync<object>(sql);
            }

            return await _conn.QueryAsync<object>(sql, dps);
        }

        public IEnumerable<dynamic> QueryDynamicObject(string sql, DynamicParameters dps = null)
        {
            if (dps == null)
            {
                return _conn.Query<dynamic>(sql);
            }

            return _conn.Query<dynamic>(sql, dps);
        }

        public async Task<IEnumerable<object>> QueryDynamicObjectAsync(string sql, DynamicParameters dps = null)
        {
            if (dps == null)
            {
                return await _conn.QueryAsync<dynamic>(sql);
            }

            return await _conn.QueryAsync<dynamic>(sql, dps);
        }


        public int? Add(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            entity.Id = default(int);
            entity.Id = _conn.Insert(entity) ?? default(int);

            return entity.Id;
        }

        public async Task<int?> AddAsync(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            entity.Id = default(int);
            entity.Id = await _conn.InsertAsync(entity) ?? default(int);

            return entity.Id;
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                Add(entity);
            }
        }

        public async Task<bool> AddRangeAsync(IEnumerable<TEntity> entities)
        {
            bool isSuccess = true;

            foreach (var entity in entities)
            {
                int? rr = await AddAsync(entity);

                if (rr.GetValueOrDefault() < 1) isSuccess = false;
            }

            return isSuccess;
        }

        public int? RemoveById(int id)
        {
            if (id < 1) throw new ArgumentNullException(nameof(id));

            int? result = 0;

            result = _conn.Delete<TEntity>(id);

            return result;
        }

        public int? Remove(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            return RemoveById(entity.Id);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                Remove(entity);
            }
        }

        public int? Update(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            int? result = 0;

            result = _conn.Update(entity);

            return result;
        }

        public async Task<int?> UpdateAsync(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            int? result = 0;

            result = await _conn.UpdateAsync(entity);

            return result;
        }

        public DataTable ReturnDataTableFromSql(string sqlText)
        {
            if (string.IsNullOrEmpty(sqlText)) throw new ArgumentNullException(nameof(sqlText));

            var dt = new DataTable();

            using (var dbCommand = _conn.CreateCommand())
            {
                dbCommand.CommandText = sqlText;
                dbCommand.CommandType = CommandType.Text;

                using (var reader = dbCommand.ExecuteReader())
                {
                    dt.Load(reader);
                }
            }

            return dt;
        }

        #endregion

        /// <summary>
        /// 取TEntity的TableName
        /// </summary>
        protected string GetTableNameMapper()
        {
            dynamic attributeTable = typeof(TEntity).GetCustomAttributes(false)
                .SingleOrDefault(attr => attr.GetType().Name == "TableAttribute");

            return attributeTable != null ? attributeTable.Name : typeof(TEntity).Name;
        }
    }
}
