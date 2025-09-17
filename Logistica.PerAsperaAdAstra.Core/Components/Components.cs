using Arch.Core;

namespace LogisticaPerAsperaAdAstra.Core.Components;

// --- CORE DATA COMPONENTS ---
public struct Connections { public List<Entity> Entities; }
public struct IndexPosition { public int Index; }
public struct Parent { public Entity Reference; }
public struct Position { public Coordinate Value; }
public struct Name { public string Value; }
public struct Condition { public double Health; }
public struct Population { public int[] MaleByYear; public int[] FemaleByYear; }
public struct Capacity { public int Value; }
public struct Quantity { public int Value; }
public struct ReorderPoint { public int Value; }
public struct Length { public double Meters; }
public struct Weight { public double Kg; }
public struct MaxPower { public double KiloWatt; }
public struct CurrentThrottle { public double Percent; }
public struct Volume { public double CubicMeters; }
public struct KineticEnergy { public double Joules; }

// --- GRAPH NODE COMPONENTS ---
public struct City;
public struct Junction;
public struct Waypoint;
public struct Station;
public struct Platform;
public struct Industry;

public struct Refinery;
public struct Smelter;
public struct RecyclingPlant;
public struct AlloyForge;
public struct FormingMill;
public struct MachineShop;
public struct AssemblyPlant;
public struct ScrapYard;
public struct Warehouse;
public struct DistributionCenter;
public struct RetailCenter;
public struct Farm;
public struct Mine;
public struct Quarry;
public struct LumberMill;
public struct OilRig;
public struct PowerPlant;

// --- GRAPH EDGE COMPONENTS ---
public struct RailTrack;
public struct Edge { public Entity NodeA; public Entity NodeB; }
public struct DirectionBoth;
public struct DirectionAToB;
public struct DirectionBToA;
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
