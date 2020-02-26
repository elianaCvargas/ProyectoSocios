using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Challenge_SuperLiga.Models
{
    public class LectorDeSocios
    {
        public PaginadorGenerico<Socio> Paginador { get; set; } = new PaginadorGenerico<Socio>();

        public LectorDeSocios(PaginadorGenerico<Socio> paginadorSocios)
        {
            Paginador = paginadorSocios;
        }

        public LectorDeSocios()
        {
        }
    }
}