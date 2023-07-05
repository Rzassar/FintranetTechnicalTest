using System;

namespace congestion.calculator.Domain
{
    public class TimeRule
    {
        #region Constructor

        public TimeRule(TimeSpan from, TimeSpan to, int taxCharge)
        {
            //TODO: "from" should be less than or equal to "to".

            From = from;
            To = to;
            TaxCharge = taxCharge;
        }

        #endregion

        #region Fields & Properties

        public TimeSpan From { get; set; }

        public TimeSpan To { get; set; }

        public int TaxCharge { get; set; }

        #endregion
    }
}