using System;

namespace ProyectoTecnoFix.Infraestructura.ManejadorArchivos;

public interface IArchivoStrategy<T>
{
    void Exportar(List<T> datos, string ruta);
    List<T> Importar(string ruta);
}
