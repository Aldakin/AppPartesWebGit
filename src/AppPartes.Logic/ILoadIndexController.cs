using System;
using System.Collections.Generic;
using System.Text;

namespace AppPartes.Logic
{
    public interface ILoadIndexController
    {
        MainDataViewLogic LoadMainController(int idAldakin);
        WeekDataViewLogic LoadWeekController(int idAldakin,string strDate = "", string strAction = "", string strId = "");
        //WeekDataViewLogic LoadWeekController(string strDate = "", string strAction = "", string strId = "", int idAldakinUser);
    }
}
