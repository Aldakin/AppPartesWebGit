
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppPartes.Logic
{
    public interface IWriteDataBase
    {
        Task<string> InsertWorkerLineAsync(WorkerLineData dataToInsertLine, int idAldakinUser);
        Task<List<SelectData>> DeleteWorkerLineAsync(int iLine, int idAldakinUser);
        Task<SelectData> EditWorkerLineAsync(WorkerLineData dataToEditLine, int idAldakinUser);
        Task<SelectData> CloseWorkerWeekAsync(string strDataSelected, int idAldakinUser);
    }
}
