using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State : MonoBehaviour
{
    protected BattleSystem BattleSystem;

    public State(BattleSystem battleSystem)
    {
        BattleSystem = battleSystem;
    }

    public virtual IEnumerator Start()
    {
        yield break;
    }

    public virtual IEnumerator Attack()
    {
        yield break;
    }

    public virtual IEnumerator Defend()
    {
        yield break;
    }

    public virtual IEnumerator ApproachPlayer()
    {
        yield break;
    }
}
