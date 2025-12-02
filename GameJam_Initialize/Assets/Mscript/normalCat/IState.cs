using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface IState
{
    void OnEnter();

    void OnKeep();


    void OnExit();

}

