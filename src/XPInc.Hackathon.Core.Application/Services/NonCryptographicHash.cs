using System.Text;
using HashDepot;

namespace XPInc.Hackathon.Core.Application.Services
{
    public static class NonCryptographicHash
    {
        public static string Create(string text)
        {
            if (text == default || text.Length == 0)
            {
                return default;
            }

            return XXHash.Hash64(Encoding.UTF8.GetBytes(text))
                            .ToString();
        }
    }
}
