using Arch.Core;
using LogisticaPerAsperaAdAstra.Core;
using LogisticaPerAsperaAdAstra.Core.Systems;

public class SimulationRunner
{
    private readonly World _world;
    private readonly TimeSystem _timeSystem = new();
    private readonly ConstructionSystem _constructionSystem = new();

    public SimulationRunner()
    {
        var manifest = new SimulationManifest();
        var instance = new SimulationInstance(manifest);
        _world = instance.EcsWorld;

        // Create the global time resource and set its starting value
        _world.Set(new GameTime {
            CurrentTime = GalacticDateTime.From(1, 1, 1, 8, 0)
        });
    }

    public void Run()
    {
        bool isRunning = true;
        while (isRunning)
        {
            // --- UPDATE PHASE ---
            // 1. Run the TimeSystem first to advance the clock.
            _timeSystem.Update(_world);

            // 2. Run game logic systems.
            _constructionSystem.Update(_world);

            // Control the simulation speed
            Thread.Sleep(50); // Sleep for 50ms for ~20 ticks per second
        }
    }
}