using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountDownUI : MonoBehaviour
{
    public Text countDownText;

    private void Start()
    {
        countDownText = GetComponent<Text>();

        EventManager.Instance.OnUpdateCountUI += UpdateCountUI;
    }

    private void UpdateCountUI(int count)
    {
        countDownText.text = count.ToString();
    }
}
