﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FacturaElectronica.Models
{
    public class ParametrosViewModel
    {
        public int id { get; set; }
        public string FETEEnc { get; set; }
        public string FETEDet { get; set; }
        public string NCEnc { get; set; }
        public string NCDet { get; set; }
        public string FECEnc { get; set; }
        public string FECDet { get; set; }
        public string Exoneracion { get; set; }
        public string UbicacionCliente { get; set; }
        public string SerieFE { get; set; }
        public string SerieTE { get; set; }
        public string SerieNC { get; set; }
        public string SerieND { get; set; }
        public string SerieFEC { get; set; }
        public string SerieFEE { get; set; }
        public string urlCyber { get; set; }
        public string urlCyberRespHacienda { get; set; }
        public string CampoConsecutivo { get; set; }
        public string CampoClave { get; set; }
        public string CampoEstado { get; set; }

        public string urlCyberAceptacion { get; set; }
    }
}
