using System;
using System.Collections.Generic;
using System.Text;

namespace AppPartes.Logic
{
    public interface IWorkPartInformation
    {
        List<SelectData> WeekHourResume(DateTime dtSelected, int idAldakinUser);
        List<SelectData> SelectedCompanyReadOt(int iEntidad, int idAldakinUser);
        List<SelectData> SelectedCompanyReadClient(int iEntidad, int idAldakinUser);
        List<SelectData> SelectedClient(int iClient, int idAldakinUser);
        List<SelectData> SelectedOt(int iOt, int idAldakinUser);
        List<SelectData> ReadLevel1(int iData, int idAldakinUser);
        List<SelectData> ReadLevel2(int iData, int iData2, int idAldakinUser);
        List<SelectData> ReadLevelGeneral(int iData, int idAldakinUser);
        List<SelectData> SelectedPayer(int iPayer, int iOt, int idAldakinUser);
    }
}
