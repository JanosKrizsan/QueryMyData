using System.Data;
using System.IO;
using System.Xml;

namespace QueryMyData.Models
{
    public class XMLSerialization : PathBuilder, IConvertable
    {
        public void ConvertData(DataSet data)
        {
            var outputPath = GetPathBuilt(data.GetHashCode().ToString(), ".xml");
            SaveToFile(data, outputPath);
        }

        public bool ConvertQuery(DataTable table, int queryHash)
        {
            var fileName = (table.GetHashCode() + queryHash).ToString();
            var outputPath = GetPathBuilt(fileName, ".xml");
            var data = new DataSet();
            data.Tables.Add(table);

            SaveToFile(data, outputPath);
            return true;
        }

        private void SaveToFile(DataSet data, string path)
        {
            File.Create(path).Close();
            var docu = new XmlDocument();
            docu.LoadXml(data.GetXml());
            docu.PreserveWhitespace = true;
            docu.Save(path);
        }
    }
}
