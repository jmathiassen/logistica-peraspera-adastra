namespace LogisticaPerAsperaAdAstra.Core;

/// <summary>
/// A utility class for common physics calculations.
/// All calculations use SI units (meters, kilograms, seconds, Newtons, Joules, Watts).
/// </summary>
public static class GeneralPhysics
{
    // --- Braking ---

    /// <summary>
    /// Calculates the maximum braking force based on friction and grip.
    /// </summary>
    /// <param name="massKg">Mass contributing to grip (e.g., whole train) in kilograms.</param>
    /// <param name="gravityMps2">Gravitational acceleration.</param>
    /// <param name="gripCoefficient">The grip coefficient of the wheels/brakes.</param>
    /// <returns>Maximum braking force in Newtons.</returns>
    public static double ComputeBrakeForce(double massKg, double gravityMps2, double gripCoefficient) => massKg * gravityMps2 * gripCoefficient;

    // --- Energy & Speed ---

    /// <summary>
    /// Calculates the kinetic energy of a body.
    /// </summary>
    /// <param name="massKg">The mass in kilograms.</param>
    /// <param name="speedInMps">The speed in meters per second.</param>
    /// <returns>The kinetic energy in Joules.</returns>
    public static double ComputeKineticEnergy(double massKg, double speedInMps) => 0.5 * massKg * speedInMps * speedInMps;

    /// <summary>
    /// Calculates the speed of a body from its kinetic energy.
    /// </summary>
    /// <param name="massKg">The mass in kilograms.</param>
    /// <param name="kineticEnergyJ">The kinetic energy in Joules.</param>
    /// <returns>The speed in meters per second.</returns>
    public static double ComputeSpeed(double massKg, double kineticEnergyJ) => kineticEnergyJ > 0 ? Math.Sqrt(2 * kineticEnergyJ / massKg) : 0;
}

public static class AtmospherePhysics
{
    /// <summary>
    /// Calculates the atmospheric pressure at a given altitude using the Barometric Formula.
    /// </summary>
    /// <param name="altitudeMeters">The altitude above sea level in meters.</param>
    /// <param name="seaLevelPressurePa">The planet's sea-level pressure in Pascals (e.g., Earth ~101325).</param>
    /// <param name="scaleHeightMeters">The planet's atmospheric scale height in meters (e.g., Earth ~8500).</param>
    /// <returns>Atmospheric pressure in Pascals (Pa).</returns>
    public static double ComputePressureAtAltitude(double altitudeMeters, double seaLevelPressurePa, double scaleHeightMeters) => scaleHeightMeters <= 0 ? 0 : seaLevelPressurePa * Math.Exp(-altitudeMeters / scaleHeightMeters);
}

/// <summary>
/// A utility class for common land vehicle physics calculations.
/// All calculations use SI units (meters, kilograms, seconds, Newtons, Joules, Watts).
/// </summary>
public static class VehiclePhysics
{
    // --- Propulsion ---

    /// <summary>
    /// Calculates the maximum forward force an engine can produce, limited by both power and wheel adhesion.
    /// </summary>
    /// <param name="powerInWatts">Engine power in Watts (kW * 1000).</param>
    /// <param name="speedInMps">Vehicle speed in meters per second.</param>
    /// <param name="massOnDrivenWheelsKg">The mass supported by the driven wheels, contributing to grip.</param>
    /// <param name="gravityMps2">Gravitational acceleration.</param>
    /// <param name="adhesionCoefficient">Coefficient of friction/adhesion for the wheels (e.g., 0.7 for steel on steel).</param>
    /// <returns>Tractive force in Newtons.</returns>
    public static double ComputeTractiveForce(double powerInWatts, double speedInMps, double massOnDrivenWheelsKg, double gravityMps2, double adhesionCoefficient)
    {
        double powerLimitedForce = powerInWatts / (speedInMps < 0.1 ? 0.1 : speedInMps);
        double adhesionLimitedForce = massOnDrivenWheelsKg * gravityMps2 * adhesionCoefficient;
        return Math.Min(powerLimitedForce, adhesionLimitedForce);
    }

    /// <summary>
    /// Calculates the force produced by a reaction engine (a rocket), including the atmospheric pressure term.
    /// </summary>
    /// <param name="massFlowRateKgS">Rate of propellant consumption in kg/s.</param>
    /// <param name="exhaustVelocityMps">The velocity of the exhaust gas.</param>
    /// <param name="nozzleExitPressurePa">The pressure of the gas at the nozzle exit.</param>
    /// <param name="ambientPressurePa">The ambient atmospheric pressure at the current altitude.</param>
    /// <param name="nozzleAreaM2">The area of the nozzle exit in square meters.</param>
    /// <returns>Thrust force in Newtons.</returns>
    public static double ComputeThrustForce(double massFlowRateKgS, double exhaustVelocityMps, double nozzleExitPressurePa, double ambientPressurePa, double nozzleAreaM2) => (massFlowRateKgS * exhaustVelocityMps) + ((nozzleExitPressurePa - ambientPressurePa) * nozzleAreaM2);

    // --- Resistance Forces ---

    /// <summary>
    /// Calculates the force of gravity on a vehicle due to a slope.
    /// Can be positive (downhill, assisting) or negative (uphill, resisting).
    /// </summary>
    /// <param name="massKg">Total vehicle mass in kilograms.</param>
    /// <param name="gravityMps2">Gravitational acceleration (e.g., 9.81).</param>
    /// <param name="gradientRatio">The gradient as a ratio (e.g., 2% = 0.02).</param>
    /// <returns>The gradient force in Newtons.</returns>
    public static double ComputeGradientForce(double massKg, double gravityMps2, double gradientRatio) => massKg * gravityMps2 * gradientRatio;

    /// <summary>
    /// Calculates the rolling resistance for a land vehicle.
    /// </summary>
    /// <param name="massKg">Total vehicle mass in kilograms.</param>
    /// <param name="gravityMps2">Gravitational acceleration.</param>
    /// <param name="rollingResistanceCoefficient">Coefficient of rolling resistance.</param>
    /// <returns>Rolling resistance force in Newtons.</returns>
    public static double ComputeRollingForce(double massKg, double gravityMps2, double rollingResistanceCoefficient) => massKg * gravityMps2 * rollingResistanceCoefficient;

    /// <summary>
    /// Calculates aerodynamic drag on a vehicle.
    /// </summary>
    /// <param name="speedInMps">Vehicle speed in meters per second.</param>
    /// <param name="airDensity">Density of the atmosphere (e.g., Earth is ~1.225 kg/m³).</param>
    /// <param name="dragCoefficient">The vehicle's drag coefficient (dimensionless).</param>
    /// <param name="frontalAreaM2">The vehicle's frontal area in square meters.</param>
    /// <returns>Aerodynamic drag force in Newtons.</returns>
    public static double ComputeAirDragForce(double speedInMps, double airDensity, double dragCoefficient, double frontalAreaM2) => 0.5 * airDensity * dragCoefficient * frontalAreaM2 * speedInMps * speedInMps;
}

/// <summary>
/// A utility class for naval physics, including surface vessels and hydrofoils.
/// </summary>
public static class NavalPhysics
{
    /// <summary>
    /// Calculates the skin friction component of drag for a vessel in water.
    /// </summary>
    /// <param name="speedInMps">Vessel speed in meters per second.</param>
    /// <param name="waterDensity">Density of the water (e.g., fresh water is ~1000 kg/m³).</param>
    /// <param name="dragCoefficient">The vessel's skin friction drag coefficient.</param>
    /// <param name="wettedAreaM2">The vessel's wetted surface area in square meters.</param>
    /// <returns>Skin friction drag force in Newtons.</returns>
    public static double ComputeSkinFrictionDrag(double speedInMps, double waterDensity, double dragCoefficient, double wettedAreaM2) => 0.5 * waterDensity * dragCoefficient * wettedAreaM2 * speedInMps * speedInMps;

    /// <summary>
    /// Calculates the additional drag on a surface vessel from creating waves.
    /// This force increases dramatically as the vessel approaches its hull speed.
    /// </summary>
    /// <param name="speedInMps">Vessel speed in meters per second.</param>
    /// <param name="waterlineLengthMeters">Length of the vessel at the waterline.</param>
    /// <param name="displacementMassKg">The mass of the vessel (equal to displaced water mass).</param>
    /// <param name="gravityMps2">Gravitational acceleration.</param>
    /// <returns>Wave-making resistance force in Newtons.</returns>
    public static double ComputeWaveMakingDrag(double speedInMps, double waterlineLengthMeters, double displacementMassKg, double gravityMps2)
    {
        const double HullSpeedConstant = 2.43; // g / (2 * PI)
        double hullSpeedMps = HullSpeedConstant * Math.Sqrt(waterlineLengthMeters);
        if (speedInMps <= 0 || hullSpeedMps <= 0) return 0;

        // Simplified model: wave drag is a fraction of displacement weight,
        // which grows exponentially as speed approaches hull speed.
        double speedRatio = speedInMps / hullSpeedMps;
        double waveCoefficient = 0.005 * Math.Pow(speedRatio, 6); // The power of 6 creates a steep curve
        return displacementMassKg * gravityMps2 * waveCoefficient;
    }
}

/// <summary>
/// A utility class for aviation physics.
/// </summary>
public static class AviationPhysics
{
    /// <summary>
    /// Calculates parasitic drag on an aircraft (form drag + skin friction).
    /// </summary>
    /// <param name="speedInMps">Aircraft speed in meters per second.</param>
    /// <param name="airDensity">Density of the atmosphere.</param>
    /// <param name="dragCoefficient">The aircraft's zero-lift drag coefficient (Cd0).</param>
    /// <param name="wingAreaM2">The reference area, typically the wing area.</param>
    /// <returns>Parasitic drag force in Newtons.</returns>
    public static double ComputeParasiticDrag(double speedInMps, double airDensity, double dragCoefficient, double wingAreaM2) => 0.5 * airDensity * dragCoefficient * wingAreaM2 * speedInMps * speedInMps;

    /// <summary>
    /// Calculates the lift force generated by a wing.
    /// </summary>
    /// <param name="speedInMps">Aircraft speed in meters per second.</param>
    /// <param name="airDensity">Density of the atmosphere.</param>
    /// <param name="liftCoefficient">The wing's current coefficient of lift (Cl).</param>
    /// <param name="wingAreaM2">The surface area of the wing.</param>
    /// <returns>Lift force in Newtons.</returns>
    public static double ComputeLiftForce(double speedInMps, double airDensity, double liftCoefficient, double wingAreaM2) => 0.5 * airDensity * liftCoefficient * wingAreaM2 * speedInMps * speedInMps;

    /// <summary>
    /// Calculates the drag created as an unavoidable byproduct of generating lift.
    /// This drag is highest at low speeds.
    /// </summary>
    /// <param name="liftForceN">The total lift being generated, in Newtons.</param>
    /// <param name="speedInMps">Aircraft speed in meters per second.</param>
    /// <param name="airDensity">Density of the atmosphere.</param>
    /// <param name="wingSpanMeters">The total span of the wing from tip to tip.</param>
    /// <param name="oswaldEfficiency">Oswald efficiency factor (typically 0.7-0.85).</param>
    /// <returns>Lift-induced drag force in Newtons.</returns>
    public static double ComputeLiftInducedDrag(double liftForceN, double speedInMps, double airDensity, double wingSpanMeters, double oswaldEfficiency = 0.8)
    {
        if (speedInMps <= 0) return 0;
        double denominator = 0.5 * airDensity * speedInMps * speedInMps * Math.PI * (wingSpanMeters * wingSpanMeters) * oswaldEfficiency;
        return denominator <= 0 ? 0 : (liftForceN * liftForceN) / denominator;
    }
}

/// <summary>
/// A utility class for common orbital mechanics calculations.
/// Assumes stable, circular orbits for simplicity.
/// </summary>
public static class OrbitalPhysics
{
    private const double GravitationalConstant = 6.67430e-11;

    /// <summary>
    /// Calculates the required orbital velocity for a stable circular orbit.
    /// </summary>
    /// <param name="planetMassKg">Mass of the celestial body being orbited.</param>
    /// <param name="orbitalRadiusMeters">The distance from the center of the planet.</param>
    /// <returns>Orbital velocity in meters per second.</returns>
    public static double ComputeOrbitalVelocity(double planetMassKg, double orbitalRadiusMeters) => Math.Sqrt((GravitationalConstant * planetMassKg) / orbitalRadiusMeters);

    /// <summary>
    /// Calculates the altitude of a stable circular orbit from its speed.
    /// </summary>
    /// <param name="speedInMps">The orbital speed in meters per second.</param>
    /// <param name="planetMassKg">Mass of the celestial body being orbited (e.g., Earth is 5.972e24 kg).</param>
    /// <param name="planetRadiusMeters">Radius of the celestial body (e.g., Earth is 6.371e6 m).</param>
    /// <returns>The altitude above the planet's surface in meters.</returns>
    public static double ComputeAltitudeFromSpeed(double speedInMps, double planetMassKg, double planetRadiusMeters)
    {
        if (speedInMps <= 0) return 0;

        double orbitalRadius = (GravitationalConstant * planetMassKg) / (speedInMps * speedInMps);
        return (orbitalRadius - planetRadiusMeters);
    }

    /// <summary>
    /// Calculates the required thruster burn time to add a specific amount of energy to an entity.
    /// </summary>
    /// <param name="energyToAddInJoules">The desired change in kinetic energy.</param>
    /// <param name="thrusterPowerInWatts">The power output of the thruster in Watts (kW * 1000).</param>
    /// <returns>The required burn time in seconds.</returns>
    public static double ComputeBurnTimeForEnergyChange(double energyToAddInJoules, double thrusterPowerInWatts) => thrusterPowerInWatts <= 0 ? double.PositiveInfinity : energyToAddInJoules / thrusterPowerInWatts;

    /// <summary>
    /// Calculates orbital drag, the force that causes orbital decay.
    /// </summary>
    /// <param name="orbitalVelocityMps">The station's current velocity.</param>
    /// <param name="atmosphereDensity">Density of the exosphere at that altitude (very low).</param>
    /// <param name="dragCoefficient">The station's drag coefficient.</param>
    /// <param name="crossSectionalAreaM2">The station's cross-sectional area.</param>
    /// <returns>Drag force in Newtons.</returns>
    public static double ComputeOrbitalDragForce(double orbitalVelocityMps, double atmosphereDensity, double dragCoefficient, double crossSectionalAreaM2) => 0.5 * atmosphereDensity * dragCoefficient * crossSectionalAreaM2 * orbitalVelocityMps * orbitalVelocityMps;

    /// <summary>
    /// Calculates the precise force of gravity on a body at a specific altitude.
    /// </summary>
    /// <param name="planetMassKg">Mass of the celestial body.</param>
    /// <param name="bodyMassKg">Mass of the object (e.g., rocket).</param>
    /// <param name="altitudeMeters">The object's altitude above the planet's surface.</param>
    /// <param name="planetRadiusMeters">The radius of the planet.</param>
    /// <returns>Gravitational force in Newtons.</returns>
    public static double ComputeGravitationalForceAtAltitude(double planetMassKg, double bodyMassKg, double altitudeMeters, double planetRadiusMeters) => (GravitationalConstant * planetMassKg * bodyMassKg) / Math.Pow(planetRadiusMeters + altitudeMeters, 2);
}