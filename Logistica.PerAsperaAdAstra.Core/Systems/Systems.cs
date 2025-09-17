using Arch.Core;
using LogisticaPerAsperaAdAstra.Core.Components;

namespace LogisticaPerAsperaAdAstra.Core.Systems;

public class WorldGenerationSystem
{
    public void Generate(World world)
    {
        // === STEP 1: CREATE ALL THE NODES (STATIONS/CITIES) ===
        // We use a dictionary to easily access them by name later.
        Dictionary<string, Entity> nodes = new Dictionary<string, Entity>();

        // A helper lambda to reduce boilerplate when creating nodes
        Action<string, Coordinate> createNode = (string name, Coordinate coords) =>
        {
            Entity entity = world.Create(
                new Components.Position { Value = coords },
                new City(),
                new Name { Value = name },
                new IsNode()
            );
            nodes.Add(name, entity);
        };

        // Main Hub
        createNode("Oslo", new Coordinate(0, 0));

        // Line 1: Bergensbanen (Oslo-Bergen)
        createNode("Drammen", new Coordinate(-50, 10));
        createNode("Hønefoss", new Coordinate(-60, 80));
        createNode("Gol", new Coordinate(-150, 150));
        createNode("Geilo", new Coordinate(-200, 160));
        createNode("Finse", new Coordinate(-280, 180));
        createNode("Voss", new Coordinate(-380, 170));
        createNode("Bergen", new Coordinate(-450, 150));

        // Line 2: Dovrebanen (Oslo-Trondheim)
        createNode("Lillestrøm", new Coordinate(20, 20));
        createNode("Hamar", new Coordinate(30, 130));
        createNode("Lillehammer", new Coordinate(20, 190));
        createNode("Dombås", new Coordinate(-50, 300));
        createNode("Oppdal", new Coordinate(-60, 380));
        createNode("Trondheim", new Coordinate(-70, 500));

        // Line 3: Sørlandsbanen (Oslo-Stavanger)
        createNode("Kongsberg", new Coordinate(-90, 0));
        createNode("Kristiansand", new Coordinate(-200, -300));
        createNode("Egersund", new Coordinate(-400, -250));
        createNode("Stavanger", new Coordinate(-420, -200));

        // === STEP 2: BUILD THE RAILWAY LINES (EDGES) ===

        // --- Bergensbanen (Oslo-Bergen) ---
        // Mostly single track over the mountains
        CreateRailLink(world, nodes["Oslo"], nodes["Drammen"], 2);
        CreateRailLink(world, nodes["Drammen"], nodes["Hønefoss"], 1);
        CreateRailLink(world, nodes["Hønefoss"], nodes["Gol"], 1);
        CreateRailLink(world, nodes["Gol"], nodes["Geilo"], 1);
        CreateRailLink(world, nodes["Geilo"], nodes["Finse"], 1);
        CreateRailLink(world, nodes["Finse"], nodes["Voss"], 1);
        CreateRailLink(world, nodes["Voss"], nodes["Bergen"], 1);

        // --- Dovrebanen (Oslo-Trondheim) ---
        // Double track on the busy section north of Oslo
        CreateRailLink(world, nodes["Oslo"], nodes["Lillestrøm"], 2);
        CreateRailLink(world, nodes["Lillestrøm"], nodes["Hamar"], 2);
        CreateRailLink(world, nodes["Hamar"], nodes["Lillehammer"], 1);
        CreateRailLink(world, nodes["Lillehammer"], nodes["Dombås"], 1);
        CreateRailLink(world, nodes["Dombås"], nodes["Oppdal"], 1);
        CreateRailLink(world, nodes["Oppdal"], nodes["Trondheim"], 1);

        // --- Sørlandsbanen (Drammen-Stavanger) ---
        // Shares the Oslo-Drammen link, then is mostly single track
        CreateRailLink(world, nodes["Drammen"], nodes["Kongsberg"], 1);
        CreateRailLink(world, nodes["Kongsberg"], nodes["Kristiansand"], 1); // A very long segment
        CreateRailLink(world, nodes["Kristiansand"], nodes["Egersund"], 1);
        CreateRailLink(world, nodes["Egersund"], nodes["Stavanger"], 2); // Double track at the end
    }

    /// <summary>
    /// Helper method to create a two-way rail link between two nodes.
    /// </summary>
    private void CreateRailLink(World world, Entity nodeA, Entity nodeB, int trackCount)
    {
        // Create the edge entity
        var edgeEntity = world.Create(
            new Edge { NodeA = nodeA, NodeB = nodeB },
            new EdgeDirectionality { Direction = EdgeDirection.Both },
            new RailTrack { }
        );

        // Link the edge back to its nodes
        ref var nodeA_GraphData = ref world.Get<IsGraphNode>(nodeA);
        nodeA_GraphData.ConnectedEdges.Add(edgeEntity);

        ref var nodeB_GraphData = ref world.Get<IsGraphNode>(nodeB);
        nodeB_GraphData.ConnectedEdges.Add(edgeEntity);
    }
}