namespace TransportRoutesModel.ModelView
{
    public class Response
    {
        public string message { get; set; }
        public string code { get; set; }
        public bool status { get; set; }

        public Response()
        {
            message = "";
            code = "";
            status = false;
        }
    }
}
