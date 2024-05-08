// See https://aka.ms/new-console-template for more information



using lbDataBase;
using lbRobots;
using Robots;
using System.Data;
System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

cConfig config = new cConfig();
//GetExcel dataExcel = new GetExcel(@"C:\TEMP\lUsuarios.xlsx");
//RobotEjemplo rEjem = new RobotEjemplo(config.RobotPruebaUrlLogin, config.UserLogin, config.PassLogin,config.RobotPruebaUrlUsuarios, dataExcel.DataExcel.Tables[0]);
//rEjem.EjecutarRobot();

//GetData Database
DataTable dt = DBUtil.GetData(config.ConnectionString, $@"SELECT DISTINCT
                                                                    LTRIM(RTRIM(substring(MV.ORDENENTMV,1, charindex('-',MV.ORDENENTMV,1)-1))) [Codigos EPS],
                                                                    LTRIM(RTRIM(substring(MV.ORDENENTMV, charindex('-',MV.ORDENENTMV,1)+1,15))) [Consecutivo],
                                                                    MV.TIPODCTO,
                                                                    MV.NRODCTO,
                                                                    CONVERT(VARCHAR(10), MV.FECHA, 103) AS FECHA_FACTURA,
                                                                    LTRIM(RTRIM(MV.ORDENENTMV)) [ORDEN],
                                                                    LTRIM(RTRIM(PER.TIPODC)) [TIPODC],
                                                                    LTRIM(RTRIM(PER.NIT)) NIT
                                                        FROM MVTRADE MV (NOLOCK)
                                                            INNER JOIN TRADE TD (NOLOCK) ON TD.ORIGEN = MV.ORIGEN AND TD.TIPODCTO = MV.TIPODCTO AND TD.NRODCTO = MV.NRODCTO
                                                            INNER JOIN TRADEMAS AS TMAS (NOLOCK) ON TMAS.ORIGEN = TD.ORIGEN AND TMAS.TIPODCTO = TD.TIPODCTO AND TMAS.NRODCTO = TD.NRODCTO
                                                            INNER JOIN MTPROCLI PER (NOLOCK) ON PER.NIT = TD.NIT
                                                        WHERE MV.ORDENENTMV != '' AND SUBSTRING(MV.ORDENENTMV, LEN(MV.ORDENENTMV)-1, LEN(MV.ORDENENTMV)) IN ('10', '11', '12')
                                                            AND MV.ORDENENTMV NOT LIKE '%A%' AND MV.ORDENENTMV NOT LIKE '%.%' AND MV.ORDENENTMV LIKE '%-%' AND TD.ORIGEN ='FAC'
                                                            AND TMAS.CODCANAL='01' AND MV.TIPODCTO  NOT IN ( 'R2','RE', 'NT', 'NK')
                                                            AND MV.PRODUCTO NOT IN ('S3500', 'S9900', 'S9901')
                                                            AND mv.FECHA='{DateTime.Today.ToString("yyyyMMdd")}'
                                                                    ");
//Codido para cargar informacion de base de datos en lña variable dt
string SecretTFA = config.ND_UserSecret; //DBUtil.GetValue<string>(config.ConnectionString, @"select Value from Diccionario where [Key]='ClaveTFA'") ;
RobotConsultaNotifiDespachos rConsNotifidespachos = new RobotConsultaNotifiDespachos(config.ND_UrlLogin, config.ND_UserLogin, config.ND_PassLogin, config.ND_UrlNotifiDespachos, dt, SecretTFA);
await rConsNotifidespachos.EjecutarRobot();

Console.ReadLine();
