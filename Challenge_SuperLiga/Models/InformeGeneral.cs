using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Challenge_SuperLiga.Models
{
    public class InformeGeneral
    {
        public int TotalSocios { get; set; }
        public float PromedioEdadPorEquipo { get; set; }
        public List<string> NombresRecurrentesPorEquipo { get; set; }
        public List<InformeDeEquipo> InformePorEquipo { get; set; }

        public InformeGeneral()
        {
        }
    }
}