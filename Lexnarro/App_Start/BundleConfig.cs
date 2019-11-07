using System.Web.Optimization;

namespace Lexnarro
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-3.3.1.min.js",
                         "~/Scripts/jquery-ui-1.10.4.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Content/js/bootstrap.min.js",
                      "~/Scripts/respond.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/owl").Include(
                      "~/Content/js/jquery-ui-1.9.2.custom.min.js",
                      "~/Content/js/jquery.autosize.min.js",                      
                      "~/Content/js/owl.carousel.js",
                      "~/Content/js/jquery.nicescroll.js",
                      "~/Content/js/progressbar.js",
                      "~/Content/js/moment.js",
                      "~/Content/DataTables/js/jquery.dataTables.min.js",
                      "~/Content/DataTables/js/dataTables.bootstrap.min.js",
                      "~/Content/DataTables/js/dataTables.fixedHeader.min.js",
                      "~/Content/DataTables/js/dataTables.responsive.min.js",
                      "~/Content/DataTables/js/responsive.bootstrap.min.js",
                      "~/Content/js/toastr.min.js",
                      "~/Content/js/scripts.js"
                      ));

            bundles.Add(new StyleBundle("~/bundles/css").Include(
                      "~/Content/css/bootstrap.min.css",
                      "~/Content/css/jquery-ui-1.10.4.min.css",
                      "~/Content/css/circle.css",
                      "~/Content/css/owl.carousel.css",
                      "~/Content/css/toastr.min.css",
                      "~/Content/css/elegant-icons-style.css",
                      "~/Content/DataTables/css/dataTables.bootstrap.min.css",
                      "~/Content/DataTables/css/fixedHeader.bootstrap.min.css",
                      "~/Content/DataTables/css/responsive.bootstrap.min.css",
                      "~/Content/css/site.css",
                       "~/Content/css/style.css",
                      "~/Content/css/style-responsive.css",
                      "~/Content/css/Custom.css"));

            bundles.Add(new StyleBundle("~/bundles/index").Include(
                      "~/Content/IndexCSS/bootstrap.min.css",                      
                      //"~/Content/IndexCSS/font-awesome.min.css",
                      "~/Content/IndexCSS/owl.carousel.min.css",
                      "~/Content/IndexCSS/magnific-popup.css",
                      "~/Content/IndexCSS/animate.css",
                      "~/Content/IndexCSS/normalize.css",
                      "~/Content/css/toastr.min.css",
                       "~/Content/IndexCSS/style.css",
                      "~/Content/IndexCSS/responsive.css"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/indexjs").Include(
                      "~/Content/IndexJS/js/jquery-1.12.0.min.js",
                      "~/Content/IndexJS/js/bootstrap.min.js",
                      "~/Content/IndexJS/js/owl.carousel.min.js",
                      "~/Content/IndexJS/js/jquery.sticky.js",
                      "~/Content/IndexJS/js/smooth-scroll.js",
                      "~/Content/IndexJS/js/jquery.magnific-popup.min.js",
                       "~/Content/IndexJS/js/jquery.counterup.min.js",
                      "~/Content/IndexJS/js/waypoints.min.js",
                      "~/Content/IndexJS/js/jquery.syotimer.min.js",
                      "~/Content/IndexJS/js/wow.min.js",
                      "~/Content/IndexJS/js/mail.js",
                      "~/Content/js/toastr.min.js",
                      "~/Content/IndexJS/js/plugins.js",
                      "~/Content/IndexJS/js/custom.js"
                      ));


            BundleTable.EnableOptimizations = true;
        }
    }
}
