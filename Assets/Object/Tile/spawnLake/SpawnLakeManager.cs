using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility;
using Object = UnityEngine.Object;

namespace ReLeaf
{
    public class SpawnLakeGroup
    {
        readonly Dictionary<Vector2Int, SpawnLake> lakeDic;
        readonly HashSet<Vector2Int> targets = new();

        public SpawnLakeEnemyInfo EnemyInfo { get; private set; }

        WaitForSeconds wait;
        Transform enemyRoot;

        public SpawnLakeGroup(SpawnLake lake, Transform enemyRoot)
        {
            this.lakeDic = new() { { lake.TilePos, lake } };
            this.enemyRoot = enemyRoot;
        }

        public IReadOnlyDictionary<Vector2Int, SpawnLake> Dic => lakeDic;

        public bool TryAdd(SpawnLake s)
        {
            if (lakeDic.ContainsKey(s.TilePos + Vector2Int.up) ||
                lakeDic.ContainsKey(s.TilePos + Vector2Int.down) ||
                lakeDic.ContainsKey(s.TilePos + Vector2Int.left) ||
                lakeDic.ContainsKey(s.TilePos + Vector2Int.right))
            {
                lakeDic.Add(s.TilePos, s);
                return true;
            }
            return false;
        }

        public IEnumerator SetSpwanTarget()
        {
            yield return null;
            yield return null;

            Vector2Int[] upAndDownLeftRight = new Vector2Int[4];

            foreach (var lakePos in lakeDic.Keys)
            {
                upAndDownLeftRight[0] = lakePos + Vector2Int.up;
                upAndDownLeftRight[1] = lakePos + Vector2Int.down;
                upAndDownLeftRight[2] = lakePos + Vector2Int.left;
                upAndDownLeftRight[3] = lakePos + Vector2Int.right;

                foreach (var pos in upAndDownLeftRight)
                {
                    if (!DungeonManager.Singleton.TryGetTile(pos, out var tile))
                    {
                        continue;
                    }

                    if (tile is not SpawnTarget target)
                    {
                        if (tile is not Sand connectedSand)
                        {
                            continue;
                        }
                        target = connectedSand.Target as SpawnTarget;

                        if (target == null)
                            continue;
                    }

                    targets.Add(target.TilePos);
                    EnemyInfo = target.EnemyInfo;
                }
            }
            if (targets.Count == 0)
                yield break;
            wait = new WaitForSeconds(EnemyInfo.SpwanInterval);

            GameRuleManager.Singleton.OnChangeState += OnChangeState;
        }

        Coroutine coroutine;
        private void OnChangeState(GameRuleState obj)
        {
            if (obj == GameRuleState.Playing)
            {
                coroutine = GlobalCoroutine.Singleton.StartCoroutine(SpawnInvertal());
                return;
            }

            if (coroutine != null)
                GlobalCoroutine.Singleton.StopCoroutine(coroutine);
        }

        IEnumerator SpawnInvertal()
        {
            while (true)
            {
                yield return wait;

                if (lakeDic.All(t => t.Value.IsGreening))
                    yield break;

                foreach (var target in targets)
                {

                    var enemy = Object.Instantiate(EnemyInfo.EnemyPrefab,
                        DungeonManager.Singleton.TilePosToWorld(target),
                        Quaternion.identity,
                        enemyRoot);

                    GlobalCoroutine.Singleton.StartCoroutine(SpawnAnimation(enemy.transform, target));
                }
            }
        }
        IEnumerator SpawnAnimation(Transform enemy, Vector2Int target)
        {
            var (element, index) = lakeDic.Values.MinBy(lake => (lake.TilePos - target).sqrMagnitude);
            float time = 0;

            var start = DungeonManager.Singleton.TilePosToWorld(element.TilePos);
            var end = DungeonManager.Singleton.TilePosToWorld(target);

            while (true)
            {
                var t = time / EnemyInfo.SpwanInitAnimationTime;

                enemy.position = Vector3.Lerp(start, end, t * t);

                time += Time.deltaTime;
                if (time > EnemyInfo.SpwanInitAnimationTime)
                    break;

                yield return null;
            }
            enemy.position = end;
        }
    }

    public class SpawnLakeManager : SingletonBase<SpawnLakeManager>
    {
        public override bool DontDestroyOnLoad => false;

        [SerializeField]
        Transform enabledLake;
        [SerializeField]
        Transform disabledLake;
        [SerializeField]
        Transform enemyRoot;

        List<SpawnLakeGroup> groups = new();

        SortedDictionary<Vector2IntComparer, SpawnLake> spawnLakes = new();

        protected override void Init(bool isFirstInit, bool callByAwake)
        {
            if (!isFirstInit)
                return;
            spawnLakes.Clear();
        }

        void Start()
        {
            foreach (var lake in spawnLakes.Values)
            {
                // 既存のグループに入れたらcontinue
                if (groups.Any(g => g.TryAdd(lake)))
                    continue;

                groups.Add(new SpawnLakeGroup(lake, enemyRoot));
            }

            groups.ForEach(g => StartCoroutine(g.SetSpwanTarget()));
        }

        public void AddEnabledLake(SpawnLake spawnLake)
        {
            spawnLakes.Add(new(spawnLake.TilePos), spawnLake);
            spawnLake.transform.SetParent(enabledLake, true);
        }
        public void ChangeToDisabledLake(SpawnLake spawnLake)
        {
            spawnLake.transform.SetParent(disabledLake, true);
        }

        readonly struct Vector2IntComparer : IComparable<Vector2IntComparer>, IEquatable<Vector2IntComparer>
        {
            public readonly Vector2Int key;

            public Vector2IntComparer(Vector2Int key) => this.key = key;

            public int CompareTo(Vector2IntComparer other)
            {
                var xCompare = key.x - other.key.x;
                if (xCompare != 0)
                    return xCompare;
                return key.y - other.key.y;
            }

            public override bool Equals(object obj)
            {
                return obj is Vector2IntComparer comparer && Equals(comparer);
            }
            public bool Equals(Vector2IntComparer other)
            {
                return key.Equals(other.key);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(key);
            }
            public static bool operator ==(Vector2IntComparer left, Vector2IntComparer right)
            {
                return left.Equals(right);
            }
            public static bool operator !=(Vector2IntComparer left, Vector2IntComparer right)
            {
                return !(left == right);
            }
        }
    }
}
