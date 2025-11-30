using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

public class Play_UI : MonoBehaviour
{
    // Start is called before the first frame update
    GComponent playing_UI;
    void Start()
    {
        playing_UI = BasicUIMgr.Instance.ShowPanel<GComponent>("GJ_UIPackage", "Main");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
