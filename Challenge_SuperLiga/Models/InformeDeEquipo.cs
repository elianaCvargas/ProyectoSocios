using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Challenge_SuperLiga.Models
{
    public class InformeDeEquipo
    {
        public string Equipo { get; set; }
        public float PromedioEdad { get; set; }
        public int MayorEdad { get; set; }
        public int MenorEdad { get; set; }
        public int CantidadSocios { get; set; }
    }
}