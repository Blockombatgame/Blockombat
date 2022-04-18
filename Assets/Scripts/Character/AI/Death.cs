using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death : State
{
    public Death(BattleSystem battleSystem) : base(battleSystem)
    {

    }

    public override IEnumerator Start()
    {
        Debug.Log("dead");
        yield return null;
    }
}
