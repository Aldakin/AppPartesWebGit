
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppPartes.Logic
{
    public interface IWriteDataBase
    {
        Task<string> InsertWorkerLineAsync(WorkerLineData dataToInsertLine);
        Task<List<SelectData>> DeleteWorkerLineAsync(int iLine);
        Task<SelectData> EditWorkerLineAsync(WorkerLineData dataToEditLine);
        Task<SelectData> CloseWorkerWeekAsync(string strDataSelected);
    }
}
