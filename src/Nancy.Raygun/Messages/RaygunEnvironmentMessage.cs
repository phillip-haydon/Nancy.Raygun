namespace Nancy.Raygun.Messages
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Security;

    public class RaygunEnvironmentMessage
    {
        private List<double> _diskSpaceFree = new List<double>();

        public RaygunEnvironmentMessage()
        {
            // Most information about the environment is omitted because it requires
            // dependencies on Microsoft.Win32 or Microsoft.VisualBasic.Devices
            // since Nancy.Raygun is used for Web Sites, you probably know most of this info anyway

            ProcessorCount = Environment.ProcessorCount;
            Locale = CultureInfo.CurrentCulture.DisplayName;
            DateTime now = DateTime.Now;
            UtcOffset = TimeZone.CurrentTimeZone.GetUtcOffset(now).TotalHours;
            OSVersion = Environment.OSVersion.VersionString;

            try
            {
                Architecture = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
                GetDiskSpace();
            }
            catch (SecurityException)
            {
                System.Diagnostics.Trace.WriteLine("RaygunClient error: couldn't access environment variables. If you are running in Medium Trust, in web.config in RaygunSettings set mediumtrust=\"true\"");
            }
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
        public string Locale { get; private set; }
        public double UtcOffset { get; private set; }

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