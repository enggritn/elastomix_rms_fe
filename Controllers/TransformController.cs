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
    public class TransformController : Controller
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


        public async Task<ActionResult> ExportTransformToExcel(string date, string enddate, string warehouse)
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
                Uri uri = new Uri(ApiAddress + string.Format("Api/Transform/GetDataReportTransform?date={0}&enddate={1}&warehouse={2}", date, enddate, warehouse));
                var response = await client.GetAsync(uri);
                string result = response.Content.ReadAsStringAsync().Result;
                TransformResponse res = JsonConvert.DeserializeObject<TransformResponse>(result);
                #endregion

                IEnumerable<TransformDetailReportDTO> listdetail = res.list;

                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
                workSheet.TabColor = System.Drawing.Color.Black;

                workSheet.Row(1).Height = 25;
                workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Row(1).Style.Font.Bold = true;
                workSheet.Cells[1, 1].Value = "No.";
                workSheet.Cells[1, 2].Value = "Document No";
                workSheet.Cells[1, 3].Value = "WHName";
                workSheet.Cells[1, 4].Value = "SourceRMCode";
                workSheet.Cells[1, 5].Value = "SourceRMName";
                workSheet.Cells[1, 6].Value = "TransformQty";
                workSheet.Cells[1, 7].Value = "TargetRMCode";
                workSheet.Cells[1, 8].Value = "TargetRMName";
                workSheet.Cells[1, 9].Value = "SourceBinRack";
                workSheet.Cells[1, 10].Value = "SourceInDate";
                workSheet.Cells[1, 11].Value = "SourceExpDate";
                workSheet.Cells[1, 12].Value = "SourceLotNo";
                workSheet.Cells[1, 13].Value = "SourceBag";
                workSheet.Cells[1, 14].Value = "SourceFullBag";
                workSheet.Cells[1, 15].Value = "SourceTotal";
                workSheet.Cells[1, 16].Value = "PickingBy";
                workSheet.Cells[1, 17].Value = "PickingOn";
                workSheet.Cells[1, 18].Value = "TargetBinRack";
                workSheet.Cells[1, 19].Value = "TargetInDate";
                workSheet.Cells[1, 20].Value = "TargetExpDate";
                workSheet.Cells[1, 21].Value = "TargetLotNo";
                workSheet.Cells[1, 22].Value = "TargetBag";
                workSheet.Cells[1, 23].Value = "TargetFullBag";
                workSheet.Cells[1, 24].Value = "TargetTotal";
                workSheet.Cells[1, 25].Value = "PutawayBy";
                workSheet.Cells[1, 26].Value = "PutawayOn";
                workSheet.Cells[1, 27].Value = "Status";
                workSheet.Cells[1, 28].Value = "Memo";

                int recordIndex = 2;
                int recordNo = 1;
                foreach (TransformDetailReportDTO header in listdetail)
                {
                    workSheet.Cells[recordIndex, 1].Value = recordNo++;
                    workSheet.Cells[recordIndex, 2].Value = header.DocumentNo;
                    workSheet.Cells[recordIndex, 3].Value = header.WHName;
                    workSheet.Cells[recordIndex, 4].Value = header.SourceRMCode;
                    workSheet.Cells[recordIndex, 5].Value = header.SourceRMName;
                    workSheet.Cells[recordIndex, 6].Value = header.TransformQty;
                    workSheet.Cells[recordIndex, 7].Value = header.TargetRMCode;
                    workSheet.Cells[recordIndex, 8].Value = header.TargetRMName;
                    workSheet.Cells[recordIndex, 9].Value = header.SourceBinRack;
                    workSheet.Cells[recordIndex, 10].Value = header.SourceInDate;
                    workSheet.Cells[recordIndex, 11].Value = header.SourceExpDate;
                    workSheet.Cells[recordIndex, 12].Value = header.SourceLotNo;
                    workSheet.Cells[recordIndex, 13].Value = header.SourceBag;
                    workSheet.Cells[recordIndex, 14].Value = header.SourceFullBag;
                    workSheet.Cells[recordIndex, 15].Value = header.SourceTotal;
                    workSheet.Cells[recordIndex, 16].Value = header.PickingBy;
                    workSheet.Cells[recordIndex, 17].Value = header.PickingOn;
                    workSheet.Cells[recordIndex, 18].Value = header.TargetBinRack;
                    workSheet.Cells[recordIndex, 19].Value = header.TargetInDate;
                    workSheet.Cells[recordIndex, 20].Value = header.TargetExpDate;
                    workSheet.Cells[recordIndex, 21].Value = header.TargetLotNo;
                    workSheet.Cells[recordIndex, 22].Value = header.TargetBag;
                    workSheet.Cells[recordIndex, 23].Value = header.TargetFullBag;
                    workSheet.Cells[recordIndex, 24].Value = header.TargetTotal;
                    workSheet.Cells[recordIndex, 25].Value = header.PutawayBy;
                    workSheet.Cells[recordIndex, 26].Value = header.PutawayOn;
                    workSheet.Cells[recordIndex, 27].Value = header.Status;
                    workSheet.Cells[recordIndex, 28].Value = header.Memo;
                    recordIndex++;
                }

                String datedownload = DateTime.Now.ToString("yyyyMMddhhmmss");
                String fileName = String.Format("filename=Transform_{0}.xlsx", datedownload);

                for (int i = 1; i <= 29; i++)
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
                return RedirectToAction("Transform");
            }
        }
    }
}
