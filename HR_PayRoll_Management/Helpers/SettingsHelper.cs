using System;
using System.IO;
using System.Web;
using Newtonsoft.Json;

namespace HR_PayRoll_Management.Helpers
{
    public class SystemSettings
    {
        public string CompanyName { get; set; }
        public string LogoPath { get; set; }
    }

    public static class SettingsHelper
    {
        private static string FilePath => HttpContext.Current.Server.MapPath("~/App_Data/Settings.json");

        public static SystemSettings GetSettings()
        {
            if (File.Exists(FilePath))
            {
                var json = File.ReadAllText(FilePath);
                return JsonConvert.DeserializeObject<SystemSettings>(json);
            }
            return new SystemSettings { CompanyName = "HR PAYROLL SYSTEM", LogoPath = "~/Content/Images/logo.png" };
        }

        public static void SaveSettings(SystemSettings settings)
        {
            var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(FilePath, json);
        }
    }
}
