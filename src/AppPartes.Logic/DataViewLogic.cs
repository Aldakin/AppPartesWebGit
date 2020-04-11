using System.Collections.Generic;
using AppPartes.Data.Models;

namespace AppPartes.Logic
{
    public class DataViewLogic
    {
        public List<Ots> listOts { set; get; }
        public List<Entidad> listCompany { set; get; }
        public List<Clientes> listClient { set; get; }
        public List<Pernoctaciones> listNight { set; get; }
    }
}
