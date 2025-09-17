using Arch.Core;
using LogisticaPerAsperaAdAstra.Core.Components;

namespace LogisticaPerAsperaAdAstra.Core.Systems;

public static class NorwayNetworkBuilder
{
    public static void Build(SimulationInstance sim)
    {
        // --- 1. Define Major Nodes (Cities & Junctions) ---
        // We'll use a simplified coordinate system for logical placement.
        Entity oslo = sim.PlanCity(new Coordinate(X: 500, Y: 800), "Oslo");
        Entity drammen = sim.PlanCity(new Coordinate(X: 480, Y: 780), "Drammen");
        Entity honefoss = sim.PlanCity(new Coordinate(X: 485, Y: 750), "Hønefoss");
        Entity lillehammer = sim.PlanCity(new Coordinate(X: 510, Y: 650), "Lillehammer");
        Entity dombas = sim.PlanCity(new Coordinate(X: 490, Y: 500), "Dombås");
        Entity trondheim = sim.PlanCity(new Coordinate(X: 500, Y: 300), "Trondheim");
        Entity bodo = sim.PlanCity(new Coordinate(X: 600, Y: 100), "Bodø");
        Entity narvik = sim.PlanCity(new Coordinate(X: 750, Y: 50), "Narvik");
        Entity swedishBorder = sim.PlanCity(new Coordinate(X: 800, Y: 55), "Swedish Border"); // Represents connection to Sweden
        Entity bergen = sim.PlanCity(new Coordinate(X: 200, Y: 700), "Bergen");
        Entity kristiansand = sim.PlanCity(new Coordinate(X: 400, Y: 950), "Kristiansand");
        Entity stavanger = sim.PlanCity(new Coordinate(X: 250, Y: 900), "Stavanger");
        Entity myrdal = sim.PlanCity(new Coordinate(X: 300, Y: 680), "Myrdal"); // Junction for Flåmsbana
        Entity flam = sim.PlanCity(new Coordinate(X: 310, Y: 670), "Flåm"); // End of Flåmsbana

        // --- 2. Create Edges (Main Railway Lines) ---
        // Each edge is created as a bidirectional rail track. Waypoints are used to add simple curves.

        // Dovrebanen (The main line Oslo -> Trondheim)
        sim.PlanRailTrackEdge(oslo, lillehammer, EdgeDirection.Both, waypoints:[new Coordinate(505, 725), new Coordinate(510, 680)]);
        sim.PlanRailTrackEdge(lillehammer, dombas, EdgeDirection.Both, waypoints:[new Coordinate(500, 575)]);
        sim.PlanRailTrackEdge(dombas, trondheim, EdgeDirection.Both, waypoints:[new Coordinate(495, 400)]);

        // Nordlandsbanen (The long line Trondheim -> Bodø)
        sim.PlanRailTrackEdge(trondheim, bodo, EdgeDirection.Both, waypoints:[new Coordinate(550, 200)]);

        // Ofotbanen (Crucial iron ore line from Sweden to the coast)
        // We'll represent the connection to Sweden as just another node for now.
        sim.PlanRailTrackEdge(narvik, swedishBorder, EdgeDirection.Both, waypoints:[]);

        // Bergensbanen (Scenic route across the mountains Oslo -> Bergen)
        sim.PlanRailTrackEdge(oslo, honefoss, EdgeDirection.Both, waypoints:[new Coordinate(490, 765)]);
        sim.PlanRailTrackEdge(honefoss, myrdal, EdgeDirection.Both, waypoints:[new Coordinate(400, 710), new Coordinate(350, 690)]);
        sim.PlanRailTrackEdge(myrdal, bergen, EdgeDirection.Both, waypoints:[new Coordinate(250, 690)]);

        // Flåmsbana (Famous steep branch line from Myrdal down to Flåm)
        // Modeled as a single track.
        sim.PlanRailTrackEdge(myrdal, flam, EdgeDirection.Both, waypoints:[]);

        // Sørlandsbanen (Connects Oslo to the southern coast)
        sim.PlanRailTrackEdge(oslo, drammen, EdgeDirection.Both, waypoints:[]);
        sim.PlanRailTrackEdge(drammen, kristiansand, EdgeDirection.Both, waypoints:[new Coordinate(450, 850), new Coordinate(420, 900)]);
        sim.PlanRailTrackEdge(kristiansand, stavanger, EdgeDirection.Both, waypoints:[new Coordinate(350, 960), new Coordinate(300, 940)]);
    }
}