using ExcelDataReader;
using Microsoft.Win32;
using System;
using System.Data;
using System.IO;
using System.Text;

namespace QueryMyData.Models
{
    public class OpenFile
    {
        public string GetFilePath()
        {
            var dialog = new OpenFileDialog();
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
            dialog.Filter = "Xls files (*.xls)|*.xls|Xlsx files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            dialog.FilterIndex = 2;
            dialog.RestoreDirectory = true;

            if (dialog.ShowDialog() == true)
            {
                return dialog.FileName;
            }
            return string.Empty;
        }

        public DataSet GetFileData(string path)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var openXMLOrNot = Path.GetExtension(path) == ".xlsx";
            using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
            {
                var reader = openXMLOrNot ? ExcelReaderFactory.CreateOpenXmlReader(stream) : ExcelReaderFactory.CreateBinaryReader(stream);
                using (reader)
                {
                    var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (data) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true
                        }
                    });
                    result.DataSetName = "Temporary_Dataset";
                    return result;
                }
            }

        }
    }
}
