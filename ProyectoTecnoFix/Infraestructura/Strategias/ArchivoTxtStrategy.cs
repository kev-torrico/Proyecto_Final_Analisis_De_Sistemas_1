using System;
using ProyectoTecnoFix.enums;
using ProyectoTecnoFix.Infraestructura.ManejadorArchivos;
using ProyectoTecnoFix.modelo;
using ProyectoTecnoFix.Patrones.Strategy;

namespace ProyectoTecnoFix.Infraestructura.Strategias;

public class ArchivoTxtStrategy : IArchivoStrategy<Orden>
{
    public void Exportar(List<Orden> datos, string nombreArchivo)
    {
        string carpeta = Path.Combine(Directory.GetCurrentDirectory(), "data");

        if (!Directory.Exists(carpeta))
            Directory.CreateDirectory(carpeta);

        string rutaCompleta = Path.Combine(carpeta, nombreArchivo);

        var lineas = datos.Select(o =>
            $"{o.Id}|{o.Cliente}|{o.Equipo}|{o.Problema}|{o.Estado}");

        try
        {
            File.WriteAllLines(rutaCompleta, lineas);
            Console.WriteLine($"✔ TXT exportado en: {rutaCompleta}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Error al exportar TXT: {ex.Message}");
        }
    }

    public List<Orden> Importar(string nombreArchivo)
    {
        string ruta = Path.Combine(Directory.GetCurrentDirectory(), "data", nombreArchivo);

        var lista = new List<Orden>();

        if (!File.Exists(ruta))
            return lista;

        var lineas = File.ReadAllLines(ruta);

        foreach (var linea in lineas)
        {
            var partes = linea.Split('|');

            if (partes.Length < 5)
                continue;

            // Parse seguro
            if (!int.TryParse(partes[0], out int id))
                continue;

            string cliente = partes[1];
            string equipo = partes[2];
            string problema = partes[3];

            Enum.TryParse(partes[4], out EstadoOrden estado);

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

    // Respeta el flujo del patrón State
    private void AplicarEstado(Orden orden, EstadoOrden estado)
    {
        while (orden.Estado != estado)
        {
            orden.AvanzarEstado();
        }
    }
}