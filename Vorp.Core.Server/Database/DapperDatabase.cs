using Dapper;
using MySqlConnector;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vorp.Core.Server.Models;

namespace Vorp.Core.Server.Database
{
    internal class DapperDatabase<T>
    {
        // TODO: Use mysql connection from SQL Config
        private static string _connectionString;

        private static string ConnectionString()
        {
            if (!string.IsNullOrEmpty(_connectionString))
                return _connectionString;

            DatabaseConfig databaseConfig = ServerConfiguration.Config.DatabaseConfig;

            MySqlConnectionStringBuilder mySqlConnectionStringBuilder = new MySqlConnectionStringBuilder();
            mySqlConnectionStringBuilder.ApplicationName = "VORP Core";

            mySqlConnectionStringBuilder.Database = databaseConfig.Database;
            mySqlConnectionStringBuilder.Server = databaseConfig.Server;
            mySqlConnectionStringBuilder.Port = databaseConfig.Port;
            mySqlConnectionStringBuilder.UserID = databaseConfig.Username;
            mySqlConnectionStringBuilder.Password = databaseConfig.Password;

            mySqlConnectionStringBuilder.MaximumPoolSize = databaseConfig.MaximumPoolSize;
            mySqlConnectionStringBuilder.MinimumPoolSize = databaseConfig.MinimumPoolSize;
            mySqlConnectionStringBuilder.ConnectionTimeout = databaseConfig.ConnectionTimeout;

            return _connectionString = mySqlConnectionStringBuilder.ToString();
        }

        public static async Task<List<T>> GetListAsync(string query, DynamicParameters args = null)
        {
            var watch = Stopwatch.StartNew();
            try
            {
                using (var conn = new MySqlConnection(ConnectionString()))
                {
                    SetupTypeMap();

                    return (await conn.QueryAsync<T>(query, args)).AsList();
                }
            }
            catch (Exception ex)
            {
                SqlExceptionHandler(query, ex.Message, watch.ElapsedMilliseconds);
            }
            finally
            {
                watch.Stop();
            }
            return null;
        }

        public static async Task<T> GetSingleAsync(string query, DynamicParameters args = null)
        {
            var watch = Stopwatch.StartNew();
            try
            {
                using (var conn = new MySqlConnection(ConnectionString()))
                {
                    SetupTypeMap();
                    return (await conn.QueryAsync<T>(query, args)).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                SqlExceptionHandler(query, ex.Message, watch.ElapsedMilliseconds);
            }
            finally
            {
                watch.Stop();
            }
            return default(T);
        }

        public static async Task<bool> ExecuteAsync(string query, DynamicParameters args = null)
        {
            var watch = Stopwatch.StartNew();
            try
            {
                using (var conn = new MySqlConnection(ConnectionString()))
                {
                    return (await conn.ExecuteAsync(query, args)) > 0;
                }
            }
            catch (Exception ex)
            {
                SqlExceptionHandler(query, ex.Message, watch.ElapsedMilliseconds);
            }
            finally
            {
                watch.Stop();
            }
            return false;
        }

        private static void SqlExceptionHandler(string query, string exceptionMessage, long elapsedMilliseconds)
        {
            StringBuilder sb = new();
            sb.Append("** SQL Exception **\n");
            sb.Append($"Query: {query}\n");
            sb.Append($"Exception Message: {exceptionMessage}\n");
            sb.Append($"Time Elapsed: {elapsedMilliseconds}ms");
            Logger.Error($"{sb}");
        }

        private static void SetupTypeMap()
        {
            var map = new CustomPropertyTypeMap(typeof(T), (type, columnName) =>
                                type.GetProperties().FirstOrDefault(prop => GetDescriptionFromAttribute(prop) == columnName.ToLower()));
            SqlMapper.SetTypeMap(typeof(T), map);
        }

        public static string GetDescriptionFromAttribute(MemberInfo member)
        {
            if (member == null) return null;

            var attrib = (DescriptionAttribute)Attribute.GetCustomAttribute(member, typeof(DescriptionAttribute), false);
            return (attrib?.Description ?? member.Name).ToLower();
        }
    }
}
