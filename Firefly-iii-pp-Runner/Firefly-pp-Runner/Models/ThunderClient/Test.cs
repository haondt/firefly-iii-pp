using Firefly_iii_pp_Runner.Models.ThunderClient.Enums;

namespace Firefly_iii_pp_Runner.Models.ThunderClient
{
    public class Test : IEquatable<Test>
    {
        public TestTypeEnum Type { get; set; }
        public string Value { get; set; } = string.Empty;
        public TestActionEnum Action { get; set; }
        public string Custom { get; set; } = string.Empty;

        public bool Equals(Test? other)
        {
            return other?.GetHashCode() == this.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            return obj is Test test && Equals(test);
        }

        public override int GetHashCode()
        {
            return (Type, Value, Action, Custom).GetHashCode();
        }
    }

}
