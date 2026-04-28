using System;
using ProyectoTecnoFix.modelo;

namespace ProyectoTecnoFix.Patrones.Strategy;

// ── INTERFAZ ─────────────────────────────────────────────────────────────
    public interface ICostoStrategy
    {
        decimal Calcular(Orden orden);
        string Descripcion();
    }
 
    // ── COSTO BÁSICO: mano de obra + repuestos sin descuento ─────────────────
    public class CostoBasico : ICostoStrategy
    {
        private readonly decimal _tarifaManoObra;
 
        public CostoBasico(decimal tarifaManoObra = 50m)
        {
            _tarifaManoObra = tarifaManoObra;
        }
 
        public decimal Calcular(Orden orden)
        {
            decimal totalRepuestos = orden.Repuestos.Sum(r => r.GetSubtotal());
            return _tarifaManoObra + totalRepuestos;
        }
 
        public string Descripcion() =>
            $"Costo básico: tarifa mano de obra Bs. {_tarifaManoObra} + repuestos";
    }
 
    // ── COSTO CON DESCUENTO: aplica % sobre el total ─────────────────────────
    public class CostoConDescuento : ICostoStrategy
    {
        private readonly decimal _tarifaManoObra;
        private readonly decimal _porcentajeDescuento;
 
        public CostoConDescuento(decimal tarifaManoObra = 50m, decimal porcentajeDescuento = 10m)
        {
            _tarifaManoObra = tarifaManoObra;
            _porcentajeDescuento = porcentajeDescuento;
        }
 
        public decimal Calcular(Orden orden)
        {
            decimal totalRepuestos = orden.Repuestos.Sum(r => r.GetSubtotal());
            decimal subtotal = _tarifaManoObra + totalRepuestos;
            decimal descuento = subtotal * (_porcentajeDescuento / 100m);
            return subtotal - descuento;
        }
 
        public string Descripcion() =>
            $"Costo con descuento del {_porcentajeDescuento}% sobre mano de obra + repuestos";
    }