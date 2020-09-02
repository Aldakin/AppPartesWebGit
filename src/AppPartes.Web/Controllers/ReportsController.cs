using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
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
        private readonly IWriteDataBase _iWriteDataBase;
        int _idAldakinUser;
        public ReportsController(IHostingEnvironment hostingEnvironment,IApplicationUserAldakin iApplicationUserAldakin, ILoadIndexController iLoadIndexController, IWorkPartInformation iWorkPartInformation, IWriteDataBase iWriteDataBase)
        {


            _hostingEnvironment = hostingEnvironment;


            _iApplicationUserAldakin = iApplicationUserAldakin;
            _iLoadIndexController = iLoadIndexController;
            _iWriteDataBase = iWriteDataBase;

        }
        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult TestDownloadOtherFile()
        {
            try
            {
                string sWebRootFolder = "";
                string sFileName = @"demo.xlsx";
                string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);
                FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
                if (file.Exists)
                {
                    file.Delete();
                    file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
                }
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    // add a new worksheet to the empty workbook
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Employee");
                    //First add the headers
                    worksheet.Cells[1, 1].Value = "ID";
                    worksheet.Cells[1, 2].Value = "Name";
                    worksheet.Cells[1, 3].Value = "Gender";
                    worksheet.Cells[1, 4].Value = "Salary (in $)";

                    //Add values
                    worksheet.Cells["A2"].Value = 1000;
                    worksheet.Cells["B2"].Value = "Jon";
                    worksheet.Cells["C2"].Value = "M";
                    worksheet.Cells["D2"].Value = 5000;

                    worksheet.Cells["A3"].Value = 1001;
                    worksheet.Cells["B3"].Value = "Graham";
                    worksheet.Cells["C3"].Value = "M";
                    worksheet.Cells["D3"].Value = 10000;

                    worksheet.Cells["A4"].Value = 1002;
                    worksheet.Cells["B4"].Value = "Jenny";
                    worksheet.Cells["C4"].Value = "F";
                    worksheet.Cells["D4"].Value = 5000;

                    package.Save(); //Save the workbook.
                }
                var result = PhysicalFile(Path.Combine(sWebRootFolder, sFileName), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

                Response.Headers["Content-Disposition"] = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = file.Name
                }.ToString();

                return result;

            }
            catch(Exception  ex)
            {

            }
            return Index();
        }


        public string Export()
        {
            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            string sFileName = @"demo.xlsx";
            string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            }
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage(file))
            {
                // add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Employee");
                //First add the headers
                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "Name";
                worksheet.Cells[1, 3].Value = "Gender";
                worksheet.Cells[1, 4].Value = "Salary (in $)";

                //Add values
                worksheet.Cells["A2"].Value = 1000;
                worksheet.Cells["B2"].Value = "Jon";
                worksheet.Cells["C2"].Value = "M";
                worksheet.Cells["D2"].Value = 5000;

                worksheet.Cells["A3"].Value = 1001;
                worksheet.Cells["B3"].Value = "Graham";
                worksheet.Cells["C3"].Value = "M";
                worksheet.Cells["D3"].Value = 10000;

                worksheet.Cells["A4"].Value = 1002;
                worksheet.Cells["B4"].Value = "Jenny";
                worksheet.Cells["C4"].Value = "F";
                worksheet.Cells["D4"].Value = 5000;

                package.Save(); //Save the workbook.
            }
            return URL;
        }




    }
}