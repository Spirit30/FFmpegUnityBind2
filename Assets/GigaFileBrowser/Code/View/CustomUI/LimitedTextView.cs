namespace UnityEngine.UI
{
    public class LimitedTextView : MonoBehaviour
    {
        [SerializeField]
        Text lable = null;

        [SerializeField]
        int limitLength = 20;

        public string Text
        {
            get => lable.text;
            set => lable.text = 
                !string.IsNullOrEmpty(value) && value.Length > limitLength 
                    ? $"..{value.Substring(value.Length - limitLength)}" 
                    : value;
        }
    }
}