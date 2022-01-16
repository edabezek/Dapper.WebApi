using Dapper.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Infrastructure.Repositories
{
    public abstract class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly IConfiguration configuration;
        private string _tableName;//Dbset yapmadığımız için bir tablo  oluşturup çağıracağız
        private IEnumerable<PropertyInfo> GetProperties => typeof(T).GetProperties();
        protected GenericRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        // Generate new connection 
        private SqlConnection SqlConnection()
        {
            return new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }
        // Open new connection and return it for use
        private IDbConnection CreateConnection()
        {
            var conn = SqlConnection();
            conn.Open();
            return conn;
        }
        public async Task<int> AddAsync(T entity)
        {
            _tableName = typeof(T).Name;
            var insertQuery = GenerateInsertQuery_ScopeIdentity();
            using (var connection = CreateConnection())
            {
                return await connection.ExecuteAsync(insertQuery, entity);
            }

        }
        private static List<string> GenerateListOfProperties(IEnumerable<PropertyInfo> listOfProperties)
        {
            return (from prop in listOfProperties

                    let attributes = prop.GetCustomAttributes(typeof(DescriptionAttribute), false)
                    where (attributes.Length <= 0 || (attributes[0] as DescriptionAttribute)?.Description != "Ignore") && prop.Name != "Id"
                    select prop.Name).ToList();
        }
        private string GenerateInsertQuery_ScopeIdentity()
        {
            var insertQuery = new StringBuilder($"INSERT INTO {_tableName} ");

            insertQuery.Append("(");

            var properties = GenerateListOfProperties(GetProperties);

            properties.ForEach(prop => { insertQuery.Append($"[{prop}],"); });
            insertQuery
            .Remove(insertQuery.Length - 1, 1)
            .Append(") VALUES (");

            properties.ForEach(prop => { insertQuery.Append($"@{prop},"); });
            insertQuery
            .Remove(insertQuery.Length - 1, 1)
            .Append(")");

            insertQuery = new StringBuilder(string.Format("{0}; SELECT SCOPE_IDENTITY();", insertQuery, _tableName));
            return insertQuery.ToString();
        }     
        //INSERT INTO Customers(CustomerName, ContactName, Address, City, PostalCode, Country)
        //   
        // VALUES('Cardinal', 'Tom B. Erichsen', 'Skagen 21', 'Stavanger', '4006', 'Norway'); ;
        public async Task<int> DeleteAsync(int id)
        {
            _tableName = typeof(T).Name;
            var trialQuery = $"Delete From {_tableName} Where Id = @Id";
            using (var connection = CreateConnection())
            {
                return await connection.ExecuteAsync(trialQuery, new { Id = id });
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            _tableName = typeof(T).Name;
            var trialQuery = $"Select * From {_tableName}";
            using (var connection = CreateConnection())
            {
                return await connection.QueryAsync<T>(trialQuery);
            }

        }

        public async Task<T> GetByIdAsync(int id)
        {
            _tableName = typeof(T).Name;
            var trialQuery = $"Select * From {_tableName} Where Id = @Id";
            using (var connection = CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<T>(trialQuery, new { Id = id });
            }

        }
        public async Task<int> UpdateAsync(T entity)
        {
            _tableName = typeof(T).Name;
            var updateQuery = GenerateUpdateQuery();

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(updateQuery, entity);
                return result;
            }
        }
        private string GenerateUpdateQuery()
        {
            var updateQuery = new StringBuilder($"UPDATE {_tableName} SET ");
            var properties = GenerateListOfProperties(GetProperties);
            properties.ForEach(property =>
            {
                if (!property.Equals("id"))
                {
                    updateQuery.Append($"{property}=@{property},");
                }
            });
            updateQuery.Remove(updateQuery.Length - 1, 1);
            updateQuery.Append(" WHERE Id=@Id");
            return updateQuery.ToString();
        }

        //public Task<int> UpdateAsync(T entity)
        //{
        //    entity.ModifiedOn = DateTime.Now;
        //    var sql = "UPDATE Products SET Name = @Name, Description = @Description, Barcode = @Barcode, Rate = @Rate, ModifiedOn = @ModifiedOn  WHERE Id = @Id";
        //    using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
        //    {
        //        connection.Open();
        //        var result = await connection.ExecuteAsync(sql, entity);
        //        return result;
        //    }

            

        //}
    }
}
