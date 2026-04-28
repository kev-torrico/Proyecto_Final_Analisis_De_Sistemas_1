using System;
using ProyectoTecnoFix.enums;
using ProyectoTecnoFix.modelo;

namespace ProyectoTecnoFix.Patrones.Observer;

    // Contrato para cualquier clase que quiera escuchar cambios en una Orden
    public interface IOrdenObserver
    {
        void Actualizar(Orden orden, EstadoOrden estadoAnterior, EstadoOrden estadoNuevo);
    }