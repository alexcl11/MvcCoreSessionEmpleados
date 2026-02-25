using Microsoft.EntityFrameworkCore;
using MvcCoreSessionEmpleados.Data;
using MvcCoreSessionEmpleados.Models;

namespace MvcCoreSessionEmpleados.Repositories
{
    public class RepositoryEmpleados
    {
        private HospitalContext context;
        public RepositoryEmpleados(HospitalContext context)
        {
            this.context = context;
        }

        public async Task<List<Empleado>> GetEmpleadosAsync()
        {
            List<Empleado> empleados = await (from datos in this.context.Empleados
                             select datos).ToListAsync();
            return empleados;
        }
        public async Task<Empleado> FindEmpleadoAsync(int idEmpleado)
        {
            Empleado empleado = await (from datos in this.context.Empleados
                                 where datos.IdEmpleado == idEmpleado
                                 select datos).FirstOrDefaultAsync();
            return empleado;
        }
        public async Task<List<Empleado>> GetEmpleadosInIdListAsync(List<int> idsEmpleados)
        {
            List<Empleado> empleados = await (from datos in this.context.Empleados
                           where idsEmpleados.Contains(datos.IdEmpleado)
                           select datos).ToListAsync();
            return empleados;
        }
        public async Task<List<Empleado>> GetEmpleadosNotInIdListAsync(List<int> idsEmpleados)
        {
            List<Empleado> empleados = await (from datos in this.context.Empleados
                                              where !idsEmpleados.Contains(datos.IdEmpleado)
                                              select datos).ToListAsync();
            return empleados;
        }
    }
}
