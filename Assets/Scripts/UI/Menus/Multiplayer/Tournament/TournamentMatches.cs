using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TournamentMatches : Menu
{
    public Button back;
    public Transform tournamentListParent, empty;
    public Text errorText;
    public GameObject loadScreen;
    public float tournamentListParentX;

    private void Start()
    {
        back.onClick.AddListener(() => BackCallback());
        tournamentListParentX = tournamentListParent.GetComponent<RectTransform>().sizeDelta.x;
    }

    private void OnEnable()
    {
        StartCoroutine(TournamentMatchesListQuery());
        MultiplayerLauncher.Instance.PhotonNetworkJoinedLobby += CreateRoom;
        MultiplayerLauncher.Instance.PhotonNetworkRoomCreated += LoadCharacterSelect;

    }

    private void OnDisable()
    {
        DisableTournamentUIData();

        errorText.text = "";
        MultiplayerLauncher.Instance.PhotonNetworkJoinedLobby -= CreateRoom;
        MultiplayerLauncher.Instance.PhotonNetworkRoomCreated -= LoadCharacterSelect;
    }

    private void BackCallback()
    {
        DisableTournamentUIData();
        EventManager.Instance.Click();
        List<string> menuTagNames = new List<string>();
        menuTagNames.Add("header");
        menuTagNames.Add("tournamentJoin");
        LoadMenu(back, menuTagNames);
    }

    private void LoadCharacterSelect(string empty)
    {
        loadScreen.SetActive(false);

        DisableTournamentUIData();
        EventManager.Instance.Click();
        List<string> menuTagNames = new List<string>();
        menuTagNames.Add("header");
        menuTagNames.Add("multiplayerCharacterSelect");
        LoadMenu(back, menuTagNames);
    }

    IEnumerator TournamentMatchesListQuery()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(ApiConstants.apiBaseUrl + "/api/v2/tournaments/" + PlayerPrefs.GetString("TournamentID") + "/player-matches"))
        {
            www.SetRequestHeader("x-auth-token", PlayerPrefs.GetString("TokenID"));
            www.SetRequestHeader("alpha-sec-key", ApiConstants.alphaSecKey);

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                Debug.Log(www.downloadHandler.text);

                errorText.text = www.downloadHandler.text;
            }
            else
            {
                Debug.Log("Tournament match list form sent!");

                //Debug.LogError(www.downloadHandler.text);

                JSONNode jsondata = JSON.Parse(System.Text.Encoding.UTF8.GetString(www.downloadHandler.data));

                Debug.Log(jsondata["data"].Count);
                Debug.Log(jsondata["data"]);


                for (int i = 0; i < jsondata["data"].Count; i++)
                {
                    Debug.Log("succesful");
                    if (i > 4)
                        tournamentListParent.GetComponent<RectTransform>().sizeDelta = new Vector2(tournamentListParent.GetComponent<RectTransform>().sizeDelta.x + 250f, tournamentListParent.GetComponent<RectTransform>().sizeDelta.y);

                    TournamentMatchesModel tournamentMatchModel = FactoryManager.Instance.prefabsFactory.GetItem(FactoryManager.Instance.prefabsFactory.tournamentMatchesModel);
                    tournamentMatchModel.transform.SetParent(tournamentListParent);
                    tournamentMatchModel.transform.localPosition = Vector3.zero;
                    tournamentMatchModel.transform.localScale = Vector3.one;

                    //Debug.Log(jsondata["data"][i]["team_1"]["players"][0]["user"]["username"]);

                    List<string> userNames = new List<string>();
                    userNames.Add(jsondata["data"][i]["team_1"]["players"][0]["user"]["username"].ToString().Replace("\"", ""));
                    userNames.Add(jsondata["data"][i]["team_2"]["players"][0]["user"]["username"].ToString().Replace("\"", ""));

                    string chosenTeam = "";

                    for (int j = 0; j < userNames.Count; j++)
                    {
                        if (userNames[j] == PlayerPrefs.GetString("Username"))
                        {
                            chosenTeam = "team_" + (j + 1).ToString();
                            //Debug.Log(chosenTeam);
                            break;
                        }
                    }
                    
                    tournamentMatchModel.SetUp(userNames[0], userNames[1], chosenTeam, jsondata["data"][i]["id"], jsondata["data"][i][chosenTeam]["players"][0]["user"]["id"], true);

                    tournamentMatchModel.RegisterCallback(() => StartMatch(tournamentMatchModel));

                    if(jsondata["data"][i]["isEnded"])
                    {
                        tournamentMatchModel.startMatch.interactable = false;
                    }
                }

                if (tournamentListParent.childCount == 0)
                {
                    errorText.text = "No Active Match For You.";
                }
            }
        }
    }
    
    IEnumerator StartMatchQuery(string matchID)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(ApiConstants.apiBaseUrl + "/api/v2/tournaments/" + PlayerPrefs.GetString("TournamentID") + "/player-matches/" + matchID + "/start"))
        {
            www.SetRequestHeader("x-auth-token", PlayerPrefs.GetString("TokenID"));
            www.SetRequestHeader("alpha-sec-key", ApiConstants.alphaSecKey);

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                Debug.Log(www.downloadHandler.text);

                errorText.text = www.downloadHandler.text;
            }
            else
            {
                Debug.Log("Tournament match list form sent!");

                //Debug.LogError(www.downloadHandler.text);

                JSONNode jsondata = JSON.Parse(System.Text.Encoding.UTF8.GetString(www.downloadHandler.data));

                if(jsondata["data"] != null)
                {
                    MultiplayerLauncher.Instance.StartMultiplayer();
                    loadScreen.SetActive(true);
                }
                else
                {
                    Debug.Log("null herre");
                }
            }
        }
    }

    private void StartMatch(TournamentMatchesModel _tournamentMatchesModel)
    {
        PlayerPrefs.SetString("MultiplayerMode", "Tournament Mode");

        PlayerPrefs.SetString("TournamentTeam", _tournamentMatchesModel.teamID);
        PlayerPrefs.SetInt("MatchID", _tournamentMatchesModel.matchID);
        PlayerPrefs.SetInt("PlayerTournamentID", _tournamentMatchesModel.playerId);

        StartCoroutine(StartMatchQuery(_tournamentMatchesModel.matchID.ToString()));
    }

    private void CreateRoom()
    {
        MultiplayerLauncher.Instance.CreateOrJoinRoom("Match_" + PlayerPrefs.GetInt("MatchID").ToString());
    }

    private void DisableTournamentUIData()
    {
        for (int i = 0; i < tournamentListParent.childCount; i++)
        {
            TournamentMatchesModel tournamentMatchesModel = tournamentListParent.GetChild(0).GetComponent<TournamentMatchesModel>();

            if (tournamentMatchesModel != null)
            {
                FactoryManager.Instance.prefabsFactory.Recycle(tournamentMatchesModel);
            }

            tournamentMatchesModel.transform.SetParent(empty);

        }

        tournamentListParent.GetComponent<RectTransform>().sizeDelta = new Vector2(tournamentListParentX, tournamentListParent.GetComponent<RectTransform>().sizeDelta.y);
    }
}
