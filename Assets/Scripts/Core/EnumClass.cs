using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Stores all enums in the game
public class EnumClass : MonoBehaviour
{
    public enum FighterState
    {
        Idle,
        Motion,
        Action
    }

    public enum FighterAnimations
    {
        Idle,
        ForwardWalk,
        BackwardWalk,
        LeftSideWalk,
        RightSideWalk,
        Jump,
        Crouch,
        LightPunch,
        HeavyPunch,
        LightKick,
        HeavyKick,
        CrouchKick,
        HeadHit,
        TorsoHit,
        LegHit,
        HeavyHit,
        JumpKick,
        Death, 
        Win,
        UpperCut
    }

    public enum HitPointTypes
    {
        Head,
        Torso,
        Legs
    }

    public enum AttackPointTypes
    {
        None,
        Hands,
        Legs
    }

    public enum PrefabType
    {
        Blood,
        Characters,
        UI,
        Levels
    }

    public enum PlayerTag
    {
        Player1,
        Player2,
        Player3,
        Player4,
        Player5,
        Player6,
        Player7,
        Player8,
        Player9,
        Player10,
        Player11,
        Player12,
        Player13,
        Player14,
        Player15,
        Player16,
        Player17,
        Player18,
        None
    }

    public enum LevelTag
    {
        Level1,
        Level2,
        Level3,
        Level4,
        Level5,
        Level6,
        Level7,
        Level8
    }

    public enum ItemType
    {
        Character,
        Level,
        Skill
    }

    public enum ItemID
    {
        Item1,
        Item2,
        Item3,
        Item4,
        Item5,
        Item6,
        Item7,
        Item8,
        Item9,
        Item10,
        Item11,
        Item12,
        Item13,
        Item14,
        Item15,
        Item16,
        Item17,
        Item18,
        Item19,
        Item20,
        Item21,
        Item22,
        Item23,
        Item24,
        Item25,
        Item26,
        Item27,
        Item28,
        Item29,
        Item30
    }

    public enum ConnectionState
    {
        NotConnected,
        InitialConnection,
        Disconnected,
        Exit
    }

    public enum StatsTypes
    {
        Strength,
        Defense,
        Attack,
        Critical_Chance
    }

    public enum ItemPurchaseState
    {
        NotBought,
        Bought,
        ComingSoon
    }

    public enum SettingsType
    {
        Graphics,
        BackgroundMusic,
        SFX,
        Difficulty,
        SetPlayTime
    }

    public enum PhotonPlayerState
    {
        Free,
        Taken
    }

    public enum PlayerIdentity
    {
        Player1,
        Player2
    }

    public enum FighterMotionStates
    {
        Crouch,
        Jump,
        Motion,
        Attack
    }
}
