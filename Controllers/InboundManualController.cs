using iText.Html2pdf;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout;
using Newtonsoft.Json;
using NPOI.Util;
using Org.BouncyCastle.Crypto.Tls;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using WMS_FE.Models;
using ZXing;
using ZXing.QrCode;
using Rectangle = iText.Kernel.Geom.Rectangle;

namespace WMS_FE.Controllers
{
    public class InboundManualController : Controller
    {
        // GET: RawMate
        public ActionResult Index()
        {
            ViewBag.server = ConfigurationManager.AppSettings["server"].ToString();
            return View();
        }

        public ActionResult Label()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> GenerateLabel(ManualStockViewModel data)
        {
            string Domain = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');

            ViewBag.Code = data.Code;

            string ApiAddress = ConfigurationManager.AppSettings["server"].ToString();
            RawMaterialManualResponse res = new RawMaterialManualResponse();

            data.BagQty = data.BagQty.Replace(",", ".");
            data.FullBagQty = data.FullBagQty.Replace(",", ".");
            data.FullQty = data.FullQty.Replace(",", ".");
            data.RemainQty = data.RemainQty.Replace(",", ".");
            data.RemainQty = data.RemainQty.Replace(",", ".");

            double fb = Convert.ToDouble(data.FullBagQty);
            double remainQty = Convert.ToDouble(data.RemainQty);
            double fullQty = Convert.ToDouble(data.FullQty);

            if (fullQty > 0)
            {
                fb += 1;
            }

            if (remainQty > 0)
            {
                fb += 1;
            }



            try
            {
                var json = JsonConvert.SerializeObject(data);
                var postData = new StringContent(json, Encoding.UTF8, "application/json");

                HttpClient client = new HttpClient();
                //Uri uri = new Uri(ApiAddress + string.Format("Api/InboundManual/GetSeqNoById?id={0}&fullBag={1}&inDate={2}&lotNumber={3}", data.ID, fb, data.InDate, data.Lot, data.ExpiredDate));
                //HttpResponseMessage response = client.GetAsync(uri).Result;
                //string req = response.Content.ReadAsStringAsync().Result;
                Uri uri = new Uri(ApiAddress + string.Format("Api/InboundManual/GetSeqNoById?fullBag={0}", fb));
                var response = await client.PostAsync(uri, postData);
                string result = response.Content.ReadAsStringAsync().Result;
                res = JsonConvert.DeserializeObject<RawMaterialManualResponse>(result);
            }
            catch (Exception e)
            {

            }

            if (res == null)
            {
                return Content(@"<body>
                       <script type='text/javascript'>
                         window.close();
                       </script>
                     </body>");
            }

            if (!res.status)
            {
                string body = string.Format(@"<body>
                       <script type='text/javascript'>
                         alert('{0}')
                       </script>
                     </body>", res.message);
                return Content(body);
            }


            ManualStockViewModel mdl = res.data;
            double fbQty = Convert.ToDouble(data.FullBagQty);
            int fullBagQty = Convert.ToInt32(fbQty);

            int seq = 0;
            seq = (mdl.Sequence - fullBagQty) + 1;
            //if(fullBagQty > 0)
            //{
            //    seq = (mdl.Sequence - fullBagQty) + 1;
            //}
            //else
            //{
            //    seq = mdl.Sequence - fullBagQty;
            //}


            //if (remainQty > 0)
            //{
            //    seq -= 1;
            //}


            int fullBag = Convert.ToInt32(fb);


            List<string> bodies = new List<string>();

            if (fullQty > 0)
            {
                seq -= 1;
                string runningNumber = string.Format("{0:D5}", seq);

                string qr1 = data.Code.PadRight(7) + " " + runningNumber + " " + data.FullQty.PadLeft(6) + " " + data.Lot;
                ViewBag.Field3 = Domain + "/" + GenerateQRCode(qr1);

                string inDate = "";
                string inDate2 = "";
                string inDate3 = "";
                string expiredDate = "";
                string expiredDate2 = "";

                if (!string.IsNullOrEmpty(data.InDate))
                {
                    try
                    {

                        DateTime dt = DateTime.ParseExact(data.InDate, "dd/MM/yyyy", null);
                        //DateTime dt = Convert.ToDateTime(data.InDate);
                        ViewBag.Field4 = dt.ToString("MMMM").ToUpper();
                        //inDate = dt.ToString("yyyyMMdd");
                        inDate = dt.ToString("yyyyMMdd").Substring(1);
                        inDate2 = dt.ToString("yyyMMdd");
                        inDate2 = inDate2.Substring(1);
                        inDate3 = dt.ToString("yyyy-MM-dd");
                    }
                    catch (Exception e)
                    {
                        string msg = string.Format(@"<body>
                       <script type='text/javascript'>
                          alert('{0}')
                       </script>
                     </body>", e.Message);
                        return Content(msg);
                    }
                }

                if (!string.IsNullOrEmpty(data.ExpiredDate))
                {
                    try
                    {

                        DateTime dt = DateTime.ParseExact(data.ExpiredDate, "dd/MM/yyyy", null);
                        //DateTime dt = Convert.ToDateTime(data.ExpiredDate);
                        //expiredDate = dt.ToString("yyyyMMdd");
                        expiredDate = dt.ToString("yyyyMMdd").Substring(2);
                        expiredDate2 = dt.ToString("yyyy-MM-dd");
                    }
                    catch (Exception e)
                    {
                        string msg = string.Format(@"<body>
                       <script type='text/javascript'>
                          alert('{0}')
                       </script>
                     </body>", e.Message);
                        return Content(msg);
                    }
                }
                string qr2 = data.Code.PadRight(7) + inDate + expiredDate;
                ViewBag.Field5 = data.Lot;
                ViewBag.Field6 = Domain + "/" + GenerateQRCode(qr2);
                ViewBag.Field7 = data.MakerName;
                ViewBag.Field8 = data.Name;
                ViewBag.Field9 = data.FullQty.ToString();
                ViewBag.Field10 = data.UoM.ToUpper();
                ViewBag.Field11 = inDate2;
                ViewBag.Field12 = data.Code;
                ViewBag.Field13 = inDate3;
                ViewBag.Field14 = expiredDate2;
                String body = RenderViewToString(this.ControllerContext, "Label", null);
                bodies.Add(body);
                seq++;
            }


            if (remainQty > 0)
            {
                seq -= 1;
                string runningNumber = string.Format("{0:D5}", seq);

                string qr1 = data.Code.PadRight(7) + " " + runningNumber + " " + data.RemainQty.PadLeft(6) + " " + data.Lot;
                ViewBag.Field3 = Domain + "/" + GenerateQRCode(qr1);

                string inDate = "";
                string inDate2 = "";
                string inDate3 = "";
                string expiredDate = "";
                string expiredDate2 = "";

                if (!string.IsNullOrEmpty(data.InDate))
                {
                    try
                    {

                        DateTime dt = DateTime.ParseExact(data.InDate, "dd/MM/yyyy", null);
                        //DateTime dt = Convert.ToDateTime(data.InDate);
                        ViewBag.Field4 = dt.ToString("MMMM").ToUpper();
                        //inDate = dt.ToString("yyyyMMdd");
                        inDate = dt.ToString("yyyyMMdd").Substring(1);
                        inDate2 = dt.ToString("yyyMMdd");
                        inDate2 = inDate2.Substring(1);
                        inDate3 = dt.ToString("yyyy-MM-dd");
                    }
                    catch (Exception e)
                    {
                        string msg = string.Format(@"<body>
                       <script type='text/javascript'>
                          alert('{0}')
                       </script>
                     </body>", e.Message);
                        return Content(msg);
                    }
                }

                if (!string.IsNullOrEmpty(data.ExpiredDate))
                {
                    try
                    {

                        DateTime dt = DateTime.ParseExact(data.ExpiredDate, "dd/MM/yyyy", null);
                        //DateTime dt = Convert.ToDateTime(data.ExpiredDate);
                        //expiredDate = dt.ToString("yyyyMMdd");
                        expiredDate = dt.ToString("yyyyMMdd").Substring(2);
                        expiredDate2 = dt.ToString("yyyy-MM-dd");
                    }
                    catch (Exception)
                    {

                    }
                }
                string qr2 = data.Code.PadRight(7) + inDate + expiredDate;
                ViewBag.Field5 = data.Lot;
                ViewBag.Field6 = Domain + "/" + GenerateQRCode(qr2);
                ViewBag.Field7 = data.MakerName;
                ViewBag.Field8 = data.Name;
                ViewBag.Field9 = data.RemainQty.ToString();
                ViewBag.Field10 = data.UoM.ToUpper();
                ViewBag.Field11 = inDate2;
                ViewBag.Field12 = data.Code;
                ViewBag.Field13 = inDate3;
                ViewBag.Field14 = expiredDate2;
                String body = RenderViewToString(this.ControllerContext, "Label", null);
                bodies.Add(body);
                seq++;
            }


            if (fullQty > 0)
            {
                fullBag -= 1;
            }

            if (remainQty > 0)
            {
                fullBag -= 1;
            }




            for (int i = 0; i < fullBag; i++)
            {
                string runningNumber = string.Format("{0:D5}", seq++);

                string qr1 = data.Code.PadRight(7) + " " + runningNumber + " " + data.BagQty.PadLeft(6) + " " + data.Lot;
                ViewBag.Field3 = Domain + "/" + GenerateQRCode(qr1);

                string inDate = "";
                string inDate2 = "";
                string inDate3 = "";
                string expiredDate = "";
                string expiredDate2 = "";
                if (!string.IsNullOrEmpty(data.InDate))
                {
                    try
                    {

                        DateTime dt = DateTime.ParseExact(data.InDate, "dd/MM/yyyy", null);
                        //DateTime dt = Convert.ToDateTime(data.InDate);
                        ViewBag.Field4 = dt.ToString("MMMM").ToUpper();
                        //inDate = dt.ToString("yyyyMMdd");
                        inDate = dt.ToString("yyyyMMdd").Substring(1);
                        inDate2 = dt.ToString("yyyMMdd");
                        inDate2 = inDate2.Substring(1);
                        inDate3 = dt.ToString("yyyy-MM-dd");
                    }
                    catch (Exception e)
                    {
                        string msg = string.Format(@"<body>
                       <script type='text/javascript'>
                           alert('{0}')
                       </script>
                     </body>", e.Message);
                        return Content(msg);
                    }
                }

                if (!string.IsNullOrEmpty(data.ExpiredDate))
                {
                    try
                    {

                        DateTime dt = DateTime.ParseExact(data.ExpiredDate, "dd/MM/yyyy", null);
                        //DateTime dt = Convert.ToDateTime(data.ExpiredDate);
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
                string qr2 = data.Code.PadRight(7) + inDate + expiredDate;
                ViewBag.Field5 = data.Lot;
                ViewBag.Field6 = Domain + "/" + GenerateQRCode(qr2);
                ViewBag.Field7 = data.MakerName;
                ViewBag.Field8 = data.Name;
                ViewBag.Field9 = data.BagQty.ToString();
                ViewBag.Field10 = data.UoM.ToUpper();
                ViewBag.Field11 = inDate2;
                ViewBag.Field12 = data.Code;
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
    }
}