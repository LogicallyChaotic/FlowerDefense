using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IPower
{
    void InitPowerAmount();
    float maxPower { get; set; }
    float curPower { get; set; }

    bool isAlive { get; set; }

    void PowerChange(float hitPoints, bool taken);
}
public interface IState
{
    IEnumerator StartingState();
    IEnumerator DuringState();
    IEnumerator EndingState();

    void StateCheck();
}


