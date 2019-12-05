using System;
using System.Collections.Generic;
using System.IO;
using apiRestM.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace apiRestM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LayoutController : Controller
    {
        private string _logPath = "c:\\";


        // POST: api/Layout
        [HttpPost]
        public JsonResult Loan_ReadFromLayout([FromBody]JObject aObject)
        {
            try
            {
                string base64String = aObject["File"].ToObject<string>();
                byte[] tempBytes = Convert.FromBase64String(base64String);
                List<Loans> layout = Util.ImportCSV.GetList<Loans>(tempBytes);
                


                return Json(true);
            }
            catch (Exception aE)
            {
                LogError(aE);
                return Json(aE.Message);
            }
        }

        private void LogError(Exception aE)
        {
            try
            {
                string path = string.Format("{0}.\\TraceDB_{1}_{2:yyyyMMdd}.trace", _logPath, "apiRestM", DateTime.Today);
                int intentos = 0;

                if (Directory.Exists(_logPath))
                {

                    while (intentos < 3)
                    {
                        try
                        {
                            using (StreamWriter writer = System.IO.File.AppendText(path))
                            {
                                
                                writer.Write(DateTime.Now.ToString("HH:mm:ss.fff") + "\t");
                                writer.WriteLine(aE.Message);
                                writer.WriteLine(aE.StackTrace);
                                writer.WriteLine("");
                                writer.Close();
                                return;
                            }
                        }
                        catch (UnauthorizedAccessException)
                        {
                            //readonly error
                            try
                            {
                                intentos++;
                                System.IO.File.SetAttributes(path, System.IO.File.GetAttributes(path) & ~FileAttributes.ReadOnly);
                            }
                            catch { }
                        }
                        catch (IOException)
                        {
                            intentos++;
                        }
                    }
                }
            }
            catch { }
        }
    }
}
