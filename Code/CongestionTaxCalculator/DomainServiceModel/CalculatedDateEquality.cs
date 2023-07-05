using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace congestion.calculator.DomainServiceModel
{
    internal class CalculatedDateEquality : IEqualityComparer<CalculatedDate>
    {
        #region Fields & Properties

        private readonly int equalRangeMinutes;

        #endregion

        #region Constructor

        public CalculatedDateEquality(int equalRangeMinutes)
        {
            this.equalRangeMinutes = equalRangeMinutes;
        }

        #endregion

        #region IEqualityComparer implementation

        public bool Equals([AllowNull] CalculatedDate x, [AllowNull] CalculatedDate y)
        {
            return !x.Omitted &&
                   !y.Omitted &&
                   DateFallsInTheRange(x.DateAndTime, equalRangeMinutes, y.DateAndTime);
        }

        public int GetHashCode([DisallowNull] CalculatedDate obj)
        {
            return 0;
        }

        #endregion

        #region Methods

        private bool DateFallsInTheRange(DateTime seedDate, int minuteRange, DateTime date)
        {
            return date >= seedDate.AddMinutes(-minuteRange) &&
                    date < seedDate.AddMinutes(minuteRange);
        }

        #endregion
    }
}