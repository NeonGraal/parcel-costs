namespace SLJ.ParcelCosts.Implementation
{
  internal class ParcelCostingParameters
  {
    public ParcelCostingParameters(
      ParcelCostingType costingType,
      decimal baseCost,
      decimal maxWeight,
      decimal costPerExtraKg,
      decimal? dimensionsMax = null)
    {
      CostingType = costingType;
      BaseCost = baseCost;
      MaxWeight = maxWeight;
      CostPerExtraKg = costPerExtraKg;
      DimensionsMax = dimensionsMax;
    }

    public ParcelCostingType CostingType { get; }
    public decimal BaseCost { get; }
    public decimal MaxWeight { get; }
    public decimal CostPerExtraKg { get; }
    public decimal? DimensionsMax { get; }
  }
}
