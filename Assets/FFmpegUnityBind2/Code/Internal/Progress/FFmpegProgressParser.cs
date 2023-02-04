using System;
using System.Globalization;

namespace FFmpegUnityBind2.Internal
{
    /// <summary>
    /// This is the Helper class to get FFmpeg operation progress.
    /// </summary>
    static class FFmpegProgressParser
    {
        const string FORMAT = "hh:mm:ss.ff";
        static readonly string[] durationSeparators = { "Duration: ", ", start:" };
        static readonly string[] timeSeparators = { " time=", " bitrate=" };

        public static void Parse(string log, ref float durationMS, ref float progress)
        {
            //Try obtain Duration
            if (durationMS == 0)
            {
                string[] durationTokens = log.Split(durationSeparators, StringSplitOptions.RemoveEmptyEntries);

                if (durationTokens.Length > 0)
                {
                    UpdateDuration(durationTokens, ref durationMS);
                }
            }

            //When Duration is obtained
            if (durationMS > 0)
            {
                string timeToken = GetTimeToken(log, timeSeparators);

                if(timeToken != null)
                {
                    progress = GetMS(timeToken) / durationMS;
                }
            }
        }

        static void UpdateDuration(string[] tokens, ref float durationMiniSec)
		{
            durationMiniSec = 0;

            for (int t = 0; t < tokens.Length; ++t)
            {
                durationMiniSec += GetMS(tokens[t]);
            }
		}

		static string GetTimeToken(string log, string[] separators)
        {
            string[] tokens = log.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            if(tokens.Length > 2)
            {
                return tokens[tokens.Length - 2];
            }

            return null;
        }

        static float GetMS(string token)
        {
            if (DateTime.TryParseExact(token, FORMAT, null, DateTimeStyles.None, out DateTime time))
            {
                return (float)time.TimeOfDay.TotalMilliseconds;
            }

            return 0;
        }
	}
}