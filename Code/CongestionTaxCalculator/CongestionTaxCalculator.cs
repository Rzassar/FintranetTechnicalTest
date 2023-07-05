using congestion.calculator;
using congestion.calculator.Domain;
using congestion.calculator.DomainServiceModel;
using congestion.calculator.Repository;
using System;
using System.Linq;

namespace CongestionTaxCalculator
{
    public class CongestionTaxCalculator
    {
        #region Fields & Properties

        //TODO: Remove this hardcoded rule from here
        //      and push it into rule-set.
        private const int JULY_EXEMPTION = 7;

        //TODO: Remove this hardcoded rule from here
        //      and push it into rule-set.
        private const int SINGLE_CHARGE_RULE_MINUTES = 60;

        //TODO: Remove this hardcoded rule from here
        //      and push it into rule-set.
        private const int MAX_CHARGE_RULE_PER_DAY_AMOUNT = 60;

        private const int ONE_DAY_TIME_SPAN_MINUTES = 24 * 60;

        #endregion


        /// <summary>
        ///Calculates the total toll fee for the given dates.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="dates"></param>
        /// <returns></returns>
        public int GetTax(IVehicle vehicle, DateTime[] dates)
        {
            //NOTE: Calculate all passes.
            return dates
                    .OrderBy(date => date)
                    .Select(date => new CalculatedDate(date, GetTollFee(date, vehicle)))
                    .GroupBy(date => date.DateAndTime.Date) //NOTE: Each day is calculated separately.
                    .Sum(dateGroupItem =>
                    {
                        var dailyTax = dateGroupItem.GroupJoin(dateGroupItem, //NOTE: Each 60mins falls into a same range.
                                    outer => outer,
                                    inner => inner,
                                    (outerItem, innerList) =>
                                    {
                                        if (!innerList.Any())
                                            return 0;

                                        foreach (var item in innerList)
                                            item.Omitted = true;

                                        return innerList.Max(item => item.Tax);
                                    },
                                    new CalculatedDateEquality(SINGLE_CHARGE_RULE_MINUTES))
                        .Sum();

                        return Math.Min(dailyTax, MAX_CHARGE_RULE_PER_DAY_AMOUNT);
                    });
        }

        private bool IsTollFreeVehicle(IVehicle vehicle)
        {
            if (vehicle == null)
                return false;

            String vehicleType = vehicle.GetVehicleType();
            return Enum.TryParse(typeof(TollFreeVehicles), vehicleType, out _);
        }

        //NOTE: Although it is better to define the return value as decimal (for currency),
        //      however, I'm not gonna change it as it is a presumption of the code-challenge.
        private int GetTollFee(DateTime date, IVehicle vehicle)
        {
            if (IsTollFreeDate(date) || IsTollFreeVehicle(vehicle))
                return 0;

            //NOTE: Instead of hardcoding the time-spans, we make use of a
            //      list of fixed objects (a.k.a., table like format).
            //      Later on, we can easily change it into a parameterized fashion.

            return InMemoryDataProvider
                        .GetTimeRules()
                        .FirstOrDefault(item => date.TimeOfDay >= item.From && date.TimeOfDay <= item.To)
                        ?.TaxCharge ?? 0;
        }

        private bool IsTollFreeDate(DateTime date)
        {
            //NOTE: Instead of hardcoding the holidays and weekends, we make use of a
            //      list of fixed objects (a.k.a., table like format).
            //      Later on, we can easily change it into a parameterized fashion.
            //      There are different weekends for some countries (e.g., Iran),
            //      or different holidays for cities (e.g., Vancouver vs Toronto).

            //IMPORTANT: It is crucial to understand that:
            //      "July exemption" is not just a holiday but a rule.
            //      Thus, we need to act it differently.

            return
                InMemoryDataProvider.GetWeekends().Any(item => item.DayOfWeek == date.DayOfWeek) ||
                InMemoryDataProvider.GetHolidays().Any(item => IsInTheOneDayRange(item, date)) ||
                date.Month == JULY_EXEMPTION;
        }

        private bool IsInTheOneDayRange(Holiday holiday, DateTime date)
        {
            //NOTE: As DateTime is a struct, they are meant to be ephemeral
            //      and fast/easy to create and deallocate on the stack memory.
            //      So we won't experience any performance degradation here.
            var currentYearHoliday = new DateTime(date.Year, holiday.Month, holiday.DayOfMonth);
            return date >= currentYearHoliday.AddDays(-1) && date <= currentYearHoliday.AddDays(1);
        }
    }
}