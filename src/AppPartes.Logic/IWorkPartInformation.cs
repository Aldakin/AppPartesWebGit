using AppPartes.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppPartes.Logic
{
    public interface IWorkPartInformation
    {
        Task<List<SelectData>> WeekHourResume(DateTime dtSelected, int idAldakinUser);
        Task<List<SelectData>> SelectedCompanyReadOt(int iEntidad, int idAldakinUser);
        Task<List<SelectData>> SelectedCompanyReadClient(int iEntidad, int idAldakinUser);
        Task<List<SelectData>> SelectedClient(int iClient, int idAldakinUser);
        Task<List<SelectData>> SelectedOt(int iOt, int idAldakinUser);
        Task<List<SelectData>> ReadLevel1(int iData, int idAldakinUser);
        Task<List<SelectData>> ReadLevel2(int iData, int iData2, int idAldakinUser);
        Task<List<SelectData>> ReadLevelGeneral(int iData, int idAldakinUser);
        Task<List<SelectData>> SelectedPayerAsync(int iPayer, int iOt, int idAldakinUser);
        Task<List<string>> PendingWorkPartApiAsync(string strCalendario, string strUser, string strEntity);
        Task<List<ViewMounthResume>> StatusEntityResumeAsync(int idAldakinUser, string strCalendario, string strEntity);
        Task<string> StringWeekResumeAsync(int idAldakinUser, string strDate, string strOt, string strWorker, string strEntity);
        Task<List<List<LineaVisual>>> StatusWeekResumeAsync(int idAldakinUser, string strDate, string strOt, string strWorker, string strEntity);
        Task<List<SelectData>> GetWorkerValidationAsnc(int idAldakinUser, int iEntity);
        Task<List<SelectData>> GetOtValidationAsync(int idAldakinUser, int iEntity);
        Task<string> PrepareWorkLineAsync(WorkerLineData dataToInsertLine, int idAldakinUser, int idAdminUser, string strAction);
        Task<List<Usuarios>> ListValidationUsersAsync(int idAldakinUser, int iEntity = 0);
        Task<List<Ots>> ListValidationOtsAsync(int idAldakinUser, int iEntity = 0);
        Task<string> NewValidationUsersAsync(int idAldakinUser, string strUsers, string strWorker);
        Task<string> NewValidationOtAsync(int idAldakinUser, string strOts, string strWorker);
        Task<List<ExcelTipoHora>> ReviewHourTypeHourAsync(int iCodEnt, DateTime dtSelected);
        Task<List<ListExcel1>> ReviewHourMonthAsync(int iCodEnt, DateTime dtSelected);
        Task<List<ExcelWorkerReport>> WorkerReportAsync(int iIdUser, DateTime dtSelected);
        Task<string> GetUserNameAsync(int iUser);

    }
}

