using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//handles global events
public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public event Action<float, string> OnHealthChange;

    public void HealthChange(float normalizedHealth, string playerTag)
    {
        OnHealthChange?.Invoke(normalizedHealth, playerTag);
    }

    public event Action<int, string> OnRoundSlotChange;

    public void RoundSlotChange(int score, string playerTag)
    {
        OnRoundSlotChange?.Invoke(score, playerTag);
    }

    public event Action OnLoadFadePanel;

    public void LoadFadePanel()
    {
        OnLoadFadePanel?.Invoke();
    }

    public event Action<int> OnUpdateCountUI;

    public void UpdateCountUI(int count)
    {
        OnUpdateCountUI?.Invoke(count);
    }

    public event Action<List<string>> OnMenuChange;

    public void MenuChange(List<string> menuTagNames)
    {
        OnMenuChange?.Invoke(menuTagNames);
    }

    public event Action<EnumClass.SettingsType, int> OnSettingTypeChange;

    public void SettingTypeChange(EnumClass.SettingsType settingsType, int selectionID)
    {
        OnSettingTypeChange?.Invoke(settingsType, selectionID);
    }

    public event Action<string> OnCharacterSelect;

    public void CharacterSelect(string characterName)
    {
        OnCharacterSelect?.Invoke(characterName);
    }

    public event Action<int> OnArenaSelect;

    public void ArenaSelect(int selectionID)
    {
        OnArenaSelect?.Invoke(selectionID);
    }

    public event Action<string> OnWinGame;

    public void WinGame(string playerName)
    {
        OnWinGame?.Invoke(playerName);
    }

    public event Action<string> OnRoundFinished;

    public void RoundFinished(string playerTagName)
    {
        OnRoundFinished?.Invoke(playerTagName);
    }
    
    public event Action<GameObject> OnRoundReset;

    public void RoundReset(GameObject player)
    {
        OnRoundReset?.Invoke(player);
    }
    
    public event Action OnRoundOver;

    public void RoundOver()
    {
        OnRoundOver?.Invoke();
    }

    public event Action OnClick;

    public void Click()
    {
        OnClick?.Invoke();
    }

    public event Action OnPlayBgAudio;

    public void PlayBgAudio()
    {
        OnPlayBgAudio?.Invoke();
    }

    public event Action OnSettingsDataChanged;

    public void SettingsDataChanged()
    {
        OnSettingsDataChanged?.Invoke();
    }

    public event Action OnTimerEndGame;

    public void TimerEndGame()
    {
        OnTimerEndGame?.Invoke();
    }

    public event Action<string> OnNetworkCreateRoom;

    public void NetworkCreateRoom(string roomName)
    {
        OnNetworkCreateRoom?.Invoke(roomName);
    }

    public event Action OnNetworkLeaveRoom;

    public void NetworkLeaveRoom()
    {
        OnNetworkLeaveRoom?.Invoke();
    }

    public event Action<Action, Action, string, string> OnOpenPopMenuAction;

    public void OpenPopMenuAction(Action acceptCallback, Action declineCallback, string hostName, string stakeAmount)
    {
        OnOpenPopMenuAction?.Invoke(acceptCallback, declineCallback, hostName, stakeAmount);
    }

    public event Action OnClosePopMenu;

    public void ClosePopMenu()
    {
        OnClosePopMenu?.Invoke();
    }

    public event Action OnWaitRoomLoading;

    public void WaitRoomLoading()
    {
        OnClosePopMenu?.Invoke();
    }
    
    public event Action<EnumClass.PlayerIdentity, string> OnCharacterDataAdded;

    public void CharacterDataAdded(EnumClass.PlayerIdentity playerIdentity, string selectedCharacterTag)
    {
        OnCharacterDataAdded?.Invoke(playerIdentity, selectedCharacterTag);
    }
    
    public event Action OnFactoryReset;

    public void FactoryReset()
    {
        OnFactoryReset?.Invoke();
    }

    //public event Action<int, int, Vector3> OnDamageEventFired;

    //public void DamageEventFired(int damage, EnumClass.HitPointTypes hitPointType, Vector3 bloodSpawnPoint)
    //{
    //    OnDamageEventFired?.Invoke(damage, (int)hitPointType, bloodSpawnPoint);
    //}

    public event Action OnPlayerInputReset;

    public void PlayerInputReset()
    {
        OnPlayerInputReset?.Invoke();
    }

    public event Action<string> OnNetworkPlayerWin;

    public void NetworkPlayerWin(string playerTag)
    {
        OnNetworkPlayerWin?.Invoke(playerTag);
    }

    public event Action<string> OnNetworkPlayerLost;

    public void NetworkPlayerLost(string playerTag)
    {
        OnNetworkPlayerLost?.Invoke(playerTag);
    }

    public event Action<string> OnNetworkPlayerLeave;

    public void NetworkPlayerLeave(string playerTag)
    {
        OnNetworkPlayerLeave?.Invoke(playerTag);
    }
}
