# ANALISIS DE SISTEMAS 1

# TECNOFIX — Sistema de Gestión de Servicios Técnicos

Sistema de gestión de órdenes de servicio para la empresa TECNOFIX, dedicada a la reparación y mantenimiento de equipos tecnológicos (computadoras, impresoras y dispositivos móviles).

---

## Estructura del proyecto

```
TECNOFIX/
│
├── Model/                        # Entidades y lógica de negocio
│   ├── Orden.cs                  # Entidad central del sistema
│   ├── OrdenModelo.cs            # Repositorio en memoria + consultas LINQ
│   ├── Tecnico.cs                # Entidad técnico con historial y carga
│   ├── Repuesto.cs               # Pieza o insumo utilizado en la reparación
│   ├── Movimiento.cs             # Registro histórico de cada cambio de estado
│   └── EstadoOrden.cs            # Enum con los 5 estados posibles
│
├── View/                         # Presentación en consola
│   └── OrdenVista.cs             # Menú interactivo y visualización de datos
│
├── Controller/                   # Coordinación entre Vista y Modelo
│   └── OrdenControlador.cs       # Lógica de flujo y validaciones de entrada
│
└── Patterns/                     # Implementaciones de patrones de diseño
    ├── Observer/
    │   ├── IOrdenObserver.cs     # Contrato del observador
    │   └── NotificadorConsola.cs # Observer concreto: imprime notificaciones
    ├── State/
    │   ├── IEstadoOrden.cs       # Contrato del estado
    │   └── EstadosConcretos.cs   # Los 5 estados: Recepcionado → Entregado
    ├── Strategy/
    │   └── ICostoStrategy.cs     # Interfaz + CostoBasico + CostoConDescuento
    └── Factory/
        └── OrdenFactory.cs       # Crea la Orden con la estrategia correcta
```

---

## Arquitectura MVC

El proyecto sigue el patrón **Modelo-Vista-Controlador** con separación estricta de responsabilidades. Ninguna capa se comunica directamente con otra que no le corresponde.

| Capa           | Responsabilidad                                                                                    |
| -------------- | -------------------------------------------------------------------------------------------------- |
| **Model**      | Datos, reglas de negocio y acceso al repositorio. No sabe nada de consola ni de UI.                |
| **View**       | Muestra información al usuario y captura su elección de menú. No toca el Modelo directamente.      |
| **Controller** | Recibe las acciones de la Vista, valida la entrada y coordina las operaciones sobre el Modelo.     |
| **Patterns**   | Implementaciones de los patrones de diseño, reutilizables e independientes de la capa que los use. |

El flujo siempre sigue esta dirección:

```
Vista  →  Controlador  →  Modelo
Vista  ←  Controlador  ←  Modelo
```

---

## Patrones de diseño

### Factory Method — `Patrones/Factory/OrdenFactory.cs`

**Propósito:** centralizar la creación de objetos `Orden` según el tipo de equipo.

Sin este patrón, el código que decide qué estrategia de costo usar estaría duplicado en el Controlador, la Vista y cualquier otro lugar donde se cree una orden. La Factory encapsula esa decisión en un único punto.

```csharp
// Un solo switch decide la estrategia; el resto del sistema no lo sabe
ICostoStrategy estrategia = tipo.ToLower() switch
{
    "computadora" => new CostoBasico(tarifaManoObra: 80m),
    "impresora"   => new CostoBasico(tarifaManoObra: 50m),
    "movil"       => new CostoConDescuento(tarifaManoObra: 60m, porcentajeDescuento: 10m),
    _             => throw new ArgumentException($"Tipo no soportado: {tipo}")
};
return new Orden(id, cliente, tipo, problema, estrategia);
```

Para agregar un nuevo tipo de equipo (por ejemplo, tablets), solo se añade una línea aquí. El Controlador y la Vista no cambian.

---

### Strategy — `Patrones/Strategy/ICostoStrategy.cs`

**Propósito:** permitir que el algoritmo de cálculo de costo sea intercambiable en tiempo de ejecución sin modificar la clase `Orden`.

La clase `Orden` no sabe qué fórmula se usa; simplemente llama a `_estrategiaCosto.Calcular(this)`. La estrategia concreta es la que contiene la lógica.

| Estrategia          | Descripción                                                    | Usada para               |
| ------------------- | -------------------------------------------------------------- | ------------------------ |
| `CostoBasico`       | Tarifa de mano de obra + suma de repuestos                     | Computadoras, impresoras |
| `CostoConDescuento` | Igual que básico, pero aplica un % de descuento sobre el total | Dispositivos móviles     |

```csharp
// Orden delega; no contiene la fórmula
public decimal CalcularCosto() => _estrategiaCosto.Calcular(this);

// La estrategia puede cambiarse en cualquier momento
public void SetEstrategiaCosto(ICostoStrategy nueva) => _estrategiaCosto = nueva;
```

---

### Observer — `Patrones/Observer/`

**Propósito:** notificar automáticamente a otros componentes cuando el estado de una `Orden` cambia, sin que `Orden` conozca a sus suscriptores.

`Orden` es el **sujeto**: mantiene una lista de observadores y los notifica al cambiar de estado. Cualquier clase que implemente `IOrdenObserver` puede suscribirse y reaccionar al cambio. Actualmente existe `NotificadorConsola`; para agregar notificaciones por email o SMS basta con crear una nueva clase y suscribirla, sin modificar `Orden`.

```csharp
// Registrar un observador
orden.Suscribir(new NotificadorConsola());

// Orden notifica internamente en SetEstado()
private void Notificar(EstadoOrden ant, EstadoOrden nvo)
{
    foreach (var obs in _observadores)
        obs.Actualizar(this, ant, nvo);
}
```

---

### State — `Patrones/State/`

**Propósito:** eliminar los bloques `if/switch` sobre el estado de la orden. Cada estado es un objeto que sabe a qué estados puede transicionar y qué operaciones están prohibidas.

El flujo de estados válidos es:

```
Recepcionado  →  EnDiagnostico  →  EnReparacion  →  Finalizado  →  Entregado
```

Cada estado concreto implementa `Avanzar()` y `Retroceder()`. Si un estado no permite una transición (por ejemplo, retroceder desde `Entregado`), el método simplemente imprime un mensaje y no hace nada, sin necesidad de validaciones externas.

```csharp
// Orden delega la transición al estado actual
public void AvanzarEstado() => _estadoActual.Avanzar(this);

// Cada estado concreto decide a dónde va
public class EstadoEnReparacion : IEstadoOrden
{
    public void Avanzar(Orden o)
        => o.SetEstado(new EstadoFinalizado(), "Reparación completada");

    public void Retroceder(Orden o)
        => o.SetEstado(new EstadoEnDiagnostico(), "Requiere re-diagnóstico");
}
```

**Punto de coordinación:** el método `SetEstado()` en `Orden` es donde State y Observer se integran. Al cambiar de estado, se guarda el `Movimiento` en el historial y se disparan las notificaciones del Observer, todo en un solo lugar.

---

## Entidades principales

### `Orden`

Entidad central del sistema. Actúa como sujeto Observer y contexto State. Contiene la referencia a la estrategia de costo activa y delega en ella el cálculo.

### `Tecnico`

Registra su especialidad y lleva su propia lista de órdenes asignadas. El método `ObtenerCarga()` devuelve cuántas órdenes activas tiene usando LINQ.

### `Repuesto`

Pieza o insumo utilizado durante la reparación. Calcula su propio subtotal (`Precio × Cantidad`).

### `Movimiento`

Registro inmutable de cada transición de estado: guarda el estado anterior, el nuevo, la fecha exacta y una observación opcional. Forma el historial completo de la orden.

---

## Consultas LINQ en `OrdenModelo`

El modelo expone consultas que utilizan LINQ y expresiones lambda para filtrar y agregar datos:

```csharp
// Órdenes por estado
_ordenes.Where(o => o.Estado == estado).ToList();

// Búsqueda por nombre de cliente (sin distinción de mayúsculas)
_ordenes.Where(o => o.Cliente.Contains(cliente, StringComparison.OrdinalIgnoreCase)).ToList();

// Técnicos ordenados por carga de trabajo descendente
_tecnicos.OrderByDescending(t => t.ObtenerCarga()).ToList();

// Total de ingresos de órdenes ya entregadas
_ordenes.Where(o => o.Estado == EstadoOrden.Entregado).Sum(o => o.CalcularCosto());
```

---

## Roles del sistema

| Rol               | Acciones principales                                                                       |
| ----------------- | ------------------------------------------------------------------------------------------ |
| **Recepcionista** | Crear órdenes, registrar el equipo, asignar técnico inicial, entregar el equipo al cliente |
| **Técnico**       | Avanzar el estado de la orden, agregar repuestos, registrar observaciones                  |
| **Administrador** | Acceso completo: gestión de técnicos, reportes, cambio de estrategia de costo              |

---

## Cómo ejecutar la aplicación consola

### Dentro la carpeta ProyectoTecnoFix

```bash
# Requiere .NET 6 o superior
dotnet run
```

Al iniciar, el sistema presenta el menú principal de órdenes desde `OrdenVista.MostrarMenu()`.

---

## Cómo ejecutar los tests

### Dentro la carpeta OrdenesTests

```bash
# Requiere .NET 6 o superior
dotnet test
```

Se visualizan los tests que pasaron y los que no.

---
