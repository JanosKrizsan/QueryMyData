using System.Data;

namespace QueryMyData.Models
{
    public interface IConvertable
    {
        public void ConvertData(DataSet data);
        public bool ConvertQuery(DataTable data, int queryHash);
    }
}
