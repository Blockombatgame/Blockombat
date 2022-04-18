using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : StateMachine
{
    internal FighterControllerBase controllerBase;
    internal bool movePlayer;

    public List<string> movementKeys = new List<string>();
    public List<string> attackKeys = new List<string>();
    public List<string> defendKeys = new List<string>();

    private void Start()
    {
        controllerBase = GetComponent<FighterControllerBase>();

        //movementKeys.Add("W");
        //movementKeys.Add("S");
        movementKeys.Add("A");
        movementKeys.Add("D");

        attackKeys.Add("Space");
        movementKeys.Add("C");
        attackKeys.Add("I");
        attackKeys.Add("O");
        attackKeys.Add("U");
        attackKeys.Add("P");

        defendKeys.Add("S");
        defendKeys.Add("A");
        defendKeys.Add("D");

        foreach (var skill in FactoryManager.Instance.itemsFactory.GetItems(EnumClass.ItemType.Skill))
        {
            if (skill.itemPurchaseState == EnumClass.ItemPurchaseState.Bought)
            {
                attackKeys.Add(skill.itemTagName);
            }
        }

        if (!controllerBase.displayForUI && controllerBase.aiController && controllerBase.movementController.otherFighter != null)//check for null
            controllerBase.movementController.otherFighter.GetComponent<FighterControllerBase>().Attack += Defend; 
    }

    public void StartAI()
    {
        if(controllerBase == null)
            controllerBase = GetComponent<FighterControllerBase>();

        if (controllerBase.aiController)
        {
            controllerBase = GetComponent<FighterControllerBase>();

            SetState(new FindPlayer(this));
        }
    }

    public void Defend()
    {
        if (controllerBase.aiController)
        {
            SetState(new Defend(this));
        }
    }

    public void StopAI()
    {
        movePlayer = false;
        controllerBase.movementController.rotate = false;

        StopAllCoroutines();
    }

    public void MovePlayer()
    {
        controllerBase.movementController.rotate = true;
        controllerBase.MoveByInput("W");
    }

    private void Update()
    {
        if (controllerBase.aiController)
        {
            if (movePlayer)
                MovePlayer();
        }
    }
}
