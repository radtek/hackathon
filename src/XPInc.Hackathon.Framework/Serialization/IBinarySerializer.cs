namespace XPInc.Hackathon.Framework.Serialization
{
    public interface IBinarySerializer
    {
        TTarget FromBinary<TTarget>(byte[] bytes)
            where TTarget : class;

        byte[] ToBinary<TTarget>(TTarget input)
            where TTarget : class;
    }
}
