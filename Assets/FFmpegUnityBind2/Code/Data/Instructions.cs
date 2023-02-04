namespace FFmpegUnityBind2
{
    static class Instructions
    {
        public const string REWRITE_INSTRUCTION = "-y";
        public const string INPUT_INSTRUCTION = "-i";
        public const string INDEX_PREFIX_INSTRUCTION = "%";
        public const string INDEX_SUFIX_INSTRUCTION = "d";
        public const string FORCE_INPUT_OR_OUTPUT_INSTRUCTION = "-f";
        public const string FPS_INSTRUCTION = "-r";
        public const string NATIVE_FRAMERATE_INSTRUCTION = "-re";
        public const string FRAMERATE_INSTRUCTION = "-framerate";
        public const string RESOLUTION_INSTRUCTION = "-s";
        public const string ACCURATE_SEEK_INSTRUCTION = "-ss";
        public const string CODEC_INSTRUCTION = "-codec";
        public const string VIDEO_CODEC_INSTRUCTION = "-vcodec";
        public const string C_CODEC_INSTRUCTION = "-c";
        public const string COPY_INSTRUCTION = "copy";
        public const string TIME_INSTRUCTION = "-t";
        public const string CODEC_VIDEO_INSTRUCTION = "-c:v";
        public const string CODEC_AUDIO_INSTRUCTION = "-c:a";
        public const string LIB_X264_INSTRUCTION = "libx264";
        public const string CONSTANT_RATE_FACTOR_INSTRUCTION = "-crf";
        public const string FILE_FORMAT_INPUT_INSTRUCTION = "-f";
        public const string CONCAT_INSTRUCTION = "concat";
        public const string SAFE_INSTRUCTION = "-safe";
        public const string ZERO_INSTRUCTION = "0";
        public const string FILTER_COMPLEX_INSTRUCTION = "-filter_complex";
        public const string MAP_INSTRUCTION = "-map";
        public const string PRESET_INSTRUCTION = "-preset";
        public const string VIDEO_INSTRUCTION = "[v]";
        public const string AUDIO_INSTRUCTION = "[a]";
        public const string ULTRASAFE_INSTRUCTION = "ultrafast";
        public const string VIDEO_COMPLEX_FORMAT_INSTRUCTION = "[{0}:v:0] ";
        public const string AUDIO_COMPLEX_FORMAT_INSTRUCTION = "[{0}:a:0] ";
        public const string CONCAT_COMPLEX_FORMAT_INSTRUCTION = "{0}=n={1}:v=1:a=1";
        public const string FIRST_INPUT_VIDEO_CHANNEL_INSTRUCTION = "0:v";
        public const string SECOND_INPUT_AUDIO_CHANNEL_INSTRUCTION = "1:a";
        public const string IMAGE_FORMAT_INSTRUCTION = "image2";
        public const string FLV_INSTRUCTION = "flv";
        public const string SHORTEST_INSTRUCTION = "-shortest";
        public const string PIXEL_FORMAT_INSTRUCTION = "-pix_fmt";
        public const string YUV_420P_INSTRUCTION = "yuv420p";
        public const string RGB_24_INSTRUCTION = "rgb24";
        public const string VERTICAL_FLIP_INSTRUCTION = "-vf";
        public const string VERTICAL_FLIP_ARG_INSTRUCTION = "vflip";
        public const string RAW_VIDEO_INSTRUCTION = "rawvideo";
        public const char QUOTE = '\'';
        public const char DOUBLE_QUOTE = '\"';
        public const char SPACE = ' ';
        public const char X = 'x';
    }
}