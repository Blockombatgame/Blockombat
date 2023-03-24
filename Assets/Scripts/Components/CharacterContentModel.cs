using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CharacterContentModel : MonoBehaviour
{
    public Text characterName;
    public Image characterImage;
    public List<Models.CharacterStatDisplayModel> characterStatDisplayModels = new List<Models.CharacterStatDisplayModel>();
    public Text characterCostText, errorText;
    public Item item;

    public Button button;

    public void SetUp(Item _item)
    {
        item = _item;

        characterName.text = FactoryManager.Instance.prefabsFactory.playersStorageDatas.Where(x => x.playerTag == (EnumClass.PlayerTag)Enum.Parse(typeof(EnumClass.PlayerTag), item.itemTagName)).FirstOrDefault().playerPrefab.GetComponent<FighterControllerBase>().playerName;
        characterImage.sprite = item.iconImage;

        if(PlayerPrefs.GetString(item.itemID.ToString()) == "Bought")
        {
            item.itemPurchaseState = EnumClass.ItemPurchaseState.Bought;
            characterCostText.text = "Open";
        }

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

        for (int i = 0; i < characterStatDisplayModels.Count; i++)
        {
            characterStatDisplayModels[i].statSlider.value = item.characterStats.Where(x => x.statsType == characterStatDisplayModels[i].statsType).FirstOrDefault().statAmount;
            characterStatDisplayModels[i].statSlider.transform.parent.GetChild(0).GetComponent<Text>().text = characterStatDisplayModels[i].statsType.ToString();
        }

        button.onClick.AddListener(() => BuyLogic());
    }

    private void BuyLogic()
    {
        //For testing
        EventManager.Instance.Click();
        button.interactable = false;
        StartCoroutine(ProcessPayment());

        //check unique items.
    }

    IEnumerator ProcessPayment()
    {
        WWWForm form = new WWWForm();
        form.AddField("amount", item.price);
        form.AddField("walletAddress", PlayerPrefs.GetString("WalletID"));
        form.AddField("hash", "ToPurchaseFromGameStore");

        using (UnityWebRequest www = UnityWebRequest.Post(ApiConstants.apiBaseUrl + "/api/v2/wallet/withdraw", form))
        {
            www.SetRequestHeader("x-auth-token", PlayerPrefs.GetString("TokenID"));
            www.SetRequestHeader("alpha-sec-key", ApiConstants.alphaSecKey);

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
                    PlayerPrefs.SetString(item.itemID.ToString(), "Bought");
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
