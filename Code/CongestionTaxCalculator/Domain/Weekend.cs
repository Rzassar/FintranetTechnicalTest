using System;

namespace congestion.calculator.Domain
{
    //NOTE: This class is a good candidate for being "Value-Object" in DDD approach.
    public class Weekend
    {
        #region Constructor

        public Weekend(DayOfWeek day)
        {
            DayOfWeek = day;
        }

        #endregion

        #region Fields & Properties

        public DayOfWeek DayOfWeek { get; set; }

        #endregion
    }
}