using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Idera.SQLdm.PrescriptiveAnalyzer.Common;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Cache
{
    public class CacheFile : IDisposable
    {
        private static BBS.TracerX.Logger _logX = BBS.TracerX.Logger.GetLogger("CacheFile");

        private const uint GENERIC_READ = (uint)0x80000000;
        private const uint GENERIC_WRITE = (uint)0x40000000;
        private const uint OPEN_ALWAYS = 4;

        private static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
        private static readonly IntPtr NULL_HANDLE = IntPtr.Zero;
        private static readonly uint _dwAllocationGranularity;

        private IntPtr _fileHandle = INVALID_HANDLE_VALUE;
        private IntPtr _mapping = IntPtr.Zero;

        private IntPtr _viewAddress = IntPtr.Zero;
        private long _viewOffset = 0;
        private long _viewLength = 0;

        private long _eof = 0;

        private long _minSizeMB;
        private long _maxSizeMB;
        private long _growSizeMB;
        private long _currentSize;

        private long _writes = 0;
        private long _emptyWrites = 0;
        private long _fileExtended = 0;
        private bool _fileIsFull = false;

        private string _filename = string.Empty;

        private int _defaultViewSize = unchecked((int)(25 * MEG));

        private object _lock = new object();

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateFile(String lpFileName, uint dwDesiredAccess, uint dwShareMode,IntPtr lpSecurityAttributes, uint dwCreationDisposition,uint dwFlagsAndAttributes, IntPtr hTemplateFile);
        [DllImport("kernel32", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr handle);

        [DllImport("kernel32.dll")]
        public static extern void GetSystemInfo([MarshalAs(UnmanagedType.Struct)] ref SYSTEM_INFO lpSystemInfo);

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateFileMapping(IntPtr hFile, IntPtr lpAttributes, uint flProtect,uint dwMaximumSizeHigh, uint dwMaximumSizeLow,String lpName);
        [DllImport("kernel32", SetLastError = true)]
        public static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject, uint dwDesiredAccess, uint dwFileOffsetHigh,uint dwFileOffsetLow, IntPtr dwNumBytesToMap);
        [DllImport("kernel32", SetLastError = true)]
        public static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetDiskFreeSpaceEx(string lpDirectoryName, out ulong lpFreeBytesAvailable, out ulong lpTotalNumberOfBytes, out ulong lpTotalNumberOfFreeBytes);

        private const long _dw = 0x00000000ffffffff;
        private UInt32 HiDWord(long l) { return (unchecked((UInt32)((l >> 32) & _dw))); }
        private UInt32 LoDWord(long l) { return (unchecked((UInt32)(l & _dw))); }

        private const long MEG = (1 << 20);

        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEM_INFO
        {
            internal _PROCESSOR_INFO_UNION uProcessorInfo;
            public uint dwPageSize;
            public IntPtr lpMinimumApplicationAddress;
            public IntPtr lpMaximumApplicationAddress;
            public IntPtr dwActiveProcessorMask;
            public uint dwNumberOfProcessors;
            public uint dwProcessorType;
            public uint dwAllocationGranularity;
            public ushort dwProcessorLevel;
            public ushort dwProcessorRevision;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct _PROCESSOR_INFO_UNION
        {
            [FieldOffset(0)]
            internal uint dwOemId;
            [FieldOffset(0)]
            internal ushort wProcessorArchitecture;
            [FieldOffset(2)]
            internal ushort wReserved;
        }

        static CacheFile() 
        {
            SYSTEM_INFO sysinfo = new SYSTEM_INFO();
            GetSystemInfo(ref sysinfo);
            _dwAllocationGranularity = sysinfo.dwAllocationGranularity;
        }

        private CacheFile() { }
        ~CacheFile() { Dispose(false); }

        public CacheFile(string fileName, int minSizeMB, int maxSizeMB, int growSizeMB) 
        {
            using (_logX.DebugCall(string.Format("CacheFile(File:{0}, Min:{1}, Max:{2}, Grow:{3})", fileName, minSizeMB, maxSizeMB, growSizeMB)))
            {
                _minSizeMB = minSizeMB;
                _maxSizeMB = maxSizeMB;
                _growSizeMB = growSizeMB;

                if (_minSizeMB < 1) _minSizeMB = 1;
                if (_maxSizeMB < 1) _maxSizeMB = 1;

                _fileHandle = INVALID_HANDLE_VALUE;
                _filename = fileName;
                if (!string.IsNullOrEmpty(fileName))
                {
                    if (File.Exists(fileName))
                    {
                        _logX.Debug("Delete existing cache file.");
                        File.Delete(fileName);
                    }
                    _fileHandle = CreateFile(fileName, GENERIC_READ | GENERIC_WRITE, 0, IntPtr.Zero, OPEN_ALWAYS, 0, IntPtr.Zero);
                    if (_fileHandle == INVALID_HANDLE_VALUE)
                    {
                        var ex = new Win32Exception(Marshal.GetHRForLastWin32Error());
                        ExceptionLogger.Log(_logX, string.Format("CreateFile({0}) failed!", fileName), ex);
                        Debug.Assert(false, ex.Message);
                        throw ex;
                    }
                }

                long sizeInBytes = MEG * _minSizeMB;
                if (_defaultViewSize > sizeInBytes) _defaultViewSize = unchecked((int)sizeInBytes);
                _logX.DebugFormat("Create file mapping size {0}", sizeInBytes);
                _mapping = CreateFileMapping(_fileHandle, IntPtr.Zero, 0x04, HiDWord(sizeInBytes), LoDWord(sizeInBytes), null);

                if (_mapping == NULL_HANDLE)
                {
                    CloseAll();
                    var ex = new Win32Exception(Marshal.GetHRForLastWin32Error());
                    ExceptionLogger.Log(_logX, "CreateFileMapping() failed!", ex);
                    throw ex;
                }
                _currentSize = sizeInBytes;
            }
        }

        public bool Mapped{get { return (_mapping != NULL_HANDLE); }}

        private long EmptyWrite() { ++_emptyWrites; return (-1); }

        private bool MapView(long offset, long length)
        {
            if (!Mapped) throw new ApplicationException("No open file mapping!");
            if (_fileIsFull && ((length + offset) > _currentSize))
            {
                return (false);
            }

            //---------------------------------------------------------
            // if we already have a map view that covers the range, exit
            //
            if ((_viewOffset <= offset) && ((offset + length) <= (_viewOffset + _viewLength))) return (true);

            //---------------------------------------------------------
            //  Account for allocation granularity 
            //
            //  msdn:  They must also match the memory allocation granularity of the system. That is, 
            //         the offset must be a multiple of the allocation granularity. To obtain the memory 
            //         allocation granularity of the system, use the GetSystemInfo function, which fills 
            //         in the members of a SYSTEM_INFO structure.
            long adjust = offset % _dwAllocationGranularity;
            if (adjust > 0)
            {
                offset -= adjust;
                length += _dwAllocationGranularity;
            }

            if (length < _defaultViewSize)
            {
                if ((offset + _defaultViewSize) <= _currentSize)
                {
                    length = _defaultViewSize;
                }
            }
            _logX.DebugFormat("Map view of file size {0}", length);

            //---------------------------------------------------------
            // Throws OverflowException if (a) this is a 32-bit platform AND (b) size is out of bounds (ie. int bounds) with respect to this platform
            IntPtr mapSize = new IntPtr(length);

            UnMapView();
            if ((length + offset) > _currentSize)
            {
                ExtendFile(length + offset);
                if (_fileIsFull && ((length + offset) > _currentSize))
                {
                    return (false);
                }
            }
            _logX.Debug("Mapping view of file...");
            _viewAddress = MapViewOfFile(_mapping, 0x001f, HiDWord(offset), LoDWord(offset), mapSize);

            if (IntPtr.Zero == _viewAddress)
            {
                var ex = new Win32Exception(Marshal.GetHRForLastWin32Error());
                ExceptionLogger.Log(_logX, "MapView() failed!", ex);
                throw ex;
            }

            _logX.Debug("Map view of file complete.");
            _viewOffset = offset;
            _viewLength = length;
            return (true);
        }

        private void ExtendFile(long neededSize)
        {
            long maxSize = _maxSizeMB * MEG;
            long growSize = _growSizeMB * MEG;
            if (_currentSize >= maxSize)
            {
                _fileIsFull = true;
                return;
            }
            if (IsDriveFull())
            {
                _fileIsFull = true;
                return;
            }
            UnMapView();
            //if (INVALID_HANDLE_VALUE != _fileHandle) CloseHandle(_fileHandle);
            if (NULL_HANDLE != _mapping) CloseHandle(_mapping);
            _mapping = NULL_HANDLE;

            long sizeInBytes = _currentSize + growSize;
            if (sizeInBytes < neededSize) 
            {
                _logX.DebugFormat("Default growth of {0} increases to {1} which is not large enough for needed size of {2} ", growSize, sizeInBytes, neededSize);
                int paranoid = 0;
                while (sizeInBytes < neededSize)
                {
                    sizeInBytes += growSize;
                    if (++paranoid > 100)
                    {
                        _logX.DebugFormat("Failed paranoid check - sizeInBytes:{0} growSize:{1} neededSize:{2} ", sizeInBytes, growSize, neededSize);
                        break;
                    }
                }
            }
            if (sizeInBytes > maxSize) sizeInBytes = maxSize;

            _logX.DebugFormat("Extend file mapping from {0} to {1} ({2})", _currentSize, sizeInBytes, _filename);
            _mapping = CreateFileMapping(_fileHandle, IntPtr.Zero, 0x04, HiDWord(sizeInBytes), LoDWord(sizeInBytes), null);

            if (_mapping == NULL_HANDLE)
            {
                CloseAll();
                var ex = new Win32Exception(Marshal.GetHRForLastWin32Error());
                ExceptionLogger.Log(_logX, "CreateFileMapping() failed!", ex);
                throw ex;
            }
            _currentSize = sizeInBytes;
            _fileIsFull = (_currentSize >= maxSize);
            ++_fileExtended;
        }

        private bool IsDriveFull()
        {
            using (_logX.DebugCall(string.Format("IsDriveFull(File:{0})", _filename)))
            {
                if (string.IsNullOrEmpty(_filename)) return (false);
                try
                {
                    ulong lpFreeBytesAvailable = 0;
                    ulong lpTotalNumberOfBytes = 0;
                    ulong lpTotalNumberOfFreeBytes = 0;
                    bool result = GetDiskFreeSpaceEx(Path.GetPathRoot(_filename), out lpFreeBytesAvailable, out lpTotalNumberOfBytes, out lpTotalNumberOfFreeBytes);
                    _logX.DebugFormat("GetDiskFreeSpaceEx({0}, {1}, {2}, {3}) Success:{4}", Path.GetPathRoot(_filename), FormatHelper.FormatBytes(lpFreeBytesAvailable), FormatHelper.FormatBytes(lpTotalNumberOfBytes), FormatHelper.FormatBytes(lpTotalNumberOfFreeBytes), result);
                    if (!result)
                    {
                        throw new Win32Exception(Marshal.GetHRForLastWin32Error());
                    }
                    //--------------------------------------------------------------------
                    // Make sure that there is at least 1 gig of free disk space
                    //
                    if (lpFreeBytesAvailable < (1024 * MEG))
                    {
                        _logX.Debug("IsDriveFull() return (true);");
                        return (true);
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLogger.Log(_logX, "IsDriveFull() failed!", ex);
                }
                _logX.Debug("IsDriveFull() return (false);");
                return (false);
            }
        }

        public long Append(string s)
        {
            if (string.IsNullOrEmpty(s)) return (EmptyWrite());
            return (Append(System.Text.Encoding.Unicode.GetBytes(s)));
        }

        public long Append(byte[] buffer)
        {
            if (!Mapped) throw new ApplicationException("No open file mapping!");

            lock (_lock)
            {
                int len = buffer.Length;
                long pos = _eof;

                if (len <= 0) return (EmptyWrite());

                if (!MapView(_eof, len + sizeof(Int64))) return (-1);

                try
                {
                    Marshal.WriteInt64((IntPtr)(_viewAddress.ToInt64() + (_eof - _viewOffset)), len);
                    _eof += sizeof(Int64);
                    Marshal.Copy(buffer, 0, (IntPtr)(_viewAddress.ToInt64() + (_eof - _viewOffset)), buffer.Length);
                    _eof += buffer.Length;
                    ++_writes;
                }
                catch (Exception ex)
                {
                    Debug.Assert(false, ex.Message);
                    ExceptionLogger.Log("CacheFile.Append() Exception: ", ex);
                }
                return (pos);
            }
        }

        public long ReadInt64(long offset)
        {
            if (offset < 0) return (-1);
            lock (_lock)
            {
                if (!MapView(offset, sizeof(Int64))) return (-1);
                return (Marshal.ReadInt64((IntPtr)(_viewAddress.ToInt64() + (offset - _viewOffset))));
            }
        }

        public string ReadString(long offset, long length)
        {
            if (offset < 0) return (string.Empty);
            lock (_lock)
            {
                try
                {
                    if (length <= 0) return (string.Empty);
                    if (!MapView(offset, length)) return (string.Empty);
                    byte[] b = new byte[length];
                    Marshal.Copy((IntPtr)(_viewAddress.ToInt64() + (offset - _viewOffset)), b, 0, (int)length);
                    return (System.Text.Encoding.Unicode.GetString(b));
                }
                catch (Exception ex)
                {
                    Debug.Assert(false, ex.Message);
                    ExceptionLogger.Log(string.Format("CacheFile.ReadString({0}, {1}) Exception: ", offset, length), ex);
                }
                return (string.Empty);
            }
        }

        private void UnMapView()
        {
            using (_logX.DebugCall("UnMapView()"))
            {
                try
                {
                    if (IntPtr.Zero != _viewAddress)
                    {
                        if (!UnmapViewOfFile(_viewAddress))
                        {
                            var ex = new Win32Exception(Marshal.GetHRForLastWin32Error());
                            Debug.Assert(false, ex.Message);
                            throw ex;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLogger.Log(_logX, "CacheFile.UnMapView()", ex);
                }
                finally
                {
                    _viewAddress = IntPtr.Zero;
                    _viewOffset = 0;
                    _viewLength = 0;
                }
            }
        }

        private void CloseAll()
        {
            using (_logX.InfoCall("CloseAll()"))
            {
                _logX.InfoFormat("Cache file used {0} ({1}) of {2} ({3})", _eof, FormatHelper.FormatBytes(_eof), _currentSize, FormatHelper.FormatBytes(_currentSize));
                _logX.InfoFormat("File '{0}'  Writes:{1}  EmptyWrites:{2}  FileExtended:{3}", Path.GetFileName(_filename), _writes, _emptyWrites, _fileExtended);
                
                UnMapView();
                if (INVALID_HANDLE_VALUE != _fileHandle) CloseHandle(_fileHandle);
                if (NULL_HANDLE != _mapping) CloseHandle(_mapping);

                _fileHandle = INVALID_HANDLE_VALUE;
                _mapping = NULL_HANDLE;
                try
                {
                    if (File.Exists(_filename)) { File.Delete(_filename); }
                }
                catch (Exception ex)
                {
                    Debug.Assert(false, ex.Message);
                    ExceptionLogger.Log("CacheFile.CloseAll() Exception: ", ex);
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            using (_logX.DebugCall(string.Format("Dispose(File:{0}, Disposing:{1})", _filename, disposing)))
            {
                lock (_lock) { CloseAll(); }
                if (disposing) GC.SuppressFinalize(this);
            }
        }

    }

}
