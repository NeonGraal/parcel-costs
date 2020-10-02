using FluentAssertions;
using Moq;
using NUnit.Framework;
using SLJ.ParcelCosts.Implementation;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SLJ.ParcelCosts.Tests
{
  public class ParcelCostCalculatorTests
  {
    ParcelsCostCalculator _calculator;

    Mock<IOrder> _order;

    [SetUp]
    public void Setup()
    {
      _calculator = new ParcelsCostCalculator();

      _order = new Mock<IOrder>();
    }

    Mock<IParcel> MakeParcel(decimal dimension)
    {
      var parcel = new Mock<IParcel>();

      parcel.SetupGet(p => p.Height).Returns(dimension);
      parcel.SetupGet(p => p.Width).Returns(dimension);
      parcel.SetupGet(p => p.Depth).Returns(dimension);

      return parcel;
    }

    Expression<Func<IParcelCosting, bool>> CheckParcel(decimal cost, ParcelCostingType parcelType)
      => p => p.CostingType == parcelType && p.ParcelCost == cost;

    [TestCase(5, 3, ParcelCostingType.Small)]
    [TestCase(25, 8, ParcelCostingType.Medium)]
    [TestCase(75, 15, ParcelCostingType.Large)]
    [TestCase(125, 25, ParcelCostingType.ExtraLarge)]
    public void CalculateCosts_ForOrderWithSizedParcel_ShouldBeAmountAndType(decimal dimension, decimal cost, ParcelCostingType parcelType)
    {
      _order.SetupGet(o => o.Parcels).Returns(new[] { MakeParcel(dimension).Object });

      var result = _calculator.CalculateCosts(_order.Object);

      result.TotalCost.Should().Be(cost);
      result.ParcelCosts.Should().ContainSingle()
        .And.Contain(CheckParcel(cost, parcelType));
    }

    [Test]
    public void CalculateCosts_ForNullOrder_ShouldReturnEmptyZeroOrderCosting()
    {
      var result = _calculator.CalculateCosts(null);

      result.TotalCost.Should().Be(0);
      result.ParcelCosts.Should().BeEmpty();
    }

    [Test]
    public void CalculateCosts_ForOrderwithNullParcels_ShouldReturnEmptyZeroOrderCosting()
    {
      _order.SetupGet(o => o.Parcels).Returns((IEnumerable<IParcel>)null);

      var result = _calculator.CalculateCosts(_order.Object);

      result.TotalCost.Should().Be(0);
      result.ParcelCosts.Should().BeEmpty();
    }

    [Test]
    public void CalculateCosts_ForEmptyOrder_ShouldReturnEmptyZeroOrderCosting()
    {
      _order.SetupGet(o => o.Parcels).Returns(new IParcel[] { });

      var result = _calculator.CalculateCosts(_order.Object);

      result.TotalCost.Should().Be(0);
      result.ParcelCosts.Should().BeEmpty();
    }

    [Test]
    public void CalculateCosts_ForOrderForAllSizes_ShouldReturnCorrectTotalAndCostings()
    {
      _order.SetupGet(o => o.Parcels).Returns(new[] {
        MakeParcel(5).Object,
        MakeParcel(25).Object,
        MakeParcel(75).Object,
        MakeParcel(125).Object,
      });

      var result = _calculator.CalculateCosts(_order.Object);

      result.TotalCost.Should().Be(51);
      result.ParcelCosts.Should().HaveCount(4)
        .And.Contain(CheckParcel(3, ParcelCostingType.Small))
        .And.Contain(CheckParcel(8, ParcelCostingType.Medium))
        .And.Contain(CheckParcel(15, ParcelCostingType.Large))
        .And.Contain(CheckParcel(25, ParcelCostingType.ExtraLarge));
    }
  }
}
