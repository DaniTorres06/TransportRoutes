namespace TransportRoutesModel.ModelView
{
    public class ResponseList<T> where T : new()
    {
        public List<T> List { get; set; }
        public List<Error> Errors { get; set; }
        public Response response { get; set; }

        public ResponseList()
        {
            List = new();
            Errors = new();
            response = new();
        }
    }
}
