using System;

namespace Host.Advanced.Models.U2F
{
    public class DeviceModel
    {
        public int Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public string KeyHandle { get; set; }

        public int Counter { get; set; }

        public string DeviceName { get; set; }    }
}