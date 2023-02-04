namespace FFmpegUnityBind2.Demo
{
    class DemoCaseVersionView : DemoCaseView
    {
        protected override void OnExecuteButton()
        {
            ExecuteWithOutput("-version");
        }
    }
}