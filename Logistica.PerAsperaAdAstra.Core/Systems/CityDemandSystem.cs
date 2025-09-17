// using Arch.Core;
// using LogisticaPerAsperaAdAstra.Core.Components;
//
// namespace LogisticaPerAsperaAdAstra.Core.Systems;
//
// public class CityDemandSystem
// {
//     private readonly QueryDescription _cityQuery = new QueryDescription().WithAll<Entity, City, Population, BuildingStock>();
//     private readonly Random _random = new();
//
//     public void Update(World world)
//     {
//         world.Query(in _cityQuery, (Entity entity, ref City city, ref Population pop, ref BuildingStock stock) =>
//         {
//             // 1. Generate continuous, bulk demand (e.g., for food)
//             // (Logic would go here to create a large, ongoing order for food)
//
//             // 2. Generate stochastic, ad-hoc demand (e.g., for maintenance)
//             // This implements your "active project list" idea.
//             if (_random.NextDouble() < 0.1) // 10% chance each tick
//             {
//                 // The city decides it needs a new building or a repair.
//                 // It spawns an Order entity to get the materials.
//                 world.Create(new Order
//                 {
//                     ItemId = "comp_chassis_shunter", // Example item
//                     Quantity = 1,
//                     Requester = entity,
//                     Status = OrderStatus.Open
//                 });
//                 Console.WriteLine($"{city.Name} has placed an order for a new chassis!");
//             }
//         });
//     }
// }