using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneRoute : MonoBehaviour
{
    List<Foundation> lastTargets = new List<Foundation>();
    public IReadOnlyCollection<Foundation> LastTargets => lastTargets;
    [SerializeField]
    float speed = 5;
    [SerializeField]
    float range = 0.1f;

    Vector2Int dir;
    public void Begin()
    {
        gameObject.SetActive(true);
        lastTargets.Clear();
        StartCoroutine(Move());
    }
    public void SetDir(Vector2Int dir)
    {
        this.dir = dir;

    }
    public void End()
    {
        gameObject.SetActive(false);
    }
    private IEnumerator Move()
    {
        dir = Vector2Int.zero;
        var targetTilePos = DungeonManager.Instance.WorldToTilePos(transform.position);
        while (true)
        {
            var targetWorldPos = DungeonManager.Instance.TilePosToWorld(targetTilePos);
            var target = DungeonManager.Instance.GetGroundTileObject(targetTilePos);
            while (true)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetWorldPos, DungeonManager.CELL_SIZE * Time.unscaledDeltaTime * speed);

                yield return null;

                if (transform.position== targetWorldPos)
                {
                    if (target!=null&& target.TryGetComponent<Foundation>(out var foundation)&&foundation.IsFullGrowth&&!foundation.IsHighlighting)
                    {
                        foundation.SetHighlight(true);
                        lastTargets.Add(foundation);
                    }
                    break;
                }
            }

            dir = Vector2Int.zero;
            yield return new WaitWhile(() => dir == Vector2Int.zero);

            targetTilePos += (Vector3Int)dir;
        }
    }

}
