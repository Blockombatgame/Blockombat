using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TournamentCreate : Menu
{
    public InputField tournamentName, enddateData, startdateData, maxPlayers;
    public Button back, proceed;
    public Text errorText;
    public GameObject formPanel, congrats;

    private void Start()
    {
        back.onClick.AddListener(() => BackCallback());
        proceed.onClick.AddListener(() => CreateCallback());
    }

    private void OnEnable()
    {
        proceed.gameObject.SetActive(true);
        formPanel.SetActive(true);
        congrats.SetActive(false);
    }

    private void BackCallback()
    {
        EventManager.Instance.Click();
        List<string> menuTagNames = new List<string>();
        menuTagNames.Add("header");
        menuTagNames.Add("home");
        LoadMenu(back, menuTagNames);
    }

    private void CreateCallback()
    {
        if (!CheckDatePattern(startdateData.text) /*|| startdateData.text == ""*/)
        {
            errorText.text = "Check information on start date.";
            return;
        }

        if (!CheckDatePattern(enddateData.text) || enddateData.text == "")
        {
            errorText.text = "Check information on end date.";
            return;
        }

        if(tournamentName.text.Length < 3)
        {
            errorText.text = "Make sure tournament name is longer than 3 characters.";
            return;
        }

        if (int.Parse(maxPlayers.text) < 0 || int.Parse(maxPlayers.text) > 513)
        {
            errorText.text = "Max have exceeded limit of 512.";
            return;
        }

        StartCoroutine(CreateTournament(tournamentName.text, startdateData.text, enddateData.text, int.Parse(maxPlayers.text)));
    }

    private bool CheckDatePattern(string dateString)
    {
        if (dateString.Length < 10)
            return false;

        for (int i = 0; i < dateString.Length; i++)
        {
            if (i == 4 || i == 7)
            {
                if (dateString[i].ToString() != "-")
                {
                    return false;
                }
            }
            else
            {
                if (dateString[i].ToString() == "")
                    return false;

                bool isNumeric = int.TryParse(dateString[i].ToString(), out _);
                if (!isNumeric)
                {
                    return false;
                }

                if(i == 6)
                {
                    string number = dateString[i - 1].ToString() + dateString[i].ToString();
                    if (int.Parse(number) > 12)
                        return false;
                }

                if (i == 9)
                {
                    string number = dateString[i - 1].ToString() + dateString[i].ToString();
                    if (int.Parse(number) > 31)
                        return false;
                }
            }
        }

        return true;
    }

    IEnumerator CreateTournament(string _tournamentName, string _startDate, string _endDate, int maxPlayers)
    {
        WWWForm form = new WWWForm();
        form.AddField("name", _tournamentName);
        form.AddField("end_date", _endDate);
        form.AddField("start_date", _startDate);
        form.AddField("total_players_number", maxPlayers.ToString());

        using (UnityWebRequest www = UnityWebRequest.Post(ApiConstants.apiBaseUrl + "/api/v2/tournaments", form))
        {
            www.SetRequestHeader("x-auth-token", PlayerPrefs.GetString("TokenID"));
            www.SetRequestHeader("alpha-sec-key", ApiConstants.alphaSecKey);

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                Debug.Log(www.downloadHandler.text);

                errorText.text = www.downloadHandler.text;
                proceed.interactable = true;
            }
            else
            {
                Debug.Log("Tournament Creation complete!");

                Debug.LogError(www.downloadHandler.text);

                formPanel.SetActive(false);
                congrats.SetActive(true);
                proceed.gameObject.SetActive(false);
            }
        }
    }
}
