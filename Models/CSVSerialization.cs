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
            var text = new StringBuilder();
            var columns = new List<string>();

            foreach (DataColumn col in data.Tables[0].Columns)
            {
                columns.Add(col.ColumnName);
            }

            text.AppendLine(string.Join(",", columns));

            foreach (DataRow row in data.Tables[0].Rows)
            {
                text.AppendLine(string.Join(",", row.ItemArray.Select(f => f.ToString()).ToArray()));
            }

            File.WriteAllText(outputPath, text.ToString());
        }

        public void ConvertQuery()
        {
        }
    }
}
