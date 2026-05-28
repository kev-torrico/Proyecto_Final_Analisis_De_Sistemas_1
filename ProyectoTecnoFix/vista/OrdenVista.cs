using System;
using System.IO;
using ProyectoTecnoFix.controlador;
using ProyectoTecnoFix.enums;
using ProyectoTecnoFix.modelo;

namespace ProyectoTecnoFix.vista;

public class OrdenVista
    {
        private readonly OrdenControlador _controlador;
 
        public OrdenVista(OrdenControlador controlador)
        {
            _controlador = controlador;
        }
 
        // ── Menú principal ────────────────────────────────────────────────────
        public void MostrarMenu()
        {
            bool salir = false;
            while (!salir)
            {
                try
                {
                    Console.Clear();
                }
                catch (IOException)
                {
                }
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("╔══════════════════════════════════════╗");
                Console.WriteLine("║         TECNOFIX  -  ÓRDENES         ║");
                Console.WriteLine("╚══════════════════════════════════════╝");
                Console.ResetColor();
                Console.WriteLine("  [1]  Nueva orden");
                Console.WriteLine("  [2]  Avanzar estado de orden");
                Console.WriteLine("  [3]  Asignar técnico");
                Console.WriteLine("  [4]  Agregar repuesto");
                Console.WriteLine("  [5]  Cambiar estrategia de costo");
                Console.WriteLine("  [6]  Ver todas las órdenes");
                Console.WriteLine("  [7]  Buscar por estado");
                Console.WriteLine("  [8]  Buscar por cliente");
                Console.WriteLine("  [9]  Ver detalle de una orden");
                Console.WriteLine("  [10] Ver técnicos y carga");
                Console.WriteLine("  [11] Registrar técnico");
                Console.WriteLine("  [12] Ver ingresos totales");
                Console.WriteLine("  [13] Exportar órdenes");
                Console.WriteLine("  [14] Importar órdenes");
                Console.WriteLine("  [0]  Salir");
                Console.Write("\nOpción: ");
 
                string? opcion = Console.ReadLine();
                Console.WriteLine();
 
                switch (opcion)
                {
                    case "1":  _controlador.CrearOrden(); break;
                    case "2":  _controlador.AvanzarEstado(); break;
                    case "3":  _controlador.AsignarTecnico(); break;
                    case "4":  _controlador.AgregarRepuesto(); break;
                    case "5":  _controlador.CambiarEstrategiaCosto(); break;
                    case "6":  MostrarTodasLasOrdenes(); break;
                    case "7":  MostrarPorEstado(); break;
                    case "8":  MostrarPorCliente(); break;
                    case "9":  MostrarDetalleOrden(); break;
                    case "10": MostrarTecnicos(); break;
                    case "11": _controlador.RegistrarTecnico(); break;
                    case "12": MostrarIngresos(); break;
                    case "13":
                        Console.Write("Formato (json/csv/txt): ");
                        var formatoExp = Console.ReadLine();
                        _controlador.ExportarOrdenes(formatoExp);
                        break;

                    case "14":
                        Console.Write("Formato (json/csv/txt): ");
                        var formatoImp = Console.ReadLine();
                        _controlador.ImportarOrdenes(formatoImp);
                        break;
                    case "0":
                        // _controlador.ExportarOrdenes("json");
                        salir = true;
                        break;
                    default:   Console.WriteLine("  ✗ Opción inválida."); break;
                }
 
                if (!salir)
                {
                    Console.Write("\nPresione Enter para continuar...");
                    Console.ReadLine();
                }
            }
        }
 
        // ── Listados ──────────────────────────────────────────────────────────
        public void MostrarTodasLasOrdenes()
        {
            var ordenes = _controlador.ObtenerOrdenes();
            Console.WriteLine("── TODAS LAS ÓRDENES ───────────────────────");
 
            if (ordenes.Count == 0)
            {
                Console.WriteLine("  No hay órdenes registradas."); return;
            }
 
            foreach (var o in ordenes)
                ImprimirOrdenResumen(o);
        }
 
        public void MostrarPorEstado()
        {
            Console.WriteLine("Estados: Recepcionado | EnDiagnostico | EnReparacion | Finalizado | Entregado");
            Console.Write("Estado a buscar: ");
            string? input = Console.ReadLine();
 
            if (!Enum.TryParse<EstadoOrden>(input, true, out EstadoOrden estado))
            {
                Console.WriteLine("  ✗ Estado no reconocido."); return;
            }
 
            var resultado = _controlador.ObtenerPorEstado(estado);
            Console.WriteLine($"\n── ÓRDENES EN ESTADO: {estado} ({resultado.Count}) ────");
 
            foreach (var o in resultado)
                ImprimirOrdenResumen(o);
        }
 
        public void MostrarPorCliente()
        {
            Console.Write("Nombre del cliente: ");
            string? nombre = Console.ReadLine();
 
            var resultado = _controlador.ObtenerPorCliente(nombre);
            Console.WriteLine($"\n── ÓRDENES DE '{nombre.ToUpper()}' ({resultado.Count}) ────");
 
            foreach (var o in resultado)
                ImprimirOrdenResumen(o);
        }
 
        public void MostrarDetalleOrden()
        {
            var ordenes = _controlador.ObtenerOrdenes();
            if (ordenes.Count == 0) { Console.WriteLine("  No hay órdenes."); return; }
 
            Console.Write("ID de la orden: ");
            if (!int.TryParse(Console.ReadLine(), out int id)) return;
 
            var orden = ordenes.FirstOrDefault(o => o.Id == id);
            if (orden == null) { Console.WriteLine("  ✗ Orden no encontrada."); return; }
 
            Console.WriteLine($"\n── DETALLE ORDEN #{orden.Id} ─────────────────────");
            Console.WriteLine($"  Cliente   : {orden.Cliente}");
            Console.WriteLine($"  Equipo    : {orden.Equipo}");
            Console.WriteLine($"  Problema  : {orden.Problema}");
            Console.WriteLine($"  Estado    : {orden.Estado}");
            Console.WriteLine($"  Situación : {orden.DescripcionEstado()}");
            Console.WriteLine($"  Técnico   : {orden.TecnicoAsignado?.Nombre ?? "Sin asignar"}");
            Console.WriteLine($"  Ingreso   : {orden.FechaCreacion:dd/MM/yyyy HH:mm}");
            Console.WriteLine($"  Entrega   : {(orden.FechaEntrega.HasValue ? orden.FechaEntrega.Value.ToString("dd/MM/yyyy HH:mm") : "Pendiente")}");
 
            Console.WriteLine($"\n  Repuestos ({orden.Repuestos.Count}):");
            if (orden.Repuestos.Count == 0)
                Console.WriteLine("    Ninguno");
            else
                foreach (var r in orden.Repuestos)
                    Console.WriteLine($"    • {r}");
 
            Console.WriteLine($"\n  {orden.DescripcionCosto()}");
            Console.WriteLine($"  COSTO TOTAL: Bs. {orden.CalcularCosto():F2}");
 
            Console.WriteLine($"\n  Historial de movimientos ({orden.Movimientos.Count}):");
            if (orden.Movimientos.Count == 0)
                Console.WriteLine("    Sin movimientos aún.");
            else
                foreach (var m in orden.Movimientos)
                    Console.WriteLine($"    {m}");
        }
 
        public void MostrarTecnicos()
        {
            var tecnicos = _controlador.ObtenerTecnicos();
            Console.WriteLine("── TÉCNICOS ─────────────────────────────────");
 
            if (tecnicos.Count == 0)
            {
                Console.WriteLine("  No hay técnicos registrados."); return;
            }
 
            foreach (var t in tecnicos)
                Console.WriteLine($"  {t}");
        }
 
        public void MostrarIngresos()
        {
            decimal total = _controlador.ObtenerIngresosTotales();
            Console.WriteLine($"── INGRESOS TOTALES (órdenes entregadas) ────");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  Bs. {total:F2}");
            Console.ResetColor();
        }
 
        // ── Helper ─────────────────────────────────────────────────────────────
        private void ImprimirOrdenResumen(Orden o)
        {
            Console.WriteLine($"  #{o.Id,-4} | {o.Cliente,-20} | {o.Equipo,-12} | {o.Estado,-14} | Bs. {o.CalcularCosto():F2}");
        }
    }