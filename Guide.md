# 260720
Se ha iniciado el proyecto. Creado la DB en un inicio como dice la guía de C# para BBDD.
En vez de usar el DataSources, se ha creado los dirs Data, Models, Views, ViewModels, Services y dentro Venta.cs, también creamos `SachielContext.cs` para que conecte con la DB `Sachiel` y creamos luego las tablas de Productos y DetalleVenta.

# 260721
Se han introducido datos a BBDD y se ha creado la ventana de Productos junto con `ProductoService.cs`.
Posteriormente, se han añadido funcionalidades para Crear, Eliminar y Actualizar (falta Buscar individual).

# 260722
Ya se pueden buscar individualmente los productos en `ProductoView`.
Se ha comenzado a diseñar la UI para `VentaView`.
Continua el diseño de `VentaView`, agregados botones y añadir productos para ventas.

# 260723
Finalizado el `VentaView` para agregar nuevas ventas.

# 260724
Renombrado `VentaView` a `AddVentaView` y `VentaView` ahora refiere a la ventana de gestión de ventas.
Ya diseñada la UI para `VentaView`.
Modificado levemente `Venta.cs` y `DetallesVenta.cs`, no es necesaria migración (creo).

# 260725
`VentaView` ahora muestra las ventas (pudiendo ser filtradas).
`VentaView` finalizada, con la posibilidad de buscar, filtrar y modificar la facturación.

# 260726
Creada `DashboardView` para el inicio (bastante bien eh).

# 260727
Añadida funcionalidad a botones y cards de `DashboardView`.

# 260728
Comenzado a indagar sobre la exportación a Excel/Pdf

# 260729
Diseñado por completo `ExportVentasView`, falta hacer que los services funcionen (y agregar opción a `VentaView`?)

# 260730
TO DO: Implementar `Filter.cs` a todo lo que use filtros (check mod GetVentasFilter).
Realizada la exportación a Excel (revisar formatos celda de período y monedas).

# 260802
Corregido el formato en Excel, agregado el detalle de productos y finalizada la exportación también a PDF.

# 260803
Comenzado a diseñar el sistema para la importación de precios y productos.

# 260804
Finalizado el sistema de importación, se ha comenzado a unificar los estilos (ver chat para sidebar)

# 260806
Sigue el diseño.

# 260807
Diseño casi terminado, falta modificar los MessageBox.

# 260809
MessageBox modificados.
Logos e íconos incorporados.