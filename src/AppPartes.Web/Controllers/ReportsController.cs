using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using AppPartes.Logic;
using AppPartes.Web.Controllers.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

//https://www.talkingdotnet.com/import-export-xlsx-asp-net-core/


namespace AppPartes.Web.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {

        private readonly IHostingEnvironment _hostingEnvironment;

        private readonly ILoadIndexController _iLoadIndexController;
        private readonly IApplicationUserAldakin _iApplicationUserAldakin;
        private readonly IWorkPartInformation _iWorkPartInformation;
        int _idAldakinUser;
        public ReportsController(IHostingEnvironment hostingEnvironment, IApplicationUserAldakin iApplicationUserAldakin, ILoadIndexController iLoadIndexController, IWorkPartInformation iWorkPartInformation)
        {


            _hostingEnvironment = hostingEnvironment;

            _iWorkPartInformation = iWorkPartInformation;
            _iApplicationUserAldakin = iApplicationUserAldakin;
            _iLoadIndexController = iLoadIndexController;
            //_iWriteDataBase = iWriteDataBase;

        }
        public async Task<IActionResult> Index(string strMessage = "")
        {

            ViewBag.Message = strMessage;
            var oReturn = new ReportsViewLogic();
            _idAldakinUser = await _iApplicationUserAldakin.GetIdUserAldakin(HttpContext.User);
            oReturn = await _iLoadIndexController.LoadReportsAsync(_idAldakinUser);
            return View(oReturn);
        }

        //public IActionResult TestDownloadOtherFile()
        //{
        //    try
        //    {
        //        string sWebRootFolder = "";
        //        string sFileName = @"demo.xlsx";
        //        string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);
        //        FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
        //        if (file.Exists)
        //        {
        //            file.Delete();
        //            file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
        //        }
        //        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        //        using (ExcelPackage package = new ExcelPackage(file))
        //        {
        //            // add a new worksheet to the empty workbook
        //            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Employee");
        //            //First add the headers
        //            worksheet.Cells[1, 1].Value = "ID";
        //            worksheet.Cells[1, 2].Value = "Name";
        //            worksheet.Cells[1, 3].Value = "Gender";
        //            worksheet.Cells[1, 4].Value = "Salary (in $)";

        //            //Add values
        //            worksheet.Cells["A2"].Value = 1000;
        //            worksheet.Cells["B2"].Value = "Jon";
        //            worksheet.Cells["C2"].Value = "M";
        //            worksheet.Cells["D2"].Value = 5000;

        //            worksheet.Cells["A3"].Value = 1001;
        //            worksheet.Cells["B3"].Value = "Graham";
        //            worksheet.Cells["C3"].Value = "M";
        //            worksheet.Cells["D3"].Value = 10000;

        //            worksheet.Cells["A4"].Value = 1002;
        //            worksheet.Cells["B4"].Value = "Jenny";
        //            worksheet.Cells["C4"].Value = "F";
        //            worksheet.Cells["D4"].Value = 5000;

        //            package.Save(); //Save the workbook.
        //        }
        //        var result = PhysicalFile(Path.Combine(sWebRootFolder, sFileName), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

        //        Response.Headers["Content-Disposition"] = new ContentDispositionHeaderValue("attachment")
        //        {
        //            FileName = file.Name
        //        }.ToString();

        //        return result;

        //    }
        //    catch(Exception  ex)
        //    {

        //    }
        //    return Index();
        //}


        public async Task<string> Export(string strCalendario = "", string strEntidad = "")
        {
            int iRow = 1, iCodEnt = 1;
            DateTime dtSelected = DateTime.Now;
            _idAldakinUser = await _iApplicationUserAldakin.GetIdUserAldakin(HttpContext.User);
            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            string sFileName = @"demo.xlsx";
            string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            }
            List<Excel> lExcel = await _iWorkPartInformation.ReviewHourTypeHourAsync(iCodEnt, dtSelected);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage(file))
            {
                // add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Employee");
                //First add the headers
                //worksheet.Cells[1, 1].Value = "ID";
                //worksheet.Cells[1, 2].Value = "Name";
                worksheet.Cells[iRow, 1].Value = "Nombre:";
                worksheet.Cells[iRow, 2].Value = "Codigo Empleado:";
                worksheet.Cells[iRow, 3].Value = "OT";
                worksheet.Cells[iRow, 4].Value = "Desc. OT";
                worksheet.Cells[iRow, 5].Value = "Presupuesto";
                worksheet.Cells[iRow, 6].Value = "Capitulo";
                worksheet.Cells[iRow, 7].Value = "Desc. Capitulo";
                worksheet.Cells[iRow, 8].Value = "Anexo";
                worksheet.Cells[iRow, 9].Value = "Version";
                worksheet.Cells[iRow, 10].Value = "Horas";
                worksheet.Cells[iRow, 11].Value = "Tipo de Hora";

                //Add values
                //foreach(Excel e in lExcel)
                //{
                //    iRow++;
                //    worksheet.Cells[iRow, 1].Value = e.str1;
                //    worksheet.Cells[iRow, 2].Value = e.str2;
                //    worksheet.Cells[iRow, 3].Value = e.str3;
                //    worksheet.Cells[iRow, 4].Value = e.str4;
                //    worksheet.Cells[iRow, 5].Value = e.str5;
                //    worksheet.Cells[iRow, 6].Value = e.str6;
                //    worksheet.Cells[iRow, 7].Value = e.str7;
                //    worksheet.Cells[iRow, 8].Value = e.str8;
                //    worksheet.Cells[iRow, 9].Value = e.str9;
                //    worksheet.Cells[iRow, 10].Value = e.str10;
                //    worksheet.Cells[iRow, 11].Value = e.str11;
                //}



                ////worksheet.Cells["A2"].Value = 1000;
                ////worksheet.Cells["B2"].Value = "Jon";
                ////worksheet.Cells["C2"].Value = "M";
                ////worksheet.Cells["D2"].Value = 5000;

                ////worksheet.Cells["A3"].Value = 1001;
                ////worksheet.Cells["B3"].Value = "Graham";
                ////worksheet.Cells["C3"].Value = "M";
                ////worksheet.Cells["D3"].Value = 10000;

                ////worksheet.Cells["A4"].Value = 1002;
                ////worksheet.Cells["B4"].Value = "Jenny";
                ////worksheet.Cells["C4"].Value = "F";
                ////worksheet.Cells["D4"].Value = 5000;

                package.Save(); //Save the workbook.
            }
            return URL;
        }



        public async Task<IActionResult> ExportHoursPerMonth(string strCalendario = "", string strEntidad = "")
        {
            var oReturn = new ReportsViewLogic();
            try
            {
                int iRow = 1, iCodEnt = 0;
                DateTime dtSelected = DateTime.Now;
                try
                {
                    dtSelected = Convert.ToDateTime(strCalendario);
                    iCodEnt = Convert.ToInt32(strEntidad);
                }
                catch (Exception ex)
                {

                }
                oReturn = await _iLoadIndexController.LoadReportsAsync(_idAldakinUser);
                _idAldakinUser = await _iApplicationUserAldakin.GetIdUserAldakin(HttpContext.User);
                string user = await _iWorkPartInformation.GetUserNameAsync(_idAldakinUser);
                string sWebRootFolder = _hostingEnvironment.WebRootPath;
                string sFileName = @"Horas_" + _idAldakinUser + ".xlsx";
                string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);
                FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
                if (file.Exists)
                {
                    file.Delete();
                    file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
                }
                List<Excel> lExcel = await _iWorkPartInformation.ReviewHourMonthAsync(iCodEnt, dtSelected);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    // add a new worksheet to the empty workbook
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Horas");
                    //First add the headers
                    worksheet.Cells[1, 1].Value = user;
                    worksheet.Cells[1, 2].Value = "Creado:" + DateTime.Now.Date.ToString();
                    iRow = iRow + 1;
                    worksheet.Cells[iRow, 1].Value = "TRABAJADOR:";
                    worksheet.Cells[iRow, 2].Value = "FECHA:";
                    worksheet.Cells[iRow, 3].Value = "HORAS:";
                    //Add values
                    foreach (Excel e in lExcel)
                    {
                        iRow++;
                        worksheet.Cells[iRow, 1].Value = e.str1;
                        worksheet.Cells[iRow, 2].Value = e.str2;
                        worksheet.Cells[iRow, 3].Value = e.str3;
                    }



                    ////worksheet.Cells["A2"].Value = 1000;
                    ////worksheet.Cells["B2"].Value = "Jon";
                    ////worksheet.Cells["C2"].Value = "M";
                    ////worksheet.Cells["D2"].Value = 5000;

                    ////worksheet.Cells["A3"].Value = 1001;
                    ////worksheet.Cells["B3"].Value = "Graham";
                    ////worksheet.Cells["C3"].Value = "M";
                    ////worksheet.Cells["D3"].Value = 10000;

                    ////worksheet.Cells["A4"].Value = 1002;
                    ////worksheet.Cells["B4"].Value = "Jenny";
                    ////worksheet.Cells["C4"].Value = "F";
                    ////worksheet.Cells["D4"].Value = 5000;

                    package.Save(); //Save the workbook.
                }
                var result = PhysicalFile(Path.Combine(sWebRootFolder, sFileName), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

                Response.Headers["Content-Disposition"] = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = file.Name
                }.ToString();

                return result;
            }
            catch (Exception ex)
            {

            }
            return await Index("Reports");
        }
        public async Task<IActionResult> ExportHoursTypeHour(string strCalendario = "", string strEntidad = "")
        {
            var oReturn = new ReportsViewLogic();
            try
            {
                int iRow = 1, iCodEnt = 0;
                DateTime dtSelected = DateTime.Now;
                try
                {
                    dtSelected = Convert.ToDateTime(strCalendario);
                    iCodEnt = Convert.ToInt32(strEntidad);
                }
                catch (Exception ex)
                {

                }
                oReturn = await _iLoadIndexController.LoadReportsAsync(_idAldakinUser);
                _idAldakinUser = await _iApplicationUserAldakin.GetIdUserAldakin(HttpContext.User);
                string user = await _iWorkPartInformation.GetUserNameAsync(_idAldakinUser);
                string sWebRootFolder = _hostingEnvironment.WebRootPath;
                string sFileName = @"InformeHoras_" + _idAldakinUser + ".xlsx";
                string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);
                FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
                if (file.Exists)
                {
                    file.Delete();
                    file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
                }
                List<Excel> lExcel = await _iWorkPartInformation.ReviewHourTypeHourAsync(iCodEnt, dtSelected);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    // add a new worksheet to the empty workbook
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Horas");
                    //First add the headers
                    worksheet.Cells[1, 1].Value = user;
                    worksheet.Cells[1, 2].Value = "Creado:" + DateTime.Now.Date.ToString();
                    iRow = iRow + 1;
                    //        //worksheet.Cells[1, 1].Value = "ID";
                    //        //worksheet.Cells[1, 2].Value = "Name";
                    worksheet.Cells[iRow, 1].Value = "Nombre:";
                    worksheet.Cells[iRow, 2].Value = "Codigo Empleado:";
                    worksheet.Cells[iRow, 3].Value = "OT";
                    worksheet.Cells[iRow, 4].Value = "Desc. OT";
                    worksheet.Cells[iRow, 5].Value = "Presupuesto";
                    worksheet.Cells[iRow, 6].Value = "Capitulo";
                    worksheet.Cells[iRow, 7].Value = "Desc. Capitulo";
                    worksheet.Cells[iRow, 8].Value = "Anexo";
                    worksheet.Cells[iRow, 9].Value = "Version";
                    worksheet.Cells[iRow, 10].Value = "Horas";
                    worksheet.Cells[iRow, 11].Value = "Tipo de Hora";
                    //Add values
                    foreach (Excel e in lExcel)
                    {
                        iRow++;
                        worksheet.Cells[iRow, 1].Value = e.str1;
                        worksheet.Cells[iRow, 2].Value = e.str2;
                        worksheet.Cells[iRow, 3].Value = e.str3;
                        worksheet.Cells[iRow, 4].Value = e.str4;
                        worksheet.Cells[iRow, 5].Value = e.str5;
                        worksheet.Cells[iRow, 6].Value = e.str6;
                        worksheet.Cells[iRow, 7].Value = e.str7;
                        worksheet.Cells[iRow, 8].Value = e.str8;
                        worksheet.Cells[iRow, 9].Value = e.str9;
                        worksheet.Cells[iRow, 10].Value = e.str10;
                        worksheet.Cells[iRow, 11].Value = e.str11;
                    }



                    ////worksheet.Cells["A2"].Value = 1000;
                    ////worksheet.Cells["B2"].Value = "Jon";
                    ////worksheet.Cells["C2"].Value = "M";
                    ////worksheet.Cells["D2"].Value = 5000;

                    ////worksheet.Cells["A3"].Value = 1001;
                    ////worksheet.Cells["B3"].Value = "Graham";
                    ////worksheet.Cells["C3"].Value = "M";
                    ////worksheet.Cells["D3"].Value = 10000;

                    ////worksheet.Cells["A4"].Value = 1002;
                    ////worksheet.Cells["B4"].Value = "Jenny";
                    ////worksheet.Cells["C4"].Value = "F";
                    ////worksheet.Cells["D4"].Value = 5000;

                    package.Save(); //Save the workbook.
                }
                var result = PhysicalFile(Path.Combine(sWebRootFolder, sFileName), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

                Response.Headers["Content-Disposition"] = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = file.Name
                }.ToString();

                return result;
            }
            catch (Exception ex)
            {

            }
            return await Index("Reports");



        }

        //Ejemplo
        //public async Task<string> Export(string strCalendario = "", string strEntidad = "")
        //{
        //    int iRow = 1, iCodEnt = 1;
        //    DateTime dtSelected = DateTime.Now;
        //    _idAldakinUser = await _iApplicationUserAldakin.GetIdUserAldakin(HttpContext.User);
        //    string sWebRootFolder = _hostingEnvironment.WebRootPath;
        //    string sFileName = @"demo.xlsx";
        //    string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);
        //    FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
        //    if (file.Exists)
        //    {
        //        file.Delete();
        //        file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
        //    }
        //    List<Excel> lExcel = await _iWorkPartInformation.ReviewHourTypeHourAsync(iCodEnt, dtSelected);
        //    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        //    using (ExcelPackage package = new ExcelPackage(file))
        //    {
        //        // add a new worksheet to the empty workbook
        //        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Employee");
        //        //First add the headers
        //        //worksheet.Cells[1, 1].Value = "ID";
        //        //worksheet.Cells[1, 2].Value = "Name";
        //        worksheet.Cells[iRow, 1].Value = "Nombre:";
        //        worksheet.Cells[iRow, 2].Value = "Codigo Empleado:";
        //        worksheet.Cells[iRow, 3].Value = "OT";
        //        worksheet.Cells[iRow, 4].Value = "Desc. OT";
        //        worksheet.Cells[iRow, 5].Value = "Presupuesto";
        //        worksheet.Cells[iRow, 6].Value = "Capitulo";
        //        worksheet.Cells[iRow, 7].Value = "Desc. Capitulo";
        //        worksheet.Cells[iRow, 8].Value = "Anexo";
        //        worksheet.Cells[iRow, 9].Value = "Version";
        //        worksheet.Cells[iRow, 10].Value = "Horas";
        //        worksheet.Cells[iRow, 11].Value = "Tipo de Hora";

        //        //Add values
        //        //foreach(Excel e in lExcel)
        //        //{
        //        //    iRow++;
        //        //    worksheet.Cells[iRow, 1].Value = e.str1;
        //        //    worksheet.Cells[iRow, 2].Value = e.str2;
        //        //    worksheet.Cells[iRow, 3].Value = e.str3;
        //        //    worksheet.Cells[iRow, 4].Value = e.str4;
        //        //    worksheet.Cells[iRow, 5].Value = e.str5;
        //        //    worksheet.Cells[iRow, 6].Value = e.str6;
        //        //    worksheet.Cells[iRow, 7].Value = e.str7;
        //        //    worksheet.Cells[iRow, 8].Value = e.str8;
        //        //    worksheet.Cells[iRow, 9].Value = e.str9;
        //        //    worksheet.Cells[iRow, 10].Value = e.str10;
        //        //    worksheet.Cells[iRow, 11].Value = e.str11;
        //        //}



        //        ////worksheet.Cells["A2"].Value = 1000;
        //        ////worksheet.Cells["B2"].Value = "Jon";
        //        ////worksheet.Cells["C2"].Value = "M";
        //        ////worksheet.Cells["D2"].Value = 5000;

        //        ////worksheet.Cells["A3"].Value = 1001;
        //        ////worksheet.Cells["B3"].Value = "Graham";
        //        ////worksheet.Cells["C3"].Value = "M";
        //        ////worksheet.Cells["D3"].Value = 10000;

        //        ////worksheet.Cells["A4"].Value = 1002;
        //        ////worksheet.Cells["B4"].Value = "Jenny";
        //        ////worksheet.Cells["C4"].Value = "F";
        //        ////worksheet.Cells["D4"].Value = 5000;

        //        package.Save(); //Save the workbook.
        //    }
        //    return URL;
        //}
    }
}