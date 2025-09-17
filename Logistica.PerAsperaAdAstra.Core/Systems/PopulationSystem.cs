// using Arch.Core;
// using LogisticaPerAsperaAdAstra.Core.Components;
//
// namespace LogisticaPerAsperaAdAstra.Core.Systems;
//
// public class PopulationSystem
// {
//     // A query that finds all entities that have a Population component.
//     private readonly QueryDescription _populationQuery = new QueryDescription().WithAll<Population>();
//
//     public void Update(World world)
//     {
//         // world.Query() is a highly optimized way to iterate over entities.
//         world.Query(in _populationQuery, (ref Population pop) =>
//         {
//             // 1. Age the population (shift the array)
//             // This is a simplified loop. A real one would be more careful.
//             for (int i = pop.MaleByYear.Length - 1; i > 0; i--)
//             {
//                 pop.MaleByYear[i] = pop.MaleByYear[i - 1];
//                 pop.FemaleByYear[i] = pop.FemaleByYear[i - 1];
//             }
//             pop.MaleByYear[0] = 0; // Clear the newborn slot
//             pop.FemaleByYear[0] = 0;
//
//             // 2. Calculate deaths for each cohort
//             // This would use a detailed mortality rate table.
//             for (int i = 0; i < pop.MaleByYear.Length; i++)
//             {
//                 // Simplified mortality logic
//                 double mortalityRate = i * 0.001d; // Risk increases with age
//                 pop.MaleByYear[i] -= (int)(pop.MaleByYear[i] * mortalityRate);
//                 pop.FemaleByYear[i] -= (int)(pop.FemaleByYear[i] * mortalityRate);
//             }
//
//             // 3. Calculate new births
//             // This would be a complex calculation based on fertile cohorts.
//             int newBirths = 10; // Simplified
//             pop.MaleByYear[0] = newBirths / 2;
//             pop.FemaleByYear[0] = newBirths - (newBirths / 2);
//         });
//     }
// }