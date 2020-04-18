using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AppPartes.Logic
{
    public interface ILoadIndexController
    {
        Task<MainDataViewLogic> LoadMainController(int idAldakinUser);
        Task<WeekDataViewLogic> LoadWeekController(int idAldakin,string strDate = "", string strAction = "", string strId = "");
        //WeekDataViewLogic LoadWeekController(string strDate = "", string strAction = "", string strId = "", int idAldakinUser);
    }
}
