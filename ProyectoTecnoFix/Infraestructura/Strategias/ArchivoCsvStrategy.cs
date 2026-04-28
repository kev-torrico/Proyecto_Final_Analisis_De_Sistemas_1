using System;
using ProyectoTecnoFix.enums;
using ProyectoTecnoFix.Infraestructura.ManejadorArchivos;
using ProyectoTecnoFix.modelo;
using ProyectoTecnoFix.Patrones.Strategy;

namespace ProyectoTecnoFix.Infraestructura.Strategias;

public class ArchivoCsvStrategy : IArchivoStrategy<Orden>
{
    public void Exportar(List<Orden> datos, string nombreArchivo)
    {
        string carpeta = Path.Combine(Directory.GetCurrentDirectory(), "data");

        if (!Directory.Exists(carpeta))
            Directory.CreateDirectory(carpeta);

        string rutaCompleta = Path.Combine(carpeta, nombreArchivo);

        var lineas = datos.Select(o =>
            $"{o.Id},{o.Cliente},{o.Equipo},{o.Problema},{o.Estado}");

        try
        {
            File.WriteAllLines(rutaCompleta, lineas);
            Console.WriteLine($"✔ CSV exportado en: {rutaCompleta}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Error al exportar CSV: {ex.Message}");
        }
    }

    public List<Orden> Importar(string nombreArchivo)
    {
        string ruta = Path.Combine(Directory.GetCurrentDirectory(), "data", nombreArchivo);

        var lista = new List<Orden>();

        if (!File.Exists(ruta))
            return lista;

        var lineas = File.ReadAllLines(ruta);

        foreach (var l in lineas)
        {
            var p = l.Split(',');

            if (p.Length < 5)
                continue;

            // Parse seguro
            if (!int.TryParse(p[0], out int id))
                continue;

            string cliente = p[1];
            string equipo = p[2];
            string problema = p[3];

            Enum.TryParse(p[4], out EstadoOrden estado);

            var orden = new Orden(
                id,
                cliente,
                equipo,
                problema,
                new CostoBasico()
            );

            AplicarEstado(orden, estado);

            lista.Add(orden);
        }

        return lista;
    }

    private void AplicarEstado(Orden orden, EstadoOrden estado)
    {
        while (orden.Estado != estado)
        {
            orden.AvanzarEstado();
        }
    }
}