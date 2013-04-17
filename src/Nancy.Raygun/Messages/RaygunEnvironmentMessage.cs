using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Nancy.Raygun.Messages
{
    public class RaygunEnvironmentMessage
    {
        private List<double> _diskSpaceFree = new List<double>();

        public RaygunEnvironmentMessage()
        {
            // Most information about the environment is omitted because it requires
            // dependencies on Microsoft.Win32 or Microsoft.VisualBasic.Devices
            // since Nancy.Raygun is used for Web Sites, you probably know most of this info anyway

            ProcessorCount = Environment.ProcessorCount;

            OSVersion = Environment.OSVersion.VersionString;
            Architecture = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
            Location = CultureInfo.CurrentCulture.DisplayName;

            GetDiskSpace();
        }

        private void GetDiskSpace()
        {
            foreach (var drive in DriveInfo.GetDrives().Where(drive => drive.IsReady))
            {
                DiskSpaceFree.Add((double)drive.AvailableFreeSpace / 0x40000000); // in GB
            }
        }

        public int ProcessorCount { get; private set; }
        public string OSVersion { get; private set; }
        public string Architecture { get; private set; }
        public string Location { get; private set; }

        public List<double> DiskSpaceFree
        {
            get
            {
                return _diskSpaceFree;
            }
            set { _diskSpaceFree = value; }
        }
    }
}