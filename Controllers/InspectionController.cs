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