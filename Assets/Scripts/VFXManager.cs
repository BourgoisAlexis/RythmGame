using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class QueueDatas {
    public GameObject prefab;
    public Queue<GameObject> queue;

    public QueueDatas(GameObject prefab) {
        this.prefab = prefab;
        queue = new Queue<GameObject>();
    }
}

public class VFXManager : MonoBehaviour {
    [SerializeField] private Transform _vfxParent;
    [SerializeField] private List<GameObject> _prefabs = new List<GameObject>();

    private Dictionary<string, QueueDatas> _queues;
    private int _baseInstanceNumber = 10;


    private void Awake() {
        _queues = new Dictionary<string, QueueDatas>();

        foreach (GameObject go in _prefabs) {
            QueueDatas queueDatas = new QueueDatas(go);
            _queues.Add(go.name, queueDatas);

            for (int j = 0; j < _baseInstanceNumber; j++) {
                GameObject obj = Instantiate(go, _vfxParent);
                Enqueue(obj, go.name);
            }
        }
    }

    public void Enqueue(GameObject obj, string key) {
        if (string.IsNullOrEmpty(key) || !_queues.ContainsKey(key))
            return;

        Queue<GameObject> q = _queues[key].queue;

        if (q.Contains(obj))
            return;

        q.Enqueue(obj);
        obj.SetActive(false);
        obj.transform.SetParent(_vfxParent);
    }

    public GameObject Dequeue(string key, Transform parent = null) {
        if (string.IsNullOrEmpty(key) || !_queues.ContainsKey(key))
            return null;

        GameObject result = null;
        Queue<GameObject> q = _queues[key].queue;

        if (q.Count <= 0) {
            GameObject obj = Instantiate(_queues[key].prefab, _vfxParent);
            result = obj;
        }
        else
            result = q.Dequeue();

        result.SetActive(true);
        Transform t = result.transform;
        if (parent != null)
            t.SetParent(parent);
        else
            t.SetParent(_vfxParent);
        t.localScale = Vector3.one;
        t.localEulerAngles = Vector3.zero;

        return result;
    }

    public void DelayedEnqueue(GameObject obj, string key, float delay) {
        StartCoroutine(DelayedEnqueueCoroutine(obj, key, delay));
    }

    private IEnumerator DelayedEnqueueCoroutine(GameObject obj, string key, float delay) {
        yield return new WaitForSeconds(delay);
        Enqueue(obj, key);
    }
}
