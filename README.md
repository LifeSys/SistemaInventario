# SistemaInventario

## Estructura en capas

```text
SistemaInventario/
├── Datos/
│   ├── IProductoRepository.cs
│   └── ProductoDAO.cs
├── Entidades/
│   └── Producto.cs
├── Negocio/
│   ├── ProductoService.cs
│   ├── ProductoValidator.cs
│   └── ValidationResult.cs
├── Form1.cs
├── Form1.Designer.cs
└── Program.cs
```

## Capas

- **Presentación (WinForms)**: `Form1` solo maneja eventos/UI, estados de pantalla y llamadas a la capa de negocio.
- **Negocio**: `ProductoService` centraliza reglas, validaciones y mensajes de resultado.
- **Datos**: `ProductoDAO` encapsula todo el acceso a SQL Server con consultas parametrizadas y métodos asíncronos.
- **Entidades**: `Producto` modela el dominio y viaja entre capas.

## Inyección de dependencias básica

`Program.cs` compone las dependencias manualmente (`ProductoDAO` -> `ProductoService` -> `Form1`).
