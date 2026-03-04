using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MvcCoreSessionEmpleados.Extensions;
using MvcCoreSessionEmpleados.Models;
using MvcCoreSessionEmpleados.Repositories;
using System.Threading.Tasks;

namespace MvcCoreSessionEmpleados.Controllers
{
    public class EmpleadosController : Controller
    {
        private IMemoryCache memoryCache;
        private RepositoryEmpleados repo;
        public EmpleadosController(RepositoryEmpleados repo, IMemoryCache memoryCache)
        {
            this.repo = repo;
            this.memoryCache = memoryCache;
        }
        public async Task<IActionResult> SessionSalarios(int? salario)
        {
            if (salario != null)
            {
                // QUEREMOS ALMACENAR LA SUMA TOTAL DE SALARIOS
                // QUE TENGAMOS EN SESSION
                int sumaTotal = 0;
                if (HttpContext.Session.GetString("SUMASALARIAL") != null)
                {
                    // SI YA TENEMOS DATOS ALMACENADOS, LOS RECUPERAMOS
                    sumaTotal = HttpContext.Session.GetObject<int>("SUMASALARIAL");
                }
                //SUMAMOS EL NUEVO SALARIO A LA SUMA TOTAL
                sumaTotal += salario.Value;
                HttpContext.Session.SetObject("SUMASALARIAL", sumaTotal);
                ViewData["MENSAJE"] = "Salario almacenado: " + salario + " €";
            }
            List<Empleado> empleados = await this.repo.GetEmpleadosAsync();
            return View(empleados);
        }

        public IActionResult SumaSalarial()
        {
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> EmpleadosSession(int? idEmpleado)
        {
            if (idEmpleado != null)
            {
                Empleado emp = await this.repo.FindEmpleadoAsync(idEmpleado.Value);
                // EN SESSION TENDREMOS ALMACENADOS UN CONJUNTO DE EMPLEADOS
                List<Empleado> empleadosList;
                if (HttpContext.Session.GetObject<List<Empleado>>("EMPLEADOS") != null)
                {
                    // RECUPERAMOS LA LISTA DE SESSION
                    empleadosList = HttpContext.Session.GetObject<List<Empleado>>("EMPLEADOS");
                 }
                else
                {
                    empleadosList = new List<Empleado>();
                }
                // AGREGAMOS EL EMPLEADO A LA LISTA
                empleadosList.Add(emp);
                // ALMACENAMOS LA LISTA EN SESSION
                HttpContext.Session.SetObject("EMPLEADOS", empleadosList);
                ViewData["MENSAJE"] = "Empleado " + emp.Apellido + " almacenado correctamente";
            }
            List<Empleado> empleados = await this.repo.GetEmpleadosAsync();
            return View(empleados);
        }

        public IActionResult EmpleadosAlmacenados()
        {
            return View();
        }

        public async Task<IActionResult> EmpleadosSessionOk(int? idEmpleado)
        {
            if (idEmpleado != null)
            {
                List<int> idsEmpleados;
                if (HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS") != null)
                {
                    idsEmpleados = HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
                }
                else
                {
                    idsEmpleados = new List<int>();
                }
                idsEmpleados.Add(idEmpleado.Value);
                HttpContext.Session.SetObject("IDSEMPLEADOS", idsEmpleados);
                ViewData["MENSAJE"] = "Id de Empleado " + idEmpleado + " almacenado correctamente";
            }
            List<Empleado> empleados = await this.repo.GetEmpleadosAsync();
            return View(empleados);
        }
        public async Task<IActionResult> EmpleadosAlmacenadosOk()
        {
            if (HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS") != null)
            {
                List<int> idsEmpleados = HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
                List<Empleado> empleados = await this.repo.GetEmpleadosInIdListAsync(idsEmpleados);
                return View(empleados);
            }
            return View();
        }
        public async Task<IActionResult> SessionEmpleadosV4(int? idEmpleado)
        {
            List<int> idsEmpleados;
            List<Empleado> empleados;
            if (idEmpleado != null)
            {
                
                if (HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS") != null)
                {
                    idsEmpleados = HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
                }
                else
                {
                    idsEmpleados = new List<int>();
                }
                idsEmpleados.Add(idEmpleado.Value);
                HttpContext.Session.SetObject("IDSEMPLEADOS", idsEmpleados);
                ViewData["MENSAJE"] = "Id de Empleado " + idEmpleado + " almacenado correctamente";
            }
            if (HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS") != null)
            {
                idsEmpleados = HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
                empleados = await this.repo.GetEmpleadosNotInIdListAsync(idsEmpleados);
            }
            else
            {
                empleados = await this.repo.GetEmpleadosAsync();
            }
                return View(empleados);
        }
        public async Task<IActionResult> EmpleadosAlmacenadosV4()
        {
            if (HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS") != null)
            {
                List<int> idsEmpleados = HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
                List<Empleado> empleados = await this.repo.GetEmpleadosInIdListAsync(idsEmpleados);
                return View(empleados);
            }
            return View();
        }
        public async Task<IActionResult> SessionEmpleadosV5(int? idEmpleado, int? idFavorito)
        {
            List<int> idsEmpleados;
            List<Empleado> empleados;

            if (idFavorito != null )
            {
                // COMO ESTOY ALMACENANDO EN CACHE, VAMOS A GUARDAR 
                // DIRECTAMENTE LOS OBJETOS EN LUGAR DE LOS IDS
                List<Empleado> empleadosFavoritos;
                if (this.memoryCache.Get("FAVORITOS") == null)
                {
                    // NO EXISTE NADA EN CACHE
                    empleadosFavoritos = new List<Empleado>();
                }
                else
                {
                    // RECUPERAMOS EL CACHE
                    empleadosFavoritos = this.memoryCache.Get<List<Empleado>>("FAVORITOS");
                }
                // BUSCAMOS AL EMPLEADO PARA GUARDARLO 
                Empleado empFav = await this.repo.FindEmpleadoAsync(idFavorito.Value);
                empleadosFavoritos.Add(empFav);
                this.memoryCache.Set("FAVORITOS", empleadosFavoritos);

            }

            if (idEmpleado != null)
            {

                if (HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS") != null)
                {
                    idsEmpleados = HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
                }
                else
                {
                    idsEmpleados = new List<int>();
                }
                idsEmpleados.Add(idEmpleado.Value);
                HttpContext.Session.SetObject("IDSEMPLEADOS", idsEmpleados);
                ViewData["MENSAJE"] = "Id de Empleado " + idEmpleado + " almacenado correctamente";
            }
            //if (HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS") != null)
            //{
            //    idsEmpleados = HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
            //    empleados = await this.repo.GetEmpleadosNotInIdListAsync(idsEmpleados);
            //}
            //else
            //{
            //    empleados = await this.repo.GetEmpleadosAsync();
            //}
            empleados = await this.repo.GetEmpleadosAsync();
            return View(empleados);
        }

        public async Task<IActionResult> EmpleadosAlmacenadosV5(int? ideliminar)
        {
            if (HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS") != null)
            {
                List<int> idsEmpleados = HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
                if (ideliminar != null)
                {
                    idsEmpleados.Remove(ideliminar.Value);
                    if (idsEmpleados.Count() == 0)
                    {
                        HttpContext.Session.Remove("IDSEMPLEADOS");
                    }
                    else
                    {
                        HttpContext.Session.SetObject("IDSEMPLEADOS", idsEmpleados);
                    }
                }                
                List<Empleado> empleados = await this.repo.GetEmpleadosInIdListAsync(idsEmpleados);
                return View(empleados);
            }
            return View();
        }
        public IActionResult EmpleadosFavoritos()
        {
            if (this.memoryCache.Get("FAVORITOS") == null)
            {
                ViewData["MENSAJE"] = "No tenemos favoritos";
                return View();
            }
            else
            {
                List<Empleado> favoritos = this.memoryCache.Get<List<Empleado>>("FAVORITOS");
                return View(favoritos);
            }
        }
    }
}
