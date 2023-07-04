namespace congestion.calculator.Domain
{
    public class Holiday
    {
        #region Constructor

        public Holiday(int month, int dayOfMonth)
        {
            Month = month;
            DayOfMonth = dayOfMonth;
        }

        #endregion

        #region Fields & Properties

        public int Month { get; set; }

        public int DayOfMonth { get; set; }

        #endregion
    }
}
