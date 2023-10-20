using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class HpSliderHandler : MonoBehaviour
{
    [field: SerializeField]
    public float SlidingTime { get; set; }
    float defaultSlidingTime;

    Slider slider;
    float valueDiff = 0;
    float valueDiffPerFrameAmount = 0;

    // Start is called before the first frame update
    void Awake()
    {
        if (slider == null)
        {
            slider = GetComponent<Slider>();
        }
    }
    void Start()
    {
        defaultSlidingTime = SlidingTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (valueDiff != 0)
        {
            if (SlidingTime == 0)
            {
                SetValuePerFrame(valueDiff);
            } else
            {
                float diffPerFrame = valueDiff * Time.deltaTime / SlidingTime;
                SetValuePerFrame(diffPerFrame);
            }
        }
    }

    public void SetMaxValue(int maxHp)
    {
        if (slider == null)
        {
            slider = GetComponent<Slider>();
        }
        slider.maxValue = maxHp;
    }

    // HPバーの増減をする準備だけ行い、実際にはUpdateのたびにHPバーの増減が行われる
    public void SetValue(float Hp)
    {
        if (valueDiff != 0)
        {
            FinishSetValue();
        }

        valueDiff = Hp - slider.value;

        if (valueDiff == 0)
        {
            FinishSetValue();
        }
    }

    // HPバーの増減をする準備だけ行い、実際にはUpdateのたびにHPバーの増減が行われる
    public void SetValue(float Hp, float slidingTime)
    {
        if (valueDiff != 0)
        {
            FinishSetValue();
        }

        valueDiff = Hp - slider.value;
        SlidingTime = slidingTime;

        if (valueDiff == 0)
        {
            FinishSetValue();
        }
    }

    void SetValuePerFrame(float diffPerFrame)
    {
        valueDiffPerFrameAmount += diffPerFrame;
        slider.value += diffPerFrame;

        if ((valueDiff < 0 && valueDiff >= valueDiffPerFrameAmount) || (valueDiff >= 0 && valueDiff <= valueDiffPerFrameAmount) )
        {
            FinishSetValue();
        }
    }

    void FinishSetValue()
    {
        slider.value += valueDiff - valueDiffPerFrameAmount;
        valueDiff = 0;
        valueDiffPerFrameAmount = 0;
        SlidingTime = defaultSlidingTime;
    }
}
