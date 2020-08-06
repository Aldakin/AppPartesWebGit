
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
        Task<SelectData> ValidateWorkerLineAsync(string strLine, int idAldakinUser);
        Task<string> ValidateGlobalLineAsync(int idAldakinUser,string strLine);
        Task<bool> ReadUserMessageAsync(int iIdMessage);
        Task<string> AnswerMessageAsync(LineMessage line);
        Task<string> UpdateEntityDataOrCsvAsync(int iIdEntity, int idAldakinUser, string strAction = "AC");
        Task<List<Usuarios>> GetAllUsersAsync(int iEntity);
        Task<List<List<LineaVisual>>> CreateVisualWorkerPartAsync(List<Lineas> lTemp);
        Task<string> WritetUdObrePresuNewAsync(string strDescription, string strRef, string strEntidad);
        Task<string> DeletetUdObrePresuNewAsync(string strId);
        Task<string> InsertHoliDayAsync(string strCalendario, string strEntidad, string strJornada, string strAction);
        Task<string> DeleteHoliDayAsync(string strId);
        Task<string> WriteAllHolidaysAsync(string strAllHoliDays);
        Task<string> EditWorkerLineAdminAsync(WorkerLineData lineData);
    }
}