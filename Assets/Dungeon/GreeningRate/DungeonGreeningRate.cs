using Utility;

namespace ReLeaf
{

    [ClassSummary("緑化率マネージャー")]
    public class DungeonGreeningRate : GreeningRateBase
    {
        protected override void CalculateMaxGreeningCount()
        {
            foreach (var tile in DungeonManager.Singleton.TileDic.Values)
            {
                if (tile.CanOrAleeadyGreening(true))
                {
                    MaxGreeningCount++;
                }
            }
        }

        protected override void UpdateValue()
        {
            DungeonGreeningRateUI.Singleton.Slider.value = ValueRate;
        }
        protected override void Finish()
        {
            GameRuleManager.Singleton.Finish(true);
        }

    }
}
