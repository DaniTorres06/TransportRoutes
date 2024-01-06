using Microsoft.AspNetCore.Mvc.Rendering;

namespace TransportRoutesModel.ModelView
{
    public class ResponseSelectListItem
    {
        public List<SelectListItem> List { get; set; }
        public Response Response { get; set; }

        public ResponseSelectListItem()
        {
            List = new();
            Response = new();
        }
    }
}
