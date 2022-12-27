using Utility;


namespace ReLeaf
{
    public class TitleState : SingletonBase<TitleState>
    {
        public override bool DontDestroyOnLoad => true;

        public bool IsSkipTitle { get; set; }

        protected override void Init(bool isFirstInit, bool callByAwake)
        {
        }
    }
}
