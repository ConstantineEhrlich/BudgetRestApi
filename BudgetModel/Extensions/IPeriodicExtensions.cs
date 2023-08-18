using BudgetModel.Interfaces;

namespace BudgetModel.Extensions
{
    public static class IPeriodicExtensions
    {
        public static IEnumerable<T> UpToPeriod<T>(this IEnumerable<T> input, int toYear, int toPeriod) where T: IPeriodic
        {
            return input.Where(el => el.Year < toYear ||
                                     (el.Year == toYear && el.Period <= toPeriod));
        }

        public static IEnumerable<T> ForPeriod<T>(this IEnumerable<T> input, int forYear, int forPeriod) where T : IPeriodic
        {
            return input.Where(el => el.Year == forYear && el.Period == forPeriod);
        }

        public static IEnumerable<T> StartingFrom<T>(this IEnumerable<T> input, int fromYear, int fromPeriod) where T : IPeriodic
        {
            return input.Where(el => (el.Year == fromYear &&  el.Period >= fromPeriod)  ||
                                     (el.Year > fromYear));
        }


        public static IEnumerable<T> Between<T>(this IEnumerable<T> input, int fromYear, int fromPeriod, int toYear, int toPeriod) where T : IPeriodic
        {
            ValidateInput(fromYear, fromPeriod, toYear, toPeriod);
            return input.Where(el =>
                (el.Year > fromYear || el.Year == fromYear && el.Period >= fromPeriod) &&
                (el.Year < toYear || el.Year == toYear && el.Period <= toPeriod));
        }
        
        private static void ValidateInput(int fromYear, int fromPeriod, int toYear, int toPeriod)
        {
            if (fromYear > toYear)
                throw new ArgumentOutOfRangeException(nameof(fromYear), $"Start year ({fromYear}) is bigger than end year ({toYear})");
            else if (fromYear == toYear && fromPeriod > toPeriod)
                throw new ArgumentOutOfRangeException(nameof(fromPeriod), $"Start period ({fromPeriod}) is bigger than end period ({toPeriod})");
        }
    }
}