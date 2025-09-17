using System.Diagnostics;
using Arch.Core;
using LogisticaPerAsperaAdAstra.Core;
using LogisticaPerAsperaAdAstra.Core.Systems;

// --- SETUP ---
RealitySetup realitySetup = new();
World world = World.Create();

// WorldGenerationSystem worldGenSystem = new();
// PopulationSystem populationSystem = new();
// CityDemandSystem cityDemandSystem = new();
// ... create other systems like FactorySystem, LogisticsSystem etc.

// --- INITIALIZATION ---
// worldGenSystem.Generate(world);
GalacticDateTime currentTime = new(0);

// --- MAIN LOOP ---
while (true)
{
    Stopwatch sw = Stopwatch.StartNew();
    Console.WriteLine($"--- Tick {currentTime} - {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} ---");

    // These systems run every tick
    // cityDemandSystem.Update(world);

    // This system might only run once per simulated year
    if (currentTime is { Day: 1, Hour: 0, Minute: 0 })
    {
        // populationSystem.Update(world);
        Console.WriteLine("A year has passed. Population updated.");
    }

    int timeToWait = 1000 - (int)sw.ElapsedMilliseconds;
    if (timeToWait > 0)
        Thread.Sleep(timeToWait);

    currentTime++;
}