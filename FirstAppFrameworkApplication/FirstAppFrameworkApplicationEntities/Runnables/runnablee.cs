using AppFramework.AppClasses;
using AppFramework.AppClasses.EDTs;
using AppFramework.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FirstAppFrameworkApplicationEntities.Runnables
{
    class runnablee : Runnable
    {
        public override string Title
        {
            get { return "Change DB Runnable"; }
        }

        public string companyName { get; set; }

        //public override bool Interactive
        //{
        //    get
        //    {
        //        return base.Interactive;
        //    }
        //    set
        //    {
        //        base.Interactive = false;
        //    }
        //}

        public override bool prompt()
        {
            IValueDataControl company = this.addParameter(new CompanyEDT(), "Company Name");
            bool ret = base.prompt();
            companyName = company.StringValue;
            return ret;
        }

        protected override object doRun(bool dialog)
        {
            using (XmlReader xmlReader = XmlReader.Create(System.Windows.Forms.Application.StartupPath + "\\serverconfiguration.cfg"))
            {
                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "company" && xmlReader.GetAttribute("name") == companyName)
                    {
                        setDbConnectionsWithConfig(xmlReader);
                        Session.changeCompany(companyName);
                    }
                }
                //xmlReader.Close();
            }
            return null;
        }

        private void setDbConnectionsWithConfig(XmlReader xmlReader)
        {
            string[] configLines = getConfigLines(xmlReader);
            setDbSettings(configLines);
        }

        private string[] getConfigLines(XmlReader xmlReader)
        {
            var configLines = new string[] { 
                        xmlReader.GetAttribute("server"), xmlReader.GetAttribute("database"), xmlReader.GetAttribute("username"), xmlReader.GetAttribute("password"), xmlReader.GetAttribute("misc")  
                    };
            return configLines;
        }

        private void setDbSettings(string[] configLines)
        {
            AppSettings.DefaultConnectionParameters = new String[] { configLines[0], configLines[1], configLines[2], configLines[3] };
            AppSettings.QueryTimeOut = configLines.Length >= 7 ? int.Parse(configLines[5]) : AppSettings.QueryTimeOut;
            AppSettings.MaxRecordLevelSecurityCacheAge = new TimeSpan(0, 5, 0);
            MySqlDatabaseHandler dbHandler = new MySqlDatabaseHandler(configLines);
            DatabaseHandler.DefaultDatabaseHandlerObject = dbHandler;
        }
    }
}