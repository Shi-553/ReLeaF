using System.Collections;

namespace ReLeaf
{
    public class TitleRuleManager : GameRuleManager
    {
        protected override IEnumerator Start()
        {

            State = GameRuleState.Playing;
            yield return null;
        }
    }
}
