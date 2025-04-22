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
    public class IndexModel : PageModel
    {
        private readonly IConfiguration configuration;
        private readonly ICrudApi<BandejaEntradaViewModel, int> sBandeja;
        private readonly ICrudApi<RecibidoRoles, int> compras;
        private readonly ICrudApi<ParametrosViewModel, int> service;
        private readonly ICrudApi<UsuariosViewModel, int> usuarios;

        [BindProperty]
        public BandejaEntradaViewModel[] Bandejas { get; set; }

        [BindProperty]
        public UsuariosViewModel[] Usuarios { get; set; }

        [BindProperty]
        public ParametrosViewModel Parametros { get; set; }

        [BindProperty(SupportsGet = true)]
        public ParametrosFiltros filtro { get; set; }

        public IndexModel(ICrudApi<BandejaEntradaViewModel, int> sBandeja, ICrudApi<RecibidoRoles, int> compras, ICrudApi<ParametrosViewModel, int> service, ICrudApi<UsuariosViewModel, int> usuarios)
        {
            this.sBandeja = sBandeja;
            this.compras = compras;
            this.service = service;
            this.usuarios = usuarios;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {


                var Roles1 = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles1.Where(a => a == "18").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }


                Parametros = await service.ObtenerPorId(1);

                DateTime time = new DateTime();

                if (time == filtro.FechaInicial)
                {

                    //
                   // await compras.RealizarLecturaEmails(); // 
                    filtro.FechaInicial = DateTime.Now;

                    //filtro.FechaInicial = filtro.FechaInicial.AddDays(-((filtro.FechaInicial.DayOfWeek - System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.FirstDayOfWeek + 7) % 7)).Date;
                    
                    DateTime primerDia = filtro.FechaInicial;


                    DateTime ultimoDia = primerDia;//.AddDays(2);
                    filtro.FechaFinal = ultimoDia;
                    filtro.Estado = "0";
                    filtro.Codigo1 = 0;
                    filtro.CodMoneda = "CRC";
                    filtro.Texto = "";
                }

                Bandejas = await sBandeja.ObtenerLista(filtro); //aqui por el rango de fechas por eso se cuelga

                Usuarios = await usuarios.ObtenerLista(""); //

                return Page();
            }
            catch (ApiException ex)
            {
                Errores error = JsonConvert.DeserializeObject<Errores>(ex.Content.ToString());
                ModelState.AddModelError(string.Empty, error.Message);

                return Page();
            }
        }
    }
}
