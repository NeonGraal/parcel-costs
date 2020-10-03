namespace SLJ.ParcelCosts.Implementation
{
  internal class ParcelCostingParameters
  {
    public ParcelCostingParameters(decimal baseCost, decimal maxWeight, decimal costPerExtraKg)
    {
      BaseCost = baseCost;
      MaxWeight = maxWeight;
      CostPerExtraKg = costPerExtraKg;
    }

    public decimal BaseCost { get; }
    public decimal MaxWeight { get; }
    public decimal CostPerExtraKg { get; }
  }
}
