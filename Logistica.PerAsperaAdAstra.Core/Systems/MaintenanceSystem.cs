// using Arch.Core;
// using LogisticaPerAsperaAdAstra.Core.Components;
//
// namespace LogisticaPerAsperaAdAstra.Core.Systems;
//
// public class MaintenanceSystem
// {
//     private readonly QueryDescription _maintenanceQuery = new QueryDescription().WithAll<BuildingStock>();
//
//     public void Update(World world)
//     {
//         world.Query(in _maintenanceQuery, (ref BuildingStock stock) =>
//         {
//             // Logic to check building conditions and schedule maintenance
//             // For example, if a building's condition drops below a threshold,
//             // create an Order for repair materials.
//         });
//     }
// }