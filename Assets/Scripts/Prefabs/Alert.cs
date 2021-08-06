using System;
using UnityEngine;
using UnityEngine.UI;

public class Alert : MonoBehaviour
{
    public Text title;
    public Text info;
    public RectTransform infoContent;
    public Action callback;

    public void InitText(string titleText, string infoText, Action callback)
    {
        title.text = titleText;
        info.text = infoText;
        infoContent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, info.preferredHeight);
        this.callback = callback;
    }

    public void CloseAlert()
    {
        callback?.Invoke();
        Destroy(gameObject);
    }
}
