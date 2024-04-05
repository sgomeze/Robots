using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Robots
{
    public class cConfig
    {
        IConfigurationRoot config;

        public string ConnectionString { get { return config["Robots:ConnectionString"]; } }

        //Prueba
        public string RobotPruebaUrlLogin { get { return config["RobotPrueba:UrlLogin"]; } }
        public string RobotPruebaUrlUsuarios { get { return config["RobotPrueba:UrlUsuarios"]; } }
        public string UserLogin { get { return config["RobotPrueba:UserLogin"]; } }
        public string PassLogin { get { return config["RobotPrueba:Passlogin"]; } }

        //Notificacion Despachos
        public string ND_UrlLogin { get { return config["RobotConsultaSura:UrlLogin"]; } }
        public string ND_UrlNotifiDespachos { get { return config["RobotConsultaSura:UrlNotificaciondespachos"]; } }
        public string ND_UserLogin { get { return config["RobotConsultaSura:UserLogin"]; } }
        public string ND_PassLogin { get { return config["RobotConsultaSura:Passlogin"]; } }

        public cConfig()
        {
             config = new ConfigurationBuilder()
            .AddUserSecrets(Assembly.GetExecutingAssembly(),true)
            .Build();
        }
    }
}
