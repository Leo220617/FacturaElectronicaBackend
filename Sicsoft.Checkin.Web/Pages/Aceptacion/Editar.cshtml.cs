using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FacturaElectronica.Models;
using InversionGloblalWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Refit;
using Sicsoft.Checkin.Web.Servicios;

namespace FacturaElectronica.Pages.Aceptacion
{
    public class EditarModel : PageModel
    {
       
        private readonly ICrudApi<BandejaEntradaViewModel, int> service;

        [BindProperty]
        public BandejaEntradaViewModel Bandeja { get; set; }

        public EditarModel(ICrudApi<BandejaEntradaViewModel, int> service )
        {
            this.service = service;
      
        }


        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var Roles = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles.Where(a => a == "19").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }

                Bandeja = await service.ObtenerPorId(id);
                Bandeja.Impuesto = Math.Round(Bandeja.Impuesto);
                Bandeja.TotalComprobante = Math.Round(Bandeja.TotalComprobante.Value);



                return Page();
            }
            catch (ApiException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                switch(Bandeja.tipo)
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
                var a = await service.Agregar(Bandeja);
                return RedirectToPage("./Index");
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
