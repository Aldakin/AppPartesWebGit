
using AppPartes.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppPartes.Logic
{
    public interface IWriteDataBase
    {
        Task<UserData> GetUserDataAsync(int idAldakinUser);
        Task<string> InsertWorkerLineAsync(WorkerLineData dataToInsertLine, int idAldakinUser);
        Task<List<SelectData>> DeleteWorkerLineAsync(int iLine, int idAldakinUser);
        Task<SelectData> EditWorkerLineAsync(WorkerLineData dataToEditLine, int idAldakinUser);
        Task<SelectData> CloseWorkerWeekAsync(string strDataSelected, int idAldakinUser);
        Task<bool> ReadUserMessageAsync(int iIdMessage);
        Task<string> AnswerMessageAsync(LineMessage line);
        Task<string> UpdateEntityDataOrCsvAsync(int iIdEntity, int idAldakinUser, string strAction = "AC");
        Task<List<Usuarios>> GetAllUsersAsync(int iEntity);
    }
}
