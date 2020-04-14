using System.Data;

namespace QueryMyData.Models
{
    public interface IConvertable
    {
        public void ConvertData(DataSet data);
        public void ConvertQuery();
    }
}
