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

namespace FacturaElectronica.Pages.Documentos
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration configuration;
        private readonly ICrudApi<EncDocumentosViewModel, int> service;
        private readonly ICrudApi<DetDocumentoViewModel, int> serviceD;


        [BindProperty]
        public EncDocumentosViewModel[] Documentos { get; set; }
        [BindProperty]
        public List<DetDocumentoViewModel> Det { get; set; }
        [BindProperty(SupportsGet = true)]
        public ParametrosFiltros filtro { get; set; }

        public IndexModel(ICrudApi<EncDocumentosViewModel, int> service, ICrudApi<DetDocumentoViewModel, int> serviceD)
        {
            this.service = service;
            this.serviceD = serviceD;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var Roles1 = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles1.Where(a => a == "16").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }

                DateTime time = new DateTime();

                if (time == filtro.FechaInicial)
                {


                    filtro.FechaInicial = DateTime.Now.AddDays(-1);

                    
                    filtro.FechaFinal = filtro.FechaInicial.AddDays(2);


                }

                Documentos = await service.ObtenerLista(filtro);
                Documentos = Documentos.Where(a => a.code == 1).ToArray();


                

                return Page();
            }
            catch (ApiException ex)
            {
                Errores error = JsonConvert.DeserializeObject<Errores>(ex.Content.ToString());
                ModelState.AddModelError(string.Empty, error.Message);

                return Page();
            }
        }


        public async Task<IActionResult> OnGetReenviar(int id, string sucursal,string correos)
        {
            try
            {

                await service.ReenvioFacturas(id,sucursal,correos);
                return new JsonResult(true);
            }
            catch (Exception ex)
            {
                return new JsonResult(false);
            }
        }
    }
}
