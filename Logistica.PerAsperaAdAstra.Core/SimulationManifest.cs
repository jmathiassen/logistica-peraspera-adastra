namespace LogisticaPerAsperaAdAstra.Core;

public enum UnitType { Count, Mass, Volume, Length }
public enum StateOfMatter { Solid, Liquid, Gas, Electricity }

public interface IProducible
{
    string Id { get; }
    string Name { get; }
    List<Recipe> Recipes { get; }
}
public interface IManufacturable : IProducible
{
    double MassPerUnitKg { get; }
}
public record FormDefinitionDto(string FormId, string Name, string Symbol, UnitType UnitType, double VolumeM3);
public record MaterialDefinitionDto(string MaterialId, string Name, StateOfMatter State, double DensityKgM3, List<RecipeDto>? Recipes);
public record ResourceDefinitionDto(string ResourceId, string Name, string MaterialId, string FormId, List<RecipeDto>? Recipes, List<string>? Tags = null);
public record ComponentDefinitionDto(string ComponentId, string Name, List<RecipeDto>? Recipes, List<IComponentSpecification> Specifications, List<string>? Tags = null);

public record RecipeDto(RecipeCategory Category, List<RecipeInputDto> Inputs, List<RecipeOutputDto> Outputs, int ConstructionTime, bool IsPrimaryMassDefinition = false);
public record RecipeInputDto(string ItemId, double Quantity);
public record RecipeOutputDto(string ItemId, double MassRatio);

public sealed record RecipeInputItem(IManufacturable Item, double Quantity);
public sealed record RecipeOutputItem(IManufacturable Item, double MassRatio);
public sealed record Recipe(RecipeCategory Category, List<RecipeInputItem> Inputs, List<RecipeOutputItem> Outputs, bool IsPrimaryMassDefinition = false);

public sealed record StorageRule(List<string> RequiredTags, List<string> ForbiddenTags);

public interface IComponentSpecification;
public sealed record ContainerSpecification(double Quantity, StorageRule Rule) : IComponentSpecification;
public sealed record ChassisSpecification(double LengthMeters, bool HasCogwheelDrive = false) : IComponentSpecification;
public sealed record ElectricMotorSpecification(double MaxPowerKw, double EfficiencyPercent) : IComponentSpecification;
public sealed record FuelType(string ResourceId, double ConsumptionRatePerSecond);
public sealed record CombustionEngineSpecification(double MaxPowerKw, List<FuelType> FuelTypes, double SpecificFuelConsumptionGramsPerKwh) : IComponentSpecification;
public sealed record RocketEngineSpecification(double MaxPowerKw, List<FuelType> FuelTypes, double SpecificImpulseSeconds, double ExhaustVelocityMps, double ExhaustPressurePa, double NozzleAreaM2) : IComponentSpecification;

public sealed record FormDefinition(string Id, string Name, string Symbol, UnitType Type, double VolumeM3);
public sealed record MaterialDefinition(string Id, string Name, StateOfMatter State, List<Recipe> Recipes, double DensityKgM3) : IProducible;
public sealed record ResourceDefinition(string Id, string Name, MaterialDefinition Material, FormDefinition Form, List<Recipe> Recipes, double MassPerUnitKg, List<string> Tags) : IManufacturable;
public sealed record ComponentDefinition(string Id, string Name, List<Recipe> Recipes, double MassPerUnitKg, List<string> Tags) : IManufacturable;

public enum RecipeCategory {
    Refining,
    Smelting,
    Recycling,
    Alloying,
    Forming,
    Stamping,
    Machining,
    Assembling,
    Disassembling
}

public class SimulationManifest
{
    private readonly Dictionary<string, FormDefinition> _forms = [];
    private readonly Dictionary<string, MaterialDefinition> _materials = [];
    private readonly Dictionary<string, ResourceDefinition> _resources = [];
    private readonly Dictionary<string, ComponentDefinition> _components = [];
    private readonly Dictionary<Type, Dictionary<string, IComponentSpecification>> _specificationsByType = [];

    public IReadOnlyDictionary<string, FormDefinition> Forms => _forms;
    public IReadOnlyDictionary<string, MaterialDefinition> Materials => _materials;
    public IReadOnlyDictionary<string, ResourceDefinition> Resources => _resources;
    public IReadOnlyDictionary<string, ComponentDefinition> Components => _components;

    public SimulationManifest(string? dataPath = null)
    {
        if (!string.IsNullOrEmpty(dataPath))
            PopulateDtosFromJson(dataPath); // Reads from JSON files
        else
            PopulateDtosFromDefaults();

        Bootstrap();
    }
    public FormDefinition GetForm(string id) => !_forms.TryGetValue(id, out FormDefinition? form) ? throw new KeyNotFoundException($"Form with ID '{id}' not found.") : form;
    public MaterialDefinition GetMaterial(string id) => !_materials.TryGetValue(id, out MaterialDefinition? material) ? throw new KeyNotFoundException($"Material with ID '{id}' not found.") : material;
    public ResourceDefinition GetResource(string id) => !_resources.TryGetValue(id, out ResourceDefinition? resource) ? throw new KeyNotFoundException($"Resource with ID '{id}' not found.") : resource;
    public IManufacturable GetItem(string id)
    {
        if (_resources.TryGetValue(id, out ResourceDefinition? resource)) return resource;
        if (_components.TryGetValue(id, out ComponentDefinition? component)) return component;

        throw new KeyNotFoundException($"Producible item with ID '{id}' not found.");
    }
    public T? GetSpecification<T>(string componentId) where T : class, IComponentSpecification
    {
        if (!_specificationsByType.TryGetValue(typeof(T), out Dictionary<string, IComponentSpecification>? specs)) return null;
        if (!specs.TryGetValue(componentId, out IComponentSpecification? spec)) return null;

        return spec as T;
    }

    private void PopulateDtosFromDefaults()
    {
        // All of these are currently temporary and definitely incomplete at this point. I'll focus on fleshing this out later.
        List<FormDefinitionDto> formDtos =
        [
            new(FormId:"form_kwh", Name:"Kilowatt-Hour", Symbol:"kWh", UnitType.Count, VolumeM3:0),
            new(FormId:"form_pile", Name:"Pile", Symbol:"m³", UnitType.Volume, VolumeM3:1),
            new(FormId:"form_ingot", Name:"Ingot", Symbol:"", UnitType.Count, VolumeM3:0.00089),
            new(FormId:"form_plate", Name:"Plate", Symbol:"", UnitType.Count, VolumeM3:0.001),
            new(FormId:"form_scrap", Name:"Scrap", Symbol:"kg", UnitType.Mass, VolumeM3:0.00035),
            new(FormId:"form_nut", Name:"Nut", Symbol:"m", UnitType.Count, VolumeM3:0.000001),
            new(FormId:"form_bolt", Name:"Bolt", Symbol:"m", UnitType.Count, VolumeM3:0.000001),
            new(FormId:"form_barrel", Name:"Barrel", Symbol:"bbl", UnitType.Volume, VolumeM3:0.159),
            new(FormId:"form_wire_18ga_1m", Name:"18-Gauge Wire (1m)", Symbol:"m", UnitType.Length, VolumeM3:0.00000082),
            new(FormId:"form_wire_roll_1km", Name:"1km Wire Roll", Symbol:"m", UnitType.Count, VolumeM3:0.00085)
        ];
        List<MaterialDefinitionDto> materialDtos =
        [
            new(MaterialId:"mat_electron_flow", State:StateOfMatter.Solid, Name:"Electron Flow", DensityKgM3: 0, Recipes:[]),
            new(MaterialId:"mat_coal", Name:"Coal", State:StateOfMatter.Solid, DensityKgM3: 1500, Recipes:[]),
            new(MaterialId:"mat_iron_ore", Name:"Iron Ore", State:StateOfMatter.Solid, DensityKgM3: 3500, Recipes:[]),
            new(MaterialId:"mat_chromium_ore", Name:"Chromium Ore", State:StateOfMatter.Solid, DensityKgM3: 4500, Recipes:[]),
            new(MaterialId:"mat_copper_ore", Name:"Copper Ore", State:StateOfMatter.Solid, DensityKgM3: 4000, Recipes:[]),
            new(MaterialId:"mat_slag", Name:"Slag", State:StateOfMatter.Solid, DensityKgM3:2800, Recipes:[]),
            new(MaterialId:"mat_iron", Name:"Iron", State:StateOfMatter.Solid, DensityKgM3:7870, Recipes:[]),
            new(MaterialId:"mat_copper", Name:"Copper", State:StateOfMatter.Solid, DensityKgM3:8960, Recipes:[]),
            new(MaterialId:"mat_stainless_steel", Name:"Stainless Steel", State:StateOfMatter.Solid, DensityKgM3:8000, Recipes:[]),
        ];
        List<ResourceDefinitionDto> resourceDtos =
        [
            new(ResourceId:"util_electricity_kwh", Name:"Electricity", MaterialId:"mat_electron_flow", FormId:"form_kwh", Recipes:[], Tags:["fuel", "electricity"]),
            new(ResourceId:"res_coal", Name:"Coal", MaterialId:"mat_coal", FormId:"form_pile", Recipes:[], Tags:["ore", "bulk_solid", "flammable"]),
            new(ResourceId:"res_iron_ore", Name:"Iron Ore", MaterialId:"mat_iron_ore", FormId:"form_pile", Recipes:[], Tags:["ore", "bulk_solid"]),
            new(ResourceId:"res_chromium_ore", Name:"Chromium Ore", MaterialId:"mat_chromium_ore", FormId:"form_pile", Recipes:[], Tags:["ore", "bulk_solid"]),
            new(ResourceId:"res_copper_ore", Name:"Copper Ore", MaterialId:"mat_copper_ore", FormId:"form_pile", Recipes:[], Tags:["ore", "bulk_solid"]),
            new(ResourceId:"res_slag", Name:"Slag", MaterialId:"mat_slag", FormId:"form_pile", Recipes:[]),
            new(ResourceId:"res_iron_ingot", Name:"Iron Ingot", MaterialId:"mat_iron", FormId:"form_ingot", Recipes:
            [
                new RecipeDto(
                    Category:RecipeCategory.Smelting,
                    Inputs: [
                        new RecipeInputDto(ItemId:"res_iron_ore", Quantity:10),
                        new RecipeInputDto(ItemId:"res_coal", Quantity:5)
                    ],
                    Outputs: [
                        new RecipeOutputDto(ItemId:"res_iron_ingot", MassRatio:0.583),
                        new RecipeOutputDto(ItemId:"res_slag", MassRatio:0.417)
                    ],
                    ConstructionTime:1,
                    IsPrimaryMassDefinition: true
                ),
                new RecipeDto(
                    Category:RecipeCategory.Smelting,
                    Inputs: [
                        new RecipeInputDto(ItemId:"res_iron_scrap", Quantity:10),
                        new RecipeInputDto(ItemId:"util_electricity_kwh", Quantity:30)
                    ],
                    Outputs: [
                        new RecipeOutputDto(ItemId:"res_iron_ingot", MassRatio:0.95),
                        new RecipeOutputDto(ItemId:"res_slag", MassRatio:0.05)
                    ],
                    ConstructionTime:1
                )
            ]),
            new(ResourceId:"res_copper_ingot", Name:"Copper Ingot", MaterialId:"mat_copper", FormId:"form_ingot", Recipes:
            [
                new RecipeDto(
                    Category:RecipeCategory.Smelting,
                    Inputs: [new RecipeInputDto(ItemId:"res_copper_ore", Quantity:10), new RecipeInputDto(ItemId:"res_coal", Quantity:1)],
                    Outputs: [new RecipeOutputDto(ItemId:"res_copper_ingot", MassRatio:0.98)],
                    ConstructionTime:1,
                    IsPrimaryMassDefinition: true
                )
            ]),
            new(ResourceId:"res_stainless_steel_ingot", Name:"Stainless Steel Ingot", MaterialId:"mat_stainless_steel", FormId:"form_ingot", Recipes:
            [
                new RecipeDto(
                    Category:RecipeCategory.Alloying,
                    Inputs: [new RecipeInputDto(ItemId:"res_iron_ingot", Quantity:10), new RecipeInputDto(ItemId:"res_chromium_ore", Quantity:1.5)],
                    Outputs: [new RecipeOutputDto(ItemId:"res_stainless_steel_ingot", MassRatio:0.98)],
                    ConstructionTime:1,
                    IsPrimaryMassDefinition: true
                )
            ]),
            new(ResourceId:"res_copper_wire_1g_1m", Name:"Copper Wire, 1 Gauge, 1m", MaterialId:"mat_copper", FormId:"form_wire_18ga_1m", Recipes:
            [
                new RecipeDto(
                    Category:RecipeCategory.Forming,
                    Inputs: [new RecipeInputDto(ItemId:"res_copper_ingot", Quantity:10), new RecipeInputDto(ItemId:"res_plastic", Quantity:1.5)],
                    Outputs: [new RecipeOutputDto(ItemId:"res_copper_wire_1g_1m", MassRatio:0.98)],
                    ConstructionTime:1,
                    IsPrimaryMassDefinition: true
                )
            ]),
            new(ResourceId:"res_copper_wire_18g_1m", Name:"Copper Wire, 18 Gauge, 1m", MaterialId:"mat_copper", FormId:"form_wire_18ga_1m", Recipes:
            [
                new RecipeDto(
                    Category:RecipeCategory.Forming,
                    Inputs: [new RecipeInputDto(ItemId:"res_copper_ingot", Quantity:10), new RecipeInputDto(ItemId:"res_plastic", Quantity:1.5)],
                    Outputs: [new RecipeOutputDto(ItemId:"res_copper_wire_18g_1m", MassRatio:0.98)],
                    ConstructionTime:1,
                    IsPrimaryMassDefinition: true
                )
            ]),
            new(ResourceId:"res_copper_wire_18g_roll_1km", Name:"Copper Wire, 18 Gauge, Roll, 1km", MaterialId:"mat_copper", FormId:"form_wire_18ga_1m", Recipes:
            [
                new RecipeDto(
                    Category:RecipeCategory.Assembling,
                    Inputs: [new RecipeInputDto(ItemId:"res_copper_wire_18g_1m", Quantity:1002)],
                    Outputs: [new RecipeOutputDto(ItemId:"res_copper_wire_18g_roll_1km", MassRatio:0.95), new RecipeOutputDto(ItemId:"res_copper_scrap", MassRatio:0.05)],
                    ConstructionTime:1,
                    IsPrimaryMassDefinition: true
                ),
                new RecipeDto(
                    Category:RecipeCategory.Disassembling,
                    Inputs: [new RecipeInputDto(ItemId:"res_copper_wire_1g_roll_1km", Quantity:1)],
                    Outputs: [new RecipeOutputDto(ItemId:"res_copper_wire_1g_1m", MassRatio:0.95), new RecipeOutputDto(ItemId:"res_copper_scrap", MassRatio:0.05)],
                    ConstructionTime:1
                )
            ]),
            new(ResourceId:"res_stainless_steel_plate", Name:"Stainless Steel Plate", MaterialId:"mat_stainless_steel", FormId:"form_plate", Recipes:[]),
            new(ResourceId:"res_stainless_steel_scrap", Name:"Stainless Steel Scrap", MaterialId:"mat_stainless_steel", FormId:"form_scrap_pile", Recipes:[], Tags:["bulk_solid"]),
            new(ResourceId:"res_stainless_steel_bolt", Name:"Stainless Steel Bolt", MaterialId:"mat_stainless_steel", FormId:"form_bolt", Recipes:
            [
                new RecipeDto(
                    Category:RecipeCategory.Stamping,
                    Inputs: [new RecipeInputDto(ItemId:"res_stainless_steel_plate", Quantity:1)],
                    Outputs: [new RecipeOutputDto(ItemId:"res_stainless_steel_bolt", MassRatio:0.8), new RecipeOutputDto(ItemId:"res_stainless_steel_scrap", MassRatio:0.2)],
                    ConstructionTime:1,
                    IsPrimaryMassDefinition: true
                )
            ], Tags:["bulk_solid"])
        ];
        List<ComponentDefinitionDto> componentDtos =
        [
            new(ComponentId:"comp_chassis_shunter", Name:"Shunter Chassis", Recipes:
            [
                new RecipeDto(
                    Category:RecipeCategory.Assembling,
                    Inputs: [new RecipeInputDto(ItemId:"res_stainless_steel_plate", Quantity:1000)],
                    Outputs: [new RecipeOutputDto(ItemId:"comp_chassis_shunter", MassRatio:1)], // Note: MassRatio is 1.0 for single-output recipes
                    ConstructionTime:1,
                    IsPrimaryMassDefinition: true
                )
            ],
            Specifications:[
                new ChassisSpecification(LengthMeters:12),
                new ContainerSpecification(Quantity:1000, new StorageRule(RequiredTags:["fuel", "electricity"], ForbiddenTags:[]))
            ], Tags:["train", "chassis"]),
            new(ComponentId:"comp_engine_electric_small", Name:"Small Electric Engine", Recipes:
            [
                new RecipeDto(
                    Category:RecipeCategory.Assembling,
                    Inputs: [new RecipeInputDto(ItemId:"res_copper_wire_18g_roll_1km", Quantity:2), new RecipeInputDto(ItemId:"res_stainless_steel_bolt", Quantity:50)],
                    Outputs: [new RecipeOutputDto(ItemId:"comp_engine_electric_small", MassRatio:1)],
                    ConstructionTime:1,
                    IsPrimaryMassDefinition: true
                )
            ],
            Specifications:[new ElectricMotorSpecification(MaxPowerKw:1200, EfficiencyPercent:95)], Tags:["engine", "electric"])
        ];

        Hydrate(formDtos, materialDtos, resourceDtos, componentDtos);
    }
    private void PopulateDtosFromJson(string path) { /* Deserializes JSON files */ }

    private void Hydrate(List<FormDefinitionDto> formDtos, List<MaterialDefinitionDto> materialDtos, List<ResourceDefinitionDto> resourceDtos, List<ComponentDefinitionDto> componentDtos)
    {
        // First, populate collections that have no dependencies
        foreach(FormDefinitionDto dto in formDtos) _forms.Add(dto.FormId, new FormDefinition(dto.FormId, dto.Name, dto.Symbol, dto.UnitType, dto.VolumeM3));

        // Create placeholders for materials and resources to allow cross-referencing
        foreach(MaterialDefinitionDto dto in materialDtos) _materials.Add(dto.MaterialId, new MaterialDefinition(dto.MaterialId, dto.Name, dto.State, Recipes:[], dto.DensityKgM3));
        foreach(ResourceDefinitionDto dto in resourceDtos) _resources.Add(dto.ResourceId, new ResourceDefinition(dto.ResourceId, dto.Name, GetMaterial(dto.MaterialId), GetForm(dto.FormId), Recipes:[], MassPerUnitKg:0, dto.Tags ?? []));
        foreach(ComponentDefinitionDto dto in componentDtos)
        {
            _components.Add(dto.ComponentId, new ComponentDefinition(dto.ComponentId, dto.Name, Recipes:[], MassPerUnitKg:0, dto.Tags ?? []));
            foreach (IComponentSpecification spec in dto.Specifications)
            {
                Type specType = spec.GetType();
                if (!_specificationsByType.TryGetValue(specType, out Dictionary<string, IComponentSpecification>? specification))
                    _specificationsByType[specType] = specification = [];

                specification.Add(dto.ComponentId, spec);
            }
        }

        // Now, hydrate the recipes, which have dependencies
        foreach (MaterialDefinitionDto matDto in materialDtos)
        {
            List<Recipe> hydratedRecipes = HydrateRecipes(matDto.Recipes);
            _materials[matDto.MaterialId] = _materials[matDto.MaterialId] with { Recipes = hydratedRecipes };
        }
        foreach (ResourceDefinitionDto resDto in resourceDtos)
        {
            List<Recipe> hydratedRecipes = HydrateRecipes(resDto.Recipes);
            _resources[resDto.ResourceId] = _resources[resDto.ResourceId] with { Recipes = hydratedRecipes };
        }
        foreach (ComponentDefinitionDto compDto in componentDtos)
        {
            List<Recipe> hydratedRecipes = HydrateRecipes(compDto.Recipes);
            _components[compDto.ComponentId] = _components[compDto.ComponentId] with { Recipes = hydratedRecipes };
        }
    }
    private List<Recipe> HydrateRecipes(List<RecipeDto>? recipeDtos)
    {
        if (recipeDtos is null) return [];

        return recipeDtos.Select(dto => new Recipe(
            dto.Category,
            dto.Inputs.Select(input => new RecipeInputItem(GetItem(input.ItemId), input.Quantity)).ToList(),
            dto.Outputs.Select(output => new RecipeOutputItem(GetItem(output.ItemId), output.MassRatio)).ToList(),
            dto.IsPrimaryMassDefinition
        )).ToList();
    }

    private void Bootstrap()
    {
        // First pass: Calculate mass for all RAW resources.
        foreach (ResourceDefinition resource in _resources.Values.Where(resource => !resource.Recipes.Any()))
            _resources[resource.Id] = resource with { MassPerUnitKg = resource.Material.DensityKgM3 * resource.Form.VolumeM3 };

        // Second pass: Topologically sort materials based on their recipe dependencies.
        List<IProducible> producibles = [];
        producibles.AddRange(_materials.Values);
        producibles.AddRange(_resources.Values);
        producibles.AddRange(_components.Values);

        Dictionary<string, IProducible> allProduciblesDict = producibles.ToDictionary(manufacturable => manufacturable.Id);
        Dictionary<string, HashSet<string>> dependencyGraph = producibles.ToDictionary(manufacturable => manufacturable.Id, p => new HashSet<string>());
        Dictionary<string, int> dependencyCount = producibles.ToDictionary(manufacturable => manufacturable.Id, manufacturable => 0);

        foreach (IProducible item in producibles.Where(producible => producible.Recipes.Any()))
            foreach (Recipe recipe in item.Recipes)
                foreach (RecipeInputItem input in recipe.Inputs)
                    if (dependencyGraph[input.Item.Id].Add(item.Id)) dependencyCount[item.Id]++;

        Queue<string> queue = new(dependencyCount.Where(kvp => kvp.Value == 0).Select(kvp => kvp.Key));
        List<string> sortedItemIds = [];
        while (queue.Count > 0)
        {
            string currentId = queue.Dequeue();
            sortedItemIds.Add(currentId);

            foreach (string dependentId in dependencyGraph[currentId])
                if (--dependencyCount[dependentId] == 0)
                    queue.Enqueue(dependentId);
        }

        if (sortedItemIds.Count < dependencyCount.Count)
            throw new InvalidOperationException($"A circular dependency was detected. Could not resolve: {string.Join(", ", dependencyCount.Where(kvp => kvp.Value > 0).Select(kvp => kvp.Key))}");

        foreach (string itemId in sortedItemIds)
        {
            IProducible item = allProduciblesDict[itemId];
            if (!item.Recipes.Any()) continue;

            Recipe? primaryRecipe = item.Recipes.FirstOrDefault(r => r.IsPrimaryMassDefinition)
                ?? throw new InvalidDataException($"Could not determine the primary recipe for defining mass for '{item.Id}'. Please mark one recipe with IsPrimaryMassDefinition = true.");

            double totalInputMass = primaryRecipe.Inputs.Sum(recipeItem => recipeItem.Item.MassPerUnitKg * recipeItem.Quantity);

            RecipeOutputItem primaryOutputItem = primaryRecipe.Outputs.FirstOrDefault(output => output.Item.Id == itemId)
                ?? throw new InvalidOperationException($"Item {itemId} recipe does not produce itself as output.");

            double outputMass = totalInputMass * primaryOutputItem.MassRatio;

            switch (item)
            {
                // Update the item based on its type
                case MaterialDefinition material:
                {
                    ResourceDefinition outputResource = primaryOutputItem.Item as ResourceDefinition
                        ?? throw new InvalidOperationException($"Material recipe output must be a resource, but was {primaryOutputItem.Item.GetType().Name}.");

                    double outputVolume = outputResource.Form.VolumeM3;
                    double newDensity = outputVolume > 0 ? outputMass / outputVolume : 0;
                    _materials[itemId] = material with { DensityKgM3 = newDensity };

                    // Update all resources using this newly calculated material density
                    foreach (ResourceDefinition res in _resources.Values.Where(resource => resource.Material.Id == itemId))
                        _resources[res.Id] = res with { MassPerUnitKg = newDensity * res.Form.VolumeM3 };
                    break;
                }
                case ResourceDefinition resource:
                    _resources[itemId] = resource with { MassPerUnitKg = outputMass };
                    break;
                case ComponentDefinition component:
                    _components[itemId] = component with { MassPerUnitKg = outputMass };
                    break;
            }
        }
    }
}