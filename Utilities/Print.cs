using System;
using System.Diagnostics;
using System.Text;

public static class FastConcatUtility
{
    static readonly StringBuilder _stringBuilder = new StringBuilder(256);

    public static string FastConcat(this string str1, string str2)
    {
        lock (_stringBuilder)
        {
            _stringBuilder.Length = 0;

            _stringBuilder.Append(str1);
            _stringBuilder.Append(str2);

            return _stringBuilder.ToString();
        }
    }

    public static string FastConcat(this string str1, string str2, string str3)
    {
        lock (_stringBuilder)
        {
            _stringBuilder.Length = 0;

            _stringBuilder.Append(str1);
            _stringBuilder.Append(str2);
            _stringBuilder.Append(str3);

            return _stringBuilder.ToString();
        }
    }

    public static string FastConcat(this string str1, string str2, string str3, string str4)
    {
        lock (_stringBuilder)
        {
            _stringBuilder.Length = 0;

            _stringBuilder.Append(str1);
            _stringBuilder.Append(str2);
            _stringBuilder.Append(str3);
            _stringBuilder.Append(str4);


            return _stringBuilder.ToString();
        }
    }

    public static string FastConcat(this string str1, string str2, string str3, string str4, string str5)
    {
        lock (_stringBuilder)
        {
            _stringBuilder.Length = 0;

            _stringBuilder.Append(str1);
            _stringBuilder.Append(str2);
            _stringBuilder.Append(str3);
            _stringBuilder.Append(str4);
            _stringBuilder.Append(str5);

            return _stringBuilder.ToString();
        }
    }

    public static string FastJoin(this string[] str)
    {
        lock (_stringBuilder)
        {
            _stringBuilder.Length = 0;

            for (int i = 0; i < str.Length; i++)
                _stringBuilder.Append(str[i]);

            return _stringBuilder.ToString();
        }
    }

    public static string FastJoin(this string[] str, string str1)
    {
        lock (_stringBuilder)
        {
            _stringBuilder.Length = 0;

            for (int i = 0; i < str.Length; i++)
                _stringBuilder.Append(str[i]);

            _stringBuilder.Append(str1);

            return _stringBuilder.ToString();
        }
    }
}

namespace Utility
{
    /// <summary>
    /// Error, Warning, Log and Exception int values correspond to UnityEngine's LogType enum values.
    /// </summary>
    public enum LogMessageType
    {
        Error = 0,
        Warning = 2,
        Log = 3,
        Exception = 4,

        DebugError = 0 + 10,
        DebugWarning = 2 + 10,
        DebugLog = 3 + 10,
        DebugException = 4 + 10
    }

    public interface ILogger
    {
        void Log (string txt, string stack = null, LogMessageType type = LogMessageType.Log);
    }

    public static class Console
    {
        static StringBuilder _stringBuilder = new StringBuilder(256);

        public static ILogger logger = new SystemConsoleLogger();
        public static volatile bool BatchLog = false;

        public static void Log(string txt)
        {
            logger.Log(txt);
        }

        public static void LogError(string txt, bool showCurrentStack = true)
        {
            string toPrint;
        
            lock (_stringBuilder)
            {
                _stringBuilder.Length = 0;
                _stringBuilder.Append("-!!!!!!-> ");
                _stringBuilder.Append(txt);

                toPrint = _stringBuilder.ToString();
            }

            logger.Log(toPrint, showCurrentStack == true ? new StackTrace().ToString() : null, LogMessageType.Error);
        }

        public static void LogError(string txt, string stack)
        {
            string toPrint;

            lock (_stringBuilder)
            {
                _stringBuilder.Length = 0;
                _stringBuilder.Append("-!!!!!!-> ");
                _stringBuilder.Append(txt);

                toPrint = _stringBuilder.ToString();
            }

            logger.Log(toPrint, stack, LogMessageType.Error);
        }

        public static void LogException(Exception e)
        {
            LogException(e, null);
        }

        public static void LogException(Exception e, UnityEngine.Object obj)
        {
            string toPrint;
            string stackTrace;

            lock (_stringBuilder)
            {
                _stringBuilder.Length = 0;
                _stringBuilder.Append("-!!!!!!-> ").Append(e.Message);

                stackTrace = e.StackTrace;

                if (e.InnerException != null)
                {
                    _stringBuilder.Append(" Inner Message: ").Append(e.InnerException.Message).Append(" Inner Stacktrace:")
                        .Append(e.InnerException.StackTrace);
                }

                toPrint = _stringBuilder.ToString();
                
            }

            UnityEngine.Debug.LogException(e, obj);
        }

        public static void LogWarning(string txt)
        {
            string toPrint;

            lock (_stringBuilder)
            {
                _stringBuilder.Length = 0;
                _stringBuilder.Append("------> ");
                _stringBuilder.Append(txt);

                toPrint = _stringBuilder.ToString();
            }

            logger.Log(toPrint, null, LogMessageType.Warning);
        }

        /// <summary>
        /// Use this function if you don't want the message to be batched
        /// </summary>
        /// <param name="txt"></param>
        public static void SystemLog(string txt)
        {
            string toPrint;

            lock (_stringBuilder)
            {
                string currentTimeString = DateTime.UtcNow.ToLongTimeString(); //ensure includes seconds
                string processTimeString = (DateTime.Now - Process.GetCurrentProcess().StartTime).ToString();

                _stringBuilder.Length = 0;
                _stringBuilder.Append("[").Append(currentTimeString);
                _stringBuilder.Append("][").Append(processTimeString);
                _stringBuilder.Length = _stringBuilder.Length - 3; //remove some precision that we don't need
                _stringBuilder.Append("] ").AppendLine(txt);

                toPrint = _stringBuilder.ToString();
            }

#if !UNITY_EDITOR
            System.Console.WriteLine(toPrint);
#else
            UnityEngine.Debug.Log(toPrint);
#endif
        }
    }

    public class SystemConsoleLogger : ILogger
    {
#if DEBUG || UNITY_EDITOR || PRINTDEBUG
        readonly static string Debug = "[DEBUG] ";
#endif

        public void Log(string txt, string stack = null, LogMessageType type = LogMessageType.Log)
        {
            switch (type)
            {
                case LogMessageType.Log:
                    System.Console.WriteLine(stack != null ? txt.FastConcat(stack) : txt);
                break;
                case LogMessageType.Warning:
                    System.Console.WriteLine(stack != null ? txt.FastConcat(stack) : txt);
                break;
                case LogMessageType.Error:
                    System.Console.WriteLine(stack != null ? txt.FastConcat(stack) : txt);
                break;

#if DEBUG || UNITY_EDITOR || PRINTDEBUG
                case LogMessageType.DebugLog:
                    System.Console.WriteLine(stack != null ? Debug.FastConcat(txt, stack) : Debug.FastConcat(txt));
                break;
                case LogMessageType.DebugWarning:
                    System.Console.WriteLine(stack != null ? Debug.FastConcat(txt, stack) : Debug.FastConcat(txt));
                break;
                case LogMessageType.DebugError:
                    System.Console.WriteLine(stack != null ? Debug.FastConcat(txt, stack) : Debug.FastConcat(txt));
                break;
#endif
            }
        }
    }
}
