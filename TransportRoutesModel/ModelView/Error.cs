namespace TransportRoutesModel.ModelView
{
    public class Error
    {
        public int HasErrors { get; set; }
        public int LineNumber { get; set; }
        public int ColumnNumber { get; set; }
        public int Id { get; set; }
        public string ErrorMessage { get; set; }
        public int ErrorSeverity { get; set; }

        public Error()
        {
            HasErrors = 0;
            LineNumber = 0;
            Id = 0;
            ErrorMessage = "";
            ErrorSeverity = 0;
        }
    }
}
