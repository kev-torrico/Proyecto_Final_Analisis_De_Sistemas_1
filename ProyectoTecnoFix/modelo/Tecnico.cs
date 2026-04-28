using System;
using ProyectoTecnoFix.enums;

namespace ProyectoTecnoFix.modelo;

public class Tecnico
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Especialidad { get; set; }
        public List<Orden> Ordenes { get; set; }
 
        public Tecnico(int id, string nombre, string especialidad)
        {
            Id = id;
            Nombre = nombre;
            Especialidad = especialidad;
            Ordenes = new List<Orden>();
        }
 
        public void AsignarOrden(Orden orden)
        {
            Ordenes.Add(orden);
        }
 
        public int ObtenerCarga()
        {
            return Ordenes.Count(o =>
                o.Estado != EstadoOrden.Entregado &&
                o.Estado != EstadoOrden.Finalizado);
        }
 
        public override string ToString()
        {
            return $"[{Id}] {Nombre} | Especialidad: {Especialidad} | Órdenes activas: {ObtenerCarga()}";
        }
    }