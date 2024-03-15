using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Castle.Core.Configuration;
using FacturaElectronica.Models;
using InversionGloblalWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Refit;
using Sicsoft.Checkin.Web.Servicios;

namespace FacturaElectronica.Pages.Aceptacion
{
    public class AceptacionMasivaModel : PageModel
    {
        private readonly ICrudApi<BandejaEntradaViewModel, int> sBandeja;
        private readonly ICrudApi<RecibidoRoles, int> compras;
        private readonly ICrudApi<ParametrosViewModel, int> service;

        [BindProperty]
        public BandejaEntradaViewModel[] Bandejas { get; set; }

        [BindProperty]
        public ParametrosViewModel Parametros { get; set; }

        [BindProperty]
        public string Sucursal { get; set; }

        [BindProperty(SupportsGet = true)]
        public ParametrosFiltros filtro { get; set; }

        public AceptacionMasivaModel(ICrudApi<BandejaEntradaViewModel, int> sBandeja, ICrudApi<RecibidoRoles, int> compras, ICrudApi<ParametrosViewModel, int> service)
        {
            this.sBandeja = sBandeja;
            this.compras = compras;
            this.service = service;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {


                var Roles1 = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles1.Where(a => a == "20").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }


                Parametros = await service.ObtenerPorId(1);

                DateTime time = new DateTime();

                if (time == filtro.FechaInicial)
                {

                    //await compras.RealizarLecturaEmails();
                    

                    filtro.FechaInicial = DateTime.Now;

                   // filtro.FechaInicial = filtro.FechaInicial.AddDays(-((filtro.FechaInicial.DayOfWeek - System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.FirstDayOfWeek + 7) % 7)).Date;

                    DateTime primerDia = filtro.FechaInicial;


                    DateTime ultimoDia = primerDia;//.AddDays(2);
                    filtro.FechaFinal = ultimoDia;
                    filtro.CodMoneda = "CRC";
                    filtro.Estado = "0";

                }

                
                Bandejas = await sBandeja.ObtenerLista(filtro);

                Bandejas = Bandejas.Where(a => a.Procesado != "1").ToArray();

                Sucursal = Bandejas.Where(a => !string.IsNullOrEmpty(a.CodigoActividad)).FirstOrDefault() == null ? "" : Bandejas.Where(a => !string.IsNullOrEmpty(a.CodigoActividad)).FirstOrDefault().CodigoActividad;

                return Page();
            }
            catch (ApiException ex)
            {
                Errores error = JsonConvert.DeserializeObject<Errores>(ex.Content.ToString());
                ModelState.AddModelError(string.Empty, error.Message);

                return Page();
            }
        }


        public async Task<IActionResult> OnPostGenerar(int[] recibidos, AceptacionParametros acep, ParametrosFiltros filtroAcep)
        {
            string error = "";


            try
            {
                Bandejas = await sBandeja.ObtenerLista(filtroAcep);
                Bandejas = Bandejas.Where(a => a.Procesado != "1").ToArray();

                for (int i = 0; i < recibidos.Length; i++)
                {
                    var Bandeja = Bandejas.Where(a => a.Id == recibidos[i]).FirstOrDefault();

                    Bandeja.tipo = acep.tipo;
                    switch (Bandeja.tipo)
                    {
                        case "05":
                            {
                                Bandeja.Mensaje = "1";
                                break;
                            }
                        case "06":
                            {
                                Bandeja.Mensaje = "2";
                                break;
                            }
                        case "07":
                            {
                                Bandeja.Mensaje = "3";
                                break;
                            }
                    }
                    Bandeja.DetalleMensaje = "Aceptacion Masiva";
                    Bandeja.CondicionImpuesto = acep.condicionImpuesto;
                    Bandeja.CodigoActividad = acep.AEconomica;
                    Bandeja.impuestoAcreditar = Math.Round(Bandeja.Impuesto);
                    Bandeja.gastoAplicable = Math.Round(Bandeja.TotalComprobante.Value);
                    Bandeja.idAceptador = Convert.ToInt32(((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == ClaimTypes.NameIdentifier).Select(s1 => s1.Value).FirstOrDefault());

                    var a = await sBandeja.Agregar(Bandeja);
                }

                //await service.Agregar(recibidos);


                return new JsonResult(true);
            }
            catch (ApiException ex)
            {
                Errores errores = JsonConvert.DeserializeObject<Errores>(ex.Content.ToString());
                ModelState.AddModelError(string.Empty, errores.Message);
                return new JsonResult(errores.Message);
                //return new JsonResult(false);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError(string.Empty, ex.Message);

                return new JsonResult(false);
            }
        }

    }
}
