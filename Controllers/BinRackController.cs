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

namespace WMS_FE.Controllers
{
    [SessionCheck]
    public class BinRackController : Controller
    {
        // GET: BinRack
        public ActionResult Index()
        {
            
            ViewBag.BaseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');
            ViewBag.Server = ConfigurationManager.AppSettings["server"].ToString();
            return View();
        }


        public ActionResult Create()
        {
            ViewBag.Param = this.Request.QueryString["aid"];
            ViewBag.Param2 = this.Request.QueryString["wid"];
            ViewBag.BaseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');
            ViewBag.Server = ConfigurationManager.AppSettings["server"].ToString();
            return View();
        }

        public ActionResult Edit()
        {
            ViewBag.Param = this.Request.QueryString["id"];
            ViewBag.Param2 = this.Request.QueryString["aid"];
            ViewBag.Param3 = this.Request.QueryString["wid"];
            ViewBag.BaseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');
            ViewBag.Server = ConfigurationManager.AppSettings["server"].ToString();
            return View();
        }

        public ActionResult Detail()
        {
            ViewBag.Param = this.Request.QueryString["id"];
            ViewBag.Param2 = this.Request.QueryString["aid"];
            ViewBag.Param3 = this.Request.QueryString["wid"];
            ViewBag.BaseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');
            ViewBag.Server = ConfigurationManager.AppSettings["server"].ToString();
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> GenerateLabel(string id)
        {
            string Domain = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');



            string ApiAddress = ConfigurationManager.AppSettings["server"].ToString();
            BinRackResponse res = new BinRackResponse();


            try
            {

                HttpClient client = new HttpClient();
                Uri uri = new Uri(ApiAddress + string.Format("Api/BinRack/GetDataById?&id={0}", id));
                var response = await client.GetAsync(uri);
                string result = response.Content.ReadAsStringAsync().Result;
                res = JsonConvert.DeserializeObject<BinRackResponse>(result);
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
                     </body> ");
            }


            BinRack mdl = res.data;



            List<string> bodies = new List<string>();

            ViewBag.Logo = Domain + "/Content/img/logo.jpg";
            ViewBag.Barcode = Domain + "/" + GenerateQRCode(mdl.Code);
            ViewBag.RackName = mdl.Name;
            ViewBag.AreaName = mdl.BinRackAreaName;
            ViewBag.WarehouseName = mdl.WarehouseName;
            ViewBag.WarehouseCode = mdl.WarehouseCode;
            ViewBag.BinRackCode = mdl.Code;

            bodies.Add(RenderViewToString(this.ControllerContext, "Label", null));


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
                        //Rectangle rectangle = new Rectangle(283.464566928f, 212.598425232f);
                        Rectangle rectangle = new Rectangle(252f, 108f);
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
            string folderPath = "/Content/img/rack_codes";
            string imagePath = folderPath + "/" + string.Format("{0}.png", qr_path);
            // If the directory doesn't exist then create it.
            if (!Directory.Exists(Server.MapPath("~" + folderPath)))
            {
                Directory.CreateDirectory(Server.MapPath("~" + folderPath));
            }

            var barcodeWriter = new BarcodeWriter();
            barcodeWriter.Format = BarcodeFormat.CODE_128;
            barcodeWriter.Options = new QrCodeEncodingOptions
            {
                Width = 600,
                Height = 100,
                PureBarcode = true
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

        [HttpGet]
        public async Task<ActionResult> GenerateLabelAll(string areaId)
        {
            string Domain = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');



            string ApiAddress = ConfigurationManager.AppSettings["server"].ToString();
            List<BinRackList> res = new List<BinRackList>();


            try
            {

                HttpClient client = new HttpClient();
                Uri uri = new Uri(ApiAddress + string.Format("Api/BinRack/AreaRack?&id={0}", areaId));
                var response = await client.GetAsync(uri);
                string result = response.Content.ReadAsStringAsync().Result;
                res = JsonConvert.DeserializeObject<List<BinRackList>>(result);
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
                     </body> ");
            }




            List<string> bodies = new List<string>();

            foreach(BinRackList mdl in res)
            {
                ViewBag.Barcode = Domain + "/" + GenerateQRCode(mdl.Code);
                ViewBag.RackName = mdl.Name;
                ViewBag.AreaName = mdl.BinRackAreaName;
                ViewBag.WarehouseName = mdl.WarehouseName;
                ViewBag.WarehouseCode = mdl.WarehouseCode;
                ViewBag.BinRackCode = mdl.Code;

                bodies.Add(RenderViewToString(this.ControllerContext, "Label", null));
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

                        //72f = 1 inch
                        Rectangle rectangle = new Rectangle(252f, 108f);
                        //Rectangle rectangle = new Rectangle(283.464566928f, 212.598425232f);
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

                    return File(output, "application/pdf");


                }
            }
        }
    }
}