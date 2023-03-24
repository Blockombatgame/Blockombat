using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TournamentRegister : Menu
{
    public Button back;
    public Transform tournamentListParent, empty;
    public Text errorText;
    public float tournamentListParentX;

    private void Start()
    {
        back.onClick.AddListener(() => BackCallback());
        tournamentListParentX = tournamentListParent.GetComponent<RectTransform>().sizeDelta.x;
    }

    private void OnEnable()
    {
        StartCoroutine(TournamentListQuery());
    }

    private void BackCallback()
    {
        DisableTournamentUIData();
        EventManager.Instance.Click();
        List<string> menuTagNames = new List<string>();
        menuTagNames.Add("header");
        menuTagNames.Add("tournamentStart");
        LoadMenu(back, menuTagNames);
    }

    private void OnDisable()
    {
        DisableTournamentUIData();

        errorText.text = "";
    }

    IEnumerator TournamentListQuery()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(ApiConstants.apiBaseUrl + "/api/v2/tournaments"))
        {
            www.SetRequestHeader("x-auth-token", PlayerPrefs.GetString("TokenID"));
            www.SetRequestHeader("alpha-sec-key", ApiConstants.alphaSecKey);

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                Debug.Log(www.downloadHandler.text);

                //errorText.text = "check internet connection and try again";
            }
            else
            {
                Debug.Log("Tournament list form sent!");

                //Debug.LogError(www.downloadHandler.text);

                JSONNode jsondata = JSON.Parse(System.Text.Encoding.UTF8.GetString(www.downloadHandler.data));

                //Debug.Log(jsondata["data"][0]);

                for (int i = 0; i < jsondata["data"].Count; i++)
                {
                    if (i > 4)
                        tournamentListParent.GetComponent<RectTransform>().sizeDelta = new Vector2(tournamentListParent.GetComponent<RectTransform>().sizeDelta.x + 250f, tournamentListParent.GetComponent<RectTransform>().sizeDelta.y);

                    TournamentContentModel tournamentContentModel = FactoryManager.Instance.prefabsFactory.GetItem(FactoryManager.Instance.prefabsFactory.tournamentContentModel);
                    tournamentContentModel.transform.SetParent(tournamentListParent);
                    tournamentContentModel.transform.localPosition = Vector3.zero;
                    tournamentContentModel.transform.localScale = Vector3.one;
                    tournamentContentModel.SetUp(jsondata["data"][i]["name"], jsondata["data"][i]["joinedPlayerNumber"], false, jsondata["data"][i]["id"]);
                    tournamentContentModel.register.gameObject.SetActive(true);

                    //Debug.Log(jsondata["message"][i]["id"]);
                    //PlayerPrefs.SetString("TournamentID", jsondata["message"][i]["id"]);
                    tournamentContentModel.RegisterCallback(() => RegisterTournament(tournamentContentModel));
                }

                if (tournamentListParent.childCount == 0)
                {
                    errorText.text = "No Active Tournament.";
                }
            }
        }
    }

    public void RegisterTournament(TournamentContentModel tournamentContentModel)
    {
        tournamentContentModel.register.gameObject.SetActive(false);
        tournamentContentModel.Registered.gameObject.SetActive(true);
    }

    private void DisableTournamentUIData()
    {
        for (int i = 0; i < tournamentListParent.childCount; i++)
        {
            TournamentContentModel tournamentContentModel = tournamentListParent.GetChild(0).GetComponent<TournamentContentModel>();

            if (tournamentContentModel != null)
            {
                FactoryManager.Instance.prefabsFactory.Recycle(tournamentContentModel);
            }

            tournamentContentModel.transform.SetParent(empty);
        }

        tournamentListParent.GetComponent<RectTransform>().sizeDelta = new Vector2(tournamentListParentX, tournamentListParent.GetComponent<RectTransform>().sizeDelta.y);
    }
}
