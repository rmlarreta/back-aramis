using backaramis.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace backaramis.Services
{
    public class StoreProcedure : IStoreProcedure
    {
        private static string _connectionString = "";
        private readonly IConfiguration _config;
        public StoreProcedure(IConfiguration config)
        {
            _config = config;
            _connectionString = _config.GetConnectionString("DefaultConnection");
        }

        public DataTable SpWhithDataSet(string Sp, List<SqlParameter>? Param = null!)
        {
            DataTable Ds = new();
            SqlConnection conn = new(_connectionString);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            SqlCommand sqlComm = new();
            sqlComm.CommandType = CommandType.StoredProcedure;
            sqlComm.Connection = conn;
            sqlComm.CommandText = Sp;
            try
            {
                if (Param != null)
                {
                    foreach (SqlParameter param in Param)
                    {
                        sqlComm.Parameters.Add(param);
                    }
                }
            }
            catch (Exception)
            {
            }
            SqlDataAdapter sqlAdap = new(sqlComm);
            sqlAdap.Fill(Ds);
            conn.Close();
            return Ds;
        }

        public bool ExecuteNonQuery(string Sp, List<SqlParameter>? Param = null)
        {
            SqlConnection conn = new(_connectionString);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            SqlCommand sqlComm = new();
            sqlComm.CommandType = CommandType.StoredProcedure;
            sqlComm.Connection = conn;
            sqlComm.CommandText = Sp;
            try
            {
                if (Param != null)
                {
                    foreach (SqlParameter param in Param)
                    {
                        sqlComm.Parameters.Add(param);
                    }
                }
            }
            catch (Exception)
            {
            }
            sqlComm.ExecuteNonQuery();
            conn.Close();
            return true;
        }

        public object ExecuteScalar(string Sp, List<SqlParameter>? Param = null!)
        {
            SqlConnection conn = new(_connectionString);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            SqlCommand sqlComm = new();
            sqlComm.CommandType = CommandType.StoredProcedure;
            sqlComm.Connection = conn;
            sqlComm.CommandText = Sp;
            try
            {
                if (Param != null)
                {
                    foreach (SqlParameter param in Param)
                    {
                        sqlComm.Parameters.Add(param);
                    }
                }
            }
            catch (Exception)
            {
            }
            object? Response = sqlComm.ExecuteScalar();
            conn.Close();
            return Response;
        }

        public DataSet SpWhithDataSetPure(string Sp, List<SqlParameter>? Param = null!)
        {
            DataSet Ds = new();
            SqlConnection conn = new(_connectionString);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            SqlCommand sqlComm = new();
            sqlComm.CommandType = CommandType.StoredProcedure;
            sqlComm.Connection = conn;
            sqlComm.CommandText = Sp;
            try
            {
                if (Param != null)
                {
                    foreach (SqlParameter param in Param)
                    {
                        sqlComm.Parameters.Add(param);
                    }
                }
            }
            catch (Exception)
            {
            }
            SqlDataAdapter sqlAdap = new(sqlComm);
            sqlAdap.Fill(Ds);
            conn.Close();
            return Ds;
        }
    }
}