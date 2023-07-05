using congestion.calculator.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace congestion.calculator.Repository
{
    /// <summary>
    /// This class is temporary "Repository pattern" and only there to mimic
    /// The rule sets and predefined stuff.
    /// Later on, we'll fetch actual data from database.
    /// </summary>
    public static class InMemoryDataProvider
    {
        #region Fields & Properties

        private static IEnumerable<TimeRule> timeRules =
            new List<TimeRule>
            {
                new TimeRule(new TimeSpan(6,0,0), new TimeSpan(6,29,59), 8),
                new TimeRule(new TimeSpan(6,30,0), new TimeSpan(6,59,59), 13),
                new TimeRule(new TimeSpan(7,0,0), new TimeSpan(7,59,59), 18),
                new TimeRule(new TimeSpan(8,0,0), new TimeSpan(8,29,59), 13),
                new TimeRule(new TimeSpan(8,30,0), new TimeSpan(14,59,59), 8),
                new TimeRule(new TimeSpan(15,0,0), new TimeSpan(15,29,59), 13),
                new TimeRule(new TimeSpan(15,30,0), new TimeSpan(16,59,59), 18),
                new TimeRule(new TimeSpan(17,0,0), new TimeSpan(17,59,59), 13),
                new TimeRule(new TimeSpan(18,0,0), new TimeSpan(18,29,59), 8)
            };

        private static IEnumerable<Weekend> weekends =
            new List<Weekend>
            {
                new Weekend(DayOfWeek.Saturday),
                new Weekend(DayOfWeek.Sunday)
            };

        private static IEnumerable<Holiday> holidays =
            new List<Holiday>
            {
                new Holiday(1,1),
                new Holiday(3,28),
                new Holiday(3,29),
                new Holiday(4, 1),
                new Holiday(4, 30),
                new Holiday(5, 1),
                new Holiday(5, 8),
                new Holiday(5, 9),
                new Holiday(6, 5),
                new Holiday(6, 6),
                new Holiday(6, 21),
                new Holiday(11, 1),
                new Holiday(12, 24),
                new Holiday(12, 25),
                new Holiday(12, 26),
                new Holiday(12, 31)
            };

        #endregion

        #region Methods

        /// <summary>
        /// Returns the time rules and cache them.
        /// </summary>
        public static IEnumerable<TimeRule> GetTimeRules()
            => timeRules; //TODO: Implement caching system.

        /// <summary>
        /// Caches and Returns the weekends.
        /// </summary>
        public static IEnumerable<Weekend> GetWeekends()
            => weekends; //TODO: Implement caching system.

        /// <summary>
        /// Caches and Returns the public holidays.
        /// </summary>
        public static IEnumerable<Holiday> GetHolidays()
            => holidays; //TODO: Implement caching system.

        #endregion
    }
}