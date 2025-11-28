using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Dapper;

namespace WorkFlowConsoleApp.DataAccess
{

    public class DapperRepository
    {
        private readonly string _connectionString;

        public DapperRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["WorkflowEngine"]?.ConnectionString
                ?? "Data Source=localhost;Initial Catalog=WorkflowDB;Integrated Security=True;";
        }

        public DapperRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }


        public IEnumerable<T> Query<T>(string sql, object parameters = null)
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                return connection.Query<T>(sql, parameters);
            }
        }

        public T QueryFirstOrDefault<T>(string sql, object parameters = null)
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                return connection.QueryFirstOrDefault<T>(sql, parameters);
            }
        }

        public T ExecuteScalar<T>(string sql, object parameters = null)
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                return connection.ExecuteScalar<T>(sql, parameters);
            }
        }

        public int Execute(string sql, object parameters = null)
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                return connection.Execute(sql, parameters);
            }
        }

        public List<ProcessInstanceDto> GetProcessInstances()
        {
            string sql = @"
                SELECT 
                    Id,
                    ProcessId,
                    VoucherKind,
                    IsClosed
                FROM ProcessInstances
                ORDER BY Id DESC";

            try
            {
                return Query<ProcessInstanceDto>(sql).AsList();
            }
            catch (SqlException ex)
            {
                // در صورت عدم وجود جدول، لیست خالی برمی‌گردانیم
                Console.WriteLine($"⚠ هشدار: خطا در خواندن از دیتابیس: {ex.Message}");
                return new List<ProcessInstanceDto>();
            }
        }

        public ProcessInstanceDto GetProcessInstanceById(int id)
        {
            string sql = @"
                SELECT 
                    Id,
                    ProcessId,
                    VoucherKind,
                    IsClosed
                FROM ProcessInstances
                WHERE Id = @Id";

            return QueryFirstOrDefault<ProcessInstanceDto>(sql, new { Id = id });
        }

        public int GetProcessInstanceCount()
        {
            string sql = "SELECT COUNT(*) FROM ProcessInstances";

            try
            {
                return ExecuteScalar<int>(sql);
            }
            catch (SqlException)
            {
                return 0;
            }
        }

        public bool TestConnection()
        {
            try
            {
                using (IDbConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var result = Dapper.SqlMapper.ExecuteScalar<int>(connection, "SELECT 1");
                    return result == 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ خطا در اتصال به دیتابیس: {ex.Message}");
                return false;
            }
        }
    }
    public class ProcessInstanceDto
    {
        public int Id { get; set; }
        public int ProcessId { get; set; }
        public int VoucherKind { get; set; }
        public bool IsClosed { get; set; }
    }
}

