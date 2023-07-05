using congestion.calculator;
using congestion.calculator.Repository;
using Moq;

namespace CongestionTaxCalculator.Tests
{
    public class TaxCalculatorTest
    {
        #region Fields & Properties

        private const int DEFAULT_YEAR = 2013;

        private DateTime holidayDate
            => new DateTime(2013, 1, 1);

        private DateTime aDayBeforeHoliday
            => new DateTime(2013, 3, 27);

        private DateTime aDayAfterHoliday
            => new DateTime(2013, 1, 2);

        private DateTime nonHolidayDateExemptedHour
            => new DateTime(2013, 1, 3, 2, 0, 0);

        private DateTime nonHolidayDateNonExemptedHour
            => new DateTime(2013, 1, 3, 6, 0, 0);

        private DateTime nonHolidayDateNonExemptedHour2
            => new DateTime(2013, 1, 3, 7, 0, 0);

        private CongestionTaxCalculator calculator;

        #endregion

        #region Constructor

        public TaxCalculatorTest()
        {
            calculator = new CongestionTaxCalculator();
        }

        #endregion

        #region Happy path tests

        [Fact]
        public void ExemptedVehicles_ExpectZero()
        {
            var dates = new DateTime[] { new DateTime(DEFAULT_YEAR, 2, 2) };
            var mockedVehicle = GetExemptedVehicle();

            var tax = calculator.GetTax(mockedVehicle, dates);

            Assert.Equal(0, tax);
        }

        [Fact]
        public void InHoliday_ExpectZero()
        {
            var dates = new DateTime[] { holidayDate };
            var mockedVehicle = GetNonExemptedVehicle();

            var tax = calculator.GetTax(mockedVehicle, dates);

            Assert.Equal(0, tax);
        }

        [Fact]
        public void InExemptedHours_ExpectZero()
        {
            var dates = new DateTime[] { nonHolidayDateExemptedHour };
            var mockedVehicle = GetNonExemptedVehicle();

            var tax = calculator.GetTax(mockedVehicle, dates);

            Assert.Equal(0, tax);
        }

        [Fact]
        public void InNonExemptedHoures_ExpectNonZero()
        {
            var dates = new DateTime[] { nonHolidayDateNonExemptedHour };
            var mockedVehicle = GetNonExemptedVehicle();

            var tax = calculator.GetTax(mockedVehicle, dates);

            Assert.Equal(8, tax);
        }

        #endregion

        #region Marginal tests

        [Fact]
        public void InADayBeforeHoliday_ExpectZero()
        {
            var dates = new DateTime[] { aDayBeforeHoliday };
            var mockedVehicle = GetNonExemptedVehicle();

            var tax = calculator.GetTax(mockedVehicle, dates);

            Assert.Equal(0, tax);
        }

        [Fact]
        public void InADayAfterHoliday_ExpectZero()
        {
            var dates = new DateTime[] { aDayAfterHoliday };
            var mockedVehicle = GetNonExemptedVehicle();

            var tax = calculator.GetTax(mockedVehicle, dates);

            Assert.Equal(0, tax);
        }

        [Fact]
        public void MoreThanOnePassWithin60Min_ExpectChargeOnce()
        {
            var dateTime = nonHolidayDateNonExemptedHour;
            var dates = new DateTime[]
            {
                dateTime,
                dateTime.AddMinutes(1),
                dateTime.AddMinutes(2),
                dateTime.AddMinutes(3)
            };
            var mockedVehicle = GetNonExemptedVehicle();

            var tax = calculator.GetTax(mockedVehicle, dates);

            Assert.Equal(8, tax);
        }

        [Fact]
        public void Two_MoreThanOnePassWithin60Min_ExpectChargeTwice()
        {
            var dateTime1 = nonHolidayDateNonExemptedHour;
            var dateTime2 = nonHolidayDateNonExemptedHour2;
            var dates = new DateTime[]
            {
                dateTime1,
                dateTime1.AddMinutes(1),
                dateTime1.AddMinutes(2),
                dateTime1.AddMinutes(3),
                dateTime2,
                dateTime2.AddMinutes(1),
                dateTime2.AddMinutes(2),
                dateTime2.AddMinutes(3)
            };
            var mockedVehicle = GetNonExemptedVehicle();

            var tax = calculator.GetTax(mockedVehicle, dates);

            Assert.Equal(8 + 18, tax);
        }

        [Fact]
        public void MoreThanOnePassWithin60Min_ExpectMaxTaxCharge()
        {
            var dateTime = nonHolidayDateNonExemptedHour2;
            var dates = new DateTime[]
            {
                dateTime,
                dateTime.AddMinutes(-1), //NOTE: This time falls into the previous TimeRule which is 13SEK.
            };
            var mockedVehicle = GetNonExemptedVehicle();

            var tax = calculator.GetTax(mockedVehicle, dates);

            Assert.Equal(18, tax);
        }

        [Fact]
        public void ChargeMoreThan60SekInADay_Expect60()
        {
            var dates = InMemoryDataProvider    //NOTE: Sum of tax charges is 112SEK.
                            .GetTimeRules()
                            .Select(item => nonHolidayDateNonExemptedHour.Date + item.From)
                            .ToArray();
            var mockedVehicle = GetNonExemptedVehicle();

            var tax = calculator.GetTax(mockedVehicle, dates);

            Assert.Equal(60, tax);
        }

        [Fact]
        public void ChargeMoreThan60SekInTwoDays_Expect120()
        {
            var date1 = nonHolidayDateNonExemptedHour;
            var date2 = nonHolidayDateNonExemptedHour.AddDays(1);
            var dates = InMemoryDataProvider    //NOTE: Sum of tax charges is 112SEK for each day.
                            .GetTimeRules()
                            .SelectMany(item => new[] 
                            {
                                date1.Date + item.From ,
                                date2.Date + item.From
                            })
                            .ToArray();
            var mockedVehicle = GetNonExemptedVehicle();

            var tax = calculator.GetTax(mockedVehicle, dates);

            Assert.Equal(120, tax);
        }

        #endregion

        #region Private Methods

        private IVehicle GetExemptedVehicle()
        {
            var mockVehicle = new Mock<IVehicle>();
            mockVehicle.Setup(vehicle => vehicle.GetVehicleType()).Returns(TollFreeVehicles.Buss.ToString());
            return mockVehicle.Object;
        }

        private IVehicle GetNonExemptedVehicle()
        {
            var mockVehicle = new Mock<IVehicle>();
            mockVehicle.Setup(vehicle => vehicle.GetVehicleType()).Returns("Tractor");
            return mockVehicle.Object;
        }

        #endregion
    }
}