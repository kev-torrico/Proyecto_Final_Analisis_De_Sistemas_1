// See https://aka.ms/new-console-template for more information
using ProyectoTecnoFix.controlador;
using ProyectoTecnoFix.modelo;
using ProyectoTecnoFix.vista;

Console.WriteLine("Hello, World!");

OrdenModelo modelo = new OrdenModelo();

OrdenControlador controlador = new OrdenControlador(modelo);

// controlador.ImportarOrdenes("json");

OrdenVista vista = new OrdenVista(controlador);
vista.MostrarMenu();