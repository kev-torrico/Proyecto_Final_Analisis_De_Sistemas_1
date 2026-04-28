using System;
using ProyectoTecnoFix.enums;
using ProyectoTecnoFix.Infraestructura.ManejadorArchivos;
using ProyectoTecnoFix.Infraestructura.Strategias;
using ProyectoTecnoFix.modelo;
using ProyectoTecnoFix.Patrones.Factory;
using ProyectoTecnoFix.Patrones.Observer;
using ProyectoTecnoFix.Patrones.Strategy;

namespace ProyectoTecnoFix.controlador;

public class OrdenControlador
    {
        private readonly OrdenModelo _modelo;
        private readonly IOrdenObserver _notificador;
        private int _siguienteId = 1;
        private readonly ArchivoService<Orden> _fileServiceOrden;
private readonly ArchivoService<Tecnico> _fileServiceTecnico;
 
        public OrdenControlador(OrdenModelo modelo)
        {
            _modelo = modelo;
            _notificador = new NotificadorConsola();
            _fileServiceOrden = new ArchivoService<Orden>();
            _fileServiceTecnico = new ArchivoService<Tecnico>();
        }
 
        // ── Crear orden por consola (interactivo) ────────────────────────────
        public void CrearOrden()
        {
            Console.WriteLine("\n── NUEVA ORDEN ──────────────────────────────");
 
            Console.Write("Nombre del cliente    : ");
            string cliente = Console.ReadLine();
 
            Console.Write("Tipo de equipo (computadora/impresora/movil): ");
            string tipo = Console.ReadLine();
 
            Console.Write("Problema reportado    : ");
            string problema = Console.ReadLine();
 
            try
            {
                // Factory crea la orden con la estrategia de costo correcta
                Orden orden = OrdenFactory.Crear(tipo, _siguienteId++, cliente, problema);
                orden.Suscribir(_notificador);    // Observer
                _modelo.AgregarOrden(orden);
 
                Console.WriteLine($"\n  ✓ Orden #{orden.Id} creada correctamente.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"\n  ✗ Error: {ex.Message}");
            }
        }
 
        // ── Crear orden directamente (para pruebas/datos iniciales) ──────────
        public Orden CrearOrdenManualmente(string cliente, string tipoEquipo, string problema)
        {
            Orden orden = OrdenFactory.Crear(tipoEquipo, _siguienteId++, cliente, problema);
            orden.Suscribir(_notificador);
            _modelo.AgregarOrden(orden);
            return orden;
        }
 
        // ── Avanzar estado (patrón State) ────────────────────────────────────
        public void AvanzarEstado()
        {
            Orden orden = SeleccionarOrden();
            if (orden == null) return;
 
            Console.Write("Observación (Enter para omitir): ");
            string obs = Console.ReadLine();
 
            orden.AvanzarEstado();
            Console.WriteLine($"  ✓ Estado actualizado a: {orden.Estado}");
        }
 
        // ── Asignar técnico ───────────────────────────────────────────────────
        public void AsignarTecnico()
        {
            Orden orden = SeleccionarOrden();
            if (orden == null) return;
 
            MostrarTecnicos();
            Console.Write("ID del técnico a asignar: ");
            if (!int.TryParse(Console.ReadLine(), out int idTec))
            {
                Console.WriteLine("  ✗ ID inválido."); return;
            }
 
            Tecnico tecnico = _modelo.ObtenerTecnicoPorId(idTec);
            if (tecnico == null)
            {
                Console.WriteLine("  ✗ Técnico no encontrado."); return;
            }
 
            orden.TecnicoAsignado = tecnico;
            tecnico.AsignarOrden(orden);
            Console.WriteLine($"  ✓ Técnico {tecnico.Nombre} asignado a la orden #{orden.Id}");
        }
 
        // ── Agregar repuesto a una orden ──────────────────────────────────────
        public void AgregarRepuesto()
        {
            Orden orden = SeleccionarOrden();
            if (orden == null) return;
 
            Console.Write("Nombre del repuesto : ");
            string nombre = Console.ReadLine();
 
            Console.Write("Precio unitario (Bs.): ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal precio))
            {
                Console.WriteLine("  ✗ Precio inválido."); return;
            }
 
            Console.Write("Cantidad            : ");
            if (!int.TryParse(Console.ReadLine(), out int cantidad))
            {
                Console.WriteLine("  ✗ Cantidad inválida."); return;
            }
 
            orden.AgregarRepuesto(new Repuesto(nombre, precio, cantidad));
            Console.WriteLine($"  ✓ Repuesto '{nombre}' agregado.");
        }
 
        // ── Cambiar estrategia de costo ───────────────────────────────────────
        public void CambiarEstrategiaCosto()
        {
            Orden orden = SeleccionarOrden();
            if (orden == null) return;
 
            Console.WriteLine("  [1] Costo básico");
            Console.WriteLine("  [2] Costo con descuento");
            Console.Write("Opción: ");
            string op = Console.ReadLine();
 
            ICostoStrategy nueva = op switch
            {
                "1" => new CostoBasico(),
                "2" => new CostoConDescuento(),
                _   => null
            };
 
            if (nueva == null) { Console.WriteLine("  ✗ Opción inválida."); return; }
 
            orden.SetEstrategiaCosto(nueva);
            Console.WriteLine($"  ✓ Estrategia cambiada: {nueva.Descripcion()}");
        }
 
        // ── Consultas ─────────────────────────────────────────────────────────
        public List<Orden> ObtenerOrdenes() => _modelo.ObtenerOrdenes();
 
        public List<Orden> ObtenerPorEstado(EstadoOrden estado) =>
            _modelo.ObtenerPorEstado(estado);
 
        public List<Orden> ObtenerPorCliente(string cliente) =>
            _modelo.ObtenerPorCliente(cliente);
 
        public decimal ObtenerIngresosTotales() =>
            _modelo.CalcularIngresosTotales();
 
        // ── Técnicos ──────────────────────────────────────────────────────────
        public void RegistrarTecnico()
        {
            Console.Write("Nombre del técnico   : ");
            string nombre = Console.ReadLine();
            Console.Write("Especialidad         : ");
            string esp = Console.ReadLine();
 
            int id = _modelo.ObtenerTecnicos().Count + 1;
            _modelo.AgregarTecnico(new Tecnico(id, nombre, esp));
            Console.WriteLine($"  ✓ Técnico #{id} registrado.");
        }
 
        public List<Tecnico> ObtenerTecnicos() => _modelo.ObtenerTecnicos();
        //Metodos para importar o exportar archivos
        public void ExportarOrdenes(string formato)
        {
            switch (formato.ToLower())
            {
                case "json":
                    try
                    {
                        _fileServiceOrden.SetStrategy(new ArchivoJsonStrategy());

                        _fileServiceOrden.Exportar(
                            _modelo.ObtenerOrdenes(),
                            "ordenes2.json"
                        );

                        Console.WriteLine("✔ Exportación en JSON completada.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"✗ Error al exportar JSON: {ex.Message}");
                    }
                    break;

                case "csv":
                    try
                    {
                        _fileServiceOrden.SetStrategy(new ArchivoCsvStrategy());

                        _fileServiceOrden.Exportar(
                            _modelo.ObtenerOrdenes(),
                            "ordenes2.csv"
                        );

                        Console.WriteLine("✔ Exportación en CSV completada.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"✗ Error al exportar CSV: {ex.Message}");
                    }
                    break;
                case "txt":
                try
                    {
                        _fileServiceOrden.SetStrategy(new ArchivoCsvStrategy());

                        _fileServiceOrden.Exportar(
                            _modelo.ObtenerOrdenes(),
                            "ordenes2.txt"
                        );

                        Console.WriteLine("✔ Exportación en TXT completada.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"✗ Error al exportar TXT: {ex.Message}");
                    }
                    break;

                default:
                    Console.WriteLine("Formato no soportado.");
                    return;
            }

            Console.WriteLine("✔ Órdenes exportadas correctamente.");
        }
        public void ImportarOrdenes(string formato)
        {
            List<Orden> ordenes = new();

            switch (formato.ToLower())
            {
                case "json":
                    _fileServiceOrden.SetStrategy(new ArchivoJsonStrategy());
                    var rutaJson = Path.Combine(Directory.GetCurrentDirectory(), "Data", "ordenes.json");
                    ordenes = _fileServiceOrden.Importar(rutaJson);
                    break;

                case "csv":
                    _fileServiceOrden.SetStrategy(new ArchivoCsvStrategy());
                    var rutaCsv = Path.Combine(Directory.GetCurrentDirectory(), "Data", "ordenes.csv");
                    ordenes = _fileServiceOrden.Importar(rutaCsv);
                    break;

                case "txt":
                    _fileServiceOrden.SetStrategy(new ArchivoTxtStrategy());
                    var rutaTxt = Path.Combine(Directory.GetCurrentDirectory(), "Data", "ordenes.txt");
                    ordenes = _fileServiceOrden.Importar(rutaTxt);
                    break;

                default:
                    Console.WriteLine("Formato no soportado.");
                    return;
            }

            foreach (var o in ordenes)
                _modelo.AgregarOrden(o);

            Console.WriteLine($"✔ Se importaron {ordenes.Count} órdenes.");
        }
 
        // ── Helpers privados ──────────────────────────────────────────────────
        private Orden SeleccionarOrden()
        {
            var lista = _modelo.ObtenerOrdenes();
            if (lista.Count == 0)
            {
                Console.WriteLine("  ✗ No hay órdenes registradas.");
                return null;
            }
 
            Console.WriteLine("\nÓrdenes disponibles:");
            foreach (var o in lista)
                Console.WriteLine($"  [{o.Id}] {o.Cliente} | {o.Equipo} | {o.Estado}");
 
            Console.Write("Ingrese el ID de la orden: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
                return null;
 
            return _modelo.ObtenerOrdenPorId(id);
        }
 
        private void MostrarTecnicos()
        {
            Console.WriteLine("\nTécnicos disponibles:");
            foreach (var t in _modelo.ObtenerTecnicosPorCarga())
                Console.WriteLine($"  {t}");
        }
    }
