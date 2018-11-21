namespace Host.Advanced.Data.U2F
{
    public class ServerRegisterResponse
    {
        public string AppId { get; set; }
        public string Challenge { get; set; }
        public string Version { get; set; }
    }
}