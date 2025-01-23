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
    public class OutboundController : Controller
    {
        // GET: Supplier
        public ActionResult Index()
        {
            if (Session["token"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            ViewBag.BaseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');
            ViewBag.Server = ConfigurationManager.AppSettings["server"].ToString();
            return View();
        }

        public ActionResult Create()
        {
            if (Session["token"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            ViewBag.Server = ConfigurationManager.AppSettings["server"].ToString();
            return View();
        }

        public ActionResult Detail()
        {
            if (Session["token"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            ViewBag.Param = this.Request.QueryString["id"];
            ViewBag.Server = ConfigurationManager.AppSettings["server"].ToString();
            return View();
        }

        public ActionResult Edit()
        {
            if (Session["token"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            ViewBag.Param = this.Request.QueryString["id"];
            ViewBag.Server = ConfigurationManager.AppSettings["server"].ToString();
            return View();
        }

        public async Task<ActionResult> ExportOutboundToExcel(string date, string enddate, string warehouse)
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
                Uri uri = new Uri(ApiAddress + string.Format("Api/Outbound/GetDataReportOtherOutbound?date={0}&enddate={1}&warehouse={2}", date, enddate, warehouse));

                var response = await client.GetAsync(uri);
                string result = response.Content.ReadAsStringAsync().Result;
                OutboundResponse res = JsonConvert.DeserializeObject<OutboundResponse>(result);
                #endregion

                IEnumerable<OutboundDetailReportDTO> listdetail = res.list;

                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
                workSheet.TabColor = System.Drawing.Color.Black;

                workSheet.Row(1).Height = 25;
                workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Row(1).Style.Font.Bold = true;
                workSheet.Cells[1, 1].Value = "No.";
                workSheet.Cells[1, 2].Value = "DocumentNo";
                workSheet.Cells[1, 3].Value = "WHName";
                workSheet.Cells[1, 4].Value = "RMCode";
                workSheet.Cells[1, 5].Value = "RMName";
                workSheet.Cells[1, 6].Value = "InDate";
                workSheet.Cells[1, 7].Value = "ExpDate";
                workSheet.Cells[1, 8].Value = "LotNo";
                workSheet.Cells[1, 9].Value = "Total";
                workSheet.Cells[1, 10].Value = "CreateBy";
                workSheet.Cells[1, 11].Value = "CreateOn";
                workSheet.Cells[1, 12].Value = "PickingBag";
                workSheet.Cells[1, 13].Value = "PickingFullBag";
                workSheet.Cells[1, 14].Value = "PickingTotal";
                workSheet.Cells[1, 15].Value = "PickingBinRack";
                workSheet.Cells[1, 16].Value = "PickingBy";
                workSheet.Cells[1, 17].Value = "PickingOn";
                workSheet.Cells[1, 18].Value = "PutawayBag";
                workSheet.Cells[1, 19].Value = "PutawayFullBag";
                workSheet.Cells[1, 20].Value = "PutawayTotal";
                workSheet.Cells[1, 21].Value = "PutawayBinRack";
                workSheet.Cells[1, 22].Value = "PutawayBy";
                workSheet.Cells[1, 23].Value = "PutawayOn";
                workSheet.Cells[1, 24].Value = "Status";
                workSheet.Cells[1, 25].Value = "Memo";

                int recordIndex = 2;
                int recordNo = 1;
                foreach (OutboundDetailReportDTO header in listdetail)
                {
                    workSheet.Cells[recordIndex, 1].Value = recordNo++;
                    workSheet.Cells[recordIndex, 2].Value = header.DocumentNo;
                    workSheet.Cells[recordIndex, 3].Value = header.WHName;
                    workSheet.Cells[recordIndex, 4].Value = header.RMCode;
                    workSheet.Cells[recordIndex, 5].Value = header.RMName;
                    workSheet.Cells[recordIndex, 6].Value = header.InDate;
                    workSheet.Cells[recordIndex, 7].Value = header.ExpDate;
                    workSheet.Cells[recordIndex, 8].Value = header.LotNo;
                    workSheet.Cells[recordIndex, 9].Value = header.Total;
                    workSheet.Cells[recordIndex, 10].Value = header.CreateBy;
                    workSheet.Cells[recordIndex, 11].Value = header.CreateOn;
                    workSheet.Cells[recordIndex, 12].Value = header.PickingBag;
                    workSheet.Cells[recordIndex, 13].Value = header.PickingFullBag;
                    workSheet.Cells[recordIndex, 14].Value = header.PickingTotal;
                    workSheet.Cells[recordIndex, 15].Value = header.PickingBinRack;
                    workSheet.Cells[recordIndex, 16].Value = header.PickingBy;
                    workSheet.Cells[recordIndex, 17].Value = header.PickingOn;
                    workSheet.Cells[recordIndex, 18].Value = header.PutawayBag;
                    workSheet.Cells[recordIndex, 19].Value = header.PutawayFullBag;
                    workSheet.Cells[recordIndex, 20].Value = header.PutawayTotal;
                    workSheet.Cells[recordIndex, 21].Value = header.PutawayBinRack;
                    workSheet.Cells[recordIndex, 22].Value = header.PutawayBy;
                    workSheet.Cells[recordIndex, 23].Value = header.PutawayOn;
                    workSheet.Cells[recordIndex, 24].Value = header.Status;
                    workSheet.Cells[recordIndex, 25].Value = header.Memo;
                    recordIndex++;
                }

                String datedownload = DateTime.Now.ToString("yyyyMMddhhmmss");
                String fileName = String.Format("filename=Outbound_{0}.xlsx", date);

                for (int i = 1; i <= 26; i++)
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
                return RedirectToAction("Outbound");
            }
        }
    }
}