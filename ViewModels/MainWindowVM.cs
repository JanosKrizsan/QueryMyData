using QueryMyData.Model;
using QueryMyData.Models;
using System.Data;
using System.Text;

namespace QueryMyData.ViewModels
{
    public class MainWindowVM : BasePropertyChange
    {
        private DataSet _currentData = null;
        private readonly XMLSerialization _toXML = new XMLSerialization();
        private readonly CSVSerialization _toCSV = new CSVSerialization();
        private readonly OpenFile _openFile = new OpenFile();
        private readonly SQLHandler _sqlHandler = new SQLHandler();
        private readonly Popup _popup = new Popup();
        private string _query = string.Empty;
        public string Query
        {
            get => _query;
            set
            {
                SetProperty(ref _query, value);
            }

        }

        public DelegateCommand<string> ReadFileCommand { get; private set; }
        public DelegateCommand<string> QueryToCSVCommand { get; private set; }
        public DelegateCommand<string> QueryToXMLCommand { get; private set; }
        public DelegateCommand<string> DataToCSVCommand { get; private set; }
        public DelegateCommand<string> DataToXMLCommand { get; private set; }
        public DelegateCommand<string> AddQueryCommand { get; private set; }
        public DelegateCommand<string> ShowGuideCommand { get; private set; }
        public DelegateCommand<string> ShowTableInfoCommand { get; private set; }

        public MainWindowVM()
        {
            SetupCommands();
        }

        private void SetupCommands()
        {
            ReadFileCommand = new DelegateCommand<string>(ReadFile_Execute);
            QueryToCSVCommand = new DelegateCommand<string>(QueryToCSV_Execute);
            QueryToXMLCommand = new DelegateCommand<string>(QueryToXML_Execute);
            DataToCSVCommand = new DelegateCommand<string>(DataToCSV_Execute);
            DataToXMLCommand = new DelegateCommand<string>(DataToXML_Execute);
            AddQueryCommand = new DelegateCommand<string>(AddQuery_Execute);
            ShowGuideCommand = new DelegateCommand<string>(ShowGuide_Execute);
            ShowTableInfoCommand = new DelegateCommand<string>(ShowInfo_Execute);
        }

        private void QueryToXML_Execute(string obj)
        {
            QueryToData_Execute("XML");
        }

        private void QueryToCSV_Execute(string obj)
        {
            QueryToData_Execute("CSV");
        }

        private void QueryToData_Execute(string toDataType)
        {
            if (!string.IsNullOrWhiteSpace(_query))
            {
                if (_currentData is null)
                {
                    _popup.Show(PopupTexts.DataMissingError, PopupTexts.DataMissingErrorCptn, 0, 3);
                    return;
                }

                var data = _sqlHandler.RunQuery(_query, _currentData.Tables[0].TableName);
                if (data.Rows.Count > 0)
                {
                    _ = toDataType == "XML" ? _toXML.ConvertQuery(data, _query.GetHashCode()) : _toCSV.ConvertQuery(data, _query.GetHashCode());
                    SuccessfulCompletion();
                    return;
                }
                _popup.Show(PopupTexts.NoDataMessage, PopupTexts.NoDataCaption, 0, 0);
                return;
            }
            _popup.Show(PopupTexts.QueryMissingError, PopupTexts.QueryMissingCpt, 0, 1);
        }

        private void ShowInfo_Execute(string filler)
        {
            var tables = _currentData == null ? _sqlHandler.ReadSchemaInfo(true) : _currentData.Tables[0].TableName;
            var cols = string.IsNullOrWhiteSpace(_sqlHandler.Columns) ? _sqlHandler.ReadSchemaInfo(false) : _sqlHandler.Columns;
            if (string.IsNullOrWhiteSpace(cols) && string.IsNullOrWhiteSpace(tables))
            {
                _popup.Show(PopupTexts.ColumnError, PopupTexts.ColumnsCaption, 0, 4);
                return;
            }

            var tableInfo = new StringBuilder()
                .Append("Tables: \n")
                .Append(tables)
                .Append("\n")
                .Append("Columns: \n")
                .Append(cols);

            _popup.Show(tableInfo.ToString(), PopupTexts.ColumnsCaption, 0, 0);
        }

        private void DataToXML_Execute(string filler)
        {
            if (CurrentDataCheck())
            {
                _toXML.ConvertData(_currentData);
                SuccessfulCompletion();
            }
        }

        private void DataToCSV_Execute(string filler)
        {
            if (CurrentDataCheck())
            {
                _toCSV.ConvertData(_currentData);
                SuccessfulCompletion();
            }
        }

        private void ShowGuide_Execute(string input)
        {
            _popup.Show(PopupTexts.Guide, PopupTexts.GuideCaption, 0, 0);
        }

        private void ReadFile_Execute(string filler)
        {
            _currentData = _openFile.GetFileData(_openFile.GetFilePath());
            SetupDatabase(_currentData);
        }

        private void AddQuery_Execute(string query)
        {
            if (_sqlHandler.CheckQuery(query))
            {
                Query = query;
                SuccessfulCompletion();
                return;
            }
            _popup.Show(PopupTexts.QueryError, PopupTexts.QueryErrorCaption, 0, 4);
        }

        private void SetupDatabase(DataSet data)
        {
            var tableName = data.Tables[0].TableName;

            if (!_sqlHandler.CheckDatabaseExists(tableName))
            {
                _sqlHandler.CreateDatabase(tableName);
                if (!_sqlHandler.CheckTableExists(tableName))
                {
                    _sqlHandler.CreateTable(data);
                }
                SuccessfulCompletion();
            }
        }

        private bool CurrentDataCheck()
        {
            if (_currentData is null)
            {
                _popup.Show(PopupTexts.DataMissingError, PopupTexts.DataMissingErrorCptn, 0, 1);
                return false;
            }
            return true;
        }

        private void SuccessfulCompletion()
        {
            _popup.Show(PopupTexts.Success, PopupTexts.SuccessCaption, 0, 0);
        }
    }
}
