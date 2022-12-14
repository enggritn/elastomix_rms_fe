using iText.Html2pdf;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WMS_FE.Models;

namespace WMS_FE.Controllers
{
    [SessionCheck]
    public class MaterialRequestController : Controller
    {
        // GET: ProductionPlan
        public ActionResult Index()
        {
            ViewBag.BaseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');
            ViewBag.Server = ConfigurationManager.AppSettings["server"].ToString();
            return View();
        }

        public ActionResult Create()
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
                    ViewBag.Logo = Domain + "/Content/img/logo.jpg";
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


    }
}