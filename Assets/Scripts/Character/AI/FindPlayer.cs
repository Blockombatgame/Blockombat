using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindPlayer : State
{
    public FindPlayer(BattleSystem battleSystem) : base(battleSystem)
    {

    }

    public override IEnumerator Start()
    {
        //Debug.Log("Approaching Player");
        if(BattleSystem.controllerBase.isDead)
        {
            BattleSystem.movePlayer = false;
            BattleSystem.SetState(new Death(BattleSystem));
            yield return null;
        }

        BattleSystem.movePlayer = true;
        BattleSystem.controllerBase.movementController.rotate = true;


        if (BattleSystem.controllerBase.movementController.otherFighter == null)
        {
            BattleSystem.movePlayer = false;
            yield return null;
        }
        else
        {
            yield return new WaitForSeconds(0.1f);

            if (BattleSystem.controllerBase.movementController.FighterWithinRange(BattleSystem.controllerBase.movementController.minimumDistance))
            {
                BattleSystem.controllerBase.movementController.rotate = false;

                //Debug.Log("Found Player");

                BattleSystem.movePlayer = false;
                BattleSystem.SetState(new AttackTarget(BattleSystem));
            }
            else
            {
                BattleSystem.SetState(new FindPlayer(BattleSystem));
            }
        }
    }
}
