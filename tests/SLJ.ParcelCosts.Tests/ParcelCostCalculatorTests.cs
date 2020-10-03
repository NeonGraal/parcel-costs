using FluentAssertions;
using Moq;
using NUnit.Framework;
using SLJ.ParcelCosts.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
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

    Mock<IParcel> MakeParcel(decimal dimension, decimal weight)
    {
      var parcel = new Mock<IParcel>();

      parcel.SetupGet(p => p.Height).Returns(dimension);
      parcel.SetupGet(p => p.Width).Returns(dimension);
      parcel.SetupGet(p => p.Depth).Returns(dimension);
      parcel.SetupGet(p => p.Weight).Returns(weight);

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
      _order.SetupGet(o => o.Parcels).Returns(new[] { MakeParcel(dimension, 1).Object });

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
        MakeParcel(5, 1).Object,
        MakeParcel(25, 2).Object,
        MakeParcel(75, 3).Object,
        MakeParcel(125, 4).Object,
      });

      var result = _calculator.CalculateCosts(_order.Object);

      result.TotalCost.Should().Be(51);
      result.ParcelCosts.Should().HaveCount(4)
        .And.Contain(CheckParcel(3, ParcelCostingType.Small))
        .And.Contain(CheckParcel(8, ParcelCostingType.Medium))
        .And.Contain(CheckParcel(15, ParcelCostingType.Large))
        .And.Contain(CheckParcel(25, ParcelCostingType.ExtraLarge));
    }


    [Test]
    public void CalculateCosts_ForSpeedyEmptyOrder_ShouldReturnEmptyZeroOrderCostingWithZeroSpeedyShipping()
    {
      _order.SetupGet(o => o.SpeedyShipping).Returns(true);
      _order.SetupGet(o => o.Parcels).Returns(new IParcel[] { });

      var result = _calculator.CalculateCosts(_order.Object);

      result.TotalCost.Should().Be(0);
      result.ParcelCosts.Should().ContainSingle()
        .And.Contain(CheckParcel(0, ParcelCostingType.SpeedyShipping));
    }

    [TestCase(5, 3, ParcelCostingType.Small)]
    [TestCase(25, 8, ParcelCostingType.Medium)]
    [TestCase(75, 15, ParcelCostingType.Large)]
    [TestCase(125, 25, ParcelCostingType.ExtraLarge)]
    public void CalculateCosts_ForSpeedyOrderWithSizedParcel_ShouldTotalCostDoubleParcelCost(decimal dimension, decimal cost, ParcelCostingType parcelType)
    {
      _order.SetupGet(o => o.SpeedyShipping).Returns(true);
      _order.SetupGet(o => o.Parcels).Returns(new[] { MakeParcel(dimension, 1).Object });

      var result = _calculator.CalculateCosts(_order.Object);

      result.TotalCost.Should().Be(cost * 2);
      result.ParcelCosts.Should().HaveCount(2)
        .And.Contain(CheckParcel(cost, parcelType))
        .And.Contain(CheckParcel(cost, ParcelCostingType.SpeedyShipping));
    }

    [Test]
    public void CalculateCosts_ForSpeedyOrderForAllSizes_ShouldReturnCorrectTotalAndCostings()
    {
      _order.SetupGet(o => o.SpeedyShipping).Returns(true);
      _order.SetupGet(o => o.Parcels).Returns(new[] {
        MakeParcel(5, 1).Object,
        MakeParcel(25, 2).Object,
        MakeParcel(75, 3).Object,
        MakeParcel(125, 4).Object,
      });

      var result = _calculator.CalculateCosts(_order.Object);

      result.TotalCost.Should().Be(102);
      result.ParcelCosts.Should().HaveCount(5)
        .And.Contain(CheckParcel(3, ParcelCostingType.Small))
        .And.Contain(CheckParcel(8, ParcelCostingType.Medium))
        .And.Contain(CheckParcel(15, ParcelCostingType.Large))
        .And.Contain(CheckParcel(25, ParcelCostingType.ExtraLarge))
        .And.Contain(CheckParcel(51, ParcelCostingType.SpeedyShipping));
    }

    [Test]
    public void CalculateCosts_ForSpeedyOrderwithNullParcels_ShouldReturnEmptyZeroOrderCostingWithZeroSpeedyShipping()
    {
      _order.SetupGet(o => o.SpeedyShipping).Returns(true);
      _order.SetupGet(o => o.Parcels).Returns((IEnumerable<IParcel>)null);

      var result = _calculator.CalculateCosts(_order.Object);

      result.TotalCost.Should().Be(0);
      result.ParcelCosts.Should().ContainSingle()
        .And.Contain(CheckParcel(0, ParcelCostingType.SpeedyShipping));
    }

    [TestCase(5, 1, 3, ParcelCostingType.Small)]
    [TestCase(5, 1.5, 5, ParcelCostingType.Small)]
    [TestCase(5, 2.5, 7, ParcelCostingType.Small)]
    [TestCase(25, 3, 8, ParcelCostingType.Medium)]
    [TestCase(25, 3.5, 10, ParcelCostingType.Medium)]
    [TestCase(25, 5.5, 14, ParcelCostingType.Medium)]
    [TestCase(75, 6, 15, ParcelCostingType.Large)]
    [TestCase(75, 6.5, 17, ParcelCostingType.Large)]
    [TestCase(75, 9.5, 23, ParcelCostingType.Large)]
    [TestCase(125, 10, 25, ParcelCostingType.ExtraLarge)]
    [TestCase(125, 10.5, 27, ParcelCostingType.ExtraLarge)]
    [TestCase(125, 14.5, 35, ParcelCostingType.ExtraLarge)]
    public void CalculateCosts_ForOrderWithWeightedParcel_ShouldBeAmountAndType(decimal dimension, decimal weight, decimal cost, ParcelCostingType parcelType)
    {
      _order.SetupGet(o => o.Parcels).Returns(new[] { MakeParcel(dimension, weight).Object });

      var result = _calculator.CalculateCosts(_order.Object);

      result.TotalCost.Should().Be(cost);
      result.ParcelCosts.Should().ContainSingle()
        .And.Contain(CheckParcel(cost, parcelType));
    }

    [Test]
    public void CalculateCosts_ForOrderForAllSizesJustOverweight_ShouldReturnCorrectTotalAndCostings()
    {
      _order.SetupGet(o => o.Parcels).Returns(new[] {
        MakeParcel(5, 1.5m).Object,
        MakeParcel(25, 3.5m).Object,
        MakeParcel(75, 6.5m).Object,
        MakeParcel(125, 10.5m).Object,
      });

      var result = _calculator.CalculateCosts(_order.Object);

      result.TotalCost.Should().Be(59);
      result.ParcelCosts.Should().HaveCount(4)
        .And.Contain(CheckParcel(5, ParcelCostingType.Small))
        .And.Contain(CheckParcel(10, ParcelCostingType.Medium))
        .And.Contain(CheckParcel(17, ParcelCostingType.Large))
        .And.Contain(CheckParcel(27, ParcelCostingType.ExtraLarge));
    }

    [Test]
    public void CalculateCosts_ForOrderForAllSizesDoubleOverweight_ShouldReturnCorrectTotalAndCostings()
    {
      _order.SetupGet(o => o.Parcels).Returns(new[] {
        MakeParcel(5, 2).Object,
        MakeParcel(25, 6).Object,
        MakeParcel(75, 12).Object,
        MakeParcel(125, 20).Object,
      });

      var result = _calculator.CalculateCosts(_order.Object);

      result.TotalCost.Should().Be(91);
      result.ParcelCosts.Should().HaveCount(4)
        .And.Contain(CheckParcel(5, ParcelCostingType.Small))
        .And.Contain(CheckParcel(14, ParcelCostingType.Medium))
        .And.Contain(CheckParcel(27, ParcelCostingType.Large))
        .And.Contain(CheckParcel(45, ParcelCostingType.ExtraLarge));
    }

    [TestCase(5, 1, 3, ParcelCostingType.Small)]
    [TestCase(5, 1.5, 5, ParcelCostingType.Small)]
    [TestCase(5, 2.5, 7, ParcelCostingType.Small)]
    [TestCase(25, 3, 8, ParcelCostingType.Medium)]
    [TestCase(25, 3.5, 10, ParcelCostingType.Medium)]
    [TestCase(25, 5.5, 14, ParcelCostingType.Medium)]
    [TestCase(75, 6, 15, ParcelCostingType.Large)]
    [TestCase(75, 6.5, 17, ParcelCostingType.Large)]
    [TestCase(75, 9.5, 23, ParcelCostingType.Large)]
    [TestCase(125, 10, 25, ParcelCostingType.ExtraLarge)]
    [TestCase(125, 10.5, 27, ParcelCostingType.ExtraLarge)]
    [TestCase(125, 14.5, 35, ParcelCostingType.ExtraLarge)]
    public void CalculateCosts_ForSpeedyOrderWithWeightedParcel_ShouldBeAmountAndType(decimal dimension, decimal weight, decimal cost, ParcelCostingType parcelType)
    {
      _order.SetupGet(o => o.SpeedyShipping).Returns(true);
      _order.SetupGet(o => o.Parcels).Returns(new[] { MakeParcel(dimension, weight).Object });

      var result = _calculator.CalculateCosts(_order.Object);

      result.TotalCost.Should().Be(cost * 2);
      result.ParcelCosts.Should().HaveCount(2)
        .And.Contain(CheckParcel(cost, parcelType))
        .And.Contain(CheckParcel(cost, ParcelCostingType.SpeedyShipping));
    }

    [Test]
    public void CalculateCosts_ForSpeedyOrderForAllSizesJustOverweight_ShouldReturnCorrectTotalAndCostings()
    {
      _order.SetupGet(o => o.SpeedyShipping).Returns(true);
      _order.SetupGet(o => o.Parcels).Returns(new[] {
        MakeParcel(5, 1.5m).Object,
        MakeParcel(25, 3.5m).Object,
        MakeParcel(75, 6.5m).Object,
        MakeParcel(125, 10.5m).Object,
      });

      var result = _calculator.CalculateCosts(_order.Object);

      result.TotalCost.Should().Be(118);
      result.ParcelCosts.Should().HaveCount(5)
        .And.Contain(CheckParcel(5, ParcelCostingType.Small))
        .And.Contain(CheckParcel(10, ParcelCostingType.Medium))
        .And.Contain(CheckParcel(17, ParcelCostingType.Large))
        .And.Contain(CheckParcel(27, ParcelCostingType.ExtraLarge))
        .And.Contain(CheckParcel(59, ParcelCostingType.SpeedyShipping));
    }

    [TestCase(5, 23.5, 49, ParcelCostingType.Small)]
    [TestCase(5, 24.5, 50, ParcelCostingType.Heavy)]
    [TestCase(5, 50.5, 51, ParcelCostingType.Heavy)]
    [TestCase(25, 23.5, 50, ParcelCostingType.Medium)]
    [TestCase(25, 24.5, 50, ParcelCostingType.Heavy)]
    [TestCase(25, 50.5, 51, ParcelCostingType.Heavy)]
    [TestCase(75, 22.5, 49, ParcelCostingType.Large)]
    [TestCase(75, 23.5, 50, ParcelCostingType.Heavy)]
    [TestCase(75, 50.5, 51, ParcelCostingType.Heavy)]
    [TestCase(125, 21.5, 49, ParcelCostingType.ExtraLarge)]
    [TestCase(125, 22.5, 50, ParcelCostingType.Heavy)]
    [TestCase(125, 50.5, 51, ParcelCostingType.Heavy)]
    public void CalculateCosts_ForOrderWithOverWeightedParcel_ShouldBeAmountAndType(decimal dimension, decimal weight, decimal cost, ParcelCostingType parcelType)
    {
      _order.SetupGet(o => o.Parcels).Returns(new[] { MakeParcel(dimension, weight).Object });

      var result = _calculator.CalculateCosts(_order.Object);

      result.TotalCost.Should().Be(cost);
      result.ParcelCosts.Should().ContainSingle()
        .And.Contain(CheckParcel(cost, parcelType));
    }

    [Test]
    public void CalculateCosts_ForOrderForAllSizesHeavy_ShouldReturnCorrectTotalAndCostings()
    {
      _order.SetupGet(o => o.Parcels).Returns(new[] {
        MakeParcel(5, 24.5m).Object,
        MakeParcel(25, 25.5m).Object,
        MakeParcel(75, 23.5m).Object,
        MakeParcel(125, 22.5m).Object,
      });

      var result = _calculator.CalculateCosts(_order.Object);

      result.TotalCost.Should().Be(200);
      result.ParcelCosts.Should().HaveCount(4);
      result.ParcelCosts.Where(p => p.ParcelCost == 50 && p.CostingType == ParcelCostingType.Heavy).Should().HaveCount(4);
    }

    [Test]
    public void CalculateCosts_ForOrderWith4SmallParcels_ShouldCalculateSmallParcelManiaDiscount()
    {
      _order.SetupGet(o => o.Parcels).Returns(new[] {
        MakeParcel(5, 1).Object,
        MakeParcel(5, 1).Object,
        MakeParcel(5, 1).Object,
        MakeParcel(5, 1).Object,
      });

      var result = _calculator.CalculateCosts(_order.Object);

      result.TotalCost.Should().Be(9);
      result.ParcelCosts.Should().HaveCount(5)
        .And.Contain(CheckParcel(-3, ParcelCostingType.SmallParcelMania));
    }

    [Test]
    public void CalculateCosts_ForOrderWith3MediumParcels_ShouldCalculateMediumParcelManiaDiscount()
    {
      _order.SetupGet(o => o.Parcels).Returns(new[] {
        MakeParcel(25, 1).Object,
        MakeParcel(25, 1).Object,
        MakeParcel(25, 1).Object,
      });

      var result = _calculator.CalculateCosts(_order.Object);

      result.TotalCost.Should().Be(16);
      result.ParcelCosts.Should().HaveCount(4)
        .And.Contain(CheckParcel(-8, ParcelCostingType.MediumParcelMania));
    }

    [Test]
    public void CalculateCosts_ForOrderWith5MixedParcels_ShouldCalculateMixedParcelManiaDiscount()
    {
      _order.SetupGet(o => o.Parcels).Returns(new[] {
        MakeParcel(5, 1).Object,
        MakeParcel(25, 1).Object,
        MakeParcel(75, 1).Object,
        MakeParcel(125, 1).Object,
        MakeParcel(5, 1).Object,
      });

      var result = _calculator.CalculateCosts(_order.Object);

      result.TotalCost.Should().Be(51);
      result.ParcelCosts.Should().HaveCount(6)
        .And.Contain(CheckParcel(-3, ParcelCostingType.MixedParcelMania));
    }

    [Test]
    public void CalculateCosts_ForOrderWith6MediumParcels_ShouldCalculate2MediumParcelManiaDiscounts()
    {
      _order.SetupGet(o => o.Parcels).Returns(new[] {
        MakeParcel(25, 1).Object,
        MakeParcel(25, 4).Object,
        MakeParcel(25, 1).Object,
        MakeParcel(25, 4).Object,
        MakeParcel(25, 1).Object,
        MakeParcel(25, 4).Object,
      });

      var result = _calculator.CalculateCosts(_order.Object);

      result.TotalCost.Should().Be(36);
      result.ParcelCosts.Should().HaveCount(8)
        .And.Contain(CheckParcel(-8, ParcelCostingType.MediumParcelMania))
        .And.Contain(CheckParcel(-10, ParcelCostingType.MediumParcelMania));
    }

    [Test]
    public void CalculateCosts_ForOrderWith12MixedParcels_ShouldCalculateBestDiscount()
    {
      _order.SetupGet(o => o.Parcels).Returns(new[] {
        MakeParcel(5, 1).Object, // Small $3
        MakeParcel(25, 1).Object, // Medium $8
        MakeParcel(75, 1).Object, // Large $15
        MakeParcel(125, 1).Object, // XL $25
        MakeParcel(5, 2).Object, // Small $5
        MakeParcel(25, 4).Object, // Medium $10
        MakeParcel(75, 7).Object, // Large $17
        MakeParcel(125, 11).Object, // XL $27
        MakeParcel(5, 3).Object, // Small $7
        MakeParcel(25, 5).Object, // Medium $12
        MakeParcel(75, 8).Object, // Large $19
        MakeParcel(5, 6).Object, // Small $8
      });

      var result = _calculator.CalculateCosts(_order.Object);

      result.TotalCost.Should().Be(135);
      result.ParcelCosts.Should().HaveCount(15)
        .And.Contain(CheckParcel(-3, ParcelCostingType.SmallParcelMania))
        .And.Contain(CheckParcel(-8, ParcelCostingType.MediumParcelMania))
        .And.Contain(CheckParcel(-15, ParcelCostingType.MixedParcelMania));
    }

    [Test]
    public void CalculateCosts_ForSpeedyOrderWith6MediumParcels_ShouldCalculateCorrectTotal()
    {
      _order.SetupGet(o => o.SpeedyShipping).Returns(true);
      _order.SetupGet(o => o.Parcels).Returns(new[] {
          MakeParcel(25, 1).Object,
          MakeParcel(25, 4).Object,
          MakeParcel(25, 1).Object,
          MakeParcel(25, 4).Object,
          MakeParcel(25, 1).Object,
          MakeParcel(25, 4).Object,
        });

      var result = _calculator.CalculateCosts(_order.Object);

      result.TotalCost.Should().Be(72);
      result.ParcelCosts.Should().HaveCount(9)
        .And.Contain(CheckParcel(-8, ParcelCostingType.MediumParcelMania))
        .And.Contain(CheckParcel(-10, ParcelCostingType.MediumParcelMania))
        .And.Contain(CheckParcel(36, ParcelCostingType.SpeedyShipping));
    }
  }
}
