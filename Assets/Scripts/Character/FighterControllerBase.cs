using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void OnTakeDamage(int damage, EnumClass.HitPointTypes hitPointType, Vector3 bloodSpawnPoint);
public delegate void OnAttack();
//Handles any response for the fighters from external scripts
public class FighterControllerBase : LivingEntity
{
    public OnTakeDamage TakeDamage;
    public OnAttack Attack;

    public float damageRadius;
    internal Vector3 bloodSpawnPoint;

    internal FighterAnimationController animationController;
    internal FighterMovementController movementController;
    internal FighterHitDetectionController hitDetectionController;
    internal BattleSystem aIBattleSystem;

    internal float newMotionWaitTime;
    internal Coroutine attackRoutine, locomtionRoutine, jumpRoutine, crouchRoutine, hitRoutine, attackWaitTimeRoutine;
    private bool lockActions;
    internal bool lockControls;

    public bool aiController, displayForUI;
    public float timerAttack;
    public string attackKeys, playerName;

    //audio clip references here

    private void Start()
    {
        currentHealth = maxHealth;

        if (displayForUI)
        {
            return;
        }

        animationController = GetComponent<FighterAnimationController>();
        movementController = GetComponent<FighterMovementController>();
        hitDetectionController = GetComponent<FighterHitDetectionController>();
        aIBattleSystem = GetComponent<BattleSystem>();

        if (!aiController)
        {
            InputManager.Instance.MovementKeyPressed += MoveByInput;
            InputManager.Instance.AttackKeyPressed += AttackByInput;
            InputManager.Instance.AttackKeyPressed += ComboAttacks;
        }
        else
        {
            aIBattleSystem.StartAI();
        }

        if(PlayersManager.Instance != null)
            PlayersManager.Instance.Death += OnDeathEffects;
        TakeDamage += TakeDamageParams;

        EventManager.Instance.OnRoundOver += FighterReset;
    }

    private void Update()
    {
        if(!isDead)
        {
            if(animationController != null)
                animationController.AnimationUpdate();
        }

        if(movementController != null)
            movementController.MovementUpdate();
    }

    public void MoveByInput(string keyPressed)
    {
        if (lockControls)
            return;

        switch (keyPressed)
        {
            case "W":
                WalkDirection(1, 0, 1, true, EnumClass.FighterAnimations.ForwardWalk);
                break;
            case "S":
                WalkDirection(-1, 0, 1, true, EnumClass.FighterAnimations.BackwardWalk);
                break;
            case "A":
                WalkDirection(-1, 1, 0, false, EnumClass.FighterAnimations.LeftSideWalk);
                break;
            case "D":
                WalkDirection(1, 1, 0, false, EnumClass.FighterAnimations.RightSideWalk);
                break;
            case "C":
                Crouch();
                break;
            case "Space":
                Jump();
                break;
        }
    }

    public void AttackByInput(string keyPressed)
    {
        if (lockControls)
            return;

        if (attackRoutine == null && !lockActions)
        {
            CheckActionKeyPress(keyPressed);
        }
    }

    public void ComboAttacks(string keyPressed)
    {
        if (lockControls)
            return;

        attackKeys += keyPressed;

        if (attackWaitTimeRoutine != null)
        {
            StopCoroutine(attackWaitTimeRoutine);
            attackWaitTimeRoutine = StartCoroutine(AttackWaitTimeSequence());
        }else if(attackWaitTimeRoutine == null)
        {
            attackWaitTimeRoutine = StartCoroutine(AttackWaitTimeSequence());
        }
    }

    private void CheckActionKeyPress(string keyPressed)
    {
        if (attackRoutine != null)
            StopCoroutine(attackRoutine);

        switch (keyPressed)
        {
            case "U":
                hitDetectionController.activeAttackType = EnumClass.AttackPointTypes.Hands;

                attackRoutine = StartCoroutine(AttackSequence(EnumClass.FighterAnimations.LightPunch));
                break;
            case "I":
                hitDetectionController.activeAttackType = EnumClass.AttackPointTypes.Hands;

                attackRoutine = StartCoroutine(AttackSequence(EnumClass.FighterAnimations.HeavyPunch));
                break;
            case "O":
                hitDetectionController.activeAttackType = EnumClass.AttackPointTypes.Legs;

                attackRoutine = StartCoroutine(AttackSequence(EnumClass.FighterAnimations.LightKick));
                break;
            case "P":
                hitDetectionController.activeAttackType = EnumClass.AttackPointTypes.Legs;

                attackRoutine = StartCoroutine(AttackSequence(EnumClass.FighterAnimations.HeavyKick));
                break;
            case "ISpace":
                if(FactoryManager.Instance.itemsFactory.GetItem("IO").itemPurchaseState == EnumClass.ItemPurchaseState.Bought)
                {
                    hitDetectionController.activeAttackType = EnumClass.AttackPointTypes.Legs;

                    attackRoutine = StartCoroutine(AttackSequence(EnumClass.FighterAnimations.JumpKick));
                }
                else
                {
                    movementController.UnlockMovement();
                    animationController.UnlockIdleAnimation();
                    hitDetectionController.activeAttackType = EnumClass.AttackPointTypes.None;
                    lockActions = false;
                    attackKeys = "";
                    attackRoutine = null;
                    Debug.Log("buy skill");
                }
                break;
            case "SpaceI":
                if(FactoryManager.Instance.itemsFactory.GetItem("IO").itemPurchaseState == EnumClass.ItemPurchaseState.Bought)
                {
                    hitDetectionController.activeAttackType = EnumClass.AttackPointTypes.Legs;

                    attackRoutine = StartCoroutine(AttackSequence(EnumClass.FighterAnimations.JumpKick));
                }
                else
                {
                    movementController.UnlockMovement();
                    animationController.UnlockIdleAnimation();
                    hitDetectionController.activeAttackType = EnumClass.AttackPointTypes.None;
                    lockActions = false;
                    attackKeys = "";
                    attackRoutine = null;
                    Debug.Log("buy skill");
                }
                break;
            case "II":
                if (FactoryManager.Instance.itemsFactory.GetItem("II").itemPurchaseState == EnumClass.ItemPurchaseState.Bought)
                {
                    hitDetectionController.activeAttackType = EnumClass.AttackPointTypes.Hands;

                    attackRoutine = StartCoroutine(AttackSequence(EnumClass.FighterAnimations.UpperCut));
                }
                else
                {
                    movementController.UnlockMovement();
                    animationController.UnlockIdleAnimation();
                    hitDetectionController.activeAttackType = EnumClass.AttackPointTypes.None;
                    lockActions = false;
                    attackKeys = "";
                    attackRoutine = null;
                    Debug.Log("buy skill");
                }
                break;
            case "IU":
                if (FactoryManager.Instance.itemsFactory.GetItem("IU").itemPurchaseState == EnumClass.ItemPurchaseState.Bought)
                {
                    hitDetectionController.activeAttackType = EnumClass.AttackPointTypes.Legs;

                    attackRoutine = StartCoroutine(AttackSequence(EnumClass.FighterAnimations.CrouchKick));
                }
                else
                {
                    movementController.UnlockMovement();
                    animationController.UnlockIdleAnimation();
                    hitDetectionController.activeAttackType = EnumClass.AttackPointTypes.None;
                    lockActions = false;
                    attackKeys = "";
                    attackRoutine = null;
                    Debug.Log("buy skill");
                }
                break;
            default:
                movementController.UnlockMovement();
                animationController.UnlockIdleAnimation();
                hitDetectionController.activeAttackType = EnumClass.AttackPointTypes.None;
                lockActions = false;
                attackKeys = "";
                attackRoutine = null;
                Debug.Log("No attack mapping to this key");
                break;
        }
    }

    public void WalkDirection(float walkDirection, float xAxis, float yAxis, bool canSet, EnumClass.FighterAnimations animationID)
    {
        OverrideCrouchLogic();

        if (locomtionRoutine == null && !movementController.lockMovement)
        {
            movementController.isMoving = true;
            locomtionRoutine = StartCoroutine(WalkSequence(animationID));


            movementController._walkDirection = walkDirection;
            movementController._xAxis = xAxis;
            movementController._yAxis = yAxis;
            movementController._canSet = canSet;
        }
    }

    public void Jump()
    {
        lockActions = true;

        if (canJumpOrCrouch() && attackRoutine == null)
        {
            jumpRoutine = StartCoroutine(JumpSequence());
        }
    }

    public void Crouch()
    {
        lockActions = true;

        if (canJumpOrCrouch() && locomtionRoutine == null && attackRoutine == null && jumpRoutine == null && hitRoutine == null)
        {
            crouchRoutine = StartCoroutine(CrouchSeqeunce());
        }
    }

    public void TakeHit(EnumClass.HitPointTypes hitPointType)
    {
        lockActions = true;
        if(hitRoutine != null)
            StopCoroutine(hitRoutine);

        if (currentHealth > 0)
            hitRoutine = StartCoroutine(HitSequence(PlayHitAnimation(hitPointType)));
    }

    public EnumClass.FighterAnimations PlayHitAnimation(EnumClass.HitPointTypes hitPointType)
    {
        EnumClass.FighterAnimations _hitPointType = EnumClass.FighterAnimations.Idle;

        switch (hitPointType)
        {
            case EnumClass.HitPointTypes.Head:
                //Spawn Audio here
                _hitPointType = EnumClass.FighterAnimations.HeadHit;
                break;
            case EnumClass.HitPointTypes.Torso:
                //Spawn Audio here
                _hitPointType = EnumClass.FighterAnimations.TorsoHit;
                break;
            case EnumClass.HitPointTypes.Legs:
                //Spawn Audio here
                _hitPointType = EnumClass.FighterAnimations.LegHit;
                break;
            default:
                break;
        }

        return _hitPointType;
    }

    private void OverrideMovementLogic()
    {
        if (locomtionRoutine != null)
            StopCoroutine(locomtionRoutine);
        locomtionRoutine = null;
        movementController.isMoving = false;
    }

    private void OverrideCrouchLogic()
    {
        if (crouchRoutine != null)
        {
            StopCoroutine(crouchRoutine);
            movementController.UnlockMovement();
            animationController.UnlockIdleAnimation();
            movementController._currentDirection = Vector3.zero;
            crouchRoutine = null;
        }
    }

    private void OverrideAttackLogic()
    {
        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
            movementController._currentDirection = Vector3.zero;
            hitDetectionController.activeAttackType = EnumClass.AttackPointTypes.None;
            attackRoutine = null;
        }

        if (attackWaitTimeRoutine != null)
        {
            StopCoroutine(attackWaitTimeRoutine);
            attackKeys = "";
            attackWaitTimeRoutine = null;
        }
    }
    
    private void OverrideJumpLogic()
    {
        if (jumpRoutine != null)
        {
            StopCoroutine(jumpRoutine);
            movementController._currentDirection = Vector3.zero;
            jumpRoutine = null;
        }
    }

    private bool canJumpOrCrouch()
    {
        if (movementController.isGrounded() && jumpRoutine == null && crouchRoutine == null)
            return true;

        return false;
    }

    public void ExplosionDamage(List<Vector3> center, float radius, int damage)
    {
        bool hit = false;

        for (int l = 0; l < center.Count; l++)
        {
            Collider[] hitColliders = Physics.OverlapSphere(center[l], radius);

            for (int i = 0; i < hitColliders.Length; i++)
            {
                if (hitColliders[i].isTrigger)
                {
                    if (hitColliders[i].GetComponentInParent<LivingEntity>() != null && hitColliders[i].GetComponentInParent<LivingEntity>() != this)
                    {
                        hitColliders[i].GetComponentInParent<FighterControllerBase>().TakeDamage?.Invoke(damage, hitDetectionController.GetHitPointType(hitColliders[i]), hitColliders[i].transform.position + (transform.forward * 0.5f));
                        hit = true;
                        break;
                    }
                }
            }
        }

        if(!hit)
        {
            if (GetComponent<ReactionAudio>() != null)
                GetComponent<ReactionAudio>().PlaySound("miss");
        }
    }

    public void TakeDamageParams(int damage, EnumClass.HitPointTypes hitPointType, Vector3 _bloodSpawnPoint)
    {
        if (GetComponent<ReactionAudio>() != null)
            GetComponent<ReactionAudio>().PlaySound("hit");

        bloodSpawnPoint = _bloodSpawnPoint;
        if (GetComponent<AttackAudio>() != null)
            GetComponent<AttackAudio>().PlayGruntSound();

        if (movementController._walkDirection != -1)
        {
            OnDamage(damage);
            TakeHit(hitPointType);
        }
        else
        {
            Debug.Log("Block");
            //spawn block effect
        }
    }
    
    public void TakeDamageMultiplayerParams(int currentHealth, EnumClass.HitPointTypes hitPointType, Vector3 _bloodSpawnPoint)
    {
        //Debug.Log("taking hit");
        animationController.LockIdleAnimation();
        movementController.LockMovement();
        
        animationController.ProcessedAnimation(PlayHitAnimation(hitPointType));
        lockControls = true;

        SpawnBlood();


        if (hitRoutine != null)
            StopCoroutine(hitRoutine);

        hitRoutine = StartCoroutine(UnlockAfterHit(hitPointType));

        if (GetComponent<ReactionAudio>() != null)
            GetComponent<ReactionAudio>().PlaySound("hit");

        bloodSpawnPoint = _bloodSpawnPoint;
        if (GetComponent<AttackAudio>() != null)
            GetComponent<AttackAudio>().PlayGruntSound();

        movementController.PushBackwardsLogic();

        if (movementController._walkDirection != -1)
        {
            UpdateHealth(currentHealth);
        }
        else
        {
            Debug.Log("Block");
            //spawn block effect
        }
    }

    IEnumerator UnlockAfterHit(EnumClass.HitPointTypes hitPointType)
    {
        Models.AnimationSet? animationSet = animationController.fighterAnimationsSet.GetAnimation(PlayHitAnimation(hitPointType));
        yield return new WaitForSeconds(animationSet.Value.animationClip.length - 0.8f);
        animationController.UnlockIdleAnimation();
        movementController.UnlockMovement();
        lockControls = false;

        hitRoutine = null;
    }

    internal void SetWaitTime(float _waitTime) { newMotionWaitTime = _waitTime; }

    public IEnumerator WalkSequence(EnumClass.FighterAnimations animationID)
    {
        animationController.PlayAnimation(animationID, SetWaitTime);
        yield return new WaitForSeconds(newMotionWaitTime);
        locomtionRoutine = null;
        animationController.UnlockIdleAnimation();
        movementController.isMoving = false;
        movementController._currentDirection = Vector3.zero;
    }

    public IEnumerator JumpSequence()
    {
        movementController._jumpMoveDirection = movementController._currentDirection;
        movementController.LockMovement();
        OverrideMovementLogic();
        OverrideCrouchLogic();
        animationController.PlayAnimation(EnumClass.FighterAnimations.Jump, SetWaitTime);
        yield return new WaitForSeconds(newMotionWaitTime);
        movementController.UnlockMovement();
        animationController.UnlockIdleAnimation();
        movementController._currentDirection = Vector3.zero;
        lockActions = false;

        jumpRoutine = null;
    }

    public IEnumerator AttackSequence(EnumClass.FighterAnimations animationID)
    {
        if (!aiController)
            Attack?.Invoke();


        if (GetComponent<AttackAudio>() != null)
            GetComponent<AttackAudio>().PlayAttackSound();
        movementController.LockMovement();
        OverrideMovementLogic();
        OverrideJumpLogic();

        if (movementController.isGrounded() && jumpRoutine == null)
            movementController.PushForwardLogic();

        animationController.PlayAnimation(animationID, SetWaitTime);
        yield return new WaitForSeconds(newMotionWaitTime);
        movementController.UnlockMovement();
        animationController.UnlockIdleAnimation();
        hitDetectionController.activeAttackType = EnumClass.AttackPointTypes.None;
        lockActions = false;
        attackKeys = "";
        attackRoutine = null;
    }
    
    IEnumerator HitSequence(EnumClass.FighterAnimations animationID)
    {
        //Spawn blood
        SpawnBlood();
        movementController.LockMovement();
        OverrideMovementLogic();
        OverrideCrouchLogic();
        OverrideAttackLogic();
        movementController.PushBackwardsLogic();
        animationController.PlayAnimation(animationID, SetWaitTime);
        yield return new WaitForSeconds(newMotionWaitTime - 0.8f);
        movementController.UnlockMovement();
        animationController.UnlockIdleAnimation();
        attackKeys = "";
        lockActions = false;
        hitRoutine = null;
    }

    public IEnumerator CrouchSeqeunce()
    {
        movementController.LockMovement();
        OverrideMovementLogic();
        animationController.PlayAnimation(EnumClass.FighterAnimations.Crouch, SetWaitTime);
        yield return new WaitForSeconds(newMotionWaitTime);
        movementController.UnlockMovement();
        animationController.UnlockIdleAnimation();
        movementController._currentDirection = Vector3.zero;
        lockActions = false;
        crouchRoutine = null;
    }

    IEnumerator AttackWaitTimeSequence()
    {
        yield return new WaitForSeconds(timerAttack);
        if(attackKeys.Length > 1)
            CheckActionKeyPress(attackKeys);
        attackWaitTimeRoutine = null;
    }

    public void SpawnBlood()
    {
        GameObject blood = FactoryManager.Instance.prefabsFactory.GetItem<BloodIdentity>(FactoryManager.Instance.prefabsFactory.bloodEffect).gameObject;
        blood.SetActive(true);
        blood.transform.position = bloodSpawnPoint;
        blood.transform.rotation = Quaternion.Euler(0, 0, -90);
    }

    public void SubscribeToInputs()
    {
        InputManager.Instance.MovementKeyPressed += MoveByInput;
        InputManager.Instance.AttackKeyPressed += AttackByInput;
        InputManager.Instance.AttackKeyPressed += ComboAttacks;
    }

    public void UnSubscribeFromInputs()
    {
        InputManager.Instance.MovementKeyPressed -= MoveByInput;
        InputManager.Instance.AttackKeyPressed -= AttackByInput;
        InputManager.Instance.AttackKeyPressed -= ComboAttacks;
    }

    public void OnDeathEffects(FighterControllerBase player)
    {
        //What happens on death

        //Lock input controls and movement controls
        movementController.LockMovement();
        lockControls = true;

        //UnSubscribeFromInputs();

        if (aiController)
        {
            aIBattleSystem.StopAI();
            aIBattleSystem.movePlayer = false;
        }
        //death and win animations
        if (player == this)
        {
            StopAllCoroutines();
            SpawnBlood();
            movementController.PushBackwardsLogic();

            PhotonPlayerControl photonPlayerControl = GetComponent<PhotonPlayerControl>();

            if(photonPlayerControl != null)
            {
                animationController.ProcessedAnimation(EnumClass.FighterAnimations.Death);
            }
            else
            {
                animationController.PlayAnimation(EnumClass.FighterAnimations.Death, null);
            }
        }
        else
        {
            StopAllCoroutines();
            animationController.PlayAnimation(EnumClass.FighterAnimations.Win, null);
            EventManager.Instance.RoundFinished(playerTag);
        }
    }

    public void FighterReset()
    {
        EventManager.Instance.RoundReset(this.gameObject);
    }
}
