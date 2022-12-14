namespace ReLeaf
{
    public class Sand : TileObject
    {
        public TileObject Target { get; set; }

        public override bool CanEnemyMove(bool isAttackMove)
        {
            return Target ? Target.CanEnemyMove(isAttackMove) : base.CanEnemyMove(isAttackMove);
        }
    }
}
