namespace FileMakerConnect
{
    public enum Condition
    {
        Equals,
        NotEquals,
        GreaterThan,
        GreaterThanOrEqualTo,
        LessThan,
        LessThanOrEqualTo,
        IsNull,
        IsNotNull,
        WithoutValue = IsNull|IsNotNull
    }

    public enum Operator
    {
        And,
        Or,
        None
    }

    public enum SortOrder
    {
        Ascending,
        Descending
    }
}