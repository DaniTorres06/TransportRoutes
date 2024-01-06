using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportRoutesModel.ModelView;


namespace TransportRoutesData.Interfaces
{
    public interface IAccessData
    {
        Task<ResponseSelectListItem> ExecuteListSPAsync(string ValueList, string TextList, string NameSP);
        Task<ResponseSelectListItem> ExecuteListSPAsync(string ValueList, string TextList, string NameSP, object value, string paramaterName);
        Task<ResponseSelectListItem> ExecuteListSPAsync(string ValueList, string TextList, string NameSP, List<SqlParameter> sqlParameters);
        Task<ResponseList<T>> ExecuteReaderSpAsync<T>(string nameSP, List<SqlParameter> sqlParameters) where T : new();
        Task<ResponseList<T>> ExecuteReaderSpEAsync<T>(string nameSP, List<SqlParameter> sqlParameters) where T : new();        
        Task<DataSet?> GetDataset(string NameSP, object value, string paramaterName);
        Task<DataSet?> GetDataset(string NameSP, List<SqlParameter> sqlParameters);

    }
}
