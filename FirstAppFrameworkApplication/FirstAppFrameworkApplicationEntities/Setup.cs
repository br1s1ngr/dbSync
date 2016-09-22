using AppFramework.AppClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FirstAppFrameworkApplicationEntities
{
    public class Setup : AppFramework.AppClasses.AppSetup
    {

        bool changeDb;

        public override void setupWindowsClient()
        {
            setup();

            String assemblyLocation = Util.getDirectoryPath(Assembly.GetExecutingAssembly().Location);

            string clientconfigfilename = assemblyLocation + "\\clientconfig.cfg";

            String[] configLines = File.ReadAllLines(clientconfigfilename);

            if (AppSettings.Servers.Count == 0)
            {
                using (TimedLock.Lock(AppSettings.Servers))
                {
                    if (AppSettings.Servers.Count == 0)
                    {
                        String iplist = configLines[0].Trim();
                        String[] ips = iplist.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                        String portlist = configLines[1].Trim();
                        String[] ports = portlist.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);

                        for (int i = 0; i < ips.Length; i++)
                        {
                            AppSettings.Servers.Add(new AppFramework.AppClasses.AppSettings.ServerIPAndPort(ips[i], int.Parse(ports[i])));
                        }
                    }
                }
            }

            try
            {
                MySqlDatabaseHandler dbHandler = new MySqlDatabaseHandler();
            }
            catch (Exception e)
            {
                Infolog.writeToEventLog(e, InfoType.Error);
                if (System.Threading.Thread.CurrentThread.GetApartmentState() == System.Threading.ApartmentState.STA)
                {
                    MessageBox.Show("Unable to establish server connection to server. Please view event logs for further details");
                }
                Environment.Exit(205);
            }
        }

        public override void setupBatchClient()
        {
            setupWindowsClient();
        }

        public override void setupNavigationMenus()
        {
            Navigation.RootMenu.Children.Clear();

            Navigation.RootMenu.Children.Add(new NavigationMenu("Test Db"));
            Navigation.RootMenu.Children[Navigation.RootMenu.Children.Count - 1].Children.Add(new NavigationMenu("Change DB", typeof(FirstAppFrameworkApplicationEntities.Runnables.runnablee)));
            Navigation.RootMenu.Children[Navigation.RootMenu.Children.Count - 1].Children.Add(new NavigationMenu("Customers", typeof(FirstAppFrameworkApplicationEntities.Forms.CustomersForm)));
            Navigation.RootMenu.Children[Navigation.RootMenu.Children.Count - 1].Children.Add(new NavigationMenu("Customer Profile", typeof(FirstAppFrameworkApplicationEntities.Forms.ProfilesForm)));
        }

        public void setupServer(bool changeDB = false)
        {
            //throw new NotImplementedException();
            setup();

            changeDb = changeDB;

            if (!changeDB)
            {
                String[] configLines = File.ReadAllLines(Application.StartupPath + "\\serverconfig.cfg");
                AppSettings.DefaultConnectionParameters = new String[] { configLines[0], configLines[1], configLines[2], configLines[3] };

                AppSettings.QueryTimeOut = configLines.Length >= 7 ? int.Parse(configLines[5]) : AppSettings.QueryTimeOut;
                AppSettings.MaxRecordLevelSecurityCacheAge = new TimeSpan(0, 5, 0);
                MySqlDatabaseHandler dbHandler = new MySqlDatabaseHandler();
            }
            else
            {
                String[] configLines = File.ReadAllLines(Application.StartupPath + "\\serverconfig1.cfg");
                AppSettings.DefaultConnectionParameters = new String[] { configLines[0], configLines[1], configLines[2], configLines[3] };

                AppSettings.QueryTimeOut = configLines.Length >= 7 ? int.Parse(configLines[5]) : AppSettings.QueryTimeOut;
                AppSettings.MaxRecordLevelSecurityCacheAge = new TimeSpan(0, 5, 0);
                MySqlDatabaseHandler dbHandler = new MySqlDatabaseHandler(configLines);
            }
            //if (configLines.Length > 6)
            //{
            //    AppSettings.CompanyName = configLines[6];
            //}

            //int port;
            //if (configLines.Length >= 9 && int.TryParse(configLines[8], out port))
            //{
            //    AppSettings.Servers.Add(new AppSettings.ServerIPAndPort("", port));
            //}

            //AppSettings.QueryTimeOut = configLines.Length >= 7 ? int.Parse(configLines[5]) : AppSettings.QueryTimeOut;
            //AppSettings.MaxRecordLevelSecurityCacheAge = new TimeSpan(0, 5, 0);
            //MySqlDatabaseHandler dbHandler = new MySqlDatabaseHandler();
        }

        public override void setupServer()
        {
            //throw new NotImplementedException();
            setup();

            if (!changeDb)
            {
                String[] configLines = File.ReadAllLines(Application.StartupPath + "\\serverconfig.cfg");
                AppSettings.DefaultConnectionParameters = new String[] { configLines[0], configLines[1], configLines[2], configLines[3] };

                AppSettings.QueryTimeOut = configLines.Length >= 7 ? int.Parse(configLines[5]) : AppSettings.QueryTimeOut;
                AppSettings.MaxRecordLevelSecurityCacheAge = new TimeSpan(0, 5, 0);
                MySqlDatabaseHandler dbHandler = new MySqlDatabaseHandler();
            }
            else
            {
                String[] configLines = File.ReadAllLines(Application.StartupPath + "\\serverconfig1.cfg");
                AppSettings.DefaultConnectionParameters = new String[] { configLines[0], configLines[1], configLines[2], configLines[3] };
                AppSettings.QueryTimeOut = configLines.Length >= 7 ? int.Parse(configLines[5]) : AppSettings.QueryTimeOut;
                AppSettings.MaxRecordLevelSecurityCacheAge = new TimeSpan(0, 5, 0);
                MySqlDatabaseHandler dbHandler = new MySqlDatabaseHandler(configLines);
                
                DatabaseHandler.DefaultDatabaseHandlerObject = dbHandler;
                MySqlDatabaseHandler.DefaultDatabaseHandlerObject = dbHandler;
            }

            //String[] configLines = File.ReadAllLines(Application.StartupPath + "\\serverconfig.cfg");
            //    AppSettings.DefaultConnectionParameters = new String[] { configLines[0], configLines[1], configLines[2], configLines[3] };

            ////if (configLines.Length > 6)
            ////{
            ////    AppSettings.CompanyName = configLines[6];
            ////}

            ////int port;
            ////if (configLines.Length >= 9 && int.TryParse(configLines[8], out port))
            ////{
            ////    AppSettings.Servers.Add(new AppSettings.ServerIPAndPort("", port));
            ////}

            //AppSettings.QueryTimeOut = configLines.Length >= 7 ? int.Parse(configLines[5]) : AppSettings.QueryTimeOut;
            //AppSettings.MaxRecordLevelSecurityCacheAge = new TimeSpan(0, 5, 0);
            //MySqlDatabaseHandler dbHandler = new MySqlDatabaseHandler();
        }

        public static void setup()
        {
            String assemblyLocation = Util.getDirectoryPath(Assembly.GetExecutingAssembly().Location);
            AppSettings.MainAssembly = Assembly.LoadFrom(assemblyLocation + "\\FirstAppFrameworkApplicationEntities.dll");
            AppSettings.FileServerLocation = assemblyLocation;
            AppSettings.DefaultEntityNameSpace = "FirstAppFrameworkApplicationEntities.EntityClasses";
            AppSettings.DefaultEnumNameSpace = "FirstAppFrameworkApplicationEntities.EntityClasses";
            AppSettings.GetFieldAndTableInfoFromDatabase = false;
            AppSettings.UseVirtualDelete = true;
            //AppSettings.LicenseEncryptionPublicKey = new StreamReader(Util.getAssemblyResourceFileStream(AppFramework.AppClasses.AppSettings.MainAssembly, "Bursary.PublicKey.key")).ReadToEnd();
            AppSettings.LicenseEncryptionPublicKey = "waisty";
        }

        public override void setupODataService()
        {
            throw new NotImplementedException();
        }

       
    }
}
