namespace SLJ.ParcelCosts.Implementation
{
  internal class ParcelCosting : IParcelCosting
  {
    public IParcel Parcel { get; set; }
    public decimal ParcelCost { get; set; }
    public ParcelCostingType CostingType { get; set; }
  }
}
