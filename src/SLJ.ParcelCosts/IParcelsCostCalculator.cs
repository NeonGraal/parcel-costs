namespace SLJ.ParcelCosts
{
  public interface IParcelsCostCalculator
  {
    IOrderCosting CalculateCosts(IOrder order);
  }
}
