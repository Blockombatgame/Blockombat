using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using SimpleJSON;
using System;

public class TournamentJoin : Menu
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
        DisableTournamentUIData();

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

    private void SwitchToTournamentMatchesMenu()
    {
        DisableTournamentUIData();
        EventManager.Instance.Click();
        List<string> menuTagNames = new List<string>();
        menuTagNames.Add("header");
        menuTagNames.Add("tournamentMatches");
        LoadMenu(back, menuTagNames);
    }

    private void LoadMatchesCallback()
    {
        EventManager.Instance.Click();
        List<string> menuTagNames = new List<string>();
        menuTagNames.Add("header");
        menuTagNames.Add("tournamentMatches");
        LoadMenu(null, menuTagNames);
    }

    private void PrintComingSoon()
    {
        errorText.text = "Coming Soon";
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



                for (int i = 0; i < jsondata["data"].Count; i++)
                {
                    if (CanStartTournament(jsondata["data"][i]["startDate"]) && jsondata["data"][i]["isActive"])
                    {
                        if (i > 4)
                            tournamentListParent.GetComponent<RectTransform>().sizeDelta = new Vector2(tournamentListParent.GetComponent<RectTransform>().sizeDelta.x + 250f, tournamentListParent.GetComponent<RectTransform>().sizeDelta.y);

                        TournamentContentModel tournamentContentModel = FactoryManager.Instance.prefabsFactory.GetItem(FactoryManager.Instance.prefabsFactory.tournamentContentModel);
                        tournamentContentModel.transform.SetParent(tournamentListParent);
                        tournamentContentModel.transform.localPosition = Vector3.zero;
                        tournamentContentModel.transform.localScale = Vector3.one;
                        tournamentContentModel.SetUp(jsondata["data"][i]["name"], jsondata["data"][i]["joinedPlayerNumber"], false, jsondata["data"][i]["id"]);

                        //Debug.Log(jsondata["message"][i]["id"]);
                        //PlayerPrefs.SetString("TournamentID", jsondata["message"][i]["id"]);
                        tournamentContentModel.RegisterCallback(() => JoinTournament(tournamentContentModel.tournamentID));
                    }
                }

                if (tournamentListParent.childCount == 0)
                {
                    errorText.text = "No Active Tournament.";
                }
            }
        }
    }

    public void JoinTournament(string tournamentId)
    {
        PlayerPrefs.SetString("TournamentID", tournamentId);
        LoadMatchesCallback();
    }

    private bool CanStartTournament(string dateString)
    {
        for (int i = 0; i < dateString.Length; i++)
        {
            if (i == 9)
            {
                string number = dateString[i - 1].ToString() + dateString[i].ToString();
                if (DateTime.Now.Day - int.Parse(number) < 0)
                    return true;
            }
        }

        return false;
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
