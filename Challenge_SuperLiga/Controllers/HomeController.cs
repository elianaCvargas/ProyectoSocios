using Challenge_SuperLiga.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Challenge_SuperLiga.Controllers
{
    public class HomeController : Controller
    {
        private readonly int _RegistrosPorPagina = 10;
        private static List<Socio> _sociosPaginaActual = new List<Socio>();
        private static List<Socio> _allSocios = new List<Socio>();
        private static PaginadorGenerico<Socio> _PaginadorSocios = new PaginadorGenerico<Socio>();
        private static int _totalRegistros = 0;
        private static List<InformeDeEquipo> informes = new List<InformeDeEquipo>();
        private static List<string> hinchasDeRiver = new List<string>();
        private static InformeGeneral informeGeneral = new InformeGeneral();
        private static List<Socio> sociosCasadosUnivesitarios = new List<Socio>();
        private static List<InformeDeEquipo> infoEquipos = new List<InformeDeEquipo>();
        private static PaginadorGenerico<Socio> _sociosCasadosUnivesitarios = new PaginadorGenerico<Socio>();


        public ActionResult Index(int? pagina)
        {

            int paginaActualInt = pagina.HasValue ? pagina.Value : 1;
           
                _sociosPaginaActual = _allSocios.OrderBy(x => x.Nombre)
                                                     .Skip((paginaActualInt - 1) * _RegistrosPorPagina)
                                                     .Take(_RegistrosPorPagina)
                                                     .ToList();

                var _TotalPaginas = (int)Math.Ceiling((double)_totalRegistros / _RegistrosPorPagina);
                _PaginadorSocios = new PaginadorGenerico<Socio>()
                {
                    RegistrosPorPagina = _RegistrosPorPagina,
                    TotalRegistros = _totalRegistros,
                    TotalPaginas = _TotalPaginas,
                    PaginaActual = paginaActualInt,
                    Resultado = _sociosPaginaActual
                };

                if (_PaginadorSocios.Resultado.Count() == 0)
                {
                    System.Web.HttpContext.Current.Session["sessionString"] = "empty";
                    ViewData["sessionString"] = System.Web.HttpContext.Current.Session["sessionString"] as string;
                }
                else
                {
                    ViewData["sessionString"] = "hayDatos";
                }

                var lectorSocio = new LectorDeSocios(_PaginadorSocios);
                return View(lectorSocio.Paginador);
            
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase uploadFile)
        {


            ViewBag.FilePath = uploadFile;
            _allSocios = new List<Socio>();
            string filePath = string.Empty;

            if (uploadFile != null)
            {
                string path = Server.MapPath("~/Uploads/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                filePath = path + Path.GetFileName(uploadFile.FileName);
                string extension = Path.GetExtension(uploadFile.FileName);
                uploadFile.SaveAs(filePath);

                try
                {
                    string csvData = System.IO.File.ReadAllText(filePath, Encoding.GetEncoding("iso-8859-1"));
                    foreach (string row in csvData.Split('\n'))
                    {
                        if (!string.IsNullOrEmpty(row))
                        {
                            _allSocios.Add(new Socio
                            {
                                Nombre = row.Split(';')[0],
                                Edad = Convert.ToInt32(row.Split(';')[1]),
                                Equipo = row.Split(';')[2],
                                EstadoCivil = row.Split(';')[3],
                                Estudios = row.Split(';')[4]
                            });
                        }
                    }
                }
                catch (Exception)
                {
                    _PaginadorSocios = new PaginadorGenerico<Socio>()
                    {
                        MensajeError = "Formato de archivo incorrecto",
                        HayError = true
                    };

                    return View(_PaginadorSocios);
                }


                _totalRegistros = _allSocios.Count;

                _sociosPaginaActual = _allSocios.OrderBy(x => x.Edad)
                                                     .Skip(0)
                                                     .Take(_RegistrosPorPagina)
                                                     .ToList();

                //Seteo datos Informe
                informeGeneral.PromedioEdadPorEquipo = PromedioDeEdadPorClub(_allSocios, "racing");
                informeGeneral.NombresRecurrentesPorEquipo = ObtnerNombresComunesPorClub(_allSocios, "river");
                informeGeneral.TotalSocios = _totalRegistros;
                informeGeneral.InformePorEquipo = InformePorEquipo(_allSocios);

                sociosCasadosUnivesitarios = LitarSociosCasadosUniversitarios(_allSocios);
            }

            var _totalpaginas = (int)Math.Ceiling((double)_totalRegistros / _RegistrosPorPagina);
            _PaginadorSocios = new PaginadorGenerico<Socio>()
            {
                RegistrosPorPagina = _RegistrosPorPagina,
                TotalRegistros = _totalRegistros,
                TotalPaginas = _totalpaginas,
                PaginaActual = 1,
                Resultado = _sociosPaginaActual
            };

            ViewData["sessionString"] = "hayDatos";
            return View(_PaginadorSocios);
        }

        private float PromedioDeEdadPorClub(List<Socio> socios, string club)
        {
            var sociosPorClub = socios.Where(x => x.Equipo.ToLower() == club.ToLower()).ToList();
            return (sociosPorClub.Sum(x => x.Edad) / sociosPorClub.Count);
        }

        private List<Socio> LitarSociosCasadosUniversitarios(List<Socio> socios)
        {
            var sociosCasadosUniversitarios = socios.Where(x => x.EstadoCivil.ToLower() == "casado" && x.Estudios.ToLower() == "universitario\r").Take(100);

            return sociosCasadosUniversitarios.OrderBy(x => x.Edad).ToList();
        }
        private List<string> ObtnerNombresComunesPorClub(List<Socio> socios, string club)
        {
            return socios.Where(x => x.Equipo.ToLower().Equals(club))
                .GroupBy(x => x.Nombre)
                .OrderByDescending(r => r.Count())
                .Take(5)
                .Select(s => s.Key).ToList();
        }

        private List<InformeDeEquipo> InformePorEquipo(List<Socio> socios)
        {
            infoEquipos = new List<InformeDeEquipo>();

            var lista =
                (from s in socios
                 group s by s.Equipo
                 into gr
                 select new
                 {
                     Equipo = gr.Key,
                     Count = gr.Count()

                 });

            foreach (var item in lista)
            {
                infoEquipos.Add(new InformeDeEquipo
                {
                    Equipo = item.Equipo,
                    CantidadSocios = item.Count,
                    PromedioEdad = PromedioDeEdadPorClub(socios, item.Equipo),
                    MayorEdad = socios.Where(x => x.Equipo.ToLower() == item.Equipo.ToLower()).Max(x => x.Edad),
                    MenorEdad = socios.Where(x => x.Equipo.ToLower() == item.Equipo.ToLower()).Min(x => x.Edad)
                });
            }

            return infoEquipos.OrderByDescending(x => x.CantidadSocios).ToList();
        }

        public ActionResult InfoEquipo()
        {
            return View(informeGeneral);
        }

        public ActionResult ListadoPersonasCasadasConUniversitarias(int? pagina)
        {
            int paginaActualInt = pagina.HasValue ? pagina.Value : 1;

            int totalRegistros = sociosCasadosUnivesitarios.Count;

            List<Socio> sociosPaginaActual = sociosCasadosUnivesitarios.OrderBy(x => x.Edad)
                                                 .Skip(0)
                                                 .Take(_RegistrosPorPagina)
                                                 .ToList();

            var totalpaginas = (int)Math.Ceiling((double)totalRegistros / _RegistrosPorPagina);
            _sociosCasadosUnivesitarios = new PaginadorGenerico<Socio>()
            {
                RegistrosPorPagina = _RegistrosPorPagina,
                TotalRegistros = totalRegistros,
                TotalPaginas = totalpaginas,
                PaginaActual = paginaActualInt,
                Resultado = sociosPaginaActual
            };

            return View(_sociosCasadosUnivesitarios);
        }
    }
}