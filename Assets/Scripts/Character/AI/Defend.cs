using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defend : State
{
    public Defend(BattleSystem battleSystem) : base(battleSystem)
    {

    }

    public override IEnumerator Start()
    {
        Debug.Log("Defending Player");

        if (!BattleSystem.controllerBase.movementController.FighterWithinRange(BattleSystem.controllerBase.movementController.defenseDistance))
        {
            Debug.Log("not close");
            yield return null;
        }

        if (Random.Range(0, 2) == 3)
        {
            
        }
        BattleSystem.controllerBase.movementController.rotate = true;

        BattleSystem.controllerBase.movementController.rotate = true;
        BattleSystem.controllerBase.MoveByInput(BattleSystem.defendKeys[Random.Range(0, BattleSystem.defendKeys.Count)]);

        BattleSystem.movePlayer = true;
        yield return new WaitUntil(() => BattleSystem.controllerBase.locomtionRoutine == null);

        BattleSystem.controllerBase.movementController.rotate = false;

        //Debug.Log("Finished attacking Player");
        //yield return new WaitForSeconds(0.1f);
        BattleSystem.movePlayer = false;
        BattleSystem.controllerBase.movementController.rotate = false;


        if (BattleSystem.controllerBase.movementController.otherFighter == null)
            yield return null;
        else
        {
            if (BattleSystem.controllerBase.movementController.FighterWithinRange(BattleSystem.controllerBase.movementController.defenseDistance))
                BattleSystem.SetState(new AttackTarget(BattleSystem));
            else
                BattleSystem.SetState(new FindPlayer(BattleSystem));
        }
    }
}
