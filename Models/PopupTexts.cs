namespace QueryMyData.Models
{
    public struct PopupTexts
    {
        #region Messages
        public const string Guide = "Please input your query as is, ending with ';', using only valid SQL synthax. To transform the data, you must read it from a file first.";
        public const string Error = "An unknown error occurred, please contact the developer.";
        public const string QueryError = "The query you entered is either missing elements, or is malformed.";
        public const string QueryMissingError = "The query either has not been given, or was not correct.";
        public const string ColumnError = "Please read the file first to retreieve column data.";
        public const string DataMissingError = "Please read the file to retrieve the data first.";
        public const string Success = "The action completed successfully.";
        public const string NoDataMessage = "The query did not return any results.";
        #endregion

        #region Captions
        public const string GuideCaption = "Usage Guide";
        public const string ErrorCaption = "Error";
        public const string QueryErrorCaption = "Malformed Query";
        public const string QueryMissingCpt = "Missing Query";
        public const string ColumnsCaption = "Your Current Tables";
        public const string DataMissingErrorCptn = "Data Not Found";
        public const string SuccessCaption = "Success!";
        public const string NoDataCaption = "No Results";
        #endregion

    }
}
