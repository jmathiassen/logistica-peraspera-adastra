// using System.Runtime.InteropServices;
// using Arch.Core;
// using Arch.System;
// using LogisticaPerAsperaAdAstra.Core.Components;
//
// namespace LogisticaPerAsperaAdAstra.Core.Systems;
//
// public partial class IntegritySystem : BaseSystem<World, float>
// {
//     private readonly Dictionary<Entity, HashSet<Entity>> _vehicleReportedOccupancy = new();
//
//     public IntegritySystem(World world) : base(world) { }
//
//     public override void Update(in float t)
//     {
//         _vehicleReportedOccupancy.Clear();
//
//         // 1. First Pass: Build a map of where vehicles THINK they are.
//         BuildVehicleReportedOccupancyQuery(World);
//
//         // 2. Second Pass: Compare against where segments THINK vehicles are.
//         CheckSegmentOccupancyQuery(World);
//     }
//
//     [Query]
//     private void BuildVehicleReportedOccupancy([In] ref Occupying occupying, in Entity entity)
//     {
//         foreach (Entity segment in occupying.Segments)
//         {
//             if (!_vehicleReportedOccupancy.TryGetValue(segment, out HashSet<Entity>? occupants))
//             {
//                 occupants = new HashSet<Entity>();
//                 _vehicleReportedOccupancy[segment] = occupants;
//             }
//             occupants.Add(entity);
//         }
//     }
//
//     [Query]
//     private void CheckSegmentOccupancy([In] ref Occupants occupants, in Entity entity)
//     {
//         HashSet<Entity>? reportedOccupants = _vehicleReportedOccupancy.GetValueOrDefault(entity);
//         HashSet<Entity> actualOccupants = new(occupants.Entities);
//
//         // If the vehicle-reported set is null (meaning no vehicles claim to be here) but the segment says it has occupants...
//         if (reportedOccupants == null && actualOccupants.Count > 0)
//         {
//             Console.WriteLine($"[INTEGRITY ERROR] Desync on Segment {entity}: Segment reports occupants, but no vehicles claim to be here.");
//             return;
//         }
//
//         // If vehicles claim to be here, but the segment doesn't...
//         if (reportedOccupants != null && actualOccupants.Count == 0)
//         {
//              Console.WriteLine($"[INTEGRITY ERROR] Desync on Segment {entity}: Vehicles claim to be here, but segment reports no occupants.");
//             return;
//         }
//
//         // If both report occupants, but the sets don't match...
//         if (reportedOccupants != null && !reportedOccupants.SetEquals(actualOccupants))
//         {
//             Console.WriteLine($"[INTEGRITY ERROR] Desync on Segment {entity}: Occupant mismatch.");
//         }
//     }
// }