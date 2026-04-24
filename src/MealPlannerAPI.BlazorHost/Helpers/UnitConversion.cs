using System.Collections.Generic;
using System.Linq;

namespace MealPlannerAPI.BlazorHost.Helpers;

public enum UnitCategory { Weight, Volume, Count }

public record Unit(string Label, string FullName, UnitCategory Category, double? Factor);

public static class Units
{
    // ── Weight ────────────────────────────────────────────────────────────────
    public static readonly Unit G = new("g", "Gram", UnitCategory.Weight, 1);
    public static readonly Unit Kg = new("kg", "Kilogram", UnitCategory.Weight, 1000);
    public static readonly Unit Oz = new("oz", "Ounce", UnitCategory.Weight, 28.35);
    public static readonly Unit Lb = new("lb", "Pound", UnitCategory.Weight, 453.6);

    // ── Volume ────────────────────────────────────────────────────────────────
    public static readonly Unit Ml = new("ml", "Milliliter", UnitCategory.Volume, 1);
    public static readonly Unit L = new("L", "Liter", UnitCategory.Volume, 1000);
    public static readonly Unit Tsp = new("tsp", "Teaspoon", UnitCategory.Volume, 5);
    public static readonly Unit Tbsp = new("tbsp", "Tablespoon", UnitCategory.Volume, 15);
    public static readonly Unit FlOz = new("fl oz", "Fluid Ounce", UnitCategory.Volume, 29.6);
    public static readonly Unit Cup = new("cup", "Cup", UnitCategory.Volume, 240);

    // ── Count ─────────────────────────────────────────────────────────────────
    public static readonly Unit Piece = new("piece", "Piece", UnitCategory.Count, null);
    public static readonly Unit Slice = new("slice", "Slice", UnitCategory.Count, null);
    public static readonly Unit Clove = new("clove", "Clove", UnitCategory.Count, null);
    public static readonly Unit Pinch = new("pinch", "Pinch", UnitCategory.Count, null);
    public static readonly Unit Handful = new("handful", "Handful", UnitCategory.Count, null);
    public static readonly Unit Bunch = new("bunch", "Bunch", UnitCategory.Count, null);
    public static readonly Unit Can = new("can", "Can", UnitCategory.Count, null);
    public static readonly Unit Strip = new("strip", "Strip", UnitCategory.Count, null);

    public static readonly IReadOnlyList<Unit> All = new List<Unit>
    {
        G, Kg, Oz, Lb,
        Ml, L, Tsp, Tbsp, FlOz, Cup,
        Piece, Slice, Clove, Pinch, Handful, Bunch, Can, Strip
    };

    public static readonly IReadOnlyDictionary<string, Unit> ByLabel =
        All.ToDictionary(u => u.Label, u => u);

    public static readonly IReadOnlyList<Unit> WeightUnits =
        All.Where(u => u.Category == UnitCategory.Weight).ToList();

    public static readonly IReadOnlyList<Unit> VolumeUnits =
        All.Where(u => u.Category == UnitCategory.Volume).ToList();

    public static readonly IReadOnlyList<Unit> CountUnits =
        All.Where(u => u.Category == UnitCategory.Count).ToList();
}

public record DensityPreset(string Label, double Density);

public static class DensityPresets
{
    public static readonly IReadOnlyList<DensityPreset> All = new List<DensityPreset>
    {
        new("Water / liquid",    1.0),
        new("Milk",              1.03),
        new("Oil / fat",         0.92),
        new("Honey / syrup",     1.4),
        new("All-purpose flour", 0.53),
        new("Sugar (white)",     0.85),
        new("Brown sugar",       0.72),
        new("Salt",              1.22),
        new("Cocoa powder",      0.6),
        new("Rice (dry)",        0.75),
        new("Oats (dry)",        0.34),
    };
}

public static class UnitConverter
{
    /// <summary>
    /// Convert grams → target unit quantity.
    /// For volume units, provide densityGPerMl (default 1.0 = water).
    /// Returns null if conversion is not possible.
    /// </summary>
    public static double? FromGrams(
        double grams,
        Unit targetUnit,
        double densityGPerMl = 1.0)
    {
        if (grams <= 0) return null;

        return targetUnit.Category switch
        {
            UnitCategory.Weight => grams / targetUnit.Factor!.Value,
            UnitCategory.Volume => grams / (targetUnit.Factor!.Value * densityGPerMl),
            UnitCategory.Count => null, // needs manual input
            _ => null
        };
    }

    /// <summary>
    /// Convert quantity + unit → grams.
    /// For count units, pass gramsOverride directly.
    /// </summary>
    public static double? ToGrams(
        double quantity,
        Unit unit,
        double densityGPerMl = 1.0,
        double? gramsOverride = null)
    {
        if (quantity <= 0) return null;

        return unit.Category switch
        {
            UnitCategory.Weight => quantity * unit.Factor!.Value,
            UnitCategory.Volume => quantity * unit.Factor!.Value * densityGPerMl,
            UnitCategory.Count => gramsOverride > 0 ? gramsOverride : null,
            _ => null
        };
    }

    /// <summary>
    /// Format grams into a human-readable string for a target unit.
    /// </summary>
    public static string Format(
        double grams,
        Unit targetUnit,
        double densityGPerMl = 1.0)
    {
        var quantity = FromGrams(grams, targetUnit, densityGPerMl);
        if (quantity is null) return $"{grams:0.##} g";
        return $"{quantity:0.##} {targetUnit.Label}";
    }

    /// <summary>
    /// Auto-pick the best unit and format — mirrors your TS fallback logic.
    /// </summary>
    public static string AutoFormat(double grams)
    {
        return grams switch
        {
            >= 1000 => Format(grams, Units.Kg),
            >= 1 => $"{grams:0.##} g",
            _ => $"{grams * 1000:0.##} mg"
        };
    }
}