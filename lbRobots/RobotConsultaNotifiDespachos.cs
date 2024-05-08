using Google.Authenticator;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using OtpNet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace lbRobots
{
    public class RobotConsultaNotifiDespachos
    {
        void ComplementaDT()
        {
            Dt.Columns.Add(new DataColumn() { ColumnName = "estadoDespacho", DataType= typeof(System.String) });
            Dt.Columns.Add(new DataColumn() { ColumnName = "fechaDespacho", DataType = typeof(System.DateTime) });
            Dt.Columns.Add(new DataColumn() { ColumnName = "estadoAutoriza", DataType = typeof(System.String) });
            Dt.Columns.Add(new DataColumn() { ColumnName = "fechaVence", DataType = typeof(System.DateTime) });
            Dt.Columns.Add(new DataColumn() { ColumnName = "tipoIdent", DataType = typeof(System.String) });
            Dt.Columns.Add(new DataColumn() { ColumnName = "numeroIdent", DataType = typeof(System.String) });
            Dt.Columns.Add(new DataColumn() { ColumnName = "observacion", DataType = typeof(System.String) });
        }
        string GetValor(string Texto, string TextoBusca)
        {
            try
            {
                TextoBusca = $"{TextoBusca}</td>";
                int posIni = Texto.IndexOf(TextoBusca);
                int posfin = Texto.IndexOf("</td>", posIni + TextoBusca.Length);
                string retorno = Texto.Substring(posIni + TextoBusca.Length, posfin - (posIni + TextoBusca.Length));
                return retorno.Replace('\r', ' ').Replace('\n', ' ').Replace("<td>", "").Trim();
            }
            catch 
            {
                return "Valor No Existe";
            }
        }

        string getClaveTFA(string secretPass)
        {
            byte[] claveTFAb = Encoding.UTF8.GetBytes($"{secretPass}");
            var totp = new Totp(claveTFAb,step:30, mode:OtpHashMode.Sha1);
            string CodigoTFA = totp.ComputeTotp(DateTime.Now);
            return CodigoTFA;
        }

        string getClaveTFAGA(string secretPass)
        {
            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
           
            var secretAsBytes = Encoding.UTF8.GetBytes(secretPass);
            var secretAsBase32 = Google.Authenticator.Base32Encoding.ToString(secretAsBytes);
            var CodigoTFA = tfa.GetCurrentPIN(secretAsBase32,true);
            var Claves = tfa.GetCurrentPINs(secretAsBase32,true);
            return CodigoTFA;
            
        }
        public async Task EjecutarRobot()
        {
            try
            {
                var TFAG = getClaveTFAGA(SecretTFA.ToLower());
                var TFA = getClaveTFA(SecretTFA);
                //Iniciar Navegador
                EdgeOptions options = new EdgeOptions();
                options.AddArgument("--disable-notifications");
                options.AddArgument("start-maximized");
                EdgeDriver edgeDriver = new EdgeDriver(options);
                
                //Login en pagina web
                edgeDriver.Url = UrlLogin;
                await Task.Delay(1000);
                var elemnt = edgeDriver.FindElement(By.XPath("/html/body/form/div[3]/div/div/div[4]/div[1]/div/div[2]/div[1]/div[2]/input"));
                elemnt.Clear();
                elemnt.SendKeys(Userlogin);
                elemnt = edgeDriver.FindElement(By.XPath("/html/body/form/div[3]/div/div/div[4]/div[1]/div/div[2]/div[1]/div[3]/input"));
                elemnt.Clear();
                elemnt.SendKeys(PassLogin);
                var wait = new WebDriverWait(edgeDriver, TimeSpan.FromSeconds(10));
                var element = wait.Until(webDriver => webDriver.FindElement(By.XPath("/html/body/form/div[3]/div/div/div[4]/div[1]/div/div[2]/div[1]/div[5]/div[2]/input")));
                element.SendKeys("");
                ((IJavaScriptExecutor)edgeDriver).ExecuteScript("arguments[0].click();", element);
                await Task.Delay(1000);

                //Doble factor de autenticacion
                if(SecretTFA.Trim().Length>0)
                {
                    elemnt = edgeDriver.FindElement(By.XPath("/html/body/div[1]/div[3]/form/input[4]"));
                    elemnt.Clear();
                    elemnt.SendKeys(getClaveTFAGA(SecretTFA));
                    //element = wait.Until(webDriver => webDriver.FindElement(By.XPath("/html/body/div[1]/div[3]/form/div[1]/label/input")));
                    //((IJavaScriptExecutor)edgeDriver).ExecuteScript("arguments[0].click();", element);

                    element = wait.Until(webDriver => webDriver.FindElement(By.XPath("/html/body/div[1]/div[3]/form/div[2]/input")));
                    element.SendKeys("");
                    ((IJavaScriptExecutor)edgeDriver).ExecuteScript("arguments[0].click();", element);
                    await Task.Delay(1000);
                }
                
                //Generar Busquedas
                foreach (DataRow dr in Dt.Rows)
                {
                    try
                    {
                        edgeDriver.Url = $"{UrlNotfiDespachos}?codigoIpsEmite={dr.Field<string>("Codigos EPS")}&consecutivoAutorizacion={dr.Field<string>("Consecutivo")}&llamadaDesde=consultaAutorizaciones";
                        await Task.Delay(1000);
                        string fuente = edgeDriver.PageSource;

                        dr["estadoDespacho"] = GetValor(fuente, "Estado Despacho");
                        DateTime fechaTMP = DateTime.Now;
                        DateTime.TryParse(GetValor(fuente, "Fecha Despacho (aaaa/mm/dd) 24hrs (hh:mm:ss)"), out fechaTMP);
                        dr["fechaDespacho"] = fechaTMP;
                        dr["estadoAutoriza"] = GetValor(fuente, "Estado Autorización");
                        DateTime.TryParse(GetValor(fuente, "Fecha Vencimiento (aaaa/mm/dd)"), out fechaTMP);
                        dr["fechaVence"] = fechaTMP;
                        dr["tipoIdent"] = GetValor(fuente, "Tipo de Identificación");
                        dr["numeroIdent"] = GetValor(fuente, "Número de Identificación");
                    }
                    catch(Exception ex)
                    {
                        dr["observacion"] = $"Error:{ex.Message}";
                    }
                    

                }


            }
            catch (Exception ex) 
            {
                throw new Exception($"Error ejecutando robot Notificacion Despachos error:{ex.Message}");
            }
        }
        public RobotConsultaNotifiDespachos(string urlLogin, string userlogin, string passLogin, string urlNotfiDespachos, DataTable dt, string secretTFA)
        {
            UrlLogin = urlLogin;
            Userlogin = userlogin;
            PassLogin = passLogin;
            UrlNotfiDespachos = urlNotfiDespachos;
            Dt = dt;
            SecretTFA = secretTFA;
            ComplementaDT();
        }

        public string UrlLogin { get; }
        public string Userlogin { get; }
        public string PassLogin { get; }
        public string UrlNotfiDespachos { get; }
        public DataTable Dt { get; }
        public string SecretTFA { get; }
    }
}
