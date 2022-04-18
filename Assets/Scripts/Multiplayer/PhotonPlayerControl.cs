using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PhotonPlayerControl : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        if (!GetComponent<PhotonView>().IsMine)
        {
            GetComponent<FighterControllerBase>().UnSubscribeFromInputs();
            GetComponent<Rigidbody>().isKinematic = true;
            FindObjectOfType<PlayersManager>().AddSpawnedPlayer(gameObject);
        }

        EventManager.Instance.OnPlayerInputReset += ResetInput;

        GetComponent<FighterAnimationController>().AnimationChange += SendAnimation;
        GetComponent<FighterControllerBase>().TakeDamage += SendOtherPlayerDamage;
    }

    public void SendAnimation(int viewID, int animationId)
    {
        GetComponent<PhotonView>().RPC(nameof(ReceiveAnimation), RpcTarget.Others, viewID, animationId);
    }

    [PunRPC]
    private void ReceiveAnimation(int viewID, int animationId)
    {
        if (GetComponent<FighterControllerBase>().isDead)
            return;

        PhotonNetwork.GetPhotonView(viewID).GetComponent<FighterAnimationController>().ProcessedAnimation((EnumClass.FighterAnimations)animationId);
    }

    public void SendOtherPlayerDamage(int currentHealth, EnumClass.HitPointTypes hitPointType, Vector3 bloodSpawnPoint)
    {
        if (GetComponent<PhotonView>().IsMine)
            return;

        GetComponent<PhotonView>().RPC(nameof(SendTakeDamage), RpcTarget.Others, GetComponent<FighterControllerBase>().currentHealth, hitPointType, bloodSpawnPoint);
    }

    [PunRPC]
    public void SendTakeDamage(int currentHealth, int hitPointType, Vector3 _bloodSpawnPoint)
    {
        if (!GetComponent<PhotonView>().IsMine)
            return;

        GetComponent<FighterControllerBase>().TakeDamageMultiplayerParams(currentHealth, (EnumClass.HitPointTypes)hitPointType, _bloodSpawnPoint);
    }

    public void ResetInput()
    {
        if (!GetComponent<PhotonView>().IsMine)
            return;

        GetComponent<FighterControllerBase>().SubscribeToInputs();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        EventManager.Instance.NetworkPlayerLeave(GetComponent<FighterControllerBase>().playerTag);
    }
}
