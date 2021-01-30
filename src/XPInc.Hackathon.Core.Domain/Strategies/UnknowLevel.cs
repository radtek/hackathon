namespace XPInc.Hackathon.Core.Domain.Strategies
{
    public sealed class UnknowLevel : Level
    {
        public override string Code => "N/A";

        public override string Name => "Not Classified";

        public override string Color => "#FFF";
    }
}