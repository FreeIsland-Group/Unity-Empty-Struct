using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputConfirm : MonoBehaviour
{
    public Text title;
    public Text info;
    public Text placeholder;
    public InputField input;
    public Text cancel;
    public Text sure;
    public Action cancelCallback;
    public Action<string> sureCallback;
    
    public void InitText(
        string titleText,
        string infoText,
        Action<string> choiceSureCallback,
        Action choiceCancelCallback,
        string placeholderText = "请输入...",
        string cancelText = "放 弃",
        string sureText = "确 定"
    )
    {
        title.text = titleText;
        info.text = infoText;
        placeholder.text = placeholderText;
        cancel.text = cancelText;
        sure.text = sureText;
        cancelCallback = choiceCancelCallback;
        sureCallback = choiceSureCallback;
    }

    public void SureConfirm()
    {
        sureCallback?.Invoke(input.text);
        Destroy(gameObject);
    }

    public void CancelConfirm()
    {
        cancelCallback?.Invoke();
        Destroy(gameObject);
    }
}
