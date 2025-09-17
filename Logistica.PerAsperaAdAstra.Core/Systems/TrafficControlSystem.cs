// using Arch.Core;
// using Arch.System;
// using LogisticaPerAsperaAdAstra.Core.Components;
//
// namespace LogisticaPerAsperaAdAstra.Core.Systems;
//
// // Note: Using [Query] attributes is a modern Arch feature for source-generated performance.
// public partial class TrafficControlSystem : BaseSystem<SimulationInstance, float>
// {
//     public TrafficControlSystem(SimulationInstance simulationInstance) : base(simulationInstance) { }
//
//     // We only care about vehicles that have a route and are actively moving.
//     [Query]
//     private void FindAndCommandVehicles(in Entity entity, ref Route route, ref Occupying occupying, ref Velocity velocity, ref StoppingDistance stoppingDistance)
//     {
//         // Get the last segment the vehicle is on (for multi-segment vehicles like long trains)
//         Entity currentSegmentEntity = occupying.Segments[^1];
//         if (!world.TryGet(currentSegmentEntity, out ProgressAlongEdge progress)) return; // Should not happen
//
//         // TODO: A better way to get edge length. Store it in a component on the edge.
//         double edgeLength = 1000.0; // Assuming 1km for now
//
//         // Is the vehicle in the "decision window"? Let's define that as being within its stopping distance of the end.
//         if ((edgeLength - progress.Meters) > stoppingDistance.Meters)
//         {
//             // Path ahead is clear for at least one full stop. Remove any old commands.
//             SimulationInstance.TryRemove<TargetSpeed>(entity);
//             return;
//         }
//
//         // --- Lookahead Logic ---
//         double distanceScanned = 0.0;
//         for (int i = route.CurrentSegmentIndex + 1; i < route.Segments.Count; i++)
//         {
//             Entity nextSegmentEntity = route.Segments[i];
//             distanceScanned += 1000.0; // Assuming 1km segments
//
//             if (world.TryGet(nextSegmentEntity, out Occupants occupants) && occupants.Entities.Count > 0)
//             {
//                 // CONFLICT! The segment is occupied.
//                 Entity leadVehicle = occupants.Entities[0]; // Assuming one vehicle per segment for trains
//                 double leadVehicleSpeed = Arch.Core.World.Get<Velocity>(leadVehicle).MetersPerSecond;
//
//                 // Issue the command to match speed over the available distance.
//                 world.Add(entity, new TargetSpeed
//                 {
//                     MetersPerSecond = leadVehicleSpeed,
//                     DistanceToTarget = distanceScanned - 1000.0 // Distance to the *start* of the conflict segment
//                 });
//                 return; // Command issued, our job is done for this vehicle.
//             }
//
//             if (distanceScanned >= stoppingDistance.Meters) break; // Scanned far enough
//         }
//
//         // If we finished the loop without finding a conflict, the path is clear.
//         Arch.Core.World.TryRemove<TargetSpeed>(entity);
//     }
// }