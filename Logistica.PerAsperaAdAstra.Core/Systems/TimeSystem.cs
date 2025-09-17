using Arch.Core;
using Arch.Core.Extensions;
using LogisticaPerAsperaAdAstra.Core.Components;

namespace LogisticaPerAsperaAdAstra.Core.Systems;

public class TimeSystem
{
    private readonly QueryDescription _timeQuery = new QueryDescription().WithAll<GameTime>();

    public void Update(World world, double deltaTime)
    {
        ref GameTime gameTime = ref Arch.Core.Archetype..Get<GameTime>();
        world.Query(in _timeQuery, (ref GameTime time) =>
        {
            // Handle time-based events, e.g., daily updates
            if (time.CurrentTime.Hour == 8 && time.CurrentTime.Minute == 0)
            {
            }
            time.CurrentTime++;
        });
    }
}

public class ConstructionSystem
{
    private readonly QueryDescription _pendingConstructionQuery = new QueryDescription().WithAll<PendingConstruction>();
    private readonly QueryDescription _underConstructionQuery = new QueryDescription().WithAll<UnderConstruction>();

    public void Update(World world)
    {
        GameTime gameTime = Arch.Core.World.Get<GameTime>();
        world.Query(in _underConstructionQuery, (Entity entity, ref UnderConstruction project) =>
        {
            // if (project.ConstructingUntil)
            // foreach (string resourceId in project.RequiredItems.Keys)
            // {
            //     if (project.DeliveredItems.TryGetValue(resourceId, out int delivered))
            //     {
            //         int required = project.RequiredItems[resourceId];
            //         if (delivered < required)
            //         {
            //             // check if there are any resources for this in project.ConstructionEntity
            //         }
            //     }
            // }
        });
    }
}