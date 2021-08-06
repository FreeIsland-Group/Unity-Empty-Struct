using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public int maxValue;
    public int value;
    public Image bar;

    public void SetValue(int val, int max = -1)
    {
        if (max != -1)
        {
            maxValue = max;
        }
        value = val;
        bar.fillAmount = (float)value / maxValue;
    }

    public void SetValue(float val, int max = -1)
    {
        if (max != -1)
        {
            maxValue = max;
        }
        value = (int) val;
        bar.fillAmount = val / maxValue;
    }
}
