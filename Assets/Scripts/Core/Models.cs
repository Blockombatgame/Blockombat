using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Models : MonoBehaviour
{
    [System.Serializable]
    public struct AnimationSet
    {
        public AnimationClip animationClip;
        public EnumClass.FighterAnimations animationID;
    }
    
    [System.Serializable]
    public class ShopDisplayData
    {
        public Sprite image;
        public EnumClass.ItemType itemType;
        public Action callback;
    }

    [System.Serializable]
    public struct HitPointsData
    {
        public EnumClass.HitPointTypes hitPointType;
        public Transform hitPoint;
        public BoxCollider triggers;
        public Vector3 scale;
        public Vector3 offset;
    }

    [System.Serializable]
    public struct AttackPointData
    {
        public EnumClass.AttackPointTypes attackPointType;
        public Transform attackPoint;
    }

    [System.Serializable]
    public struct PlayersStorageData
    {
        public EnumClass.PlayerTag playerTag;
        public GameObject playerPrefab;
    }

    [System.Serializable]
    public struct CharacterStat
    {
        public EnumClass.StatsTypes statsType;
        [Range(0,1)]
        public float statAmount;
    }

    [System.Serializable]
    public struct CharacterStatDisplayModel
    {
        public EnumClass.StatsTypes statsType;
        public Slider statSlider;
    }

    [System.Serializable]
    public struct SettingsData
    {
        public EnumClass.SettingsType settingsType;
        public int id;
        public Transform buttonsParent;
    }

    [System.Serializable]
    public struct SceneLoadModel
    {
        public string sceneName;
        public int sceneLoadType;
    }

    [System.Serializable]
    public struct RoundData
    {
        public string playerTagName;
        public int score;
    }

    [System.Serializable]
    public struct MultiplayerLoadingData
    {
        public List<EnumClass.ConnectionState> connectionStates;
        public string loadingWord;
        [TextArea]
        public string hintWord;
    }

    [System.Serializable]
    public struct DuelMatchData
    {
        public string matchName;
        public string otherPlayerName;
        public string betAmount;
    }

    [System.Serializable]
    public class CharacterSelectData
    {
        public EnumClass.PlayerIdentity playerIdentity;
        public string selectedCharacterTag;
        public bool full;
    }
}
