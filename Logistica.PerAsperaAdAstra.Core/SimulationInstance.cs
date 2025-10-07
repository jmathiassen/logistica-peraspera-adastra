using Arch.Core;
using LogisticaPerAsperaAdAstra.Core.Components;

namespace LogisticaPerAsperaAdAstra.Core;

public sealed class SimulationInstance(SimulationManifest simulationManifest)
{
    public World EcsWorld { get; } = World.Create();
    private GalacticDateTime CurrentTime = new();

    // Spatial grid for quick lookup of entities by their coordinates.
    private readonly Dictionary<(int X, int Y), List<Entity>> _spatialGrid = [];
    public List<Entity> GetEntitiesAt(Coordinate coordinate) => _spatialGrid.GetValueOrDefault((coordinate.X, coordinate.Y)) ?? [];
    public IEnumerable<Entity> GetEntitiesInRect(Coordinate cornerA, Coordinate cornerB)
    {
        int minX = Math.Min(cornerA.X, cornerB.X);
        int maxX = Math.Max(cornerA.X, cornerB.X);
        int minY = Math.Min(cornerA.Y, cornerB.Y);
        int maxY = Math.Max(cornerA.Y, cornerB.Y);

        for (int x = minX; x <= maxX; x++)
            for (int y = minY; y <= maxY; y++)
                foreach (Entity entity in _spatialGrid.GetValueOrDefault((x, y)) ?? [])
                    yield return entity;
    }
    private void RegisterEntityAt(Entity entity, Coordinate coordinate)
    {
        List<Entity> entities = _spatialGrid.GetValueOrDefault((coordinate.X, coordinate.Y)) ?? [];
        entities.Add(entity);
        _spatialGrid[(coordinate.X, coordinate.Y)] = entities;
    }
    private void DeregisterEntityAt(Entity entity, Coordinate coordinate)
    {
        _spatialGrid.Remove((coordinate.X, coordinate.Y));
    }

     // --- TEST NETWORK GENERATOR ---

    /// <summary>
    /// For testing purposes, this function generates a simplified but representative
    /// transport network of Norway's major cities and connections.
    /// </summary>
    public void GenerateNorwegianTestNetwork()
    {
        List<(string Name, double Lat, double Lon)> cityData =
        [
            ("Oslo", 59.9139, 10.7522),
            ("Bergen", 60.3913, 5.3221),
            ("Trondheim", 63.4305, 10.3951),
            ("Stavanger", 58.9690, 5.7331),
            ("Kristiansand", 58.1472, 7.9944),
            ("Bodø", 67.2800, 14.4050),

            // Key junctions and towns
            ("Drammen", 59.7439, 10.2045),
            ("Lillehammer", 61.1153, 10.4663),
            ("Dombås", 62.0747, 9.1294),
            ("Hamar", 60.7945, 11.0679),
            ("Fauske", 67.2588, 15.3926)
        ];

        List<(string From, string To)> railConnections =
        [
            ("Oslo", "Drammen"), ("Drammen", "Kristiansand"), ("Kristiansand", "Stavanger"), // Sørlandsbanen
            ("Oslo", "Hamar"), ("Hamar", "Lillehammer"), ("Lillehammer", "Dombås"), ("Dombås", "Trondheim"), // Dovrebanen
            ("Trondheim", "Fauske"), ("Fauske", "Bodø"), // Nordlandsbanen
            ("Oslo", "Bergen") // Bergensbanen (simplified)
        ];

        List<(string From, string To)> roadConnections =
        [
            ("Oslo", "Drammen"), ("Drammen", "Kristiansand"), ("Kristiansand", "Stavanger"), ("Stavanger", "Bergen"),
            ("Oslo", "Hamar"), ("Hamar", "Lillehammer"), ("Lillehammer", "Trondheim"),
            ("Trondheim", "Bodø")
        ];

        // --- Map Generation Logic ---
        Dictionary<string, (Entity city, Entity trainStation, Entity busStation)> nodeEntities = new();

        // Simple projection to map Lat/Lon to game coordinates
        Coordinate MapToCoordinate(double lat, double lon)
        {
            // These values scale and center Norway in a reasonable coordinate space
            const double scaleX = 20.0;
            const double scaleY = 40.0;
            const double offsetX = -5.0;
            const double offsetY = -58.0;

            int x = (int)((lon + offsetX) * scaleX);
            int y = (int)((lat + offsetY) * scaleY);
            return new Coordinate(x, y);
        }

        // 1. Create all city and station nodes
        foreach ((string name, double lat, double lon) in cityData)
        {
            Coordinate coordinate = MapToCoordinate(lat, lon);
            Entity cityEntity = PlanCity(coordinate, name);
            Entity trainStationEntity = PlanTrainStation(coordinate, $"{name} Central Station");
            Entity busStationEntity = PlanBusStation(coordinate, $"{name} Bus Terminal");
            nodeEntities[name] = (cityEntity, trainStationEntity, busStationEntity);
        }

        // 2. Create rail connections
        foreach (var (from, to) in railConnections)
        {
            Entity fromStation = nodeEntities[from].trainStation;
            Entity toStation = nodeEntities[to].trainStation;
            PlanRailTrackEdge(fromStation, toStation, EdgeDirection.Both, []);
        }

        // 3. Create road connections
        foreach ((string from, string to) in roadConnections)
        {
            Entity fromStation = nodeEntities[from].busStation;
            Entity toStation = nodeEntities[to].busStation;
            PlanRoadEdge(fromStation, toStation, []);
        }
    }


    // --- Lifecycle Management ---
    public void RevertToPlannedState(Entity entity)
    {
        EcsWorld.Remove<Surveying, SurveyComplete, UnderConstruction, HaltedConstruction>(entity);
        EcsWorld.Add(entity, new Planned());
    }
    public void BeginSurvey(Entity plannedEntity)
    {
        if (!EcsWorld.Has<Planned>(plannedEntity))
            throw new InvalidOperationException("Cannot begin survey on an entity that is not in the 'Planned' state.");
        if (EcsWorld.Has<Surveying>(plannedEntity))
            throw new InvalidOperationException("Cannot begin survey on an entity that already has the 'Surveying' state.");

        EcsWorld.Remove<Planned>(plannedEntity);
        EcsWorld.Add(plannedEntity, new Surveying());
    }
    public void ApproveSurvey(Entity surveyedEntity)
    {
        if (!EcsWorld.Has<SurveyComplete>(surveyedEntity))
            throw new InvalidOperationException("Cannot approve a plan that has not completed its survey.");

        EcsWorld.Remove<SurveyComplete>(surveyedEntity);
        EcsWorld.Add(surveyedEntity, new UnderConstruction());

        // TODO: Calculate the total required resources based on the generated segments (track, bridges, tunnels).
        // var requiredItems = CalculateRequirements(surveyedEntity);
        // EcsWorld.Add(surveyedEntity, new ConstructionRequirements { /* ... requiredItems ... */ });
        // TODO: The Logistics system would then generate Orders based on these requirements.
        // TODO: Calculate construction requirements using RealitySetup.
        // For example:
        // double trackLengthKm = ... calculate from segments ...;
        // ResourceDefinition steelBeams = _realitySetup.GetResource("res_steel_beam");
        // int requiredBeams = (int)(trackLengthKm * 10); // 10 beams per km
        // ConstructionRequirements requirements = new() { RequiredItems = [ (steelBeams.Id, requiredBeams) ] };
        // EcsWorld.Add(surveyedEntity, requirements);
    }
    public void StartConstruction(Entity entity)
    {
        if (!EcsWorld.Has<PendingConstruction>(entity))
            throw new InvalidOperationException("Cannot complete construction on an entity that is not under construction.");

        EcsWorld.Remove<PendingConstruction>(entity);
        EcsWorld.Add(entity, new UnderConstruction());
    }
    public void EndConstruction(Entity entity)
    {
        if (!EcsWorld.Has<UnderConstruction>(entity))
            throw new InvalidOperationException("Cannot complete construction on an entity that is not under construction.");

        EcsWorld.Remove<UnderConstruction>(entity);
        EcsWorld.Add(entity, new Condition { Value = 100 });
    }
    public void PrepareDemolition(Entity entity)
    {
        EcsWorld.Remove<UnderConstruction, HaltedConstruction>(entity);
        EcsWorld.Add(entity, new PendingDemolition());
    }
    public void StartDemolition(Entity entity)
    {
        if (!EcsWorld.Has<PendingDemolition>(entity))
            throw new InvalidOperationException("Cannot start demolition on an entity that has not been prepared for demolition.");

        EcsWorld.Remove<PendingDemolition>(entity);
        EcsWorld.Add(entity, new ActiveDemolition());
    }

    // --- Planning and creation ---
    public Entity PlanRailTrackEdge(Entity startNode, Entity endNode, EdgeDirection direction, List<Coordinate> waypoints) => PlanEdge(startNode, endNode, waypoints, direction, new RailTrack());
    public Entity PlanRoadEdge(Entity startNode, Entity endNode, List<Coordinate> waypoints) => PlanEdge(startNode, endNode, waypoints, null, new Road());

    private Entity PlanEdge(Entity fromNode, Entity toNode, List<Coordinate> waypoints, EdgeDirection? direction, params object[] components)
    {
        Connections connections = new() { Entities = [] };
        List<object> allComponents =
        [
            new IsEdge(),
            new Edge { NodeA = fromNode, NodeB = toNode },
            .. direction switch
            {
                EdgeDirection.AToB => [new DirectionAToB()],
                EdgeDirection.BToA => [new DirectionBToA()],
                EdgeDirection.Both => [new DirectionBoth()],
                null => Array.Empty<object>(),
                _ => throw new ArgumentException($"Invalid direction type: {direction}.")
            },
            connections,
            ..components
        ];
        Entity edgeEntity = PlanEntity(null, [..allComponents.ToArray()]);

        int index = 0;
        foreach (Coordinate waypoint in waypoints)
            connections.Entities.Add(PlanWaypoint(waypoint, index++, edgeEntity));

        return edgeEntity;
    }
    public void InsertWaypoint(Entity edgeEntity, Coordinate position, int index)
    {
        if (!EcsWorld.Has<Planned>(edgeEntity) && !EcsWorld.Has<SurveyComplete>(edgeEntity))
            throw new InvalidOperationException("Can only insert waypoints on an entity in a pre-construction state.");

        ref Connections connections = ref EcsWorld.Get<Connections>(edgeEntity);

        index = Math.Clamp(index, 0, connections.Entities.Count);
        connections.Entities.Insert(index, PlanWaypoint(position, index, edgeEntity));

        for (int i = index + 1; i < connections.Entities.Count; i++)
            EcsWorld.Get<IndexPosition>(connections.Entities[i]).Value = i;
    }
    public void ReorderWaypoints(Entity edgeEntity, List<Entity> orderedWaypoints)
    {
        if (!EcsWorld.Has<Planned>(edgeEntity) && !EcsWorld.Has<SurveyComplete>(edgeEntity))
            throw new InvalidOperationException("Can only reorder waypoints on an entity in a pre-construction state.");

        ref Connections connections = ref EcsWorld.Get<Connections>(edgeEntity);
        connections.Entities = orderedWaypoints;

        for (int i = 0; i < connections.Entities.Count; i++)
            EcsWorld.Get<IndexPosition>(connections.Entities[i]).Value = i;
    }

    public Entity PlanComponent(string componentId, Entity parentEntity)
    {
        ComponentDefinition componentDef = simulationManifest.Components[componentId];
        Entity entity = EcsWorld.Create(
            new Planned(),
            new Name { Value = componentDef.Name },
            new Parent { Value = parentEntity }
        );

        if (simulationManifest.GetSpecification<ElectricMotorSpecification>(componentId) is { } electricMotorSpecification)
        {
            EcsWorld.Add(entity,
                new Engine(),
                new MaxPower { KiloWatt = electricMotorSpecification.MaxPowerKw },
                new Consumes{ ResourceIds = ["util_electricity_kwh"] },
                new EngineEfficiency { Value = electricMotorSpecification.EfficiencyPercent, Type = EfficiencyType.Percentage }
            );
        }
        else if (simulationManifest.GetSpecification<CombustionEngineSpecification>(componentId) is { } combustionEngineSpecification)
        {
            EcsWorld.Add(entity,
                new Engine(),
                new MaxPower { KiloWatt = combustionEngineSpecification.MaxPowerKw },
                new Consumes{ ResourceIds = combustionEngineSpecification.FuelTypes.Select(fuelType => fuelType.ResourceId).ToList() },
                new EngineEfficiency { Value = combustionEngineSpecification.SpecificFuelConsumptionGramsPerKwh, Type = EfficiencyType.SpecificFuelConsumption }
            );
        }
        else if (simulationManifest.GetSpecification<RocketEngineSpecification>(componentId) is { } rocketEngineSpecification)
        {
            EcsWorld.Add(entity,
                new Engine(),
                new MaxPower { KiloWatt = rocketEngineSpecification.MaxPowerKw },
                new Consumes{ ResourceIds = rocketEngineSpecification.FuelTypes.Select(fuelType => fuelType.ResourceId).ToList() },
                new EngineEfficiency { Value = rocketEngineSpecification.SpecificImpulseSeconds, Type = EfficiencyType.SpecificImpulse }
            );
        }

        return entity;
    }
    public Entity PlanRefinery(Coordinate position, string name) => PlanIndustry(position, name, new Refinery());
    public Entity PlanMine(Coordinate position, string name) => PlanIndustry(position, name, new Mine());
    private Entity PlanIndustry(Coordinate position, string name, params object[] components) => PlanNode(position, new IsIndustry(), new Name { Value = name }, components);
    public Entity PlanCity(Coordinate position, string name) => PlanNode(position, new City(), new Name { Value = name });
    public Entity PlanTrainStation(Coordinate position, string name) => PlanNode(position, new TrainStation(), new Name { Value = name });
    public Entity PlanBusStation(Coordinate position, string name) => PlanNode(position, new BusStation(), new Name { Value = name });
    private Entity PlanWaypoint(Coordinate position, int index, Entity parentEdge, params object[] components) => PlanNode(position, new Waypoint(), new IndexPosition { Value = index }, new Parent { Value = parentEdge }, components);
    private Entity PlanNode(Coordinate position, params object[] components) => PlanEntity(position, new IsNode(), components);
    private Entity PlanEntity(Coordinate? position, params object[] components)
    {
        List<object> allComponents =
        [
            new Planned(),
            .. position is not null ? [new Position { Value = position.Value }] : Array.Empty<Position>(),
            .. components
        ];
        Entity entity = EcsWorld.Create([..allComponents.Select(c => c.GetType())]);
        EcsWorld.SetRange(entity, allComponents.ToArray());

        if (position != null)
            RegisterEntityAt(entity, position.Value);
        return entity;
    }

    private readonly QueryDescription _pendingConstructionQuery = new QueryDescription().WithAll<PendingConstruction>();
    private readonly QueryDescription _underConstructionQuery = new QueryDescription().WithAll<UnderConstruction>();
    public void Tick()
    {
        ConsumptionTick();

        TimeTick();
    }

    private void ConsumptionTick()
    {

    }
    private void TimeTick()
    {
        CurrentTime++;
    }
}
