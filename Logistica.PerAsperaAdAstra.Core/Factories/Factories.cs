// using Arch.Core;
// using LogisticaPerAsperaAdAstra.Core.Components;
//
// namespace LogisticaPerAsperaAdAstra.Core.Factories;
//
// public class VehicleFactory
// {
//     private readonly RealitySetup _realitySetup;
//
//     public VehicleFactory(RealitySetup realitySetup) { _realitySetup = realitySetup; }
//
//     public void CreateTrain(World world, string definitionId, Coordinate startPosition)
//     {
//         if (_realitySetup.GetItem(definitionId) is not ComponentDefinition definition) return;
//
//         // Create the entity in the world
//         Entity trainEntity = world.Create(
//             new Train(),
//             new Position { Value = startPosition }
//         );
//
//         // Add components based on the blueprint's specifications
//         if (definition.Propulsion != null)
//         {
//             world.Add<PropulsionComponent>(trainEntity, new()
//             {
//                 MaxPowerKw = definition.Propulsion.MaxPowerKw,
//                 // ... copy other specs, initialize dynamic state
//             });
//         }
//         // ... add other components like ChassisComponent, ContainerComponent, etc.
//     }
// }
//
