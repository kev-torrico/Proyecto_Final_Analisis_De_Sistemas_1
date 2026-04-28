using System;
using ProyectoTecnoFix.enums;
using ProyectoTecnoFix.modelo;

namespace ProyectoTecnoFix.Patrones.State;

    // Cada estado concreto sabe a qué estados puede transicionar
    public interface IEstadoOrden
    {
        EstadoOrden Estado { get; }
        void Avanzar(Orden orden);
        void Retroceder(Orden orden);
        string Descripcion();
    }
