using TransportRoutesModel;
using TransportRoutesModel.ModelView;

namespace TransportRoutesData.Interfaces
{
    public interface IEmployeeData
    {
        Task<ResponseList<Employee>?> Validateuser(string User, string Pass);
    }
}