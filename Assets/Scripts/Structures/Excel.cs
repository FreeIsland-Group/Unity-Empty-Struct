using System.Collections.Generic;
using UnityEngine;

public class Excel
{
    public string[] building;

    public void RevertDictionary()
    {
        foreach (var item in building)
        {
            var icon = Resources.Load<Sprite>(item);
        }
    }
}
