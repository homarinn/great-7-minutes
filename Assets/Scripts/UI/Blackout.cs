using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Blackout : MonoBehaviour
{
    Image image;

    float a = 0f;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (a < 255f)
        {
            a += Time.deltaTime * 255 * 5;
        }

        if (a >= 255f)
        {
            a = 255f;
        }

        image.color = new Color32(0, 0, 0, (byte) a);
    }
}
