
using AppPartes.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppPartes.Logic
{
    public interface IWriteDataBase
    {
        Task<UserData> GetUserDataAsync(int idAldakinUser);
        Task<int> InsertWorkerLineAsync(Lineas oLinea);
        Task<string> InsertUserWorkExpensesAsync(List<Gastos> lGastos, int iIdLine);
        //Task<string> InsertWorkerLineAsync(WorkerLineData dataToInsertLine, int idAldakinUser);
        Task<List<SelectData>> DeleteWorkerLineAsync(int iLine, int idAldakinUser, int idAdminUser);
        //Task<SelectData> EditWorkerLineAsync(WorkerLineData dataToEditLine, int idAldakinUser);
        Task<SelectData> CloseWorkerWeekAsync(string strDataSelected, int idAldakinUser);
        Task<SearchViewLogic> ValidateWorkerLineAsync(string strLine, int idAldakinUser,sbyte? sValue);
        Task<string> ValidateGlobalLineAsync(int idAldakinUser,string strLine,sbyte sValue);
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
        Task<string> OpenWeek(string strLine);
        Task<string> WritePermissionAsync(string strAldakinUser, string strUsers, string strAutor, string strData);
        //Task<string> EditWorkerLineAdminAsync(WorkerLineData lineData);
    }
}