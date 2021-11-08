using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Castle.Core.Configuration;
using FacturaElectronica.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Refit;
using Sicsoft.Checkin.Web.Servicios;

namespace FacturaElectronica.Pages.Documentos
{
    public class EditarModel : PageModel
    {
        private readonly IConfiguration configuration;
        private readonly ICrudApi<EncDocumentosViewModel, int> service;
        private readonly ICrudApi<DetDocumentoViewModel, int> serviceD;

        [BindProperty]
        public EncDocumentosViewModel Enc { get; set; }
        [BindProperty]
        public DetDocumentoViewModel[] Det { get; set; }

        public EditarModel(ICrudApi<EncDocumentosViewModel, int> service, ICrudApi<DetDocumentoViewModel, int> serviceD)
        {
            this.service = service;
            this.serviceD = serviceD;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var Roles = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles.Where(a => a == "17").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }

                Enc = await service.ObtenerPorId(id);
                Det = await serviceD.ObtenerDetalles(id);



                return Page();
            }
            catch (ApiException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }
    }
}
