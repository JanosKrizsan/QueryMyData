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
            File.Create(outputPath).Close();
            var docu = new XmlDocument();
            docu.LoadXml(data.GetXml());
            docu.PreserveWhitespace = true;
            docu.Save(outputPath);
        }

        public void ConvertQuery()
        {
            throw new System.NotImplementedException();
        }

    }
}
