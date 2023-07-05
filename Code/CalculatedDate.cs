using System;

namespace congestion.calculator
{
    internal class CalculatedDate
    {
        #region Constructor

        public CalculatedDate(DateTime dateAndTime, int tax, bool omitted = false)
        {
            DateAndTime = dateAndTime;
            Tax = tax;
            Omitted = omitted;
        }

        #endregion

        #region Fields & Properties

        public DateTime DateAndTime { get; set; }

        public int Tax { get; set; }

        public bool Omitted { get; set; }

        #endregion
    }
}