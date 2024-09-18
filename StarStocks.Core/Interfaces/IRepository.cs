// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Dapper;
using StarStocks.Core.Models;

namespace StarStocks.Core.Interfaces
{
    public interface IRepository<TEntity> where TEntity : BaseModel
    {
        TEntity FindById(int id);

        Task<TEntity> FindByIdAsync(int id);

        IEnumerable<TEntity> GetAll();

        Task<IEnumerable<TEntity>> GetAllAsync();

        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);

        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);

        IEnumerable<TEntity> QueryBySql(string sql, DynamicParameters dps = null);

        Task<IEnumerable<TEntity>> QueryBySqlAsync(string sql, DynamicParameters dps = null);

        TEntity QuerySingle(string sql, DynamicParameters dps = null);

        Task<TEntity> QuerySingleAsync(string sql, DynamicParameters dps = null);

        object QuerySingleValue(string sql, DynamicParameters dps = null);

        Task<object> QuerySingleValueAsync(string sql, DynamicParameters dps = null);

        IEnumerable<object> QueryListValue(string sql, DynamicParameters dps = null);

        Task<IEnumerable<object>> QueryListValueAsync(string sql, DynamicParameters dps = null);

        IEnumerable<dynamic> QueryDynamicObject(string sql, DynamicParameters dps = null);

        Task<IEnumerable<dynamic>> QueryDynamicObjectAsync(string sql, DynamicParameters dps = null);

        int? Add(TEntity entity);

        Task<int?> AddAsync(TEntity entity);

        void AddRange(IEnumerable<TEntity> entities);

        Task<bool> AddRangeAsync(IEnumerable<TEntity> entities);

        int? RemoveById(int id);

        int? Remove(TEntity entity);

        void RemoveRange(IEnumerable<TEntity> entities);

        int? Update(TEntity entity);

        Task<int?> UpdateAsync(TEntity entity);

        DataTable ReturnDataTableFromSql(string sqlText);

    }
}
