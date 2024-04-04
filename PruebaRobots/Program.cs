// See https://aka.ms/new-console-template for more information



using lbDataBase;
using lbRobots;
using Robots;
System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

cConfig config = new cConfig();
GetExcel dataExcel = new GetExcel(@"C:\TEMP\lUsuarios.xlsx");
RobotEjemplo rEjem = new RobotEjemplo(config.RobotPruebaUrlLogin, config.UserLogin, config.PassLogin,config.RobotPruebaUrlUsuarios, dataExcel.DataExcel.Tables[0]);
rEjem.EjecutarRobot();

Console.ReadLine();
