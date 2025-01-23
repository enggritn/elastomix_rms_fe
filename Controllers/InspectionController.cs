using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using iText.Html2pdf;
using iText.Kernel.Pdf;
using iText.Layout;
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
    public class InspectionController : Controller
    {
        // GET: Supplier
        public ActionResult Index()
        {
            ViewBag.BaseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');
            ViewBag.Server = ConfigurationManager.AppSettings["server"].ToString();
            return View();
        }

        public ActionResult Detail()
        {
            ViewBag.Param = this.Request.QueryString["id"];
            ViewBag.Server = ConfigurationManager.AppSettings["server"].ToString();
            return View();
        }

        public ActionResult ReceivingRM()
        {
            ViewBag.BaseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');
            ViewBag.Server = ConfigurationManager.AppSettings["server"].ToString();
            return View();
        }

        public ActionResult ReceivingSFG()
        {
            ViewBag.BaseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');
            ViewBag.Server = ConfigurationManager.AppSettings["server"].ToString();
            return View();
        }

        public ActionResult Operation()
        {
            ViewBag.BaseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');
            ViewBag.Server = ConfigurationManager.AppSettings["server"].ToString();
            return View();
        }

        // GET: PDF
        public async Task<ActionResult> PDF(string id)
        {
            try
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
                    Uri uri = new Uri(ApiAddress + string.Format("Api/Receiving/GetDetailById?id={0}", id));
                    var response = await client.GetAsync(uri);
                    string result = response.Content.ReadAsStringAsync().Result;
                    ReceivingResponse res = JsonConvert.DeserializeObject<ReceivingResponse>(result);
                    #endregion

                    ReceivingDetailDTO receiveRequest = res.datadetail;

                    ViewBag.Title = "Document COA";
                    ViewBag.Logo = Domain + "/Content/img/logo.jpg";
                    //ViewBag.url = Domain + "/Content/captureCOA/";
                    //ViewBag.url = "http://localhost/WMS_BE/Content/captureCOA/"; 
                    ViewBag.url = "http://localhost/rms_api/Content/captureCOA/";
                    ViewBag.ext = ".jpg";
                    ViewBag.foto = "";

                    String body = Helper.RenderViewToString(this.ControllerContext, "View_Foto", receiveRequest);

                    #region Set dynamic table Approval
                    string strTbl = "tableApproval";
                    int strtIdx = body.IndexOf(strTbl) + strTbl.Length + 2;

                    string strTableApproval = "<tbody><tr>";

                    strTableApproval += "</tr><tr>";

                    strTableApproval += "</tr></tbody>";
                    string newBody = body;
                    #endregion
                    using (MemoryStream stream = new System.IO.MemoryStream())
                    {
                        using (var pdfWriter = new PdfWriter(stream))
                        {
                            PdfDocument pdf = new PdfDocument(pdfWriter);
                            Document document = new Document(pdf);
                            HtmlConverter.ConvertToPdf(newBody, pdf, null);
                            byte[] file = stream.ToArray();
                            MemoryStream output = new MemoryStream();
                            output.Write(file, 0, file.Length);
                            output.Position = 0;

                            Response.AddHeader("content-disposition", "inline; filename=form.pdf");
                            // Return the output stream
                            return File(output, "application/pdf");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return Content(string.Format(@"<body>
                       <script type='text/javascript'>
                        alert({0});
                         window.close();
                       </script>
                     </body> ", e.Message));
            }
        }

        public async Task<ActionResult> ExportInspectionToExcel(string date, string enddate, string warehouse)
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
                Uri uri = new Uri(ApiAddress + string.Format("Api/QCInspection/GetDataReportInspection?date={0}&enddate={1}&warehouse={2}", date, enddate, warehouse));
                var response = await client.GetAsync(uri);
                string result = response.Content.ReadAsStringAsync().Result;
                InspectionResponse res = JsonConvert.DeserializeObject<InspectionResponse>(result);
                #endregion

                IEnumerable<InspectionDetailReportDTO> listdetail = res.list;

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
                workSheet.Cells[1, 4].Value = "RMCode";
                workSheet.Cells[1, 5].Value = "RMName";
                workSheet.Cells[1, 6].Value = "InDate";
                workSheet.Cells[1, 7].Value = "ExpDate";
                workSheet.Cells[1, 8].Value = "LotNo";
                workSheet.Cells[1, 9].Value = "Bag";
                workSheet.Cells[1, 10].Value = "FullBag;";
                workSheet.Cells[1, 11].Value = "Total";
                workSheet.Cells[1, 12].Value = "CreatedBy";
                workSheet.Cells[1, 13].Value = "CreatedOn";
                workSheet.Cells[1, 14].Value = "PickingBag";
                workSheet.Cells[1, 15].Value = "PickingFullBag";
                workSheet.Cells[1, 16].Value = "PickingTotal";
                workSheet.Cells[1, 17].Value = "PickingBinRack";
                workSheet.Cells[1, 18].Value = "PickingBy";
                workSheet.Cells[1, 19].Value = "PickingOn";
                workSheet.Cells[1, 20].Value = "ActionExtendDuration";
                workSheet.Cells[1, 21].Value = "ActionExpDate";
                workSheet.Cells[1, 22].Value = "ActionDispose";
                workSheet.Cells[1, 23].Value = "ApproveBy";
                workSheet.Cells[1, 24].Value = "ApproveOn";
                workSheet.Cells[1, 25].Value = "PrintLabelBy";
                workSheet.Cells[1, 26].Value = "PrintLabelOn";
                workSheet.Cells[1, 27].Value = "PutawayBag";
                workSheet.Cells[1, 28].Value = "PutawayFullBag";
                workSheet.Cells[1, 29].Value = "PutawayTotal";
                workSheet.Cells[1, 30].Value = "PutawayBinRack";
                workSheet.Cells[1, 31].Value = "PutawayBy";
                workSheet.Cells[1, 32].Value = "PutawayOn";
                workSheet.Cells[1, 33].Value = "Status";
                workSheet.Cells[1, 34].Value = "Memo";

                int recordIndex = 2;
                int recordNo = 1;
                foreach (InspectionDetailReportDTO header in listdetail)
                {
                    workSheet.Cells[recordIndex, 1].Value = recordNo++;
                    workSheet.Cells[recordIndex, 2].Value = header.DocumentNo;
                    workSheet.Cells[recordIndex, 3].Value = header.WHName;
                    workSheet.Cells[recordIndex, 4].Value = header.RMCode;
                    workSheet.Cells[recordIndex, 5].Value = header.RMName;
                    workSheet.Cells[recordIndex, 6].Value = header.InDate;
                    workSheet.Cells[recordIndex, 7].Value = header.ExpDate;
                    workSheet.Cells[recordIndex, 8].Value = header.LotNo;
                    workSheet.Cells[recordIndex, 9].Value = header.Bag;
                    workSheet.Cells[recordIndex, 10].Value = header.FullBag;
                    workSheet.Cells[recordIndex, 11].Value = header.Total;
                    workSheet.Cells[recordIndex, 12].Value = header.CreatedBy;
                    workSheet.Cells[recordIndex, 13].Value = header.CreatedOn;
                    workSheet.Cells[recordIndex, 14].Value = header.PickingBag;
                    workSheet.Cells[recordIndex, 15].Value = header.PickingFullBag;
                    workSheet.Cells[recordIndex, 16].Value = header.PickingTotal;
                    workSheet.Cells[recordIndex, 17].Value = header.PickingBinRack;
                    workSheet.Cells[recordIndex, 18].Value = header.PickingBy;
                    workSheet.Cells[recordIndex, 19].Value = header.PickingOn;
                    workSheet.Cells[recordIndex, 20].Value = header.ActionExtendDuration;
                    workSheet.Cells[recordIndex, 21].Value = header.ActionExpDate;
                    workSheet.Cells[recordIndex, 22].Value = header.ActionDispose;
                    workSheet.Cells[recordIndex, 23].Value = header.ApproveBy;
                    workSheet.Cells[recordIndex, 24].Value = header.ApproveOn;
                    workSheet.Cells[recordIndex, 25].Value = header.PrintLabelBy;
                    workSheet.Cells[recordIndex, 26].Value = header.PrintLabelOn;
                    workSheet.Cells[recordIndex, 27].Value = header.PutawayBag;
                    workSheet.Cells[recordIndex, 28].Value = header.PutawayFullBag;
                    workSheet.Cells[recordIndex, 29].Value = header.PutawayTotal;
                    workSheet.Cells[recordIndex, 30].Value = header.PutawayBinRack;
                    workSheet.Cells[recordIndex, 31].Value = header.PutawayBy;
                    workSheet.Cells[recordIndex, 32].Value = header.PutawayOn;
                    workSheet.Cells[recordIndex, 33].Value = header.Status;
                    workSheet.Cells[recordIndex, 34].Value = header.Memo;
                    recordIndex++;
                }

                String datedownload = DateTime.Now.ToString("yyyyMMddhhmmss");
                String fileName = String.Format("filename=Inspection_{0}.xlsx", datedownload);

                for (int i = 1; i <= 35; i++)
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
                return RedirectToAction("Inspection");
            }
        }


        public async Task<ActionResult> ExportShelfLifeExtensionToExcel(string date, string enddate)
        {
            if (string.IsNullOrEmpty(date) && string.IsNullOrEmpty(enddate))
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
                Uri uri = new Uri(ApiAddress + string.Format("Api/QCInspection/GetDataReportShelfLifeExtension?date={0}&enddate={1}", date, enddate));
                var response = await client.GetAsync(uri);
                string result = response.Content.ReadAsStringAsync().Result;
                InspectionResponse res = JsonConvert.DeserializeObject<InspectionResponse>(result);
                #endregion

                IEnumerable<ShelfLifeExtensionReportDTO> listdetail = res.list2;

                string InspectedOn = "";
                int count = 0;
                foreach (ShelfLifeExtensionReportDTO getdata in listdetail)
                {
                    if (count == 0)
                    {
                        InspectedOn = getdata.InspectedOn;
                    }
                }

                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
                workSheet.TabColor = System.Drawing.Color.Black;

                // Menambahkan "Documented Instructions" di B2
                workSheet.Cells[2, 2].Value = "Documented Instructions";
                workSheet.Cells[2, 2].Style.Font.Bold = true;
                workSheet.Cells[2, 2].Style.Font.UnderLine = true;
                workSheet.Cells[2, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Cells[2, 2, 2, 15].Merge = true;
                workSheet.Row(2).Height = 20;

                // Menambahkan jarak
                workSheet.Row(3).Height = 10;

                workSheet.Cells[4, 11].Value = "Date of issue";
                workSheet.Cells[4, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Cells[4, 11].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Cells[4, 11].Style.Font.Size = 7;
                workSheet.Cells[4, 11, 4, 11].Merge = true;
                workSheet.Row(4).Height = 10;

                workSheet.Cells[5, 2].Value = "Purchasing & Logistics";
                workSheet.Cells[5, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Cells[5, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Cells[5, 2].Style.Font.Size = 8;
                workSheet.Cells[5, 2, 5, 6].Merge = true;
                workSheet.Row(5).Height = 10;

                workSheet.Cells[5, 7].Value = "Warehouse persons";
                workSheet.Cells[5, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Cells[5, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Cells[5, 7].Style.Font.Size = 8;
                workSheet.Cells[5, 7, 5, 9].Merge = true;
                workSheet.Row(5).Height = 10;

                workSheet.Cells[5, 11].Value = "Charge";
                workSheet.Cells[5, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Cells[5, 11].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Cells[5, 11].Style.Font.Size = 7;
                workSheet.Cells[5, 11, 5, 11].Merge = true;
                workSheet.Row(5).Height = 10;

                workSheet.Cells[5, 12].Value = "Approved";
                workSheet.Cells[5, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Cells[5, 12].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Cells[5, 12].Style.Font.Size = 7;
                workSheet.Cells[5, 12, 5, 13].Merge = true;
                workSheet.Row(5).Height = 10;

                // Menambahkan jarak
                workSheet.Row(10).Height = 10;

                workSheet.Cells[11, 2].Value = "Purchasing Manager";
                workSheet.Cells[11, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Cells[11, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Cells[11, 2].Style.Font.Size = 7;
                workSheet.Cells[11, 2, 11, 2].Merge = true;
                workSheet.Row(11).Height = 10;

                workSheet.Cells[11, 3].Value = "Asst. Manager";
                workSheet.Cells[11, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Cells[11, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Cells[11, 3].Style.Font.Size = 7;
                workSheet.Cells[11, 3, 11, 3].Merge = true;
                workSheet.Row(11).Height = 10;

                workSheet.Cells[11, 4].Value = "Staff";
                workSheet.Cells[11, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Cells[11, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Cells[11, 4].Style.Font.Size = 7;
                workSheet.Cells[11, 4, 11, 4].Merge = true;
                workSheet.Row(11).Height = 10;

                workSheet.Cells[11, 5].Value = "Staff";
                workSheet.Cells[11, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Cells[11, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Cells[11, 5].Style.Font.Size = 7;
                workSheet.Cells[11, 5, 11, 5].Merge = true;
                workSheet.Row(11).Height = 10;

                workSheet.Cells[11, 6].Value = "Staff";
                workSheet.Cells[11, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Cells[11, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Cells[11, 6].Style.Font.Size = 7;
                workSheet.Cells[11, 6, 11, 6].Merge = true;
                workSheet.Row(11).Height = 10;

                // Menambahkan jarak
                workSheet.Row(12).Height = 10;

                workSheet.Cells[13, 2].Value = "Subject Title";
                workSheet.Cells[13, 2].Style.Font.Bold = true;
                workSheet.Cells[13, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Cells[13, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Cells[13, 2].Style.Font.Size = 10;
                workSheet.Cells[13, 2].Style.WrapText = true;
                workSheet.Cells[13, 2, 16, 2].Merge = true;
                workSheet.Row(13).Height = 10;

                workSheet.Cells[13, 3].Value = "Shelf-life extension\n" +
                                               "of raw materials";
                workSheet.Cells[13, 3].Style.Font.Bold = true;
                workSheet.Cells[13, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Cells[13, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Cells[13, 3].Style.Font.Size = 10;
                workSheet.Cells[13, 3].Style.WrapText = true;
                workSheet.Cells[13, 3, 16, 8].Merge = true;
                workSheet.Row(13).Height = 10;

                workSheet.Cells[13, 9].Value = "Enforcing\n" +
                                               "Date";
                workSheet.Cells[13, 9].Style.Font.Bold = true;
                workSheet.Cells[13, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Cells[13, 9].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Cells[13, 9].Style.Font.Size = 8;
                workSheet.Cells[13, 9].Style.WrapText = true;
                workSheet.Cells[13, 9, 14, 9].Merge = true;
                workSheet.Row(13).Height = 10;

                workSheet.Cells[13, 10].Value = InspectedOn;
                workSheet.Cells[13, 10].Style.Font.Bold = true;
                workSheet.Cells[13, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Cells[13, 10].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Cells[13, 10].Style.Font.Size = 8;
                workSheet.Cells[13, 10].Style.WrapText = true;
                workSheet.Cells[13, 10, 14, 13].Merge = true;
                workSheet.Row(13).Height = 10;

                workSheet.Cells[15, 9].Value = "Term of\n" +
                                               "Validity";
                workSheet.Cells[15, 9].Style.Font.Bold = true;
                workSheet.Cells[15, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Cells[15, 9].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Cells[15, 9].Style.Font.Size = 8;
                workSheet.Cells[15, 9].Style.WrapText = true;
                workSheet.Cells[15, 9, 16, 9].Merge = true;
                workSheet.Row(15).Height = 10;

                workSheet.Cells[17, 2].Value = "The reason for enforcement (what why) ";
                workSheet.Cells[17, 2].Style.Font.Bold = true;
                workSheet.Cells[17, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Cells[17, 2].Style.Indent = 2;
                workSheet.Cells[17, 2].Style.Font.Size = 8;
                workSheet.Cells[17, 2, 17, 15].Merge = true;
                workSheet.Row(17).Height = 10;

                workSheet.Cells[18, 2].Value = "what :";
                workSheet.Cells[18, 2].Style.Font.Bold = true;
                workSheet.Cells[18, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Cells[18, 2].Style.Font.Size = 8;
                workSheet.Cells[18, 2, 18, 15].Merge = true;
                workSheet.Row(18).Height = 10;

                workSheet.Cells[19, 2].Value = "Extend the expiration date of the raw materials according from Purchasing & Logistic information.\n" +
                                              "[Perpanjangan expire date raw material berdasarkan informasi Purchasing & Logistic]";
                // Menyusun teks agar tampil dengan wrap text
                workSheet.Cells[19, 2].Style.Font.Bold = true;
                workSheet.Cells[19, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Cells[19, 2].Style.WrapText = true; // Mengaktifkan wrap text agar teks dibungkus dengan benar
                workSheet.Cells[19, 2, 19, 15].Merge = true;
                workSheet.Row(19).Height = 40;

                workSheet.Cells[20, 2].Value = "why :";
                workSheet.Cells[20, 2].Style.Font.Bold = true;
                workSheet.Cells[20, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Cells[20, 2].Style.Font.Size = 8;
                workSheet.Cells[20, 2, 20, 15].Merge = true;
                workSheet.Row(20).Height = 10;

                workSheet.Cells[21, 2].Value = "Current stock is expired lot.";
                workSheet.Cells[21, 2].Style.Font.Bold = true;
                workSheet.Cells[21, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Cells[21, 2, 22, 15].Merge = true;
                workSheet.Row(21).Height = 25;

                workSheet.Row(22).Height = 10; 

                workSheet.Cells[23, 2].Value = "Equipment used and charge (who where)";
                workSheet.Cells[23, 2].Style.Font.Bold = true;
                workSheet.Cells[23, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Cells[23, 2].Style.Font.Size = 9;
                // Menambahkan indentasi
                workSheet.Cells[23, 2].Style.Indent = 2; // Menambahkan indentasi 10 spasi/huruf
                workSheet.Cells[23, 2, 23, 15].Merge = true;
                workSheet.Row(23).Height = 10;


                workSheet.Cells[24, 2].Value = "Purchasing & Logistics persons";
                workSheet.Cells[24, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Cells[24, 2].Style.Font.Size = 9;
                workSheet.Cells[24, 2, 24, 15].Merge = true;
                workSheet.Row(24).Height = 10;

                workSheet.Cells[25, 2].Value = "Warehouse process persons";
                workSheet.Cells[25, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Cells[25, 2].Style.Font.Size = 9;
                workSheet.Cells[25, 2, 25, 15].Merge = true;
                workSheet.Row(25).Height = 10;

                workSheet.Row(26).Height = 10; 

                workSheet.Cells[27, 2].Value = "Methods of operation (how) ";
                workSheet.Cells[27, 2].Style.Font.Bold = true;
                workSheet.Cells[27, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Cells[27, 2].Style.Indent = 2;
                workSheet.Cells[27, 2].Style.Font.Size = 9;
                workSheet.Cells[27, 2, 27, 15].Merge = true;
                workSheet.Row(27).Height = 10;

                workSheet.Row(28).Height = 10;

                workSheet.Cells[29, 2].Value = "1. Berdasarkan informasi yang diterima,\n" +
                                               "   hasil judgment untuk R/M adalah sebagai berikut :";
                workSheet.Cells[29, 2].Style.Font.Bold = true;
                workSheet.Cells[29, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Cells[29, 2].Style.WrapText = true;
                workSheet.Cells[29, 2].Style.Indent = 2;
                workSheet.Cells[29, 2, 31, 15].Merge = true;
                workSheet.Row(29).Height = 10;

                // Menambahkan "Documented Instructions" di C32
                workSheet.Cells[32, 3].Value = "OK shelf-life extension after QC check/inspect actual VS applicable SDS or COA items.";
                workSheet.Cells[32, 3].Style.Font.Bold = true;
                workSheet.Cells[32, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Cells[32, 3, 32, 15].Merge = true;
                workSheet.Row(32).Height = 10;

                workSheet.Row(33).Height = 25;
                workSheet.Row(33).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Row(33).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Row(33).Style.Font.Size = 9;
                workSheet.Row(33).Style.Font.Bold = true;
                workSheet.Cells[33, 3].Value = "R/M Name";
                workSheet.Cells[33, 4].Value = "Actual Incoming Date";
                workSheet.Cells[33, 5].Value = "R/M LOT";
                workSheet.Cells[33, 6].Value = "Total (Kg)";
                workSheet.Cells[33, 7].Value = "Expired Date";
                workSheet.Cells[33, 8].Value = "Extension";
                workSheet.Cells[33, 9].Value = "Remark";
                workSheet.Cells[33, 10].Value = "Shelf Life Base On COA";
                workSheet.Cells[33, 11].Value = "Note";

                // Set background color (light blue)
                var lightBlue = System.Drawing.ColorTranslator.FromHtml("#ADD8E6");
                for (int i = 3; i <= 11; i++)
                {
                    workSheet.Cells[33, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    workSheet.Cells[33, i].Style.Fill.BackgroundColor.SetColor(lightBlue);

                    // Set border styles for the header cells
                    workSheet.Cells[33, i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[33, i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[33, i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[33, i].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    // Enable wrap text for the header cells
                    workSheet.Cells[33, i].Style.WrapText = true;
                }

                int recordIndex = 34;
                foreach (ShelfLifeExtensionReportDTO header in listdetail)
                {
                    workSheet.Cells[recordIndex, 3].Value = header.RMName;
                    workSheet.Cells[recordIndex, 4].Value = header.InDate;
                    workSheet.Cells[recordIndex, 5].Value = header.LotNo;
                    workSheet.Cells[recordIndex, 6].Value = header.Qty;
                    workSheet.Cells[recordIndex, 7].Value = header.ExpiredDate;
                    workSheet.Cells[recordIndex, 8].Value = header.Extension;
                    workSheet.Cells[recordIndex, 9].Value = header.Remark;
                    workSheet.Cells[recordIndex, 10].Value = header.ShelfLifeBaseOnCOA;
                    workSheet.Cells[recordIndex, 11].Value = header.Note;

                    // Menambahkan warna huruf (font color) pada cell InDate dan Extension
                    workSheet.Cells[recordIndex, 4].Style.Font.Color.SetColor(System.Drawing.Color.Red); 
                    workSheet.Cells[recordIndex, 8].Style.Font.Color.SetColor(System.Drawing.Color.Red);

                    // Cek apakah Extension adalah "Dispose"
                    if (header.Extension == "Dispose")
                    {
                        // Set background color kuning untuk kolom 3 hingga 11
                        var yellow = System.Drawing.ColorTranslator.FromHtml("#FFFF00");
                        for (int i = 3; i <= 11; i++)
                        {
                            workSheet.Cells[recordIndex, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            workSheet.Cells[recordIndex, i].Style.Fill.BackgroundColor.SetColor(yellow);
                        }
                    }

                    // Menambahkan border untuk setiap data row
                    for (int i = 3; i <= 11; i++)
                    {
                        workSheet.Cells[recordIndex, i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[recordIndex, i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[recordIndex, i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[recordIndex, i].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        workSheet.Cells[recordIndex, i].Style.WrapText = true;
                        workSheet.Cells[recordIndex, i].Style.Font.Size = 9;
                        workSheet.Cells[recordIndex, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        workSheet.Cells[recordIndex, i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    }

                    recordIndex++;
                }               

                for (int i = 1; i <= 11; i++)
                {
                    workSheet.Column(i).AutoFit();
                }

                recordIndex += 2; // Menambah 5 baris dari baris terakhir yang digunakan (recordIndex terakhir)

                // Menambahkan teks di recordIndex + 5 (misalnya di kolom C)
                workSheet.Cells[recordIndex, 2].Value = "2. Mohon untuk memperpanjang masa expired/shelf-life di label QR sesuai list diatas.";
                workSheet.Cells[recordIndex, 2].Style.Font.Bold = true;
                workSheet.Cells[recordIndex, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Cells[recordIndex, 2].Style.Indent = 2;
                workSheet.Cells[recordIndex, 2, recordIndex + 1, 15].Merge = true; 
                workSheet.Row(recordIndex).Height = 10;

                recordIndex += 3; 

                workSheet.Cells[recordIndex, 2].Value = "3. Khusus DISPOSE, mohon ditindaklanjuti dengan membuat Disposal Approval Document.";
                workSheet.Cells[recordIndex, 2].Style.Font.Bold = true;
                workSheet.Cells[recordIndex, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Cells[recordIndex, 2].Style.Indent = 2;
                workSheet.Cells[recordIndex, 2, recordIndex + 1, 15].Merge = true;
                workSheet.Row(recordIndex).Height = 10;

                recordIndex += 1;

                // Setelah semua data dan AutoFit selesai, menambahkan border tebal pada rentang yang diinginkan
                int lastRow = recordIndex; // Baris terakhir yang terisi

                // Menambahkan border tebal hanya di sisi luar (di sekitar area yang diminta)
                for (int row = 13; row <= lastRow; row++)
                {
                    // Menambahkan border kiri (kolom B) dan kanan (kolom P) untuk setiap baris
                    workSheet.Cells[row, 2].Style.Border.Left.Style = ExcelBorderStyle.Thick;  // Kolom B
                    workSheet.Cells[row, 15].Style.Border.Right.Style = ExcelBorderStyle.Thick; // Kolom P
                }

                for (int col = 2; col <= 15; col++)
                {
                    // Menambahkan border atas (baris 13) dan bawah (baris terakhir)
                    workSheet.Cells[13, col].Style.Border.Top.Style = ExcelBorderStyle.Thick;   // Baris 13
                    workSheet.Cells[lastRow, col].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;  // Baris terakhir
                }

                // Menambahkan garis tipis dari kolom B (2) hingga P (16) pada baris 16, 22, dan 26
                foreach (int row in new int[] { 16, 22, 26 })
                {
                    for (int col = 2; col <= 15; col++)
                    {
                        workSheet.Cells[row, col].Style.Border.Bottom.Style = ExcelBorderStyle.Thin; // Garis tipis horizontal
                    }
                }

                foreach (int row in new int[] { 4, 5, 11 })
                {
                    for (int col = 2; col <= 9; col++)
                    {
                        workSheet.Cells[row, col].Style.Border.Bottom.Style = ExcelBorderStyle.Thin; 
                    }
                }

                foreach (int row in new int[] { 3, 4, 5, 11 })
                {
                    for (int col = 11; col <= 13; col++)
                    {
                        workSheet.Cells[row, col].Style.Border.Bottom.Style = ExcelBorderStyle.Thin; 
                    }
                }

                foreach (int row in new int[] { 14})
                {
                    for (int col = 9; col <= 15; col++)
                    {
                        workSheet.Cells[row, col].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }
                }

                // Menambahkan garis vertikal tipis 
                foreach (int col in new int[] { 2, 3, 4, 5, 7, 8 , 12}) // Kolom B (2) sampai H (8)
                {
                    for (int row = 6; row <= 11; row++) // Baris 6 sampai 11
                    {
                        workSheet.Cells[row, col].Style.Border.Right.Style = ExcelBorderStyle.Thin; // Garis tipis vertikal di sebelah kanan setiap sel
                    }
                }

                foreach (int col in new int[] { 1, 6, 9 }) 
                {
                    for (int row = 5; row <= 11; row++) 
                    {
                        workSheet.Cells[row, col].Style.Border.Right.Style = ExcelBorderStyle.Thin; 
                    }
                }

                foreach (int col in new int[] { 10, 11, 13 }) 
                {
                    for (int row = 4; row <= 11; row++) 
                    {
                        workSheet.Cells[row, col].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }
                }

                foreach (int col in new int[] { 2, 8, 9 })
                {
                    for (int row = 13; row <= 16; row++)
                    {
                        workSheet.Cells[row, col].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }
                }

                // Set lebar kolom dari B (kolom 2) sampai O (kolom 15) menjadi 15
                for (int col = 2; col <= 15; col++)
                {
                    workSheet.Column(col).Width = 13;
                }

                for (int col = 14; col <= 15; col++)
                {
                    workSheet.Column(col).Width = 3;
                }

                String datedownload = DateTime.Now.ToString("yyyyMMddhhmmss");
                String fileName = String.Format("filename=ShelfLifeExtension_{0}.xlsx", datedownload);

                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;" + fileName);
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
                return RedirectToAction("Inspection");
            }
        }

        public async Task<ActionResult> ExportShelfLifeExtensionToExcel2(string date, string enddate)
        {
            if (string.IsNullOrEmpty(date) && string.IsNullOrEmpty(enddate))
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
                Uri uri = new Uri(ApiAddress + string.Format("Api/QCInspection/GetDataReportShelfLifeExtension?date={0}&enddate={1}", date, enddate));
                var response = await client.GetAsync(uri);
                string result = response.Content.ReadAsStringAsync().Result;
                InspectionResponse res = JsonConvert.DeserializeObject<InspectionResponse>(result);
                #endregion

                IEnumerable<ShelfLifeExtensionReportDTO> listdetail = res.list2;

                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
                workSheet.TabColor = System.Drawing.Color.Black;


                workSheet.Row(1).Height = 25;
                workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Row(1).Style.Font.Bold = true;
                workSheet.Cells[1, 1].Value = "No.";
                workSheet.Cells[1, 2].Value = "RMCode";
                workSheet.Cells[1, 3].Value = "RMName";
                workSheet.Cells[1, 4].Value = "InDate";
                workSheet.Cells[1, 5].Value = "LotNo";
                workSheet.Cells[1, 6].Value = "Qty";
                workSheet.Cells[1, 7].Value = "ExpiredDate";
                workSheet.Cells[1, 8].Value = "Extension";
                workSheet.Cells[1, 9].Value = "Remark";
                workSheet.Cells[1, 10].Value = "ShelfLifeBaseOnCOA;";
                workSheet.Cells[1, 11].Value = "Note";

                int recordIndex = 2;
                int recordNo = 1;
                foreach (ShelfLifeExtensionReportDTO header in listdetail)
                {
                    workSheet.Cells[recordIndex, 1].Value = recordNo++;
                    workSheet.Cells[recordIndex, 2].Value = header.RMCode;
                    workSheet.Cells[recordIndex, 3].Value = header.RMName;
                    workSheet.Cells[recordIndex, 4].Value = header.InDate;
                    workSheet.Cells[recordIndex, 5].Value = header.LotNo;
                    workSheet.Cells[recordIndex, 6].Value = header.Qty;
                    workSheet.Cells[recordIndex, 7].Value = header.ExpiredDate;
                    workSheet.Cells[recordIndex, 8].Value = header.Extension;
                    workSheet.Cells[recordIndex, 9].Value = header.Remark;
                    workSheet.Cells[recordIndex, 10].Value = header.ShelfLifeBaseOnCOA;
                    workSheet.Cells[recordIndex, 11].Value = header.Note;
                    recordIndex++;
                }

                String datedownload = DateTime.Now.ToString("yyyyMMddhhmmss");
                String fileName = String.Format("filename=ShelfLifeExtension_{0}.xlsx", datedownload);

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
                return RedirectToAction("Inspection");
            }
        }
    }
}