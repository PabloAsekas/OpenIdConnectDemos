using System.Collections.Generic;

namespace Host.Advanced.Configuration
{
    public static class Providers
    {
        public static Dictionary<string, string> Icons { get; set; }=new Dictionary<string, string>
        {
            {"google", "/images/google.png"},
            {"microsoft", "/images/microsoft.png"}
        };
    }
}