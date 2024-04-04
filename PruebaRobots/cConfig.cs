using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robots
{
    public class cConfig
    {
        IConfigurationRoot config;
        public string ConnectionString { get { return config["Robots:ConnectionString"]; } }
        public string RobotPruebaUrlLogin { get { return config["RobotPrueba:UrlLogin"]; } }
        public string RobotPruebaUrlUsuarios { get { return config["RobotPrueba:UrlUsuarios"]; } }
        public string UserLogin { get { return config["RobotPrueba:UserLogin"]; } }
        public string PassLogin { get { return config["RobotPrueba:Passlogin"]; } }

        public cConfig()
        {
             config = new ConfigurationBuilder()
            .AddUserSecrets<cConfig>()
            .Build();
        }
    }
}
