namespace conduflex_api.DTOs
{
    public class BaseFilter
    {

        // Using List<FilterValue> causes the query not getting recognized on the request, the filter implementation has to manually convert it
        public string Filters { get; set; }
        public Range Range { get; set; } = new Range() { Start = 0, End = 9 };

        public Sort Sort { get; set; }
    }

    public class FilterValue
    {
        public string Field { get; set; }
        public string Value { get; set; }
        public string Operator { get; set; }
        public AlternativeOperators AlternativeOperator { get; set; } = AlternativeOperators.None;
    }

    public class Range
    {
        public int Start { get; set; }
        public int End { get; set; }
    }

    public class Sort
    {
        public string Field { get; set; }
        public bool IsAscending { get; set; } = true;
    }

    public enum AlternativeOperators { None, StringContains, DateTime }
}
