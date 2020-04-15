using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace QueryMyData.Models
{
    public class SQLHandler
    {
        private string _connString = string.Empty;
        private const string _master = @"Server=localhost;Integrated Security=SSPI;database=master";
        private string _filePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        private string[] _columns = new string[0];
        public string Columns { get => string.Join(",", _columns); }
        public bool CheckQuery(string query)
        {
            if (string.IsNullOrWhiteSpace(query) ||
                query.Contains("drop", StringComparison.InvariantCultureIgnoreCase) ||
                query.Contains("alter", StringComparison.InvariantCultureIgnoreCase) ||
                query.Contains("create", StringComparison.InvariantCultureIgnoreCase) ||
                !(query.Last() == ';') ||
                query.Length < 8
                )
            {
                return false;
            }
            return true;
        }

        public void CreateDatabase(string name)
        {
            ClearDatabase(name);
            SetConnectionString(name);
            using (var conn = Connect(_master))
            {
                var query = $"CREATE DATABASE {name} ON PRIMARY " +
                    $"(NAME = {name}, " +
                    $"FILENAME = '{_filePath}\\{name}.mdf', " +
                    "SIZE = 2MB, MAXSIZE = 10MB, FILEGROWTH = 10%) " +
                    $"LOG ON (NAME = {name}_log, " +
                    $"FILENAME = '{_filePath}\\{name}_log.ldf', " +
                    "SIZE = 1MB, " +
                    "MAXSIZE = 5MB, " +
                    "FILEGROWTH = 10%)";

                using var cmd = new SqlCommand(query, conn);
                cmd.ExecuteNonQuery();
            }
        }

        public DataTable RunQuery(string query, string dbName)
        {
            var resTable = new DataTable()
            {
                TableName = "results"
            };

            SetConnectionString(dbName);
            using (var conn = Connect(_connString))
            {
                using var cmd = new SqlCommand(query, conn);
                try
                {
                    using var rdr = cmd.ExecuteReader();

                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            var fieldN = rdr.FieldCount;
                            var resObjects = new object[fieldN];
                            for (var i = 0; i < fieldN; i++)
                            {
                                resObjects[i] = rdr.GetValue(i);
                            }

                            if (resTable.Columns.Count < fieldN)
                            {
                                for (var i = 0; i < resObjects.Length; i++)
                                {
                                    resTable.Columns.Add(i.ToString(), resObjects[i].GetType());
                                    if (resTable.Columns.Count == fieldN)
                                    {
                                        break;
                                    }
                                }
                            }
                            resTable.Rows.Add(resObjects);
                        }
                    }
                }
                catch
                {
                    return resTable;
                }
                return resTable;
            }
        }

        public void CreateTable(DataSet data)
        {
            var tableName = data.Tables[0].TableName;
            var cols = new List<(string, string)>();
            var uniqueRows = new Dictionary<string, object[]>();

            foreach (DataColumn col in data.Tables[0].Columns)
            {
                var colName = new string(col.ColumnName.Where(c => !char.IsControl(c)).ToArray());
                cols.Add((colName, TypeConverter(col.DataType)));
            }

            foreach (DataRow row in data.Tables[0].Rows)
            {
                var rowData = row.ItemArray.ToArray();
                var key = string.Join("", rowData);
                if (!uniqueRows.ContainsKey(key))
                {
                    uniqueRows.Add(key, rowData);
                }

            }

            _columns = cols.Select(col => col.Item1).ToArray();
            var cmdText = new StringBuilder();
            using (var conn = Connect(_connString))
            {


                cmdText.Append($"CREATE TABLE {tableName} (");

                cols.ForEach(col => cmdText.Append($"{col.Item1.Replace(" ", "_")} {col.Item2} ,"));

                cmdText.Remove(cmdText.Length - 1, 1);

                cmdText.Append(" )");

                using var cmd = new SqlCommand(cmdText.ToString(), conn);
                cmd.ExecuteNonQuery();

            }

            FillTableData(cols.Select(col => col.Item2).ToArray(), uniqueRows, tableName);
        }

        public string ReadSchemaInfo(bool isTableInfo)
        {
            var subGroup = isTableInfo ? "TABLES" : "COLUMNS";
            var resField = isTableInfo ? "TABLE_NAME" : "COLUMN_NAME";
            try
            {
                using (var conn = Connect(_connString))
                {
                    var cmdText = $"SELECT * FROM INFORMATION_SCHEMA.{subGroup}";
                    using var cmd = new SqlCommand(cmdText, conn);
                    using var rdr = cmd.ExecuteReader();

                    var columns = new List<string>();
                    while (rdr.Read())
                    {
                        columns.Add(rdr[resField].ToString());
                    }

                    return string.Join(",", columns);
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        private void FillTableData(string[] types, Dictionary<string, object[]> uniqueRows, string tableName)
        {
            using (var conn = Connect(_connString))
            {
                var cmdText = CreateInsertString(tableName);

                foreach (var row in uniqueRows)
                {
                    var items = row.Value.Select(ob => ob.ToString().Replace(",", "")).ToArray();
                    using var cmd = new SqlCommand(cmdText, conn);

                    for (var i = 0; i < _columns.Length; i++)
                    {
                        var param = new SqlParameter($"@{_columns[i]}", items[i]);
                        param.IsNullable = true;

                        cmd.Parameters.Add(param);

                    }
                    cmd.ExecuteNonQuery();
                }
            }
        }



        private string CreateInsertString(string tName)
        {
            return new StringBuilder().Append($"INSERT INTO {tName} (")
                .Append($"{Columns}) VALUES ")
                .Append($"({string.Join(", ", _columns.Select(c => $"@{c}"))})").ToString();
        }

        private SqlConnection Connect(string connString)
        {
            var conn = new SqlConnection(connString);
            conn.Open();
            return conn;
        }

        private void ClearDatabase(string name)
        {
            using (var conn = Connect(_master))
            {
                using var cmd = new SqlCommand($"DROP DATABASE IF EXISTS {name};", conn);
                cmd.ExecuteNonQuery();
            }
        }

        public bool CheckDatabaseExists(string dbName)
        {

            using (var conn = Connect(_master))
            {
                using var cmd = new SqlCommand("SELECT name FROM master.sys.databases;", conn);
                using var rdr = cmd.ExecuteReader();

                var names = new List<string>();
                while (rdr.Read())
                {
                    names.Add(rdr["name"].ToString());
                }

                if (names.Contains(dbName))
                {
                    SetConnectionString(dbName);
                    return true;
                }
                return false;
            }
        }
        public bool CheckTableExists(string table)
        {
            return ReadSchemaInfo(false).Contains(table);
        }

        private void SetConnectionString(string dbName = null)
        {
            _connString = string.IsNullOrWhiteSpace(dbName) ? _master : @$"Server=localhost;Integrated Security=SSPI;Database={dbName};";
        }


        private string TypeConverter(Type type)
        {
            switch (type.Name)
            {
                case "DateTime":
                    return "DATE";
                case "Int16":
                case "Int32":
                    return "INT";
                case "Int64":
                    return "BIGINT";
                case "Single":
                case "Double":
                    return "FLOAT";
                case "Decimal":
                    return "DECIMAL";
                case "Binary":
                case "SBinary":
                    return "VARBINARY(1024)";
                default:
                    return "VARCHAR(max)";
            }
        }
        private SqlDbType GetSqlType(string type)
        {
            return type switch
            {
                "DATE" => SqlDbType.DateTime,
                "INT" => SqlDbType.Int,
                "BIGINT" => SqlDbType.BigInt,
                "FLOAT" => SqlDbType.Float,
                "DECIMAL" => SqlDbType.Decimal,
                "VARBINARY(1024)" => SqlDbType.VarBinary,
                _ => SqlDbType.VarChar,
            };
        }
        private DbType GetDbType(string type)
        {
            return type switch
            {
                "DATE" => DbType.DateTime,
                "INT" => DbType.Int32,
                "BIGINT" => DbType.Int64,
                "FLOAT" => DbType.Double,
                "DECIMAL" => DbType.Decimal,
                "VARBINARY(1024)" => DbType.Binary,
                _ => DbType.AnsiString,
            };
        }
    }
}
