using System.Threading.Tasks;

namespace AppPartes.Logic
{
    public interface ILoadIndexController
    {
        Task<MainDataViewLogic> LoadMainControllerAsync(int idAldakinUser);
        Task<WeekDataViewLogic> LoadWeekControllerAsync(int idAldakin, string strDate = "", string strAction = "", string strId = "");
        Task<LoginDataViewLogic> LoadLoginControllerAsync();
        Task<MessageViewLogic> LoadMessageControllerAsync(int idAldakinUser, int idMessage);
        Task<HomeDataViewLogic> LoadHomeControllerAsync(int idAldakinUser);
        Task<SearchViewLogic> LoadSearchControllerAsync(int idAldakinUser, string strDate, string strDate1, string strEntity, string action, string strOt, string strWorker);
        Task<SearchPendingViewLogic> SearchPendingControllerAsync(int idAldakinUser);
    }
}

