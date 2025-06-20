﻿using iText.Html2pdf;
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
    public class ReceivingController : Controller
    {
        // GET: Receiving
        public ActionResult Index()
        {
            ViewBag.BaseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');
            ViewBag.Server = ConfigurationManager.AppSettings["server"].ToString();
            return View();
        }

        public ActionResult IndexWIP()
        {
            ViewBag.BaseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');
            ViewBag.Server = ConfigurationManager.AppSettings["server"].ToString();
            return View();
        }

        public ActionResult Edit()
        {
            ViewBag.BaseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');
            ViewBag.Param = this.Request.QueryString["id"];
            ViewBag.Server = ConfigurationManager.AppSettings["server"].ToString();
            return View();
        }

        public ActionResult Detail()
        {
            ViewBag.BaseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');
            ViewBag.Param = this.Request.QueryString["id"];
            ViewBag.Server = ConfigurationManager.AppSettings["server"].ToString();
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> GenerateLabel(string type, string id, int qty, int series, string printer)
        {
            string Domain = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');           

            string ApiAddress = ConfigurationManager.AppSettings["server"].ToString();
            ReceivingResponse res = new ReceivingResponse();
         
            try
            {

                HttpClient client = new HttpClient();
                Uri uri = new Uri(ApiAddress + string.Format("Api/Receiving/GetBarcodeById?type={0}&id={1}", type, id));
                var response = await client.GetAsync(uri);
                string result = response.Content.ReadAsStringAsync().Result;
                res = JsonConvert.DeserializeObject<ReceivingResponse>(result);
            }
            catch (Exception e)
            {

            }

            //var printerQuery = new ManagementObjectSearcher("SELECT * from Win32_Printer");
            //foreach (var printer in printerQuery.Get())
            //{
            //    var name = printer.GetPropertyValue("Name");
            //    var status = printer.GetPropertyValue("Status");
            //    var isDefault = printer.GetPropertyValue("Default");
            //    var isNetworkPrinter = printer.GetPropertyValue("Network");

            //    Console.WriteLine("{0} (Status: {1}, Default: {2}, Network: {3}",
            //                name, status, isDefault, isNetworkPrinter);
            //}

            //string printerName = "YourPrinterName";
            //string query = string.Format("SELECT * from Win32_Printer", printerName);

            //using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
            //using (ManagementObjectCollection coll = searcher.Get())
            //{
            //    try
            //    {
            //        foreach (ManagementObject printer in coll)
            //        {
            //            foreach (PropertyData property in printer.Properties)
            //            {
            //                Console.WriteLine(string.Format("{0}: {1}", property.Name, property.Value));
            //            }
            //        }
            //    }
            //    catch (ManagementException ex)
            //    {
            //        Console.WriteLine(ex.Message);
            //    }
            //}

            //var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Printer");
            //var results = searcher.Get();
            //foreach (var printer in results)
            //{
            //    var portName = printer.Properties["PortName"].Value;

            //    Console.WriteLine();
            //}

            //check printer ip
            var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_TCPIPPrinterPort where Name LIKE '" + printer + "'");
            var results = searcher.Get();

            if(results.Count > 0)
            {

            }
            else
            {
                return Content(@"<body>
                       <script type='text/javascript'>
                         window.close();
                       </script>
                     </body> ");
            }
            
            if (res == null)
            {
                return Content(@"<body>
                       <script type='text/javascript'>
                         window.close();
                       </script>
                     </body> ");
            }

            Receiving mdl = res.data;

            int seq = 0;

            decimal totalQty = Convert.ToDecimal(mdl.Qty);
            decimal qtyPerBag = Convert.ToDecimal(mdl.QtyPerBag);
            int fullBag = qty;
            seq = Convert.ToInt32(mdl.StartSeries);          

            List<string> bodies = new List<string>();

            for (int i = 0; i < fullBag; i++)
            {
                string runningNumber = "";
                if (series == 1)
                {
                    runningNumber = string.Format("{0:D5}", seq++);
                }
                else
                {
                    runningNumber = string.Format("{0:D5}", 1);
                }

                string qr1 = mdl.MaterialCode.PadRight(7) + " " + runningNumber + " " + mdl.QtyPerBag.PadLeft(6) + " " + mdl.LotNo;
                ViewBag.Field3 = Domain + "/" + GenerateQRCode(qr1);

                string inDate = "";
                string inDate2 = "";
                string inDate3 = "";
                string expiredDate = "";
                string expiredDate2 = "";
                if (!string.IsNullOrEmpty(mdl.InDate))
                {
                    try
                    {
                        DateTime dt = DateTime.ParseExact(mdl.InDate, "dd/MM/yyyy", null);
                        //DateTime dt = Convert.ToDateTime(mdl.InDate);
                        ViewBag.Field4 = dt.ToString("MMMM").ToUpper();
                        //inDate = dt.ToString("yyyyMMdd");
                        inDate = dt.ToString("yyyyMMdd").Substring(1);
                        inDate2 = dt.ToString("yyyMMdd");
                        inDate2 = inDate2.Substring(1);
                        inDate3 = dt.ToString("yyyy-MM-dd");
                    }
                    catch (Exception e)
                    {

                    }
                }

                if (!string.IsNullOrEmpty(mdl.ExpDate))
                {
                    try
                    {
                        DateTime dt = DateTime.ParseExact(mdl.ExpDate, "dd/MM/yyyy", null);
                        //DateTime dt = Convert.ToDateTime(mdl.ExpDate);
                        //expiredDate = dt.ToString("yyyyMMdd");
                        expiredDate = dt.ToString("yyyyMMdd").Substring(2);
                        expiredDate2 = dt.ToString("yyyy-MM-dd");
                    }
                    catch (Exception e)
                    {
                        //   string test = string.Format(@"<body>
                        //  <script type = 'text/javascript'>
                        //   alert('{0}');
                        //  </script>
                        //</body>", e.Message);
                        //   return Content(test);
                    }
                }
                string qr2 = mdl.MaterialCode.PadRight(7) + inDate + expiredDate;
                ViewBag.Field5 = mdl.LotNo;
                ViewBag.Field6 = Domain + "/" + GenerateQRCode(qr2);
                ViewBag.Field7 = mdl.RawMaterialMaker;
                ViewBag.Field8 = mdl.MaterialName;
                ViewBag.Field9 = mdl.QtyPerBag.ToString();
                ViewBag.Field10 = mdl.UoM.ToUpper();
                ViewBag.Field11 = inDate2;
                ViewBag.Field12 = mdl.MaterialCode;
                ViewBag.Field13 = inDate3;
                ViewBag.Field14 = expiredDate2;
                String body = RenderViewToString(this.ControllerContext, "Label", null);
                bodies.Add(body);
            }

            if (bodies.Count() < 1)
            {
                return Content(@"<body>
                       <script type='text/javascript'>
                         alert('No document created');
                         window.close();
                       </script>
                     </body> ");
            }

            using (MemoryStream stream = new MemoryStream())
            {
                using (var pdfWriter = new PdfWriter(stream))
                {

                    PdfDocument pdf = new PdfDocument(pdfWriter);
                    PdfMerger merger = new PdfMerger(pdf);
                    //loop in here, try
                    foreach (string body in bodies)
                    {
                        ByteArrayOutputStream baos = new ByteArrayOutputStream();
                        PdfDocument temp = new PdfDocument(new PdfWriter(baos));
                        Rectangle rectangle = new Rectangle(283.464566928f, 212.598425232f);
                        PageSize pageSize = new PageSize(rectangle);
                        Document document = new Document(temp, pageSize);
                        PdfPage page = temp.AddNewPage(pageSize);
                        HtmlConverter.ConvertToPdf(body, temp, null);
                        temp = new PdfDocument(new PdfReader(new ByteArrayInputStream(baos.ToByteArray())));
                        merger.Merge(temp, 1, temp.GetNumberOfPages());
                        temp.Close();
                    }
                    pdf.Close();

                    byte[] file = stream.ToArray();
                    MemoryStream output = new MemoryStream();
                    output.Write(file, 0, file.Length);
                    output.Position = 0;

                    Response.AddHeader("content-disposition", "inline; filename=form.pdf");
                    // Return the output stream

                    return File(output, "application/pdf");

                    ////print directly
                    ////Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    ////clientSocket.NoDelay = true;

                    ////IPAddress ip = IPAddress.Parse(printer);
                    ////IPEndPoint ipep = new IPEndPoint(ip, 9100);
                    ////clientSocket.Connect(ipep);


                    ////clientSocket.Send(file);
                    ////clientSocket.Close();

                    ////print directly method
                    ////1. Create PDF file, save it by ip address folder
                    ////2. Create a service apps (install it in server), to run print if file exist

                    //return Content(@"<body>
                    //   <script type='text/javascript'>
                    //     window.close();
                    //   </script>
                    // </body> ");
                }
            }
        }

        private static string RenderViewToString(ControllerContext context, String viewPath, object model = null)
        {
            context.Controller.ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindView(context, viewPath, null);
                var viewContext = new ViewContext(context, viewResult.View, context.Controller.ViewData, context.Controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(context, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        public static string MD5Hash(string input)
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }

        private string GenerateQRCode(string value)
        {
            string qr_path = MD5Hash(value);
            string folderPath = "/Content/img/qr_codes";
            string imagePath = folderPath + "/" + string.Format("{0}.png", qr_path);
            // If the directory doesn't exist then create it.
            if (!Directory.Exists(Server.MapPath("~" + folderPath)))
            {
                Directory.CreateDirectory(Server.MapPath("~" + folderPath));
            }

            var barcodeWriter = new BarcodeWriter();
            barcodeWriter.Format = BarcodeFormat.QR_CODE;
            barcodeWriter.Options = new QrCodeEncodingOptions
            {
                Width = 300,
                Height = 300
            };
            var result = barcodeWriter.Write(value);

            string barcodePath = Server.MapPath("~" + imagePath);
            var barcodeBitmap = new Bitmap(result);
            using (MemoryStream memory = new MemoryStream())
            {
                using (FileStream fs = new FileStream(barcodePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    barcodeBitmap.Save(memory, ImageFormat.Png);
                    byte[] bytes = memory.ToArray();
                    fs.Write(bytes, 0, bytes.Length);
                }
            }
            return imagePath;
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
                    Uri uri = new Uri(ApiAddress + string.Format("Api/MaterialRequest/ExcelDataById?id={0}", id));
                    var response = await client.GetAsync(uri);
                    string result = response.Content.ReadAsStringAsync().Result;
                    MaterialRequestResponse res = JsonConvert.DeserializeObject<MaterialRequestResponse>(result);
                    #endregion
                    #region Get Approval PRBox
                    uri = new Uri(ApiAddress + string.Format("api/pr-box-approval/{0}", res.data.SourceType));
                    response = await client.GetAsync(uri);
                    result = response.Content.ReadAsStringAsync().Result;
                    PRApprovalModelResponse rspApproval = JsonConvert.DeserializeObject<PRApprovalModelResponse>(result);
                    #endregion

                    MaterialRequestHeaderDTO purchaseRequest = res.data;

                    string viewName = "";
                    string title = "";

                    switch (res.data.SourceType)
                    {
                        case "VENDOR":
                            viewName = "PR_Vendor";
                            title = "PURCHASING REQUEST";
                            break;
                        case "IMPORT":
                            viewName = "PR_Vendor";
                            title = "PURCHASING REQUEST";
                            break;
                        case "CUSTOMER":
                            viewName = "PR_Customer";
                            title = "RM CUSTOMER SUPPLIED";
                            break;
                        case "OUTSOURCE":
                            viewName = "PR_Outsource";
                            title = "DELIVERY NOTE";
                            break;
                    };

                    ViewBag.Title = title;
                    ViewBag.Logo = Domain + "/Content/captureCOA/A006120.0002203080220308220903101.jpg";
                    String body = Helper.RenderViewToString(this.ControllerContext, viewName, purchaseRequest);

                    #region Set dynamic table Approval
                    string strTbl = "tableApproval";
                    int strtIdx = body.IndexOf(strTbl) + strTbl.Length + 2;

                    string strTableApproval = "<tbody><tr>";
                    if (res.data.SourceType == "OUTSOURCE")
                    {
                        strTableApproval += "<td rowspan='4' style='border: 1px solid;' width='20%' valign='top' align='left' height='80'><i><b><u>REMARK : </u></b></i></td>";

                    }
                    //set header
                    foreach (var item in rspApproval.Details)
                    {
                        string td = "<td style='border: 1px solid;' width='20%' align='center'><b>" + item.PRBoxTitle + "</b></td>";
                        strTableApproval += td;
                    }
                    strTableApproval += "</tr><tr>";

                    //set kotak kosong 
                    foreach (var item in rspApproval.Details)
                    {
                        string td = "<td style='border: 1px solid;' width='20%' align='center' height='80' valign='bottom'></td>";
                        strTableApproval += td;
                    }
                    strTableApproval += "</tr><tr>";
                    //set details -> untuk format standard menggunakan delimiter "/" untuk memisahakn nama dan jabatan
                    foreach (var item in rspApproval.Details)
                    {
                        string name = item.PRBoxName.Split('/')[0];
                        string td = "<td style='border: 1px solid;' width='20%' align='center' valign='bottom'><b>" + name + "</b></td>";
                        strTableApproval += td;
                    }
                    strTableApproval += "</tr></tr>";
                    //set jabatan
                    foreach (var item in rspApproval.Details)
                    {
                        string[] strName = item.PRBoxName.Split('/');
                        string jataban = string.Empty;
                        if (strName.Length > 1)
                        {
                            jataban = strName[1];
                        }
                        else
                        {
                            jataban = " ";
                        }

                        string td = "<td style='border: 1px solid;' width='20%' align='center' valign='bottom'><b>" + jataban + "</b></td>";
                        strTableApproval += td;
                    }

                    strTableApproval += "</tr></tbody>";
                    string newBody = body.Insert(strtIdx, strTableApproval);
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

        public async Task<ActionResult> ExportReceivingToExcel(string date, string warehouse, string sourcetype)
        {
            if (string.IsNullOrEmpty(date) && string.IsNullOrEmpty(warehouse) && string.IsNullOrEmpty(sourcetype))
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
                Uri uri = new Uri(ApiAddress + string.Format("Api/Receiving/GetDataReportReceiving?date={0}&warehouse={1}&sourcetype={2}", date, warehouse, sourcetype));
                var response = await client.GetAsync(uri);
                string result = response.Content.ReadAsStringAsync().Result;
                ReceivingResponse res = JsonConvert.DeserializeObject<ReceivingResponse>(result);
                #endregion

                IEnumerable<ReceivingDetailReportDTO> listdetail = res.list;

                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
                workSheet.TabColor = System.Drawing.Color.Black;

                workSheet.Row(1).Height = 25;
                workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Row(1).Style.Font.Bold = true;
                workSheet.Cells[1, 1].Value = "No.";
                workSheet.Cells[1, 2].Value = "Supplier";
                workSheet.Cells[1, 3].Value = "PO No.";
                workSheet.Cells[1, 4].Value = "Material Code";
                workSheet.Cells[1, 5].Value = "Material Name";
                workSheet.Cells[1, 6].Value = "In Date";
                workSheet.Cells[1, 7].Value = "Exp Date";
                workSheet.Cells[1, 8].Value = "Qty Per Bag";
                workSheet.Cells[1, 9].Value = "Qty Bag";
                workSheet.Cells[1, 10].Value = "Total";
                workSheet.Cells[1, 11].Value = "DO NO.";
                workSheet.Cells[1, 12].Value = "LOT NO.";
                workSheet.Cells[1, 13].Value = "OK";
                workSheet.Cells[1, 14].Value = "NG";
                workSheet.Cells[1, 15].Value = "Received By";
                workSheet.Cells[1, 16].Value = "Received On";

                int recordIndex = 2;
                int recordNo = 1;
                foreach (ReceivingDetailReportDTO header in listdetail)
                {
                    workSheet.Cells[recordIndex, 1].Value = recordNo++;
                    workSheet.Cells[recordIndex, 2].Value = header.SourceName;
                    workSheet.Cells[recordIndex, 3].Value = header.DocumentNo;
                    workSheet.Cells[recordIndex, 4].Value = header.MaterialCode;
                    workSheet.Cells[recordIndex, 5].Value = header.MaterialName;
                    workSheet.Cells[recordIndex, 6].Value = header.InDate;
                    workSheet.Cells[recordIndex, 6].Style.Numberformat.Format = "dd/MM/yyyy";
                    workSheet.Cells[recordIndex, 7].Value = header.ExpDate;
                    workSheet.Cells[recordIndex, 7].Style.Numberformat.Format = "dd/MM/yyyy";
                    workSheet.Cells[recordIndex, 8].Value = header.QtyPerBag;
                    workSheet.Cells[recordIndex, 9].Value = header.BagQty;
                    workSheet.Cells[recordIndex, 10].Value = header.Qty;
                    workSheet.Cells[recordIndex, 11].Value = header.DoNo;
                    workSheet.Cells[recordIndex, 12].Value = header.LotNo;
                    workSheet.Cells[recordIndex, 13].Value = header.OKQty;
                    workSheet.Cells[recordIndex, 14].Value = header.NGQty;
                    workSheet.Cells[recordIndex, 15].Value = header.ReceivedBy;
                    workSheet.Cells[recordIndex, 16].Value = header.ReceivedOn;
                    workSheet.Cells[recordIndex, 16].Style.Numberformat.Format = "yyyy-MM-dd hh:mm:ss";
                    recordIndex++;
                }

                String datedownload = DateTime.Now.ToString("yyyyMMddhhmmss");
                String fileName = String.Format("filename=Receiving_{0}.xlsx", date);

                for (int i = 1; i <= 17; i++)
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
                return RedirectToAction("Receiving");
            }
        }

        public async Task<ActionResult> ExportReceivingToExcel2(string date, string warehouse, string sourcetype)
        {
            if (string.IsNullOrEmpty(date) && string.IsNullOrEmpty(warehouse) && string.IsNullOrEmpty(sourcetype))
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
                Uri uri = new Uri(ApiAddress + string.Format("Api/Receiving/GetDataReportReceiving2?date={0}&warehouse={1}&sourcetype={2}", date, warehouse, sourcetype));
                var response = await client.GetAsync(uri);
                string result = response.Content.ReadAsStringAsync().Result;
                ReceivingResponse res = JsonConvert.DeserializeObject<ReceivingResponse>(result);
                #endregion

                IEnumerable<ReceivingDetailReportDTO2> listdetail = res.list2;

                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
                workSheet.TabColor = System.Drawing.Color.Black;

                workSheet.Row(1).Height = 25;
                workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Row(1).Style.Font.Bold = true;
                workSheet.Cells[1, 1].Value = "No.";
                workSheet.Cells[1, 2].Value = "Destination Name";
                workSheet.Cells[1, 3].Value = "PO No. (Ref Number)";
                workSheet.Cells[1, 4].Value = "Supplier (Source Name)";
                workSheet.Cells[1, 5].Value = "Code Of Raw Material";
                workSheet.Cells[1, 6].Value = "Name Of Raw Materials";
                workSheet.Cells[1, 7].Value = "In Date";
                workSheet.Cells[1, 8].Value = "Exp Date";
                workSheet.Cells[1, 9].Value = "Lot No.";
                workSheet.Cells[1, 10].Value = "/ Bag";
                workSheet.Cells[1, 11].Value = "Full Bag";
                workSheet.Cells[1, 12].Value = "Total";
                workSheet.Cells[1, 13].Value = "Area";
                workSheet.Cells[1, 14].Value = "Rack No.";
                workSheet.Cells[1, 15].Value = "Delivery Note Number";
                workSheet.Cells[1, 16].Value = "Status";

                int recordIndex = 2;
                int recordNo = 1;
                foreach (ReceivingDetailReportDTO2 header in listdetail)
                {
                    workSheet.Cells[recordIndex, 1].Value = recordNo++;
                    workSheet.Cells[recordIndex, 2].Value = header.DestinationName;
                    workSheet.Cells[recordIndex, 3].Value = header.RefNumber;
                    workSheet.Cells[recordIndex, 4].Value = header.SourceName;
                    workSheet.Cells[recordIndex, 5].Value = header.MaterialName;
                    workSheet.Cells[recordIndex, 6].Value = header.MaterialName;
                    workSheet.Cells[recordIndex, 7].Value = header.InDate;
                    workSheet.Cells[recordIndex, 7].Style.Numberformat.Format = "dd/MM/yyyy";
                    workSheet.Cells[recordIndex, 8].Value = header.ExpDate;
                    workSheet.Cells[recordIndex, 8].Style.Numberformat.Format = "dd/MM/yyyy";
                    workSheet.Cells[recordIndex, 9].Value = header.LotNo;
                    workSheet.Cells[recordIndex, 10].Value = header.PerBag;
                    workSheet.Cells[recordIndex, 11].Value = header.FullBag;
                    workSheet.Cells[recordIndex, 12].Value = header.Total;
                    workSheet.Cells[recordIndex, 13].Value = header.Area;
                    workSheet.Cells[recordIndex, 14].Value = header.RackNo;
                    workSheet.Cells[recordIndex, 15].Value = header.DoNo;
                    workSheet.Cells[recordIndex, 16].Value = header.TransactionStatus;
                    recordIndex++;
                }

                String datedownload = DateTime.Now.ToString("yyyyMMddhhmmss");
                String fileName = String.Format("filename=Receiving2_{0}.xlsx", date);

                for (int i = 1; i <= 17; i++)
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
                return RedirectToAction("Receiving2");
            }
        }

        public async Task<ActionResult> ExportReceivingToExcel3(string date, string warehouse, string sourcetype)
        {
            if (string.IsNullOrEmpty(date) && string.IsNullOrEmpty(warehouse) && string.IsNullOrEmpty(sourcetype))
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
                Uri uri = new Uri(ApiAddress + string.Format("Api/Receiving/GetDataReportReceiving3?date={0}&warehouse={1}&sourcetype={2}", date, warehouse, sourcetype));
                var response = await client.GetAsync(uri);
                string result = response.Content.ReadAsStringAsync().Result;
                ReceivingResponse res = JsonConvert.DeserializeObject<ReceivingResponse>(result);
                #endregion

                IEnumerable<ReceivingDetailReportDTO3> listdetail = res.list3;

                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
                workSheet.TabColor = System.Drawing.Color.Black;

                workSheet.Row(1).Height = 26;
                workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Row(1).Style.Font.Bold = true;
                workSheet.Cells[1, 1].Value = "No.";
                workSheet.Cells[1, 2].Value = "PO No. (Ref Number)";
                workSheet.Cells[1, 3].Value = "Supplier (Source Name)";
                workSheet.Cells[1, 4].Value = "Material Code";
                workSheet.Cells[1, 5].Value = "Material Name";
                workSheet.Cells[1, 6].Value = "Schedule";
                workSheet.Cells[1, 7].Value = "Total Qty PO";
                workSheet.Cells[1, 8].Value = "In Date";
                workSheet.Cells[1, 9].Value = "Exp Date";
                workSheet.Cells[1, 10].Value = "Lot No.";
                workSheet.Cells[1, 11].Value = "Qty Per Bag";
                workSheet.Cells[1, 12].Value = "Qty Bag";
                workSheet.Cells[1, 13].Value = "Total";
                workSheet.Cells[1, 14].Value = "Do No.";
                workSheet.Cells[1, 15].Value = "OK";
                workSheet.Cells[1, 16].Value = "NG Damage";
                workSheet.Cells[1, 17].Value = "COA";
                workSheet.Cells[1, 18].Value = "Status PO";
                workSheet.Cells[1, 19].Value = "Received By";
                workSheet.Cells[1, 20].Value = "Received On";
                workSheet.Cells[1, 21].Value = "Qty Putaway";
                workSheet.Cells[1, 22].Value = "Area";
                workSheet.Cells[1, 23].Value = "Rack No.";
                workSheet.Cells[1, 24].Value = "Status";
                workSheet.Cells[1, 25].Value = "Remarks";

                int recordIndex = 2;
                int recordNo = 1;
                foreach (ReceivingDetailReportDTO3 header in listdetail)
                {
                    workSheet.Cells[recordIndex, 1].Value = recordNo++;
                    workSheet.Cells[recordIndex, 2].Value = header.RefNumber;
                    workSheet.Cells[recordIndex, 3].Value = header.SourceName;
                    workSheet.Cells[recordIndex, 4].Value = header.MaterialCode;
                    workSheet.Cells[recordIndex, 5].Value = header.MaterialName;
                    workSheet.Cells[recordIndex, 6].Value = header.Schedule;
                    workSheet.Cells[recordIndex, 6].Style.Numberformat.Format = "dd/MM/yyyy";
                    workSheet.Cells[recordIndex, 7].Value = header.TotalQtyPo;
                    workSheet.Cells[recordIndex, 8].Value = header.InDate;
                    workSheet.Cells[recordIndex, 8].Style.Numberformat.Format = "dd/MM/yyyy";
                    workSheet.Cells[recordIndex, 9].Value = header.ExpDate;
                    workSheet.Cells[recordIndex, 9].Style.Numberformat.Format = "dd/MM/yyyy";
                    workSheet.Cells[recordIndex, 10].Value = header.LotNo;
                    workSheet.Cells[recordIndex, 11].Value = header.QtyPerBag;
                    workSheet.Cells[recordIndex, 12].Value = header.QtyBag;
                    workSheet.Cells[recordIndex, 13].Value = header.Total;
                    workSheet.Cells[recordIndex, 14].Value = header.DoNo;
                    workSheet.Cells[recordIndex, 15].Value = header.OK;
                    workSheet.Cells[recordIndex, 16].Value = header.NgDamage;
                    workSheet.Cells[recordIndex, 17].Value = header.COA;
                    workSheet.Cells[recordIndex, 18].Value = header.StatusPo;
                    workSheet.Cells[recordIndex, 19].Value = header.ReceivedBy;
                    workSheet.Cells[recordIndex, 20].Value = header.ReceivedOn;
                    workSheet.Cells[recordIndex, 20].Style.Numberformat.Format = "yyyy-MM-dd hh:mm:ss";
                    workSheet.Cells[recordIndex, 21].Value = header.QtyPutaway;
                    workSheet.Cells[recordIndex, 22].Value = header.Area;
                    workSheet.Cells[recordIndex, 23].Value = header.RackNo;
                    workSheet.Cells[recordIndex, 24].Value = header.Status;
                    workSheet.Cells[recordIndex, 25].Value = header.Remarks;

                    recordIndex++;
                }

                String datedownload = DateTime.Now.ToString("yyyyMMddhhmmss");
                String fileName = String.Format("filename=Receiving3_{0}.xlsx", date);

                for (int i = 1; i <= 25; i++)
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
                return RedirectToAction("Receiving3");
            }
        }

        public async Task<ActionResult> ExportReceivingToExcel4(string date, string enddate, string warehouse, string sourcetype)
        {
            if (string.IsNullOrEmpty(date) && string.IsNullOrEmpty(enddate) && string.IsNullOrEmpty(warehouse) && string.IsNullOrEmpty(sourcetype))
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
                Uri uri = new Uri(ApiAddress + string.Format("Api/Receiving/GetDataReportReceiving4?date={0}&enddate={1}&warehouse={2}&sourcetype={3}", date, enddate, warehouse, sourcetype));
                var response = await client.GetAsync(uri);
                string result = response.Content.ReadAsStringAsync().Result;
                ReceivingResponse res = JsonConvert.DeserializeObject<ReceivingResponse>(result);
                #endregion

                IEnumerable<ReceivingDetailReportDTO4> listdetail = res.list4;

                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
                workSheet.TabColor = System.Drawing.Color.Black;

                workSheet.Row(1).Height = 26;
                workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Row(1).Style.Font.Bold = true;
                workSheet.Cells[1, 1].Value = "No.";
                workSheet.Cells[1, 2].Value = "PO No.";
                workSheet.Cells[1, 3].Value = "Supplier";
                workSheet.Cells[1, 4].Value = "Material Code";
                workSheet.Cells[1, 5].Value = "Material Name";
                workSheet.Cells[1, 6].Value = "Schedule";
                workSheet.Cells[1, 7].Value = "Total Qty PO";
                workSheet.Cells[1, 8].Value = "In Date";
                workSheet.Cells[1, 9].Value = "Exp Date";
                workSheet.Cells[1, 10].Value = "Lot No.";
                workSheet.Cells[1, 11].Value = "Qty Per Bag";
                workSheet.Cells[1, 12].Value = "Qty Bag";
                workSheet.Cells[1, 13].Value = "Total";
                workSheet.Cells[1, 14].Value = "Do No.";
                workSheet.Cells[1, 15].Value = "OK";
                workSheet.Cells[1, 16].Value = "NG Damage";
                workSheet.Cells[1, 17].Value = "NG Wet";
                workSheet.Cells[1, 18].Value = "NG Contamination";
                workSheet.Cells[1, 19].Value = "COA";
                workSheet.Cells[1, 20].Value = "Status PO";
                workSheet.Cells[1, 21].Value = "Received By";
                workSheet.Cells[1, 22].Value = "Received On";
                workSheet.Cells[1, 23].Value = "Qty Putaway";
                workSheet.Cells[1, 24].Value = "Putaway By";
                workSheet.Cells[1, 25].Value = "Putaway On";
                workSheet.Cells[1, 26].Value = "Area";
                workSheet.Cells[1, 27].Value = "Rack No.";
                workSheet.Cells[1, 28].Value = "Status";
                workSheet.Cells[1, 29].Value = "Remarks";

                int recordIndex = 2;
                int recordNo = 1;
                foreach (ReceivingDetailReportDTO4 header in listdetail)
                {
                    workSheet.Cells[recordIndex, 1].Value = recordNo++;
                    workSheet.Cells[recordIndex, 2].Value = header.PONo;
                    workSheet.Cells[recordIndex, 3].Value = header.Supplier;
                    workSheet.Cells[recordIndex, 4].Value = header.MaterialCode;
                    workSheet.Cells[recordIndex, 5].Value = header.MaterialName;
                    workSheet.Cells[recordIndex, 6].Value = header.Schedule;
                    workSheet.Cells[recordIndex, 7].Value = header.TotalQtyPo;
                    workSheet.Cells[recordIndex, 8].Value = header.InDate;
                    workSheet.Cells[recordIndex, 9].Value = header.ExpDate;
                    workSheet.Cells[recordIndex, 10].Value = header.LotNo;
                    workSheet.Cells[recordIndex, 11].Value = header.QtyPerBag;
                    workSheet.Cells[recordIndex, 12].Value = header.QtyBag;
                    workSheet.Cells[recordIndex, 13].Value = header.Total;
                    workSheet.Cells[recordIndex, 14].Value = header.DoNo;
                    workSheet.Cells[recordIndex, 15].Value = header.Ok;
                    workSheet.Cells[recordIndex, 16].Value = header.NgDamage;
                    workSheet.Cells[recordIndex, 17].Value = header.NgWet;
                    workSheet.Cells[recordIndex, 18].Value = header.NgContamination;
                    workSheet.Cells[recordIndex, 19].Value = header.COA;
                    workSheet.Cells[recordIndex, 20].Value = header.StatusPo;
                    workSheet.Cells[recordIndex, 21].Value = header.ReceivedBy;
                    workSheet.Cells[recordIndex, 22].Value = header.ReceivedOn;
                    workSheet.Cells[recordIndex, 23].Value = header.QtyPutaway;
                    workSheet.Cells[recordIndex, 24].Value = header.PutawayBy;
                    workSheet.Cells[recordIndex, 25].Value = header.PutawayOn;
                    workSheet.Cells[recordIndex, 26].Value = header.Area;
                    workSheet.Cells[recordIndex, 27].Value = header.RackNo;
                    workSheet.Cells[recordIndex, 28].Value = header.Status;
                    workSheet.Cells[recordIndex, 29].Value = header.Remarks;
                    recordIndex++;
                }

                String datedownload = DateTime.Now.ToString("yyyyMMddhhmmss");
                String fileName = String.Format("filename=Receiving4_{0}.xlsx", date);

                for (int i = 1; i <= 30; i++)
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
                return RedirectToAction("Receiving4");
            }
        }
    }
}