using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sicsoft.Checkin.Web.Servicios;
using Sicsoft.CostaRica.Checkin.Web.Models;
using System;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Linq;

namespace FacturaElectronica.Pages.Account
{
    public class CambioContraseñaModel : PageModel
    {
        private readonly ICrudApi<LoginUsuario, int> service;

        [BindProperty]
        public LoginUsuario Input { get; set; }

        public CambioContraseñaModel(ICrudApi<LoginUsuario, int> service)
        {
            this.service = service;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                



            }
            catch (Exception)
            {


            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                Input.id = Convert.ToInt32(((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == ClaimTypes.NameIdentifier).Select(s1 => s1.Value).FirstOrDefault());

                if (string.IsNullOrEmpty(Input.Clave))
                {
                    throw new Exception("La clave dsebe contener elementos");
                }


                await service.Editar(Input);
                return Redirect("../Index");
            }
            catch (Exception ex)
            {

                ModelState.AddModelError(string.Empty, ex.Message);
                //return Redirect("/Error");
                return Page();
            }
        }
    }
}
