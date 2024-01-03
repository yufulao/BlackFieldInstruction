using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StageItem : MonoBehaviour
{
    public string stageCfgKey;

    private void Start()
    {
        if (stageCfgKey=="Stage1-1")
        {
            SaveManager.SetInt(stageCfgKey,1);
        }
        if (SaveManager.GetInt(stageCfgKey, 0) == 1)
        {
            GetComponent<Collider>().enabled = true;
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetComponent<SpriteRenderer>().color = Color.green;
            }

            return;
        }

        GetComponent<Collider>().enabled = false;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<SpriteRenderer>().color = Color.gray;
        }
    }
}