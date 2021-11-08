using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FacturaElectronica.Models;
using InversionGloblalWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Refit;
using Sicsoft.Checkin.Web.Servicios;

namespace FacturaElectronica.Pages.UnidadesMedida
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration configuration;
        private readonly ICrudApi<UnidadMedidaViewModel, string> unidad;

        [BindProperty]
        public UnidadMedidaViewModel[] Unidades { get; set; }

        [BindProperty(SupportsGet = true)]
        public ParametrosFiltros filtro { get; set; }

        public IndexModel(ICrudApi<UnidadMedidaViewModel, string> unidad)
        {
            this.unidad = unidad;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var Roles1 = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles1.Where(a => a == "10").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }

                Unidades = await unidad.ObtenerLista(filtro);



                return Page();
            }
            catch (ApiException ex)
            {
                Errores error = JsonConvert.DeserializeObject<Errores>(ex.Content.ToString());
                ModelState.AddModelError(string.Empty, error.Message);

                return Page();
            }
        }
        public async Task<IActionResult> OnGetEliminar(string id)
        {
            try
            {

                await unidad.EliminarInversion(id);
                return new JsonResult(true);
            }
            catch (ApiException ex)
            {
                return new JsonResult(false);
            }
        }
    }
}
