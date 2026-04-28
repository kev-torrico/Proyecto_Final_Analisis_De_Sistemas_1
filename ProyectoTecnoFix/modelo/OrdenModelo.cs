using System;
using ProyectoTecnoFix.enums;

namespace ProyectoTecnoFix.modelo;

public class OrdenModelo
    {
        private List<Orden> _ordenes = new List<Orden>();
        private List<Tecnico> _tecnicos = new List<Tecnico>();
 
        // ── Órdenes ──────────────────────────────────────────────────────────
        public void AgregarOrden(Orden orden)
        {
            _ordenes.Add(orden);
        }
 
        public List<Orden> ObtenerOrdenes()
        {
            return _ordenes;
        }
 
        public Orden ObtenerOrdenPorId(int id)
        {
            return _ordenes.FirstOrDefault(o => o.Id == id);
        }
 
        // ── Consultas con LINQ ───────────────────────────────────────────────
        public List<Orden> ObtenerPorEstado(EstadoOrden estado)
        {
            return _ordenes.Where(o => o.Estado == estado).ToList();
        }
 
        public List<Orden> ObtenerPorCliente(string cliente)
        {
            return _ordenes
                .Where(o => o.Cliente.Contains(cliente, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
 
        public List<Orden> ObtenerPorTecnico(int tecnicoId)
        {
            return _ordenes
                .Where(o => o.TecnicoAsignado?.Id == tecnicoId)
                .ToList();
        }
 
        public decimal CalcularIngresosTotales()
        {
            return _ordenes
                .Where(o => o.Estado == EstadoOrden.Entregado)
                .Sum(o => o.CalcularCosto());
        }
 
        // ── Técnicos ─────────────────────────────────────────────────────────
        public void AgregarTecnico(Tecnico tecnico)
        {
            _tecnicos.Add(tecnico);
        }
 
        public List<Tecnico> ObtenerTecnicos()
        {
            return _tecnicos;
        }
 
        public Tecnico ObtenerTecnicoPorId(int id)
        {
            return _tecnicos.FirstOrDefault(t => t.Id == id);
        }
 
        public List<Tecnico> ObtenerTecnicosPorCarga()
        {
            return _tecnicos.OrderByDescending(t => t.ObtenerCarga()).ToList();
        }
    }