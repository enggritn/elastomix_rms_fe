using System.Web;
using System.Web.Optimization;

namespace WMS_FE
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/bundles/css").Include(
                     "~/Content/plugins/bootstrap/dist/css/bootstrap.min.css",
                     "~/Content/plugins/ionicons/dist/css/ionicons.min.css",
                     "~/Content/plugins/perfect-scrollbar/css/perfect-scrollbar.css",
                     "~/Content/dist/css/theme.min.css"
                     ));

            bundles.Add(new ScriptBundle("~/bundles/js").Include(
                      "~/Content/plugins/jquery/jquery.min.js",
                      "~/Content/plugins/popper.js/dist/umd/popper.min.js",
                      "~/Content/plugins/bootstrap/dist/js/bootstrap.min.js",
                       "~/Content/plugins/perfect-scrollbar/dist/perfect-scrollbar.min.js",
                      "~/Content/dist/js/theme.js"
                      ));
        }
    }
}
