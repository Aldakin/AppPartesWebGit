using System;
using System.Collections.Generic;
using System.Text;

namespace AppPartes.Logic
{
    public interface ILoadIndexController
    {
        MainDataViewLogic LoadMainController();
        WeekDataViewLogic LoadWeekController(string strDate = "", string strAction = "", string strId = "");
    }
}
