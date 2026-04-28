using System;

namespace ProyectoTecnoFix.Infraestructura.ManejadorArchivos;

public class ArchivoService<T>
{
    private IArchivoStrategy<T> _strategy;

    public void SetStrategy(IArchivoStrategy<T> strategy)
    {
        _strategy = strategy;
    }

    public void Exportar(List<T> datos, string ruta)
    {
        _strategy.Exportar(datos, ruta);
    }

    public List<T> Importar(string ruta)
    {
        return _strategy.Importar(ruta);
    }
}