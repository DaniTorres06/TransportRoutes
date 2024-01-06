using TransportRoutesModel;
using TransportRoutesModel.ModelView;

namespace TransportRoutesBusiness.Interfaces
{
    public interface IEmployeeBusiness
    {        
        public Task<ResponseList<Employee>?> ValidUser(string User, string Pass);
    }
}