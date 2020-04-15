using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace QueryMyData.Models
{
    public class CSVSerialization : PathBuilder, IConvertable
    {
        public void ConvertData(DataSet data)
        {
            var outputPath = GetPathBuilt(data.GetHashCode().ToString(), ".csv");
            SaveToFile(data.Tables[0], outputPath);
        }

        public bool ConvertQuery(DataTable data, int queryHash)
        {
            var fileName = (data.GetHashCode() + queryHash).ToString();
            var outputPath = GetPathBuilt(fileName, ".csv");
            SaveToFile(data, outputPath);
            return true;
        }

        private void SaveToFile(DataTable table, string path)
        {
            var text = new StringBuilder();
            var columns = new List<string>();

            foreach (DataColumn col in table.Columns)
            {
                columns.Add(col.ColumnName);
            }

            text.AppendLine(string.Join(",", columns));

            foreach (DataRow row in table.Rows)
            {
                text.AppendLine(string.Join(",", row.ItemArray.Select(f => f.ToString()).ToArray()));
            }

            File.WriteAllText(path, text.ToString());
        }
    }
}
