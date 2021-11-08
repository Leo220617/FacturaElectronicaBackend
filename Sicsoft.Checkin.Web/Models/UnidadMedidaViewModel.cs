using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FacturaElectronica.Models
{
    public class UnidadMedidaViewModel
    {
        [StringLength(7)]
        public string codSAP { get; set; }

        [StringLength(4)]
        public string codCyber { get; set; }

        [StringLength(50)]
        public string Nombre { get; set; }
    }
}
