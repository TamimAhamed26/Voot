using Microsoft.Extensions.Configuration;
using System.Collections;
using System.Diagnostics;

namespace MDUA.Framework
{
    /// <summary>
    /// Summary description for ConfigurationBlock.
    /// </summary>
    public sealed class ConfigurationBlock
    {
        public static IConfiguration Configuration { get; set; }
        private static Hashtable _hashtable = (Hashtable)AppDomain.CurrentDomain.GetData("Configuration");

        /// <summary>
        /// default constructor for class ConfigurationBlock
        /// </summary>
        public ConfigurationBlock()
        {

        }
        /// <summary>
        /// getting Connection string
        /// </summary>
        /// 
        public static string ConnectionString
        {
            get
            {
                // --- 2. Check if Configuration is loaded ---
                if (Configuration == null)
                {
                    throw new InvalidOperationException("ConfigurationBlock.Configuration is null. You must assign 'builder.Configuration' to 'MDUA.Framework.ConfigurationBlock.Configuration' in your Program.cs file.");
                }

                // --- 3. Read from appsettings.json ---
                string conn = Configuration.GetConnectionString("ConnectionSCH");

                if (string.IsNullOrEmpty(conn))
                {
                    throw new Exception("Connection string 'ConnectionSCH' not found in appsettings.json.");
                }

                return conn;
            }
        }

        public static string MasterConnectionString
        {
            get
            {
                // --- 2. Check if Configuration is loaded ---
                if (Configuration == null)
                {
                    throw new InvalidOperationException("ConfigurationBlock.Configuration is null. You must assign 'builder.Configuration' to 'MDUA.Framework.ConfigurationBlock.Configuration' in your Program.cs file.");
                }

                // --- 3. Read from appsettings.json ---
                string conn = Configuration.GetConnectionString("ConnectionSCH");

                if (string.IsNullOrEmpty(conn))
                {
                    throw new Exception("Connection string 'ConnectionSCH' not found in appsettings.json.");
                }

                return conn; 
                //return _ConnectionString;
            }
        }

        public static string EntityFrameworkNameSpace
        {
            get
            {
                string _EntityFrameworkNameSpace = string.Empty;
                if (System.Configuration.ConfigurationManager.AppSettings["EntityFrameworkNameSpace"] != null)
                {
                    _EntityFrameworkNameSpace = System.Configuration.ConfigurationManager.AppSettings["EntityFrameworkNameSpace"];
                }
                return _EntityFrameworkNameSpace;
            }
        }

        ///// <summary>
        ///// getting Mail Settings
        ///// </summary>
        //public static MailSettingsSectionGroup MailSettings
        //{
        //    get { return _MailSettings; }
        //}

        /// <summary>
        /// Whether to display trace information or not, from configuration file
        /// </summary>
        public static bool DisplayTraceInformation
        {
            get
            {
                if (Configuration == null) return false;

                var val = Configuration["DisplayTraceInformation"];
                return bool.TryParse(val, out bool result) && result;
            }
        }
    }
    
}