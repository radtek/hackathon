using System;
using System.Collections.Generic;

namespace XPInc.Hackathon.Framework.Data.Contexts
{
    public sealed class DataContextCollection : IDataContextCollection
    {
        private readonly Dictionary<Type, IDataContext> _contexts = new Dictionary<Type, IDataContext>();

        public IReadOnlyDictionary<Type, IDataContext> Contexts => _contexts;

        public T Get<T>() where T : IDataContext =>
            (T)_contexts[typeof(T)];

        public void AddContext<T>(T context) where T : IDataContext =>
            _contexts.TryAdd(typeof(T), context);
    }
}
