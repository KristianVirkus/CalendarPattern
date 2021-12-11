using System.Collections.Generic;

namespace CalendarPattern
{
    internal class DebugIterationEventArgs
    {
        public IEnumerable<Calculator.PatternAlternative> Alternatives { get; set; }
        public int ChosenAlternative { get; set; }
    }
}
