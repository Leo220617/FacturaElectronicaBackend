using InversionGloblalWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Refit;
using Sicsoft.Checkin.Web.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FacturaElectronica.Models;

namespace Sicsoft.Checkin.Web.Pages
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
       
        private readonly ICrudApi<UsuariosViewModel, int> users;

        [BindProperty(SupportsGet = true)]
        public ParametrosFiltros filtro { get; set; }
        [BindProperty]
        public UsuariosViewModel[] Usuarios { get; set; }

 

        [BindProperty]
        public int CantAbiertos { get; set; }
        [BindProperty]
        public int CantCerrados { get; set; }
        [BindProperty]
        public int CantEspera { get; set; }


        public IndexModel(ILogger<IndexModel> logger, ICrudApi<UsuariosViewModel, int> users)
        {
            _logger = logger;
       
            this.users = users;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var Roles1 = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles1.Where(a => a == "4").FirstOrDefault()))
                {
                    filtro.Codigo1 = Convert.ToInt32(((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == ClaimTypes.NameIdentifier).Select(s1 => s1.Value).FirstOrDefault());
                }

                DateTime time = new DateTime();
                if (time == filtro.FechaInicial)
                {
                    filtro.FechaInicial = DateTime.Now;

                    filtro.FechaInicial = new DateTime(filtro.FechaInicial.Year, filtro.FechaInicial.Month, 1);


                    DateTime primerDia = new DateTime(filtro.FechaInicial.Year, filtro.FechaInicial.Month, 1);


                    DateTime ultimoDia = primerDia.AddMonths(1).AddDays(-1);

                    filtro.FechaFinal = ultimoDia;

                }

            
                Usuarios = await users.ObtenerLista("");
               
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
