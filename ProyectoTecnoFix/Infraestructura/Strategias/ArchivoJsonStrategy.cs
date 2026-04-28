using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using ProyectoTecnoFix.dto;
using ProyectoTecnoFix.enums;
using ProyectoTecnoFix.Infraestructura.ManejadorArchivos;
using ProyectoTecnoFix.modelo;
using ProyectoTecnoFix.Patrones.Strategy;

namespace ProyectoTecnoFix.Infraestructura.Strategias;

public class ArchivoJsonStrategy : IArchivoStrategy<Orden>
{
    public void Exportar(List<Orden> datos, string nombreArchivo)
    {
        string carpeta = Path.Combine(Directory.GetCurrentDirectory(), "data");

        // Crear carpeta si no existe
        if (!Directory.Exists(carpeta))
            Directory.CreateDirectory(carpeta);

        // Ruta completa
        string rutaCompleta = Path.Combine(carpeta, nombreArchivo);

        // Mapear a DTO
        var dto = datos.Select(o => new OrdenDTO
        {
            Id = o.Id,
            Cliente = o.Cliente,
            Equipo = o.Equipo,
            Problema = o.Problema,
            Estado = o.Estado
        }).ToList();

        // Serializar
        var json = JsonSerializer.Serialize(dto, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        // Guardar archivo
        File.WriteAllText(rutaCompleta, json);

        Console.WriteLine($"✔ JSON exportado en: {rutaCompleta}");
    }

    public List<Orden> Importar(string ruta)
    {
        if (!File.Exists(ruta))
            return new List<Orden>();

        var json = File.ReadAllText(ruta);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        options.Converters.Add(new JsonStringEnumConverter());

        var dto = JsonSerializer.Deserialize<List<OrdenDTO>>(json, options);

        var lista = new List<Orden>();

        foreach (var d in dto)
        {
            var orden = new Orden(
                d.Id,
                d.Cliente,
                d.Equipo,
                d.Problema,
                new CostoBasico()
            );

            AplicarEstado(orden, d.Estado);

            lista.Add(orden);
        }

        return lista;
    }

    private void AplicarEstado(Orden orden, EstadoOrden estado)
    {
        while (orden.Estado != estado)
            orden.AvanzarEstado();
    }
}