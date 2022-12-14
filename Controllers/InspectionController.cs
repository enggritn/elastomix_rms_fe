using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iText.Html2pdf;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout;
using Newtonsoft.Json;
using NPOI.Util;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Management;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WMS_FE.Models;
using ZXing;
using ZXing.QrCode;
using Rectangle = iText.Kernel.Geom.Rectangle;

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
    }
}