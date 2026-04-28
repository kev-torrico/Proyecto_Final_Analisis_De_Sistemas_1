using System;
using ProyectoTecnoFix.enums;
using ProyectoTecnoFix.modelo;

namespace ProyectoTecnoFix.Patrones.Observer;

// Observer concreto: imprime en consola cuando cambia el estado de una orden
    public class NotificadorConsola : IOrdenObserver
    {
        public void Actualizar(Orden orden, EstadoOrden estadoAnterior, EstadoOrden estadoNuevo)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\n[NOTIFICACIÓN] Orden #{orden.Id}");
            Console.WriteLine($"  Cliente  : {orden.Cliente}");
            Console.WriteLine($"  Estado   : {estadoAnterior} → {estadoNuevo}");
            Console.WriteLine($"  Fecha    : {DateTime.Now:dd/MM/yyyy HH:mm}");
            Console.ResetColor();
        }
    }