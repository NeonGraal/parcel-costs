namespace SLJ.ParcelCosts
{
  public interface IParcelCosting
  {
    IParcel Parcel { get; }
    decimal ParcelCost { get; }
    ParcelCostingType CostingType { get; }
  }
}
