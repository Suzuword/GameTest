using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

public class Play_UI : MonoBehaviour
{
    // Start is called before the first frame update
    GComponent playing_UI;
    Controller skill2controller;

    GameObject player;
    PlayerReadInput_Skill2 playerSkill2;
    void Start()
    {
        player = GameObject.Find("Player");
        playerSkill2 = player.GetComponent<PlayerReadInput_Skill2>();

        playing_UI = BasicUIMgr.Instance.ShowPanel<GComponent>("GJ_UIPackage", "Main");
        skill2controller = playing_UI.GetController("Skill2Ani");

        MusicManager.Instance.PlayMusic("desert", volume: 1f);
        print("ɳĮ");
    }

    // Update is called once per frame
    void Update()
    {
        if (playerSkill2.skillReady == false) StartCoroutine(PlayerSkill2GetCD());

    }

    IEnumerator PlayerSkill2GetCD()
    {
        skill2controller.selectedIndex = 1;
        yield return new WaitForSeconds(playerSkill2.skillCD);
        skill2controller.selectedIndex = 0;
    }
}
