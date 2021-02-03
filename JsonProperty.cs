#region Usings



#endregion

namespace TradeSystem.Json
{
    public readonly struct JsonProperty
    {
        #region Properties

        public string Name { get; }
        public JsonValue Value { get; }

        #endregion

        #region Constructors

        public JsonProperty(string name, JsonValue value)
        {
            Name = name;
            Value = value;
        }

        #endregion

        #region Methods

        public override string ToString() => Name == null ? base.ToString() : $"{JsonValue.ToJsonString(Name)}:{Value}";

        #endregion
    }
}
