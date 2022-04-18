using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTarget : State
{
    public AttackTarget(BattleSystem battleSystem) : base(battleSystem)
    {

    }

    public override IEnumerator Start()
    {
        Debug.Log("Attacking Player");
        BattleSystem.controllerBase.movementController.rotate = true;

        if (BattleSystem.controllerBase.isDead)
        {
            BattleSystem.movePlayer = false;
            BattleSystem.SetState(new Death(BattleSystem));
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);

        if (BattleSystem.controllerBase.attackRoutine == null)
        {
            if (Random.Range(0, 3) <= 2)
            {
                BattleSystem.controllerBase.movementController.rotate = true;
                BattleSystem.controllerBase.AttackByInput(BattleSystem.attackKeys[Random.Range(0, BattleSystem.attackKeys.Count)]);
            }
            else
                BattleSystem.controllerBase.MoveByInput(BattleSystem.movementKeys[Random.Range(0, BattleSystem.movementKeys.Count)]);

            BattleSystem.controllerBase.movementController.rotate = false;

            if (BattleSystem.controllerBase.movementController.otherFighter == null)
                yield return null;
            else
            {
                if (BattleSystem.controllerBase.movementController.FighterWithinRange(BattleSystem.controllerBase.movementController.minimumDistance))
                    BattleSystem.SetState(new AttackTarget(BattleSystem));
                else
                    BattleSystem.SetState(new FindPlayer(BattleSystem));
            }
            BattleSystem.controllerBase.movementController.rotate = false;
        }
        else
        {
            BattleSystem.controllerBase.movementController.rotate = false;

            if (BattleSystem.controllerBase.movementController.otherFighter == null)
                yield return null;
            else
            {
                if (BattleSystem.controllerBase.movementController.FighterWithinRange(BattleSystem.controllerBase.movementController.minimumDistance))
                    BattleSystem.SetState(new AttackTarget(BattleSystem));
                else
                    BattleSystem.SetState(new FindPlayer(BattleSystem));
            }
            BattleSystem.controllerBase.movementController.rotate = false;
        }
    }

}
