namespace FFmpegUnityBind2.Utils
{
    static class StringUtil
    {
        public static string Copy(string origin)
        {
            var deepCopy = new char[origin.Length];

            for(int i = 0; i < deepCopy.Length; ++i)
            {
                deepCopy[i] = origin[i];
            }

            return new string(deepCopy);
        }
    }
}
