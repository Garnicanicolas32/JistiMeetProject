using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace WebApplication1.Controllers
{
    class Meet
    {
        public int transaccion_id;
        public string dateTime_start;
        public int prestador_id, user_id, status_id;

        public Meet(int transaccion_id, string dateTime_start, int prestador_id, int user_id, int status_id)
        {
            this.transaccion_id = transaccion_id;
            this.dateTime_start = dateTime_start;
            this.prestador_id = prestador_id;
            this.user_id = user_id;
            this.status_id = status_id;
        }
    };
    class MeetFinal
    {
        public int transaccion_id;
        public string dateTime_start, dateTime_end, duracion;
        public int prestador_id, user_id, status_id;
        public string videollamada_ID;

        public MeetFinal(int transaccion_id, string dateTime_start, string dateTime_end, string duracion, int prestador_id, int user_id, int status_id, string videollamada_ID)
        {
            this.transaccion_id = transaccion_id;
            this.dateTime_start = dateTime_start;
            this.dateTime_end = dateTime_end;
            this.duracion = duracion;
            this.prestador_id = prestador_id;
            this.user_id = user_id;
            this.status_id = status_id;
            this.videollamada_ID = videollamada_ID;
        }
    };
    class respuesta
    {
        public string videollamada_ID;
        public int result;

        public respuesta(string videollamada_ID, int result)
        {
            this.videollamada_ID = videollamada_ID;
            this.result = result;
        }
    }

    public class DoctaController : Controller
    {
        public async Task<string> EndAPI(int transaccion_id, DateTime FormatdateTime_start, int prestador_id, int user_id, int status_id, string videollamada_ID)
        {
            //var url = "https://httpbin.org/post"; URL DE TESTEO            

            string dateTime_end = DateTime.Now.ToString("dd'/'MM'/'yyyy' 'HH':'mm':'ss");            
            
            TimeSpan ts = DateTime.Now.Subtract(FormatdateTime_start);
                        
            string dateTime_start = FormatdateTime_start.ToString("dd'/'MM'/'yyyy' 'HH':'mm':'ss");

            string duracion = ts.TotalMinutes.ToString();


            //string duracion = ts.ToString();
            //duracion = duracion.Remove(duracion.IndexOf("."));

            var meet = new MeetFinal(transaccion_id, dateTime_start, dateTime_end, duracion, prestador_id, user_id, status_id, videollamada_ID);
            var json = JsonConvert.SerializeObject(meet);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            //var url = "https://httpbin.org/post";
            var url = "http://200.55.245.19:5002/api/registro_videollamada";
            using var client = new HttpClient();
            var response = await client.PostAsync(url, data);
            string result = response.Content.ReadAsStringAsync().Result;
            return result;
        }

        public async Task<string> StartAPI(int transaccion_id, int prestador_id, int user_id)
        {
            //var url = "https://httpbin.org/post"; URL DE TESTEO
            string dateTime_start = DateTime.Now.ToString();
            var meet = new Meet(transaccion_id, dateTime_start, prestador_id, user_id, 1);
            var json = JsonConvert.SerializeObject(meet);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var url = "http://200.55.245.19:5002/api/registro_videollamada";
            using var client = new HttpClient();
            var response = await client.PostAsync(url, data);
            string result = response.Content.ReadAsStringAsync().Result;
            return result;  
        }
        
        [HttpGet]
        public async Task<IActionResult> Call(string Data)
        {
            string ID = "";
            int transaccion_id = -1;
            int prestador_id = -1;
            int user_id = -1;
            bool error = false;

            if (Data != null)
            {
                string[] Sresultado = Data.Split('-');
                int cont = 0;
                foreach (var Res in Sresultado)
                {
                    int number;
                    bool isParsable = Int32.TryParse(Res, out number);
                    if (isParsable)
                    {
                        switch (cont)
                        {
                            case 0:
                                transaccion_id = number;
                                break;
                            case 1:
                                prestador_id = number;
                                break;
                            case 2:
                                user_id = number;
                                break;
                        }
                    }
                    else
                    { error = true; }

                    cont++;
                }
                if (transaccion_id < 0 || prestador_id < 0 || user_id < 0)
                { error = true; }

                string result = await StartAPI(transaccion_id, prestador_id, user_id);
                respuesta Final = JsonConvert.DeserializeObject<respuesta>(result);
                ID = Final.videollamada_ID;
                if (Final.result != 1) { error = true; }
            }

            if (!error)
            {
                TempData["transaccion_id"] = transaccion_id;
                TempData["FormatdateTime_start"] = DateTime.Now;
                TempData["prestador_id"] = prestador_id;
                TempData["user_id"] = user_id;
                TempData["videollamada_ID"] = ID;

                ViewBag.id = ID;
            }
            else { return View("Index"); }


            return View();
        }

        [HttpGet]
        public async Task<IActionResult> EndCall(int status_id)
        {
            int transaccion_id = int.Parse(TempData["transaccion_id"].ToString());
            DateTime FormatdateTime_start = DateTime.Parse(TempData["FormatdateTime_start"].ToString());
            int prestador_id = int.Parse(TempData["prestador_id"].ToString());
            int user_id = int.Parse(TempData["user_id"].ToString());
            string videollamada_ID = TempData["videollamada_ID"].ToString();
            
            string result = await EndAPI(transaccion_id, FormatdateTime_start, prestador_id, user_id, status_id, videollamada_ID);
            ViewBag.text = result;
            return View();
        }

        private readonly ILogger<DoctaController> _logger;

        public DoctaController(ILogger<DoctaController> logger)
        {
            _logger = logger;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
