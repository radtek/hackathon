namespace XPInc.Hackathon.Framework.Data.Contexts
{
    public interface IDataContextCollection
    {
        T Get<T>() where T : IDataContext;
    }
}
