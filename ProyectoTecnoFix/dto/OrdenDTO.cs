using System;
using ProyectoTecnoFix.enums;

namespace ProyectoTecnoFix.dto;

public class OrdenDTO
{
    public int Id { get; set; }
    public string Cliente { get; set; }
    public string Equipo { get; set; }
    public string Problema { get; set; }
    public EstadoOrden Estado { get; set; }
}