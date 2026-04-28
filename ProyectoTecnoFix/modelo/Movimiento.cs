using System;
using ProyectoTecnoFix.enums;

namespace ProyectoTecnoFix.modelo;

    public class Movimiento
    {
        public DateTime Fecha { get; set; }
        public EstadoOrden EstadoAnterior { get; set; }
        public EstadoOrden EstadoNuevo { get; set; }
        public string Observacion { get; set; }
 
        public Movimiento(EstadoOrden anterior, EstadoOrden nuevo, string observacion)
        {
            Fecha = DateTime.Now;
            EstadoAnterior = anterior;
            EstadoNuevo = nuevo;
            Observacion = observacion;
        }
 
        public override string ToString()
        {
            return $"[{Fecha:dd/MM/yyyy HH:mm}] {EstadoAnterior} → {EstadoNuevo} | {Observacion}";
        }
    }