using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;

namespace UpdatePaymentsCom
{
    [Guid("03319EF5-F297-474B-8276-C66243D6B8B6"),
    ClassInterface(ClassInterfaceType.None)]
    public class UpdateDLL : IUpdateDLL
    {
        public UpdateDLL()
        {
        }

        private readonly static string PathToPaymentsCom = AppDomain.CurrentDomain.BaseDirectory + "paymentsCom.dll";
        private readonly static string PathToRegBat = AppDomain.CurrentDomain.BaseDirectory + "regPaymentsCom.bat";

        public string CheckDLL(string version)
        {
            try
            {
                if (File.Exists(PathToPaymentsCom) && CheckDLLVersion(version)) return "2";
                else
                {
                    if (InsertDll(DllFile))
                        if (bat()) return "1";
                    return "-1";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return "-1";
            }
        }

        public string Check418Version(string version)
        {
            if (Convert.ToInt32(version) == Payments.Get418Version()) return "1";
            return "-1";
        }

        private bool CheckDLLVersion(string version)
        {
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(PathToPaymentsCom);
            string ver = AssemblyName.GetAssemblyName(PathToPaymentsCom).Version.ToString().Split('.')[3];
            if (Convert.ToInt32(version) == Convert.ToInt32(ver)) return true;
            return false;
        }

        public string GetSiteAdress()
        {
            return GetUrl().Replace("/services", "");
        }

        private string GetUrl()
        {
            string file = AppDomain.CurrentDomain.BaseDirectory + "paymentsCom.config";
            if (!File.Exists(file))
            {
                MessageBox.Show("Файл paymentsCom.config не знайдено !");
                return null;
            }
            XmlReader reader = XmlReader.Create(file);
            while (reader.Read())
            {
                if (reader.Name.ToUpper() == "url".ToUpper())
                {
                    string url = reader.ReadInnerXml();
                    if (!CheckParam(url))
                    {
                        MessageBox.Show("Невірно налаштований параметр конфігурації url !");
                        return null;
                    }
                    return url;
                }
            }
            MessageBox.Show("Відсутній параметр конфігурації url !");
            return null;
        }
        private static bool CheckParam(string val)
        {
            if (val == string.Empty)
            {
                return false;
            }
            else
                return true;
        }

        private bool InsertDll(byte[] DllFile)
        {
            if (DllFile == null)
            {
                return false;
            }
            try
            {
                File.WriteAllBytes(PathToPaymentsCom, DllFile);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool bat()
        {
            try
            {
                InsertBat();
                Process.Start(PathToRegBat);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static readonly string batFileContent = @"regasm PaymentsCom.dll
                                                          @pause";

        private void InsertBat()
        {
            File.WriteAllText(PathToRegBat, batFileContent);
        }

        private Payments.Payments Payments
        {
            get
            {
                Payments.Payments svc = new Payments.Payments();
                svc.Url = GetUrl() + "/payments.asmx";
                return svc;
            }
        }

        private byte[] DllFile
        {
            get
            {
                return Payments.GetServiceDLL();
            }
        }
    }

}
