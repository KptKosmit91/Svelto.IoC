using Utility;

namespace Svelto.Unity.Log
{
    public class UnitySlowLogger : ILogger
    {
#if DEBUG || UNITY_EDITOR || PRINTDEBUG
        readonly static string Debug = "[DEBUG] ";
#endif

        public void Log(string txt, string stack = null, LogMessageType type = LogMessageType.Log)
        {
            switch (type)
            {
                case LogMessageType.Log:
                    UnityEngine.Debug.Log(stack != null ? txt.FastConcat(stack) : txt);
                break;
                case LogMessageType.Warning:
                    UnityEngine.Debug.LogWarning(stack != null ? txt.FastConcat(stack) : txt);
                break;
                case LogMessageType.Error:
                    UnityEngine.Debug.LogError(stack != null ? txt.FastConcat(stack) : txt);
                break;

#if DEBUG || UNITY_EDITOR || PRINTDEBUG
                case LogMessageType.DebugLog:
                    UnityEngine.Debug.Log(stack != null ? Debug.FastConcat(txt, stack) : Debug.FastConcat(txt));
                break;
                case LogMessageType.DebugWarning:
                    UnityEngine.Debug.LogWarning(stack != null ? Debug.FastConcat(txt, stack) : Debug.FastConcat(txt));
                break;
                case LogMessageType.DebugError:
                    UnityEngine.Debug.LogError(stack != null ? Debug.FastConcat(txt, stack) : Debug.FastConcat(txt));
                break;
#endif
            }
        }
    }
}
