using QueryMyData.Model;
using QueryMyData.Models;
using System.Data;

namespace QueryMyData.ViewModels
{
    public class MainWindowVM : BasePropertyChange
    {
        private DataSet _currentData = null;
        private XMLSerialization _toXML = new XMLSerialization();
        private CSVSerialization _toCSV = new CSVSerialization();
        private OpenFile _openFile = new OpenFile();
        private SQLHandler _sqlHandler = new SQLHandler();
        private Popup _popup = new Popup();
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
        public DelegateCommand<string> RunQueryCommand { get; private set; }
        public DelegateCommand<string> ShowGuideCommand { get; private set; }
        public DelegateCommand<string> ShowColumnInfoCommand { get; private set; }

        public MainWindowVM()
        {
            SetupCommands();
        }

        private void SetupCommands()
        {
            ReadFileCommand = new DelegateCommand<string>(ReadFile_Execute);
            QueryToCSVCommand = new DelegateCommand<string>();
            QueryToXMLCommand = new DelegateCommand<string>();
            DataToCSVCommand = new DelegateCommand<string>(DataToCSV_Execute);
            DataToXMLCommand = new DelegateCommand<string>(DataToXML_Execute);
            RunQueryCommand = new DelegateCommand<string>(RunQuery_Execute);
            ShowGuideCommand = new DelegateCommand<string>(ShowGuide_Execute);
            ShowColumnInfoCommand = new DelegateCommand<string>(ShowTables_Execute);
        }


        private void ShowTables_Execute(string filler)
        {
            var cols = _sqlHandler.Columns;
            if (cols is null || string.IsNullOrWhiteSpace(cols))
            {
                _popup.Show(PopupTexts.ColumnError, PopupTexts.ColumnsCaption, 0, 4);
                return;
            }
            _popup.Show(_sqlHandler.Columns, PopupTexts.ColumnsCaption, 0, 0);
        }

        private void DataToXML_Execute(string filler)
        {
            _toXML.ConvertData(_currentData);
        }

        private void DataToCSV_Execute(string filler)
        {
            _toCSV.ConvertData(_currentData);
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

        private void RunQuery_Execute(string query)
        {
            if (_sqlHandler.CheckQuery(query))
            {
                Query = query;
                return;
            }
            _popup.Show(PopupTexts.QueryError, PopupTexts.QueryErrorCaption, 0, 4);
        }

        private void SetupDatabase(DataSet data)
        {
            _sqlHandler.CreateDatabase(data.Tables[0].TableName);
            _sqlHandler.CreateTable(data);
        }
    }
}
