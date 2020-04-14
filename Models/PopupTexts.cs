namespace QueryMyData.Models
{
    public struct PopupTexts
    {
        public const string Guide = "Please input your query as is, ending with ';', using only valid SQL synthax. To transform the data, you must read it from a file first.";
        public const string Error = "An unknown error occurred, please contact the developer.";
        public const string QueryError = "The query you entered is either missing elements, or is malformed.";
        public const string ColumnError = "Please read the file first to retreieve column data.";

        public const string GuideCaption = "Usage Guide";
        public const string ErrorCaption = "Error";
        public const string QueryErrorCaption = "Malformed Query";
        public const string ColumnsCaption = "Your Current Tables";
    }
}
