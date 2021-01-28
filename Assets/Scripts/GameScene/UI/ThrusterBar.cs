using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThrusterBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;

    public void SetAmount(float amount)
    {
        slider.value = amount;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

    public void SetMaxAmount(float amount)
    {
        slider.maxValue = amount;
        slider.value = amount;

        fill.color = gradient.Evaluate(1f);
    }
}
