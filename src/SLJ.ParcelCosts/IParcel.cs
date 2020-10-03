namespace SLJ.ParcelCosts
{
  public interface IParcel
  {
    decimal Height { get; }
    decimal Width { get; }
    decimal Depth { get; }
    decimal Weight { get; }
  }
}
