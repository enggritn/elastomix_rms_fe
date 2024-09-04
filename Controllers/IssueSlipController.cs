using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using WMS_FE.Models;
namespace WMS_FE.Controllers
{
    [SessionCheck]
    public class IssueSlipController : Controller
    {
        // GET: Supplier
        public ActionResult Index()
        {
            ViewBag.BaseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');
            ViewBag.Server = ConfigurationManager.AppSettings["server"].ToString();
            return View();
        }

        public ActionResult Create()
        {
            ViewBag.Server = ConfigurationManager.AppSettings["server"].ToString();
            return View();
        }

        public ActionResult Detail()
        {
            ViewBag.Param = this.Request.QueryString["id"];
            ViewBag.Server = ConfigurationManager.AppSettings["server"].ToString();
            return View();
        }

        public async Task<ActionResult> ExportIssueSlipToExcel(string date)
        {
            if (string.IsNullOrEmpty(date))
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
                Uri uri = new Uri(ApiAddress + string.Format("Api/IssueSlip/GetDataReportIssueSlip?date={0}", date));
                var response = await client.GetAsync(uri);
                string result = response.Content.ReadAsStringAsync().Result;
                IssueSlipResponse res = JsonConvert.DeserializeObject<IssueSlipResponse>(result);
                #endregion

                IEnumerable<IssueSlipDTOReport> listdetail = res.list;

                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
                workSheet.TabColor = System.Drawing.Color.Black;

                workSheet.Row(1).Height = 25;
                workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Row(1).Style.Font.Bold = true;
                workSheet.Cells[1, 1].Value = "No.";
                workSheet.Cells[1, 2].Value = "R/M CODE";
                workSheet.Cells[1, 3].Value = "R/M NAME";
                workSheet.Cells[1, 4].Value = "R/M VENDOR NAME";
                workSheet.Cells[1, 5].Value = "WT REQUESTED";
                workSheet.Cells[1, 6].Value = "SUPPLY QTY";
                workSheet.Cells[1, 7].Value = "FROM RACK NO.";
                workSheet.Cells[1, 8].Value = "EXP DATE";
                workSheet.Cells[1, 9].Value = "PICKED BY.";
                workSheet.Cells[1, 10].Value = "ACTUAL RETURN QTY";
                workSheet.Cells[1, 11].Value = "GO TO RACK NO.";
                workSheet.Cells[1, 12].Value = "PUT BY.";

                int recordIndex = 2;
                int recordNo = 1;
                String ProductionDate = "";
                foreach (IssueSlipDTOReport header in listdetail)
                {
                    workSheet.Cells[recordIndex, 1].Value = recordNo++;
                    workSheet.Cells[recordIndex, 2].Value = header.RM_Code;
                    workSheet.Cells[recordIndex, 3].Value = header.RM_Name;
                    workSheet.Cells[recordIndex, 4].Value = header.RM_VendorName;
                    workSheet.Cells[recordIndex, 5].Value = header.Wt_Request;
                    workSheet.Cells[recordIndex, 6].Value = header.SupplyQty;
                    workSheet.Cells[recordIndex, 7].Value = header.FromBinRackCode;
                    workSheet.Cells[recordIndex, 8].Value = header.ExpDate;
                    workSheet.Cells[recordIndex, 8].Style.Numberformat.Format = "dd/MM/yyyy";
                    workSheet.Cells[recordIndex, 9].Value = header.PickedBy;
                    workSheet.Cells[recordIndex, 10].Value = header.ReturnQty;
                    workSheet.Cells[recordIndex, 11].Value = header.ToBinRackCode;
                    workSheet.Cells[recordIndex, 12].Value = header.PutBy;
                    recordIndex++;

                    ProductionDate = header.Header_ProductionDate;
                }

                String datedownload = DateTime.Now.ToString("yyyyMMddhhmmss");
                String fileName = String.Format("filename=IssueSlip_{0}_{1}.xlsx", ProductionDate, datedownload);

                for (int i = 1; i <= 13; i++)
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
                return RedirectToAction("IssueSlip");
            }
        }

        public async Task<ActionResult> ExportDataInOutToExcel(string materialcode, string startdate, string enddate)
        {
            if (string.IsNullOrEmpty(materialcode) || string.IsNullOrEmpty(startdate) || string.IsNullOrEmpty(enddate))
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
                Uri uri = new Uri(ApiAddress + string.Format("Api/IssueSlip/GetDataReportDataInOut?materialcode={0}&startdate={1}&enddate={2}", materialcode, startdate, enddate));
                var response = await client.GetAsync(uri);
                string result = response.Content.ReadAsStringAsync().Result;
                DataInOutResponse res = JsonConvert.DeserializeObject<DataInOutResponse>(result);
                #endregion

                IEnumerable<DataInOutDTOReport> listdetail = res.list;

                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
                workSheet.TabColor = System.Drawing.Color.Black;

                workSheet.Row(1).Style.Font.Bold = true;
                workSheet.Row(2).Style.Font.Bold = true;
                workSheet.Row(3).Style.Font.Bold = true;
                workSheet.Cells[1, 1].Value = "Material Code";
                workSheet.Cells[1, 2].Value = ": " + materialcode;
                workSheet.Cells[2, 1].Value = "Start Date";
                workSheet.Cells[2, 2].Value = ": " + startdate;
                workSheet.Cells[3, 1].Value = "End Date";
                workSheet.Cells[3, 2].Value = ": " + enddate;

                workSheet.Row(4).Height = 25;
                workSheet.Row(4).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Row(4).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Row(4).Style.Font.Bold = true;
                workSheet.Cells[4, 1].Value = "No.";
                workSheet.Cells[4, 2].Value = "Date";
                workSheet.Cells[4, 3].Value = "Handheld";
                workSheet.Cells[4, 4].Value = "Document Type";
                workSheet.Cells[4, 5].Value = "Receive Qty";
                workSheet.Cells[4, 6].Value = "Issue Slip Qty";
                workSheet.Cells[4, 7].Value = "Balance Qty";

                int recordIndex = 5;
                int recordNo = 1;
                decimal sumreceiveqty = 0;
                decimal sumissueslipqty = 0;
                decimal sumbalanceqty = 0;
                foreach (DataInOutDTOReport header in listdetail)
                {
                    workSheet.Cells[recordIndex, 1].Value = recordNo++;
                    workSheet.Cells[recordIndex, 2].Value = header.Date;
                    workSheet.Cells[recordIndex, 3].Value = header.UserHanheld;
                    workSheet.Cells[recordIndex, 4].Value = header.Type;
                    workSheet.Cells[recordIndex, 5].Value = header.ReceiveQty.Substring(0, header.ReceiveQty.Length - 2);
                    workSheet.Cells[recordIndex, 6].Value = header.IssueSlipQty.Substring(0, header.IssueSlipQty.Length - 2); 
                    workSheet.Cells[recordIndex, 7].Value = header.BalanceQty.Substring(0, header.BalanceQty.Length - 2); 
                    recordIndex++;

                    sumreceiveqty = sumreceiveqty + Convert.ToDecimal(header.ReceiveQty);
                    sumissueslipqty = sumissueslipqty + Convert.ToDecimal(header.IssueSlipQty);
                    sumbalanceqty = sumbalanceqty + Convert.ToDecimal(header.BalanceQty);
                }

                workSheet.Row(recordIndex + 1).Style.Font.Bold = true;
                workSheet.Cells[recordIndex + 1, 4].Value = "Total";
                workSheet.Cells[recordIndex + 1, 5].Value = sumreceiveqty.ToString("N2");
                //workSheet.Cells[recordIndex + 1, 5].Style.Numberformat.Format = "#,##0";
                workSheet.Cells[recordIndex + 1, 6].Value = sumissueslipqty.ToString("N2");
                workSheet.Cells[recordIndex + 1, 7].Value = sumbalanceqty.ToString("N2");

                String datedownload = DateTime.Now.ToString("yyyyMMddhhmmss");
                String fileName = String.Format("filename=DataInOut_{0}_{1}.xlsx", materialcode, datedownload);

                for (int i = 1; i <= 8; i++)
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
                return RedirectToAction("IssueSlip");
            }
        }
    }
}