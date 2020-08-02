using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Data.SqlClient;

namespace BT_SendDataMISA
{
    public class MSSQLConnection
    {
        private readonly ILogger<Worker> _logger;
        public MSSQLConnection(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        public SqlConnection GetConnection(string connectString)
        {
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(connectString);
                if (connection.State != ConnectionState.Open)
                    connection.Open();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
            }
            return connection;
        }
    }
}
