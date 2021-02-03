namespace TradeSystem.Json
{
    public enum JsonValueType
    {
        UnknownLiteral = -1,
        Undefined = 0,
        Null,
        Boolean,
        Number,
        String,
        Array,
        Object
    }
}