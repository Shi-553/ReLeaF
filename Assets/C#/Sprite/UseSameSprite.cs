using UnityEngine;

public class UseSameSprite : MonoBehaviour
{
    Sprite prevSprite;
    SpriteRenderer myself;
    [SerializeField]
    SpriteRenderer target;

    void Start()
    {
        TryGetComponent(out myself);
    }

    void LateUpdate()
    {
        var s = target.sprite;
        if (s != prevSprite)
            myself.sprite = s;
        prevSprite = s;
    }
}
