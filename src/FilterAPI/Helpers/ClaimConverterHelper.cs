using System.Security.Claims;

namespace FilterAPI.Helpers
{
    public static class ClaimConverterHelper
    {
        private static readonly string[] Fields = ["companyId", "userId"];
        public static Dictionary<string, int> FindValues(ClaimsPrincipal claims)
        {
            var values = new Dictionary<string, int> { };
            foreach (var field in Fields)
            {
                if (claims.FindFirst(field) != null)
                {
                    values.Add(field, Convert.ToInt32(claims.FindFirstValue(field)));
                }
            }
            return values;
        }
    }
}
