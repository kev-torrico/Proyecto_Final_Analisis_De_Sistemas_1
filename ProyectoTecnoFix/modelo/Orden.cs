using System;
using ProyectoTecnoFix.enums;
using ProyectoTecnoFix.Patrones.Observer;
using ProyectoTecnoFix.Patrones.State;
using ProyectoTecnoFix.Patrones.Strategy;

namespace ProyectoTecnoFix.modelo;

// Orden es el SUJETO del patrón Observer y el CONTEXTO del patrón State
    public class Orden
    {
        // ── Datos básicos ────────────────────────────────────────────────────
        public int Id { get; set; }
        public string Cliente { get; set; }
        public string Equipo { get; set; }
        public string Problema { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public Tecnico TecnicoAsignado { get; set; }
        public List<Repuesto> Repuestos { get; private set; }
        public List<Movimiento> Movimientos { get; private set; }
 
        // ── State: estado actual como objeto ────────────────────────────────
        private IEstadoOrden _estadoActual;
        public EstadoOrden Estado => _estadoActual.Estado;
 
        // ── Strategy: forma de calcular el costo ────────────────────────────
        private ICostoStrategy _estrategiaCosto;
 
        // ── Observer: lista de suscriptores ─────────────────────────────────
        private readonly List<IOrdenObserver> _observadores = new();
 
        // ── Constructor ──────────────────────────────────────────────────────
        public Orden(int id, string cliente, string equipo, string problema, ICostoStrategy estrategia)
        {
            Id = id;
            Cliente = cliente;
            Equipo = equipo;
            Problema = problema;
            FechaCreacion = DateTime.Now;
            Repuestos = new List<Repuesto>();
            Movimientos = new List<Movimiento>();
            _estadoActual = new EstadoRecepcionado();   // estado inicial
            _estrategiaCosto = estrategia;
        }
 
        // ── Observer: gestión de suscriptores ───────────────────────────────
        public void Suscribir(IOrdenObserver observador)
        {
            _observadores.Add(observador);
        }
 
        public void Desuscribir(IOrdenObserver observador)
        {
            _observadores.Remove(observador);
        }
 
        private void Notificar(EstadoOrden anterior, EstadoOrden nuevo)
        {
            foreach (var obs in _observadores)
                obs.Actualizar(this, anterior, nuevo);
        }
 
        // ── State: transición de estado (llamado por los estados concretos) ──
        // Modificador interno usado por IEstadoOrden
        public void SetEstado(IEstadoOrden nuevoEstado, string observacion = "")
        {
            EstadoOrden anterior = _estadoActual.Estado;
            _estadoActual = nuevoEstado;
 
            // Registrar en historial
            Movimientos.Add(new Movimiento(anterior, nuevoEstado.Estado, observacion));
 
            // Si se entrega, registrar fecha
            if (nuevoEstado.Estado == EstadoOrden.Entregado)
                FechaEntrega = DateTime.Now;
 
            // Notificar a todos los observadores
            Notificar(anterior, nuevoEstado.Estado);
        }
 
        public void AvanzarEstado()
        {
            _estadoActual.Avanzar(this);
        }
 
        public void RetrocederEstado()
        {
            _estadoActual.Retroceder(this);
        }
 
        public string DescripcionEstado() => _estadoActual.Descripcion();
 
        // ── Strategy: cambiar estrategia de costo en tiempo de ejecución ────
        public void SetEstrategiaCosto(ICostoStrategy estrategia)
        {
            _estrategiaCosto = estrategia;
        }
 
        // ── Lógica de negocio ────────────────────────────────────────────────
        public void AgregarRepuesto(Repuesto repuesto)
        {
            Repuestos.Add(repuesto);
        }
 
        public decimal CalcularCosto()
        {
            return _estrategiaCosto.Calcular(this);
        }
 
        public string DescripcionCosto() => _estrategiaCosto.Descripcion();
 
        public override string ToString()
        {
            return $"Orden #{Id} | Cliente: {Cliente} | Equipo: {Equipo} | Estado: {Estado}";
        }
    }
