using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using BBS.TracerX;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers
{
    public static class ProcessInfoHelper
    {
        private static Logger _logX = Logger.GetLogger("ProcessInfoHelper");

        private static List<PerformanceCounter> _counters = new List<PerformanceCounter>();
        private static object _lockCounters = new object();
        private const long MEG = (1 << 20);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private class MEMORYSTATUSEX
        {
            public UInt32 dwLength;
            public UInt32 dwMemoryLoad;
            public UInt64 ullTotalPhys;
            public UInt64 ullAvailPhys;
            public UInt64 ullTotalPageFile;
            public UInt64 ullAvailPageFile;
            public UInt64 ullTotalVirtual;
            public UInt64 ullAvailVirtual;
            public UInt64 ullAvailExtendedVirtual;
            public MEMORYSTATUSEX()
            {
                this.dwLength = (UInt32)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
            }
        }
        static ProcessInfoHelper()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            string name = Process.GetCurrentProcess().ProcessName;
            AddPerformanceCounter("Process", "Private Bytes", name);
            AddPerformanceCounter("Process", "Working Set", name);
            AddPerformanceCounter(".NET CLR Memory", "% Time in GC", name);
            AddPerformanceCounter(".NET CLR Memory", "# Bytes in all Heaps", name);
            AddPerformanceCounter(".NET CLR Memory", "# Gen 0 Collections", name);
            AddPerformanceCounter(".NET CLR Memory", "# Gen 1 Collections", name);
            AddPerformanceCounter(".NET CLR Memory", "# Gen 2 Collections", name);
            AddPerformanceCounter(".NET CLR Memory", "# of Pinned Objects", name);
            AddPerformanceCounter(".NET CLR Memory", "# GC Handles", name);
            AddPerformanceCounter(".NET CLR Memory", "# Total committed Bytes", name);
            AddPerformanceCounter(".NET CLR Memory", "# Total reserved Bytes", name);
            AddPerformanceCounter(".NET CLR Memory", "Finalization Survivors", name);
            AddPerformanceCounter(".NET CLR Memory", "Gen 0 Heap Size", name);
            AddPerformanceCounter(".NET CLR Memory", "Gen 0 Promoted Bytes/Sec", name);
            AddPerformanceCounter(".NET CLR Memory", "Gen 1 Heap Size", name);
            AddPerformanceCounter(".NET CLR Memory", "Gen 1 Promoted Bytes/Sec", name);
            AddPerformanceCounter(".NET CLR Memory", "Gen 2 Heap Size", name);
            AddPerformanceCounter(".NET CLR Memory", "Promoted Finalization-Memory from Gen 0", name);
            AddPerformanceCounter(".NET CLR Memory", "Promoted Memory from Gen 0", name);
            AddPerformanceCounter(".NET CLR Memory", "Promoted Memory from Gen 1", name);
            AddPerformanceCounter(".NET CLR Memory", "Large Object Heap Size", name);
            AddPerformanceCounter(".NET CLR Exceptions", "# of Exceps Thrown / sec", name);
            AddPerformanceCounter(".NET CLR LocksAndThreads", "Contention Rate / sec", name);
            AddPerformanceCounter(".NET CLR LocksAndThreads", "Current Queue Length", name);
            AddPerformanceCounter(".NET CLR LocksAndThreads", "# of current physical Threads", name);
        }

        private static void AddPerformanceCounter(string category, string name, string instance)
        {
            try
            {
                _counters.Add(new PerformanceCounter(category, name, instance, true));
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log(string.Format("AddPerformanceCounter({0}, {1}, {2})", category, name, instance), ex);
            }
        }

        public static void Log()
        {
            Log(_logX);
        }
        public static void Log(Logger l)
        {
            if (null == l) { l = _logX; }
            if (null == l) { return; }
            using (l.DebugCall(string.Format("ProcessInfoHelper.Log(Machine={0}, UserDomain={1}, UserName={2})", Environment.MachineName, Environment.UserDomainName, Environment.UserName)))
            {
                try
                {
                    Process p = System.Diagnostics.Process.GetCurrentProcess();
                    l.DebugFormat("BasePriority={0}", p.BasePriority);
                    l.DebugFormat("HandleCount={0}", p.HandleCount);
                    l.DebugFormat("Id={0}", p.Id);
                    l.DebugFormat("MaxWorkingSet={0} ({1})", p.MaxWorkingSet, FormatHelper.FormatBytes(p.MaxWorkingSet));
                    l.DebugFormat("MinWorkingSet={0} ({1})", p.MinWorkingSet, FormatHelper.FormatBytes(p.MinWorkingSet));
                    l.DebugFormat("NonpagedSystemMemorySize64={0} ({1})", p.NonpagedSystemMemorySize64, FormatHelper.FormatBytes(p.NonpagedSystemMemorySize64));
                    l.DebugFormat("PagedMemorySize64={0} ({1})", p.PagedMemorySize64, FormatHelper.FormatBytes(p.PagedMemorySize64));
                    l.DebugFormat("PagedSystemMemorySize64={0} ({1})", p.PagedSystemMemorySize64, FormatHelper.FormatBytes(p.PagedSystemMemorySize64));
                    l.DebugFormat("PeakPagedMemorySize64={0} ({1})", p.PeakPagedMemorySize64, FormatHelper.FormatBytes(p.PeakPagedMemorySize64));
                    l.DebugFormat("PeakVirtualMemorySize64={0} ({1})", p.PeakVirtualMemorySize64, FormatHelper.FormatBytes(p.PeakVirtualMemorySize64));
                    l.DebugFormat("PeakWorkingSet64={0} ({1})", p.PeakWorkingSet64, FormatHelper.FormatBytes(p.PeakWorkingSet64));
                    l.DebugFormat("PrivateMemorySize64={0} ({1})", p.PrivateMemorySize64, FormatHelper.FormatBytes(p.PrivateMemorySize64));
                    l.DebugFormat("PrivilegedProcessorTime={0}", p.PrivilegedProcessorTime);
                    l.DebugFormat("ProcessorAffinity={0}", p.ProcessorAffinity);
                    l.DebugFormat("StartTime={0}", p.StartTime);
                    l.DebugFormat("TotalProcessorTime={0}", p.TotalProcessorTime);
                    l.DebugFormat("UserProcessorTime={0}", p.UserProcessorTime);
                    l.DebugFormat("VirtualMemorySize64={0} ({1})", p.VirtualMemorySize64, FormatHelper.FormatBytes(p.VirtualMemorySize64));
                    l.DebugFormat("WorkingSet64={0} ({1})", p.WorkingSet64, FormatHelper.FormatBytes(p.WorkingSet64));
                    l.DebugFormat("ProcessorCount={0}", Environment.ProcessorCount);
                }
                catch (Exception ex)
                {
                    l.Error("ProcessInfoHelper.Log() Exception:", ex);
                }
                LogGlobal(l);
                LogPerfCounters(l);
            }
        }

        public static void LogPerfCounters(Logger l)
        {
            if (null == l) { l = _logX; }
            if (null == l) { return; }
            using (l.DebugCall("ProcessInfoHelper.LogPerfCounters()"))
            {
                try
                {
                    lock (_lockCounters)
                    {
                        foreach (var c in _counters)
                        {
                            l.DebugFormat("{0} = {1}", c.CounterName, GetFormattedCounterValue(c));
                        }
                    }
                }
                catch (Exception ex)
                {
                    l.Error("ProcessInfoHelper.LogPerfCounters() Exception:", ex);
                }
            }
        }

        private static string GetFormattedCounterValue(PerformanceCounter c)
        {
            if (null == c) return (string.Empty);
            try 
            {
                float f = c.NextValue();
                if (f > MEG) return (string.Format("{0:####0 MB}", f / MEG));
                return (f.ToString());
            } catch(Exception ex) {return(ex.Message);}
        }

        public static void LogGlobal(Logger l)
        {
            if (null == l) { l = _logX; }
            if (null == l) { return; }
            using (l.DebugCall("ProcessInfoHelper.LogGlobal() MEMORYSTATUSEX"))
            {
                try
                {
                    var ms = new MEMORYSTATUSEX();
                    if (GlobalMemoryStatusEx(ms))
                    {
                        l.DebugFormat("dwLength={0} ({1})", ms.dwLength, FormatHelper.FormatBytes(ms.dwLength));
                        l.DebugFormat("dwMemoryLoad={0} ({1})", ms.dwMemoryLoad, FormatHelper.FormatBytes(ms.dwMemoryLoad));
                        l.DebugFormat("ullTotalPhys={0} ({1})", ms.ullTotalPhys, FormatHelper.FormatBytes(ms.ullTotalPhys));
                        l.DebugFormat("ullAvailPhys={0} ({1})", ms.ullAvailPhys, FormatHelper.FormatBytes(ms.ullAvailPhys));
                        l.DebugFormat("ullTotalPageFile={0} ({1})", ms.ullTotalPageFile, FormatHelper.FormatBytes(ms.ullTotalPageFile));
                        l.DebugFormat("ullAvailPageFile={0} ({1})", ms.ullAvailPageFile, FormatHelper.FormatBytes(ms.ullAvailPageFile));
                        l.DebugFormat("ullTotalVirtual={0} ({1})", ms.ullTotalVirtual, FormatHelper.FormatBytes(ms.ullTotalVirtual));
                        l.DebugFormat("ullAvailVirtual={0} ({1})", ms.ullAvailVirtual, FormatHelper.FormatBytes(ms.ullAvailVirtual));
                        l.DebugFormat("ullAvailExtendedVirtual={0} ({1})", ms.ullAvailExtendedVirtual, FormatHelper.FormatBytes(ms.ullAvailExtendedVirtual));
                    }
                    else
                    {
                        l.Error("GlobalMemoryStatusEx() Failed!");
                        throw new Win32Exception(Marshal.GetHRForLastWin32Error());
                    }
                }
                catch (Exception ex)
                {
                    l.Error("ProcessInfoHelper.LogGlobal() Exception:", ex);
                }
            }
        }
        static public string GetTracerXConfig()
        {
            try
            {
                string s = Path.Combine(GetAppPath(), "TracerX.xml");
                if (File.Exists(s)) return (s);
            }
            catch { }
            return ("TracerX.xml");
        }
        static public string GetAppPath()
        {
            var assembly = Assembly.GetExecutingAssembly();

            var appPath = string.Empty;

            if (null != assembly)
            {
                appPath = Path.GetDirectoryName(assembly.Location);
            }

            if (string.IsNullOrEmpty(appPath))
            {
                var args = Environment.GetCommandLineArgs();

                if (args.Length > 0)
                {
                    appPath = Path.GetDirectoryName(args[0]);
                }
            }

            return (appPath);
        }
    }
}
