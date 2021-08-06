using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Confirm : MonoBehaviour
{
    public Text title;
    public Text info;
    public Text cancel;
    public Text sure;
    public Action cancelCallback;
    public Action sureCallback;
    
    public void InitText(string titleText, string infoText, Action choiceSureCallback, Action choiceCancelCallback, string cancelText = "放 弃", string sureText = "确 定")
    {
        title.text = titleText;
        info.text = infoText;
        cancel.text = cancelText;
        sure.text = sureText;
        cancelCallback = choiceCancelCallback;
        sureCallback = choiceSureCallback;
    }

    public void SureConfirm()
    {
        sureCallback?.Invoke();
        Destroy(gameObject);
    }

    public void CancelConfirm()
    {
        cancelCallback?.Invoke();
        Destroy(gameObject);
    }
}
