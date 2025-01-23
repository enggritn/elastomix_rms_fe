using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using WMS_FE.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace WMS_FE.Controllers
{
    [SessionCheck]
    public class MovementController : Controller
    {
        public ActionResult Index()
        {
            if(Session["token"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            ViewBag.BaseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');
            ViewBag.Server = ConfigurationManager.AppSettings["server"].ToString();
            return View();
        }

        // GET: PRBoxApproval/Details/5
        public ActionResult Detail()
        {
            if (Session["token"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            //ViewBag.BaseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');
            //ViewBag.Server = ConfigurationManager.AppSettings["server"].ToString();

            ViewBag.Param = this.Request.QueryString["id"];
            ViewBag.Server = ConfigurationManager.AppSettings["server"].ToString();
            return View();
        }

        public async Task<ActionResult> ExportMovementToExcel(string date, string enddate, string warehouse)
        {
            if (string.IsNullOrEmpty(date) && string.IsNullOrEmpty(enddate) && string.IsNullOrEmpty(warehouse))
            {
                throw new Exception();
            }
            else
            {
                //get data api
                string Domain = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');
                string ApiAddress = ConfigurationManager.AppSettings["server"].ToString();

                HttpClient client = new HttpClient();
                #region Material Req Req
                Uri uri = new Uri(ApiAddress + string.Format("Api/Movement/GetDataReportMovement?date={0}&enddate={1}&warehouse={2}", date, enddate, warehouse));
                var response = await client.GetAsync(uri);
                string result = response.Content.ReadAsStringAsync().Result;
                MovementResponse res = JsonConvert.DeserializeObject<MovementResponse>(result);
                #endregion

                IEnumerable<MovementDetailReportDTO> listdetail = res.list;

                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
                workSheet.TabColor = System.Drawing.Color.Black;

                workSheet.Row(1).Height = 25;
                workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Row(1).Style.Font.Bold = true;
                workSheet.Cells[1, 1].Value = "No.";
                workSheet.Cells[1, 2].Value = "WHName";
                workSheet.Cells[1, 3].Value = "RMCode";
                workSheet.Cells[1, 4].Value = "RMName";
                workSheet.Cells[1, 5].Value = "InDate";
                workSheet.Cells[1, 6].Value = "ExpDate";
                workSheet.Cells[1, 7].Value = "LotNo";
                workSheet.Cells[1, 8].Value = "OriginBag";
                workSheet.Cells[1, 9].Value = "OriginFullBag";
                workSheet.Cells[1, 10].Value = "OriginTotal";
                workSheet.Cells[1, 11].Value = "OriginBinRack";
                workSheet.Cells[1, 12].Value = "PutawayBy";
                workSheet.Cells[1, 13].Value = "PutawayOn";
                workSheet.Cells[1, 14].Value = "Destination Bag";
                workSheet.Cells[1, 15].Value = "DestinationFullBag";
                workSheet.Cells[1, 16].Value = "DestinationTotal";
                workSheet.Cells[1, 17].Value = "DestinationBinRack";

                int recordIndex = 2;
                int recordNo = 1;
                foreach (MovementDetailReportDTO header in listdetail)
                {
                    workSheet.Cells[recordIndex, 1].Value = recordNo++;
                    workSheet.Cells[recordIndex, 2].Value = header.WHName;
                    workSheet.Cells[recordIndex, 3].Value = header.RMCode;
                    workSheet.Cells[recordIndex, 4].Value = header.RMName;
                    workSheet.Cells[recordIndex, 5].Value = header.InDate;
                    workSheet.Cells[recordIndex, 6].Value = header.ExpDate;
                    workSheet.Cells[recordIndex, 7].Value = header.LotNo;
                    workSheet.Cells[recordIndex, 8].Value = header.OriginBag;
                    workSheet.Cells[recordIndex, 9].Value = header.OriginFullBag;
                    workSheet.Cells[recordIndex, 10].Value = header.OriginTotal;
                    workSheet.Cells[recordIndex, 11].Value = header.OriginBinRack;
                    workSheet.Cells[recordIndex, 12].Value = header.PutawayBy;
                    workSheet.Cells[recordIndex, 13].Value = header.PutawayOn;
                    workSheet.Cells[recordIndex, 14].Value = header.DestinationBag;
                    workSheet.Cells[recordIndex, 15].Value = header.DestinationFullBag;
                    workSheet.Cells[recordIndex, 16].Value = header.DestinationTotal;
                    workSheet.Cells[recordIndex, 17].Value = header.DestinationBinRack;
                    recordIndex++;
                }

                String datedownload = DateTime.Now.ToString("yyyyMMddhhmmss");
                String fileName = String.Format("filename=Movement_{0}.xlsx", datedownload);

                for (int i = 1; i <= 18; i++)
                {
                    workSheet.Column(i).AutoFit();
                }

                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;" + fileName);
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
                return RedirectToAction("Movement");
            }
        }
    }
}
