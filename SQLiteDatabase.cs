using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperaSDExporter
{
    public class SQLiteDatabase
    {
        private readonly string _dbConnection;

        public SQLiteDatabase(string dataSource)
        {
            _dbConnection = string.Format("Data Source={0}", dataSource);
        }

        public DataTable GetDataTable(SQLiteCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");

            using (SQLiteConnection connection = new SQLiteConnection(_dbConnection))
            {
                connection.Open();
                command.Connection = connection;

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    DataTable result = new DataTable();
                    result.Load(reader);
                    return result;
                }
            }
        }

        public SQLiteCommand GetCommand(string sql)
        {
            if (string.IsNullOrEmpty(sql))
                throw new ArgumentNullException("sql");

            return new SQLiteCommand { CommandText = sql, CommandType = CommandType.Text };
        }

        public int ExecuteNonQuery(SQLiteCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");

            using (SQLiteConnection connection = new SQLiteConnection(_dbConnection))
            {
                connection.Open();
                command.Connection = connection;

                return command.ExecuteNonQuery();
            }
        }
    }
}
