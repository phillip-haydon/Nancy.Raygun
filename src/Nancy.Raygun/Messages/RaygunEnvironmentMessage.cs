using System.Collections.Generic;
using System.IO;

//using System.Windows.Forms;

//using Microsoft.VisualBasic.Devices;

namespace Nancy.Raygun.Messages
{
    public class RaygunEnvironmentMessage
    {
        private List<double> _diskSpaceFree = new List<double>();

        //public RaygunEnvironmentMessage()
        //{
        //  ProcessorCount = Environment.ProcessorCount;

        //  OSVersion = Environment.OSVersion.VersionString;
        //  Architecture = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
        //  Cpu = Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER");
        //  WindowBoundsWidth = SystemInformation.VirtualScreen.Height;
        //  WindowBoundsHeight = SystemInformation.VirtualScreen.Width;      
        //  ComputerInfo info = new ComputerInfo();
        //  TotalPhysicalMemory = (ulong)info.TotalPhysicalMemory / 0x100000; // in MB
        //  AvailablePhysicalMemory = (ulong)info.AvailablePhysicalMemory / 0x100000;
        //  TotalVirtualMemory = info.TotalVirtualMemory / 0x100000;
        //  AvailableVirtualMemory = info.AvailableVirtualMemory / 0x100000;

        //  Location = CultureInfo.CurrentCulture.DisplayName;
        //  OSVersion = info.OSVersion;
        //  GetDiskSpace();
        //  //GetCpu();
        //}

        //private void GetCpu()
        //{
        //  // This introduces a ~0.5s delay into message creation so is disabled above, but produces nicer cpu names
        //  // (ie. Intel Core i5-3570k @ 3.40ghz)
        //  ManagementClass wmiManagementProcessorClass = new ManagementClass("Win32_Processor");
        //  ManagementObjectCollection wmiProcessorCollection = wmiManagementProcessorClass.GetInstances();      
        //  foreach (ManagementObject wmiProcessorObject in wmiProcessorCollection)
        //  {
        //    try
        //    {
        //      Cpu = wmiProcessorObject.Properties["Name"].Value.ToString();
        //    }
        //    catch (ManagementException)
        //    {          
        //    }
        //  }
        //}

        public int ProcessorCount { get; private set; }

        public string OSVersion { get; private set; }

        public double WindowBoundsWidth { get; private set; }

        public double WindowBoundsHeight { get; private set; }

        public string ResolutionScale { get; private set; }

        public string CurrentOrientation { get; private set; }

        public string Cpu { get; private set; }

        public string PackageVersion { get; private set; }

        public string Architecture { get; private set; }

        public string Location { get; private set; }

        public ulong TotalPhysicalMemory { get; private set; }

        public ulong AvailablePhysicalMemory { get; private set; }

        public ulong TotalVirtualMemory { get; set; }

        public ulong AvailableVirtualMemory { get; set; }

        public List<double> DiskSpaceFree
        {
            get { return _diskSpaceFree; }
            set { _diskSpaceFree = value; }
        }

        public string DeviceName { get; private set; }

        private void GetDiskSpace()
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady)
                {
                    DiskSpaceFree.Add((double) drive.AvailableFreeSpace/0x40000000); // in GB
                }
            }
        }
    }
}