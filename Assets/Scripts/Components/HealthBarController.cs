using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    public string playerTag;
    private Image healthBar;
    private Image healthBarUnder;
    private float currentFillAmount;

    public bool animate, multiplayerMode;

    public float lerpTime = 0.02f;
    private float _lerpTime = 0.02f;

    private void Start()
    {
        healthBar = GetComponent<Image>();
        healthBarUnder = transform.GetChild(0).GetComponent<Image>();
        EventManager.Instance.OnHealthChange += UpdateHealthBar;

        //if (multiplayerMode)
        //{
        //    if (!PhotonNetwork.IsMasterClient)
        //    {
        //        if (playerTag == "player1")
        //        {
        //            playerTag = "player2";
        //        }
        //        else
        //        {
        //            playerTag = "player1";
        //        }
        //    }
        //}
    }

    public void UpdateHealthBar(float normalizedHealth, string _playerTag)
    {
        if (_playerTag == playerTag)
        {
            healthBarUnder.fillAmount = normalizedHealth;
            currentFillAmount = healthBar.fillAmount;
            _lerpTime = 0;
            animate = true;
        }
    }

    private void Update()
    {
        if (animate)
        {
            _lerpTime += lerpTime * Time.deltaTime;
            healthBar.fillAmount = Mathf.Lerp(currentFillAmount, healthBarUnder.fillAmount, _lerpTime);

            if (healthBar.fillAmount == healthBarUnder.fillAmount)
                animate = false;
        }
    }
}
