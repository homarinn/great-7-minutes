using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    Transform transformCache;
    float defaultPositionY;

    // Start is called before the first frame update
    void Start()
    {
        transformCache = transform;
        defaultPositionY = transformCache.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        float y = transformCache.position.y;
        y += Time.deltaTime;
        transformCache.position = new Vector3(transformCache.position.x, y, transformCache.position.z);
        if (y > defaultPositionY + 50)
        {
            enabled = false;
        }
    }

    public void Open(Item skill)
    {
        if (skill.Name == "Attack")
        {
            enabled = true;
        }
    }
}
