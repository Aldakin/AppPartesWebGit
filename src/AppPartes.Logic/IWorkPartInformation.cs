using System;
using System.Collections.Generic;
using System.Text;

namespace AppPartes.Logic
{
    public interface IWorkPartInformation
    {
        List<SelectData> WeekHourResume(DateTime dtSelected);
        List<SelectData> SelectedCompanyReadOt(int iEntidad);
        List<SelectData> SelectedCompanyReadClient(int iEntidad);
        List<SelectData> SelectedClient(int iClient);
        List<SelectData> SelectedOt(int iOt);
        List<SelectData> ReadLevel1(int iData);
        List<SelectData> ReadLevel2(int iData, int iData2);
        List<SelectData> ReadLevelGeneral(int iData);
        List<SelectData> SelectedPayer(int iPayer, int iOt);
    }
}
