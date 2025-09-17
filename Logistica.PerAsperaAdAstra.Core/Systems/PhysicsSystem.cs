// using Arch.Core;
// using Arch.System;
// using LogisticaPerAsperaAdAstra.Core.Components;
//
// namespace LogisticaPerAsperaAdAstra.Core.Systems;
//
// public partial class PhysicsSystem : BaseSystem<Arch.Core.World, float>
// {
//     private const double TrackSpeedLimitMps = 120.0 / 3.6; // 120 km/h
//
//     public PhysicsSystem(Arch.Core.World world) : base(world) { }
//
//     [Query]
//     private void UpdateVehiclePhysics(
//         in Entity entity,
//         ref Velocity velocity, ref Mass mass, ref ProgressAlongEdge progress,
//         ref CurrentThrottle throttle, ref CurrentBrake brake, ref StoppingDistance stoppingDistance)
//     {
//         // 1. Determine Goal Speed
//         double goalSpeed = World.TryGet(entity, out TargetSpeed target)
//             ? target.MetersPerSecond
//             : TrackSpeedLimitMps;
//
//         // 2. Calculate Speed Error & Control Inputs
//         double speedError = goalSpeed - velocity.MetersPerSecond;
//
//         if (speedError > 0) // Need to accelerate
//         {
//             throttle.Percent = 1.0;
//             brake.Percent = 0.0;
//         }
//         else // Need to decelerate or maintain speed
//         {
//             throttle.Percent = 0.0;
//             if (World.TryGet(entity, out TargetSpeed targetCmd) && velocity.MetersPerSecond > targetCmd.MetersPerSecond)
//             {
//                 // Proportional Braking Logic
//                 double dist = Math.Max(1.0, targetCmd.DistanceToTarget - progress.Meters);
//                 double requiredDecel = (Math.Pow(velocity.MetersPerSecond, 2) - Math.Pow(targetCmd.MetersPerSecond, 2)) / (2 * dist);
//
//                 double requiredForce = requiredDecel * mass.Value;
//                 double maxBrakeForce = GeneralPhysics.ComputeBrakeForce(mass.Value, 9.81, 0.8); // Assuming 0.8 grip
//
//                 brake.Percent = Math.Clamp(requiredForce / maxBrakeForce, 0.0, 1.0);
//             }
//             else
//             {
//                 brake.Percent = 0.0; // Just coasting
//             }
//         }
//
//         // 3. Apply Forces (simplified - would also include rolling resistance etc.)
//         double tractiveForce = World.Get<MaxPower>(entity).KiloWatt * 1000 * throttle.Percent / Math.Max(1.0, velocity.MetersPerSecond);
//         double brakingForce = GeneralPhysics.ComputeBrakeForce(mass.Value, 9.81, 0.8) * brake.Percent;
//         double netForce = tractiveForce - brakingForce;
//
//         // 4. Update State
//         double acceleration = netForce / mass.Value;
//         velocity.MetersPerSecond += acceleration * DeltaTime;
//         progress.Meters += velocity.MetersPerSecond * DeltaTime;
//
//         // 5. Handle Transitions (Simplified: assumes 1-segment vehicles and 1km segments)
//         if (progress.Meters >= 1000.0)
//         {
//             progress.Meters -= 1000.0;
//
//             ref Route route = ref World.Get<Route>(entity);
//             ref Occupying occupying = ref World.Get<Occupying>(entity);
//
//             // Get old and new segments
//             Entity oldSegment = route.Segments[route.CurrentSegmentIndex++];
//             Entity newSegment = route.Segments[route.CurrentSegmentIndex];
//
//             // Update vehicle's state
//             occupying.Segments[0] = newSegment;
//
//             // Update segments' state
//             World.Get<Occupants>(oldSegment).Entities.Remove(entity);
//             World.Get<Occupants>(newSegment).Entities.Add(entity);
//         }
//
//         // TODO: Update StoppingDistance based on new velocity and brake force.
//     }
// }