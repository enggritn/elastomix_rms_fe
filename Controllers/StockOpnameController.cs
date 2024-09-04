using System;
using System.Configuration;
using System.Web.Mvc;
using WMS_FE.Models;
using iText.Html2pdf;
using iText.Kernel.Pdf;
using iText.Layout;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Collections.Generic;
using System.Linq;
using Excel = Microsoft.Office.Interop.Excel;

namespace WMS_FE.Controllers
{
    [SessionCheck]
    public class StockOpnameController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            if (Session["token"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            ViewBag.Title = "StockOpname";
            ViewBag.Module = "StockOpname";
            ViewBag.Method = "Index";
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
            ViewBag.Title = "StockOpname";
            ViewBag.Module = "StockOpname";
            ViewBag.Method = "Create";
            ViewBag.Server = ConfigurationManager.AppSettings["server"].ToString();
            ViewBag.BaseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');
            return View();
        }

        public ActionResult Edit()
        {
            if (Session["token"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            ViewBag.Title = "StockOpname";
            ViewBag.Module = "StockOpname";
            ViewBag.Method = "Edit";
            ViewBag.Param = this.Request.QueryString["id"];
            ViewBag.Server = ConfigurationManager.AppSettings["server"].ToString();
            ViewBag.BaseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');
            return View();
        }

        public ActionResult Detail()
        {
            ViewBag.Param = this.Request.QueryString["id"];
            ViewBag.Server = ConfigurationManager.AppSettings["server"].ToString();
            return View();
        }

        public async Task<ActionResult> ExportStockOpnameToExcel(string id)
        {
            if (string.IsNullOrEmpty(id))
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
                Uri uri = new Uri(ApiAddress + string.Format("Api/StockOpname/GetDataStockOpname?id={0}", id));
                var response = await client.GetAsync(uri);
                string result = response.Content.ReadAsStringAsync().Result;
                StockOpnameResponse res = JsonConvert.DeserializeObject<StockOpnameResponse>(result);
                #endregion

                IEnumerable<StockOpnameDetailDTO> listdetail = res.list;

                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
                workSheet.TabColor = System.Drawing.Color.Black;

                workSheet.Row(1).Height = 25;
                workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Row(1).Style.Font.Bold = true;
                workSheet.Cells[1, 1].Value = "No.";
                workSheet.Cells[1, 2].Value = "Bin Rack Code";
                workSheet.Cells[1, 3].Value = "Material Code";
                workSheet.Cells[1, 4].Value = "Material Name";
                workSheet.Cells[1, 5].Value = "Lot No";
                workSheet.Cells[1, 6].Value = "In Date";
                workSheet.Cells[1, 7].Value = "Exp Date";
                workSheet.Cells[1, 8].Value = "Bag Qty";
                workSheet.Cells[1, 9].Value = "Qty Per Bag";
                workSheet.Cells[1, 10].Value = "Total Qty";
                workSheet.Cells[1, 11].Value = "Material Type";
                workSheet.Cells[1, 12].Value = "Scanned By";
                workSheet.Cells[1, 13].Value = "Scanned On";

                int recordIndex = 2;
                int recordNo = 1;
                String Code = "";
                foreach (StockOpnameDetailDTO header in listdetail)
                {
                    workSheet.Cells[recordIndex, 1].Value = recordNo++;
                    workSheet.Cells[recordIndex, 2].Value = header.BinRackCode;
                    workSheet.Cells[recordIndex, 3].Value = header.MaterialCode;
                    workSheet.Cells[recordIndex, 4].Value = header.MaterialName;
                    workSheet.Cells[recordIndex, 5].Value = header.LotNo.Replace("`","");
                    workSheet.Cells[recordIndex, 6].Value = header.InDate;
                    workSheet.Cells[recordIndex, 6].Style.Numberformat.Format = "dd/MM/yyyy";
                    workSheet.Cells[recordIndex, 7].Value = header.ExpDate;
                    workSheet.Cells[recordIndex, 7].Style.Numberformat.Format = "dd/MM/yyyy";
                    workSheet.Cells[recordIndex, 8].Value = header.BagQty;
                    workSheet.Cells[recordIndex, 9].Value = header.QtyPerBag;
                    workSheet.Cells[recordIndex, 10].Value = header.TotalQty;
                    workSheet.Cells[recordIndex, 11].Value = header.MaterialType;
                    workSheet.Cells[recordIndex, 12].Value = header.ScannedBy;
                    workSheet.Cells[recordIndex, 13].Value = header.ScannedOn;
                    workSheet.Cells[recordIndex, 13].Style.Numberformat.Format = "yyyy-MM-dd hh:mm:ss";
                    recordIndex++;

                    Code = header.Code;
                }

                String date = DateTime.Now.ToString("yyyyMMddhhmmss");
                String fileName = String.Format("filename=StockOpname_{0}_{1}.xlsx", Code, date);

                for (int i = 1; i <= 14; i++)
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
                return RedirectToAction("StockOpname");
            }
        }
    }
}