using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using System.Data;

namespace lbRobots
{
    public class RobotEjemplo
    {
        public async Task EjecutarRobot() 
        {
            EdgeOptions options = new EdgeOptions();
            options.AddArgument("--disable-notifications");
            options.AddArgument("start-maximized");
            EdgeDriver edgeDriver = new EdgeDriver(options);
            
            //Login en pagina web
            edgeDriver.Url = UrlLogin;
            await Task.Delay(1000);
            var elemnt = edgeDriver.FindElement(By.Name("Username"));
            elemnt.Clear();
            elemnt.SendKeys(UserLogin);
            elemnt = edgeDriver.FindElement(By.Name("Password"));
            elemnt.Clear();
            elemnt.SendKeys(PassLogin);
            var wait = new WebDriverWait(edgeDriver, TimeSpan.FromSeconds(10));
            var element = wait.Until(webDriver => webDriver.FindElement(By.XPath(".//button[@type='submit']")));
            element.SendKeys("");
            ((IJavaScriptExecutor)edgeDriver).ExecuteScript("arguments[0].click();", element);

            //Insertar Usuarios
            await Task.Delay(500);
            edgeDriver.Url = UrlUsuarios;
            foreach(DataRow fila in DtUsuarios.Rows)
            {
                //Click en nuevo usuario
                await Task.Delay(1000);
                element = wait.Until(webDriver => webDriver.FindElement(By.XPath("/html/body/div[1]/main/article/form/div/div/fieldset/div/div/div[6]/div/button[2]")));
                ((IJavaScriptExecutor)edgeDriver).ExecuteScript("arguments[0].click();", element);
                await Task.Delay(500);
                //LLenar Campos
                #region LLenado de Campos
                element = edgeDriver.FindElement(By.XPath(".//input[@name='UserName']"));
                element.Clear();
                element.SendKeys(fila.Field<string>("UserName"));
                element = edgeDriver.FindElement(By.XPath(".//input[@name='Nit']"));
                element.SendKeys(fila.Field<string>("Nit"));
                element = edgeDriver.FindElement(By.XPath(".//input[@name='txEmail']"));
                element.SendKeys(fila.Field<string>("Email"));
                //Diccionario con lista de roles y ubicacion del campo de lista en el formulario
                IDictionary<string, string> roles = new Dictionary<string, string>();
                roles.Add("Administrador", "/html/body/div[4]/div/ul/li[1]");
                roles.Add("Proveedor", "/html/body/div[4]/div/ul/li[2]");
                roles.Add("Calificador", "/html/body/div[4]/div/ul/li[3]");
                //Elegir Role
                element = wait.Until(webDriver => webDriver.FindElement(By.XPath("/html/body/div[1]/main/article/form/div/div/fieldset/div/div/div[4]/div[2]/div/label")));
                ((IJavaScriptExecutor)edgeDriver).ExecuteScript("arguments[0].click();", element);
                await Task.Delay(800);
                element = edgeDriver.FindElement(By.XPath(roles[fila.Field<string>("Rol")]));
                element.Click();
                //Password
                element = edgeDriver.FindElement(By.XPath("/html/body/div[1]/main/article/form/div/div/fieldset/div/div/div[5]/div[2]/input"));
                element.SendKeys(fila.Field<string>("Password"));
                //Confirmar Password
                element = edgeDriver.FindElement(By.XPath("/html/body/div[1]/main/article/form/div/div/fieldset/div/div/div[6]/div[2]/input"));
                element.SendKeys(fila.Field<string>("Password"));
                //Hacer click en salvar
                element = wait.Until(webDriver => webDriver.FindElement(By.XPath("/html/body/div[1]/main/article/form/div/div/fieldset/div/div/div[8]/div/button[1]")));
                element.SendKeys("");
                ((IJavaScriptExecutor)edgeDriver).ExecuteScript("arguments[0].click();", element);
                #endregion

            }
            edgeDriver.Close();

        }
        public RobotEjemplo(string UrlLogin, string UserLogin, string PassLogin, string UrlUsuarios, DataTable dtUsuarios)
        {
            this.UrlLogin = UrlLogin;
            this.UserLogin = UserLogin;
            this.PassLogin = PassLogin;
            this.UrlUsuarios = UrlUsuarios;
            DtUsuarios = dtUsuarios;
        }

        public string UrlLogin { get; }
        public string UserLogin { get; }
        public string PassLogin { get; }
        public string UrlUsuarios { get; }
        public DataTable DtUsuarios { get; }
    }
}
