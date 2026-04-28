using System;
using ProyectoTecnoFix.modelo;
using ProyectoTecnoFix.Patrones.Strategy;

namespace ProyectoTecnoFix.Patrones.Factory;

// Factory que centraliza la creación de Orden según tipo de equipo
    public static class OrdenFactory
    {
        public static Orden Crear(string tipoEquipo, int id, string cliente, string problema)
        {
            // Asigna la estrategia de costo según el tipo de equipo
            ICostoStrategy estrategia = tipoEquipo.ToLower() switch
            {
                "computadora" => new CostoBasico(tarifaManoObra: 80m),
                "impresora"   => new CostoBasico(tarifaManoObra: 50m),
                "movil"       => new CostoConDescuento(tarifaManoObra: 60m, porcentajeDescuento: 10m),
                _             => throw new ArgumentException($"Tipo de equipo no soportado: {tipoEquipo}")
            };
 
            return new Orden(id, cliente, tipoEquipo, problema, estrategia);
        }
    }