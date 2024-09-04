using iText.Html2pdf;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout;
using Newtonsoft.Json;
using NPOI.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WMS_FE.Models;
using ZXing;
using ZXing.QrCode;
using Rectangle = iText.Kernel.Geom.Rectangle;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Excel = Microsoft.Office.Interop.Excel;

namespace WMS_FE.Controllers
{
    [SessionCheck]
    public class InboundController : Controller
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


        public async Task<ActionResult> ExportReceivingToExcel(string date, string warehouse)
        {
            if (string.IsNullOrEmpty(date) && string.IsNullOrEmpty(warehouse))
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
                Uri uri = new Uri(ApiAddress + string.Format("Api/Inbound/GetDataReportOtherInbound?date={0}&warehouse={1}", date, warehouse));
                var response = await client.GetAsync(uri);
                string result = response.Content.ReadAsStringAsync().Result;
                InboundResponse res = JsonConvert.DeserializeObject<InboundResponse>(result);
                #endregion

                IEnumerable<OtherInboundDetailReportDTO> listdetail = res.list;

                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
                workSheet.TabColor = System.Drawing.Color.Black;

                workSheet.Row(1).Height = 25;
                workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Row(1).Style.Font.Bold = true;
                workSheet.Cells[1, 1].Value = "No.";
                workSheet.Cells[1, 2].Value = "Receipt Date";
                workSheet.Cells[1, 3].Value = "Receipt No.";
                workSheet.Cells[1, 4].Value = "Warehouse Code";
                workSheet.Cells[1, 5].Value = "Warehouse Name";
                workSheet.Cells[1, 6].Value = "Material Code";
                workSheet.Cells[1, 7].Value = "Material Name";
                workSheet.Cells[1, 8].Value = "UOM";
                workSheet.Cells[1, 9].Value = "Qty (L)";
                workSheet.Cells[1, 10].Value = "Qty";
                workSheet.Cells[1, 11].Value = "Memo";

                int recordIndex = 2;
                int recordNo = 1;
                foreach (OtherInboundDetailReportDTO header in listdetail)
                {
                    workSheet.Cells[recordIndex, 1].Value = recordNo++;
                    workSheet.Cells[recordIndex, 2].Value = header.ReceiptDate.Substring(0,10);
                    workSheet.Cells[recordIndex, 2].Style.Numberformat.Format = "yyyy-MM-dd";
                    workSheet.Cells[recordIndex, 3].Value = header.ReceiptNo;
                    workSheet.Cells[recordIndex, 4].Value = header.WarehouseCode;
                    workSheet.Cells[recordIndex, 5].Value = header.WarehouseName;
                    workSheet.Cells[recordIndex, 6].Value = header.MaterialCode;
                    workSheet.Cells[recordIndex, 7].Value = header.MaterialName;
                    workSheet.Cells[recordIndex, 8].Value = header.Uom;
                    workSheet.Cells[recordIndex, 9].Value = header.QtyL;
                    workSheet.Cells[recordIndex, 10].Value = header.Qty;
                    workSheet.Cells[recordIndex, 11].Value = header.Memo;
                    recordIndex++;
                }

                String datedownload = DateTime.Now.ToString("yyyyMMddhhmmss");
                String fileName = String.Format("filename=OtherInbound_{0}.xlsx", date);

                for (int i = 1; i <= 12; i++)
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
                return RedirectToAction("Inbound");
            }
        }
    }
}