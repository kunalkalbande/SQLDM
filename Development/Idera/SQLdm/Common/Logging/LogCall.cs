using System;
using System.Diagnostics;
using System.Reflection;
using log4net;

namespace Idera.SQLdm.Common.Logging {
    /// <summary>
    /// This reliably logs the entry/exit of a method 
    /// regardless of how the method exits (e.g. by exception). 
    /// Call one of the static methods such as LogCall.Debug or LogCall.Info, passing
    /// the ILog object you want log through.  LogCall automatically determines the name
    /// of the method that called it.  Make the call in conjunction with
    /// a "using" block so the Dispose method can log the method's exit. For example...
    /// 
    /// using (LogCall.Debug(myLogger)
    /// {
    ///     // method code goes here.
    /// }
    /// </summary>
    public class LogCall : IDisposable {
        #region Log the calling method
        /// <summary>
        /// Log the entry of the calling method using the specified logger.
        /// The exit of the calling method will be logged in Dispose().
        /// </summary>
        /// <returns></returns>
        public static LogCall Debug(ILog logger) {
            if (logger.IsDebugEnabled) {
                string name = Caller();
                logger.Debug(name + " entered.");
                return new LogCall(new LogExit(logger.Debug), name);
            } else {
                return null;
            }
        }

        public static LogCall Info(ILog logger) {
            if (logger.IsInfoEnabled) {
                string name = Caller();
                logger.Info(name + " entered.");
                return new LogCall(new LogExit(logger.Info), name);
            } else {
                return null;
            }
        }

        public static LogCall Warn(ILog logger) {
            if (logger.IsWarnEnabled) {
                string name = Caller();
                logger.Warn(name + " entered.");
                return new LogCall(new LogExit(logger.Warn), name);
            } else {
                return null;
            }
        }

        public static LogCall Error(ILog logger) {
            if (logger.IsErrorEnabled) {
                string name = Caller();
                logger.Error(name + " entered.");
                return new LogCall(new LogExit(logger.Error), name);
            } else {
                return null;
            }
        }

        public static LogCall Fatal(ILog logger) {
            if (logger.IsFatalEnabled) {
                string name = Caller();
                logger.Fatal(name + " entered.");
                return new LogCall(new LogExit(logger.Fatal), name);
            } else {
                return null;
            }
        }
        #endregion

        #region Log the specified block name
        /// <summary>
        /// Log the entry of the specified name using the specified logger.
        /// The exit of the specified name will be logged in Dispose().
        /// </summary>
        /// <returns></returns>
        public static LogCall Debug(ILog logger, string name) {
            if (logger.IsDebugEnabled) {
                logger.Debug(name + " entered.");
                return new LogCall(new LogExit(logger.Debug), name);
            } else {
                return null;
            }
        }

        public static LogCall Info(ILog logger, string name) {
            if (logger.IsInfoEnabled) {
                logger.Info(name + " entered.");
                return new LogCall(new LogExit(logger.Info), name);
            } else {
                return null;
            }
        }

        public static LogCall Warn(ILog logger, string name) {
            if (logger.IsWarnEnabled) {
                logger.Warn(name + " entered.");
                return new LogCall(new LogExit(logger.Warn), name);
            } else {
                return null;
            }
        }

        public static LogCall Error(ILog logger, string name) {
            if (logger.IsErrorEnabled) {
                logger.Error(name + " entered.");
                return new LogCall(new LogExit(logger.Error), name);
            } else {
                return null;
            }
        }

        public static LogCall Fatal(ILog logger, string name) {
            if (logger.IsFatalEnabled) {
                logger.Fatal(name + " entered.");
                return new LogCall(new LogExit(logger.Fatal), name);
            } else {
                return null;
            }
        }
        #endregion

        #region private stuff
        /// <summary>
        /// Private ctor is only called by static methods.
        /// </summary>
        /// <param name="logExit">The ILog method to call in Dispose().</param>
        /// <param name="name">The method or block name to log (i.e. "exiting [name]").</name> </param>
        private LogCall(LogExit logExit, string name) {
            _name = name;
            _logExit = logExit;
        }

        private delegate void LogExit(string name);
        private bool _disposed;
        private string _name;
        private LogExit _logExit;

        // Return class.method namd of the caller's caller.
        private static string Caller() {
            StackTrace stack = new StackTrace(2);
            StackFrame frame = stack.GetFrame(0);
            MethodBase method = frame.GetMethod();
            return method.DeclaringType.Name + "." + method.Name;
        }
        #endregion

        #region Dispose pattern
        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        // This is always called via "using" or directly by client code.
        public void Dispose() {
            Dispose(true);
            // Take this off the Finalization queue 
            // to prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // A naughty user might arrange for this to be called by the finalizer.
        // This implementation prevents logging from the finalizer, which
        // seems like a bad idea.  Clients are SUPPOSED to use this class
        // with a "using" block so Dispose (and thus Dispose(true)) gets called automatically
        // when control exits the using block.
        protected virtual void Dispose(bool disposing) {
            // Check to see if Dispose has already been called.
            if (!this._disposed) {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources.
                if (disposing) {
                    // Safe to reference managed resources.
                    try {
                        _logExit(_name + " exiting.");
                    } catch (Exception ex) {
                        // Don't let a logging exception bring down the process.
                        Trace.WriteLine("An exception occurred in LogCall.Dispose().\n" + ex.GetType().ToString() + "\n" + ex.Message);
                    }
                }

                // Release unmanaged resources if there are any (none in this class).
                _disposed = true;
            }
        }

        ~LogCall() {
            Dispose(false);
        }
        #endregion
    }
}
