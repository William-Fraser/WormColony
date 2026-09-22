using UnityEngine;

public class DestroySelfTool : MonoBehaviour
{
    float timeActiveFor = 60; // seconds
    float timeDestroyedAt;

    void Start()
    {
        timeDestroyedAt = Time.time + timeActiveFor;
    }

    void Update()
    {
        if (timeDestroyedAt > Time.time)
            Destroy(gameObject);
    }
}
