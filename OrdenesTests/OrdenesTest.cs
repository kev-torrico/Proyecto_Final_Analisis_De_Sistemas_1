using NUnit.Framework;
using ProyectoTecnoFix.enums;
using ProyectoTecnoFix.modelo;
using ProyectoTecnoFix.Patrones.Strategy;

namespace OrdenesTests;

public class OrdenesTest
{
    [Test]
    public void CrearOrden_EstadoInicialDebeSerRecepcionado()
    {
        var estrategia = new CostoBasico();
        var orden = new Orden(1, "Juan", "Laptop", "No enciende", estrategia);

        Assert.That(orden.Estado, Is.EqualTo(EstadoOrden.Recepcionado));
    }

    [Test]
    public void AgregarRepuesto_DebeAgregarALaLista()
    {
        var estrategia = new CostoBasico();
        var orden = new Orden(1, "Juan", "Laptop", "No enciende", estrategia);

        var repuesto = new Repuesto("Disco SSD", 180, 1);

        orden.AgregarRepuesto(repuesto);

        Assert.That(orden.Repuestos.Count, Is.EqualTo(1));
        Assert.That(orden.Repuestos[0], Is.EqualTo(repuesto));
    }

    [Test]
    public void CalcularCosto_ConEstrategiaBasica_RetornaValorCorrecto()
    {
        var estrategia = new CostoBasico();
        var orden = new Orden(1, "Juan", "computadora", "No enciende", estrategia);

        orden.AgregarRepuesto(new Repuesto("RAM", 50, 1));
        orden.AgregarRepuesto(new Repuesto("SSD", 100, 1));

        var total = orden.CalcularCosto();

        Assert.That(total, Is.EqualTo(200)); 
    }

    [Test]
    public void AvanzarEstado_DebeRegistrarMovimiento()
    {
        var estrategia = new CostoBasico();
        var orden = new Orden(1, "Juan", "Laptop", "No enciende", estrategia);

        orden.AvanzarEstado();

        Assert.That(orden.Movimientos.Count, Is.EqualTo(1));
    }
}
