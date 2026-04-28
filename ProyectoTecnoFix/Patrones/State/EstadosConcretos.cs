using System;
using ProyectoTecnoFix.enums;
using ProyectoTecnoFix.modelo;

namespace ProyectoTecnoFix.Patrones.State;

// ── RECEPCIONADO ─────────────────────────────────────────────────────────
    public class EstadoRecepcionado : IEstadoOrden
    {
        public EstadoOrden Estado => EstadoOrden.Recepcionado;
 
        public void Avanzar(Orden orden)
        {
            orden.SetEstado(new EstadoEnDiagnostico(), "Equipo enviado a diagnóstico");
        }
 
        public void Retroceder(Orden orden)
        {
            Console.WriteLine("  ✗ Una orden recepcionada no puede retroceder.");
        }
 
        public string Descripcion() => "Equipo recibido en recepción, pendiente de diagnóstico.";
    }
 
    // ── EN DIAGNÓSTICO ───────────────────────────────────────────────────────
    public class EstadoEnDiagnostico : IEstadoOrden
    {
        public EstadoOrden Estado => EstadoOrden.EnDiagnostico;
 
        public void Avanzar(Orden orden)
        {
            orden.SetEstado(new EstadoEnReparacion(), "Diagnóstico completado, inicia reparación");
        }
 
        public void Retroceder(Orden orden)
        {
            orden.SetEstado(new EstadoRecepcionado(), "Regresado a recepción");
        }
 
        public string Descripcion() => "El técnico está evaluando el equipo.";
    }
 
    // ── EN REPARACIÓN ────────────────────────────────────────────────────────
    public class EstadoEnReparacion : IEstadoOrden
    {
        public EstadoOrden Estado => EstadoOrden.EnReparacion;
 
        public void Avanzar(Orden orden)
        {
            orden.SetEstado(new EstadoFinalizado(), "Reparación completada");
        }
 
        public void Retroceder(Orden orden)
        {
            orden.SetEstado(new EstadoEnDiagnostico(), "Requiere re-diagnóstico");
        }
 
        public string Descripcion() => "El técnico está realizando la reparación.";
    }
 
    // ── FINALIZADO ───────────────────────────────────────────────────────────
    public class EstadoFinalizado : IEstadoOrden
    {
        public EstadoOrden Estado => EstadoOrden.Finalizado;
 
        public void Avanzar(Orden orden)
        {
            orden.SetEstado(new EstadoEntregado(), "Equipo entregado al cliente");
        }
 
        public void Retroceder(Orden orden)
        {
            orden.SetEstado(new EstadoEnReparacion(), "Requiere reparación adicional");
        }
 
        public string Descripcion() => "Reparación lista, pendiente de entrega al cliente.";
    }
 
    // ── ENTREGADO ────────────────────────────────────────────────────────────
    public class EstadoEntregado : IEstadoOrden
    {
        public EstadoOrden Estado => EstadoOrden.Entregado;
 
        public void Avanzar(Orden orden)
        {
            Console.WriteLine("  ✗ La orden ya fue entregada. Estado final.");
        }
 
        public void Retroceder(Orden orden)
        {
            Console.WriteLine("  ✗ No se puede retroceder un equipo ya entregado.");
        }
 
        public string Descripcion() => "Equipo entregado. Proceso cerrado.";
    }