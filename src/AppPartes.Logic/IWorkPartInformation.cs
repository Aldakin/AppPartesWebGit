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
        Task<List<string>> PendingWorkPartApiAsync(string strCalendario, string strUser,string strEntity);
    }
}
