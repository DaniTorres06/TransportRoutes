using TransportRoutesModel;
using TransportRoutesModel.ModelView;

namespace TransportRoutesBusiness.Interfaces
{
    public interface IAutenticationBusiness
    {
        
        public Task<ResponseList<Autentication>> Atuentication(string User, string Pass);
    }
}