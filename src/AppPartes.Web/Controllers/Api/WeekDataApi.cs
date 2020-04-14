using AppPartes.Logic;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppPartes.Web.Controllers.Api
{
    public class WeekDataApi : ControllerBase
    {
        private readonly IWorkPartInformation _IWorkPartInformation;
        private readonly IWriteDataBase _IWriteDataBase;
        private readonly ILoadIndexController _ILoadIndexController;
        public WeekDataApi(IWorkPartInformation iWorkPartInformation, IWriteDataBase iWriteDataBase, ILoadIndexController iLoadIndexController/*,UserManager<ApplicationUser> manager*/)
        {
            _IWorkPartInformation = iWorkPartInformation;
            _IWriteDataBase = iWriteDataBase;
            _ILoadIndexController = iLoadIndexController;
            //_manager = manager;
        }
        public List<SelectData> SelectPayer(int cantidad, int cantidad2)
        {
            var listaSelect = new List<SelectData>();
            try
            {
                listaSelect = _IWorkPartInformation.SelectedPayer(cantidad, cantidad2);
            }
            catch (Exception)
            {
                return null;
            }
            return listaSelect;
        }
        public async Task<List<SelectData>> DeleteLineFunction(int cantidad)
        {
            var lReturn = new List<SelectData>();
            lReturn = await _IWriteDataBase.DeleteWorkerLineAsync(cantidad);
            return lReturn;// RedirectToAction("Index", new { strMessage = "Parte Borrado Satisfactoriamente;", strDate = strReturn, strAction = "loadWeek" });
        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CloseWeek(string strDataSelected)
        {
            var oReturn = await _IWriteDataBase.CloseWorkerWeekAsync(strDataSelected);
            if (oReturn.iValue == 0)
            {
                return RedirectToAction("Index", new { strMessage = oReturn.strText, strDate = oReturn.strValue, strAction = "" });
            }
            else
            {
                return RedirectToAction("Index", new { strMessage = oReturn.strText, strDate = oReturn.strValue, strAction = "loadWeek" });
            }
        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLine(string strIdLinea, string ot, string strCalendario, string strHoraInicio, string strMinutoInicio, string strHoraFin, string strMinutoFin, string bHorasViaje, string bGastos, string strParte, string strPernoctacion, string strObservaciones, string strGastos)
        {
            var dataEditLine = new WorkerLineData
            {
                strIdLinea = strIdLinea,
                strOt = ot,
                strCalendario = strCalendario,
                strHoraInicio = strHoraInicio,
                strMinutoInicio = strMinutoInicio,
                strHoraFin = strHoraFin,
                strMinutoFin = strMinutoFin,
                bHorasViaje = bHorasViaje,
                bGastos = bGastos,
                strParte = strParte,
                strPernoctacion = strPernoctacion,
                strObservaciones = strObservaciones,
                strGastos = strGastos
            };
            var oReturn = await _IWriteDataBase.EditWorkerLineAsync(dataEditLine);
            if (oReturn.iValue == 0)
            {
                return RedirectToAction("Index", new { strMessage = oReturn.strText, strDate = oReturn.strValue, strAction = "" });
            }
            else
            {
                return RedirectToAction("Index", new { strMessage = oReturn.strText, strDate = oReturn.strValue, strAction = "loadWeek" });
            }
        }

    }
}
