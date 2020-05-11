using System.Threading.Tasks;

namespace AppPartes.Logic
{
    public interface ILoadIndexController
    {
        Task<MainDataViewLogic> LoadMainControllerAsync(int idAldakinUser);
        Task<WeekDataViewLogic> LoadWeekControllerAsync(int idAldakin, string strDate = "", string strAction = "", string strId = "");
        Task<LoginDataViewLogic> LoadLoginControllerAsync();
        Task<MessageViewLogic> LoadMessageControllerAsync(int idAldakinUser, int idMessage);

    }
}
