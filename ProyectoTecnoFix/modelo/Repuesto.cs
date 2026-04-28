using System;

namespace ProyectoTecnoFix.modelo;

public class Repuesto
    {
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
 
        public Repuesto(string nombre, decimal precio, int cantidad)
        {
            Nombre = nombre;
            Precio = precio;
            Cantidad = cantidad;
        }
 
        public decimal GetSubtotal()
        {
            return Precio * Cantidad;
        }
 
        public override string ToString()
        {
            return $"{Nombre} x{Cantidad} = Bs. {GetSubtotal():F2}";
        }
    }
