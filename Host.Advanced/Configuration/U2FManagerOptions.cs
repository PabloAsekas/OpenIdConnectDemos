using System;

namespace Host.Advanced.Configuration
{
    public class U2FManagerOptions
    {
        public Uri AppUri { get; set; }

        public string SafeAppUri => AppUri.ToString().TrimEnd('/');
    }
}