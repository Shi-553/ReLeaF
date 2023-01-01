using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class DebugRouting : SingletonBase<DebugRouting>
{
    public override bool DontDestroyOnLoad => false;
    protected override void Init(bool isFirstInit, bool callByAwake)
    {
    }

    [SerializeField]
    float interval = 0.01f;
    [SerializeField]
    float lifetime = 0;

    float lastLifetime = 0;
    WaitForSeconds waitLifetime;

    void Start()
    {
        lastLifetime = lifetime;
        waitLifetime = new WaitForSeconds(lifetime);
        StartCoroutine(Show());
    }

    [SerializeField]
    GameObject[] prefabs;
    List<GameObject> instances = new();
    Queue<DebugRoutingInfo> infos = new();

    public void Enqueue(DebugRoutingInfo info)
    {
        infos.Enqueue(info);
    }
    public void Clear()
    {
        infos.Clear();
        instances.ForEach(instance => Destroy(instance));
        instances.Clear();
    }
    IEnumerator Show()
    {
        var wait = new WaitForSeconds(interval);
        while (true)
        {
            if (infos.TryDequeue(out var info))
            {
                var instance = Instantiate(prefabs[info.index], info.pos, info.rotation);
                instance.transform.localScale = info.scale;
                instances.Add(instance);
                if (lifetime > 0)
                    StartCoroutine(WaitLifetime(instance));
            }
            yield return wait;
        }
    }
    IEnumerator WaitLifetime(GameObject instance)
    {
        if (lifetime != lastLifetime)
        {
            lastLifetime = lifetime;
            waitLifetime = new WaitForSeconds(lifetime);
        }
        yield return waitLifetime;

        if (instances.Remove(instance))
            Destroy(instance);
    }


    public struct DebugRoutingInfo
    {
        public int index;
        public Vector3 pos;
        public Quaternion rotation;
        public Vector3 scale;

        public DebugRoutingInfo(int index, Vector3 pos, Quaternion rotation, Vector3 scale)
        {
            this.index = index;
            this.pos = pos;
            this.rotation = rotation;
            this.scale = scale;
        }
        public DebugRoutingInfo(int index, Vector3 pos, Quaternion rotation) : this(index, pos, rotation, Vector3.one)
        {
        }
        public DebugRoutingInfo(int index, Vector3 pos) : this(index, pos, Quaternion.identity, Vector3.one)
        {
        }
        public DebugRoutingInfo(Vector3 pos, Quaternion rotation, Vector3 scale) : this(0, pos, rotation, scale)
        {
        }
        public DebugRoutingInfo(Vector3 pos, Quaternion rotation) : this(0, pos, rotation, Vector3.one)
        {
        }
        public DebugRoutingInfo(Vector3 pos) : this(0, pos, Quaternion.identity, Vector3.one)
        {
        }
    }
}
