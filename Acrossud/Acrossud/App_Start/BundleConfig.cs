﻿using System.Web;
using System.Web.Optimization;

namespace Acrossud
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/dropzone").Include(
                      "~/Scripts/dropzone.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootbox").Include(
                        "~/Scripts/bootbox.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/photoswipe").Include(
                        "~/Scripts/photoswipe-ui-default.min.js",
                        "~/Scripts/photoswipe.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/slick").Include(
                        "~/Scripts/slick.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/main").Include(
                        "~/Scripts/main.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/dropzone.css",
                      "~/Content/default-skin/default-skin.css",
                      "~/Content/photoswipe.css",
                      "~/Content/slick-theme.css",
                      "~/Content/slick.css",
                      "~/Content/site.css"));
        }
    }
}
