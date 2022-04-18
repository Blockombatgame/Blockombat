using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Handles animation clips events
public class FighterAnimationsEvents : MonoBehaviour
{
    FighterControllerBase controllerBase;

    private void Start()
    {
        controllerBase = GetComponentInParent<FighterControllerBase>();
    }

    private void Jump()
    {
        controllerBase.movementController.JumpLogic();
    }
    
    private void Drop()
    {
        //controllerBase.movementController.DropLogic();
    }

    private void Attack(int damageAmount)
    {
        if (controllerBase.hitDetectionController.activeAttackType == EnumClass.AttackPointTypes.Hands)
        {
            controllerBase.ExplosionDamage(controllerBase.hitDetectionController.GetDamageCircleSpawnPosition(EnumClass.AttackPointTypes.Hands), controllerBase.damageRadius, damageAmount);
        }
        else if (controllerBase.hitDetectionController.activeAttackType == EnumClass.AttackPointTypes.Legs)
        {
            controllerBase.ExplosionDamage(controllerBase.hitDetectionController.GetDamageCircleSpawnPosition(EnumClass.AttackPointTypes.Legs), controllerBase.damageRadius, damageAmount);
        }
    }

    private void Block()
    {

    }
}
