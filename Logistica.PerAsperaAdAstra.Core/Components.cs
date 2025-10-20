using Arch.Core;

namespace LogisticaPerAsperaAdAstra.Core.Components;

// --- CORE DATA COMPONENTS ---
public struct Connections { public List<Entity> Entities; }
public struct IndexPosition { public int Value; }
public struct Parent { public Entity Value; }
public struct Position { public Coordinate Value; }
public struct Name { public string Value; }
public struct Condition { public double Value; }
public struct Population { public int[] MaleByYear; public int[] FemaleByYear; }
public struct Occupants { public List<Entity> Entities; }
public struct Occupying { public List<Entity> Segments; }
public struct SegmentCapacity { public double OccupiedMeters; }
public struct Capacity { public int Value; }
public struct Quantity { public int Value; }
public struct Volume { public double CubicMeters; }
public struct ReorderPoint { public int Value; }
public struct Length { public double Meters; }
public struct Weight { public double Kg; }
public struct FrontalAreaM2 { public double Value; }
public struct MaxPower { public double KiloWatt; }
public struct Consumes { public List<string> ResourceIds; }
public enum EfficiencyType
{
    SpecificFuelConsumption, // For thermal engines (g/kWh)
    SpecificImpulse,         // For rocket engines (seconds)
    Percentage               // For electric motors (%)
}
public struct EngineEfficiency { public double Value; public EfficiencyType Type; }
public struct StoppingDistance { public double Meters; }
public struct TargetSpeed { public double MetersPerSeconds; }
public struct CurrentSpeed { public double MetersPerSeconds; public double DistanceToTargetMeters; }
public struct CurrentThrottle { public double Percent; }
public struct KineticEnergy { public double Joules; }
public struct Route { public List<Entity> Segments; public int CurrentSegmentIndex; } // TODO: rework or replace

// --- GRAPH NODE COMPONENTS ---
public interface INode;
public interface IIndustry;
public struct IsNode;
public struct IsIndustry;
public struct City : INode;
public struct Junction : INode;
public struct Waypoint : INode;
public struct TrainStation : INode;
public struct BusStation : INode;
public struct Platform : INode;

public struct Refinery : INode, IIndustry;
public struct Smelter : INode, IIndustry;
public struct RecyclingPlant : INode, IIndustry;
public struct AlloyForge : INode, IIndustry;
public struct FormingMill : INode, IIndustry;
public struct MachineShop : INode, IIndustry;
public struct AssemblyPlant : INode, IIndustry;
public struct ScrapYard : INode, IIndustry;
public struct Warehouse : INode, IIndustry;
public struct DistributionCenter : INode, IIndustry;
public struct RetailCenter : INode, IIndustry;
public struct Farm : INode, IIndustry;
public struct Mine : INode, IIndustry;
public struct Quarry : INode, IIndustry;
public struct LumberMill : INode, IIndustry;
public struct OilRig : INode, IIndustry;
public struct PowerPlant : INode, IIndustry;

// --- GRAPH EDGE COMPONENTS ---
public interface IEdge;
public struct IsEdge;
public struct RailTrack : IEdge;
public struct Road : IEdge;
public struct Sealane : IEdge;
public struct Airway : IEdge;
public struct Pipeline : IEdge;
public struct Canal : IEdge;
public struct Highway : IEdge;
public struct Freeway : IEdge;
public struct Overpass : IEdge;
public struct Underpass : IEdge;
public struct Bridge : IEdge;
public struct Tunnel : IEdge;
public struct Edge { public Entity NodeA; public Entity NodeB; }

public enum EdgeDirection
{
    Both,
    AToB,
    BToA
}
public interface IEdgeDirectionality;
public struct DirectionBoth : IEdgeDirectionality;
public struct DirectionAToB : IEdgeDirectionality;
public struct DirectionBToA : IEdgeDirectionality;
public struct Segment { public double Incline; }

// --- RESOURCE & PRODUCTION COMPONENTS ---
public struct IndustryFocus { public string ResourceId; }

// --- VEHICLE COMPONENTS ---
public struct Chassis;
public struct Engine;
public struct Train;
public struct TrainLocomotive;
public struct TrainWagon;

// --- ORDER & LOGISTICS COMPONENTS ---
public struct Order
{
    public string ItemId;
    public int Quantity;
    public Entity Requester;
    public Entity? Supplier;
    public GalacticDateTime CreatedAt;
    public GalacticDateTime? AcceptedAt;
    public GalacticDateTime? FulfilledAt;
    public OrderStatus Status;
}

public enum OrderStatus
{
    Open,
    Accepted,
    ReadyForPickup,
    InTransit,
    Fulfilled
}

// --- LIFECYCLE TAGS ---
public struct Planned;
public struct Surveying;
public struct SurveyComplete;
public struct PendingConstruction;
public struct UnderConstruction
{
    public Entity ConstructionEntity;
    public Dictionary<string, int> RequiredItems;
    public Dictionary<string, int> DeliveredItems;
    public GalacticDateTime ConstructingUntil;
}
public struct HaltedConstruction;
public struct Obsolete;
public struct PendingDemolition;
public struct ActiveDemolition;

