using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ArenaContentModel : MonoBehaviour
{
    public Text characterName;
    public Image characterImage;
    public Text characterCostText, errorText;
    public Item item;

    public Button button;

    public void SetUp(Item _item)
    {
        item = _item;
        characterName.text = item.itemTagName;
        characterImage.sprite = item.iconImage;

        switch (item.itemPurchaseState)
        {
            case EnumClass.ItemPurchaseState.NotBought:
                characterCostText.text = item.price.ToString() + " BKB token";
                button.interactable = true;
                break;
            case EnumClass.ItemPurchaseState.Bought:
                characterCostText.text = "Free";
                button.interactable = false;
                break;
            case EnumClass.ItemPurchaseState.ComingSoon:
                characterCostText.text = "Coming Soon";
                button.interactable = false;
                break;
            default:
                break;
        }

        button.onClick.AddListener(() => BuyLogic());
    }

    private void BuyLogic()
    {
        //For testing
        EventManager.Instance.Click();
        button.interactable = false;
        StartCoroutine(ProcessPayment());
    }

    IEnumerator ProcessPayment()
    {
        WWWForm form = new WWWForm();
        form.AddField("amount", item.price);
        form.AddField("walletAddress", PlayerPrefs.GetString("WalletID"));
        form.AddField("hash", "ToPurchaseFromGameStore");

        using (UnityWebRequest www = UnityWebRequest.Post("https://backend.alphakombat.com/api/v2/wallet/withdraw", form))
        {
            www.SetRequestHeader("x-auth-token", PlayerPrefs.GetString("TokenID"));
            www.SetRequestHeader("alpha-sec-key", "f55da6945d6b8676eff0ae15690cc260d3c64d31a8aa7c6ffb665b855aecd80b5b2a1331a3868a8e11289771f3614d0d");

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                Debug.Log(www.downloadHandler.text);

                Debug.Log("check internet connection and try again");
                //errorText.text = "check internet connection and try again";
                button.interactable = true;
            }
            else
            {
                Debug.Log("Form upload complete!");

                Debug.Log(www.downloadHandler.text);

                JSONNode jsondata = JSON.Parse(System.Text.Encoding.UTF8.GetString(www.downloadHandler.data));

                if (jsondata["message"] != "insufficient fund")
                {
                    item.itemPurchaseState = EnumClass.ItemPurchaseState.Bought;
                    characterCostText.text = "Open";
                }
                else
                {
                    errorText.text = jsondata["message"];
                }
            }
        }
    }
}
