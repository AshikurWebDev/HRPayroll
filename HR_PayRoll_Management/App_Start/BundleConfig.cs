using System.Web.Optimization;

namespace HR_PayRoll_Management
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jqueryval")
                .Include("~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr")
                .Include("~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css")
                .Include("~/Content/Site.css"));

            bundles.Add(new ScriptBundle("~/bundles/chart")
                .Include("~/Scripts/lib/chartjs/chart.umd.js"));
        }
    }
}