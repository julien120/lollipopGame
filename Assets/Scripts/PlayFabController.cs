using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

public class PlayFabController : MonoBehaviour
{
    public string email;
    public static string name;
    public string pass;
    [SerializeField] InputField userNameText;
    [SerializeField] InputField passText;
    [SerializeField] InputField emailText;
    [SerializeField] GameObject loginCanvas;
    [SerializeField] GameObject signUpCanvas;

    //↓練習&試行錯誤中
    private void SubmitUserData()
    {
        PlayFabClientAPI.LoginWithCustomID(
        new LoginWithCustomIDRequest
        {
            TitleId = PlayFabSettings.TitleId,
            CustomId = "100", //TODO:固有の番号を与えないとだめ
            CreateAccount = true
        },
        result =>
        {
            Debug.Log("ログイン成功! ! ");
            RequestLeaderboard();


        },
        error => { Debug.Log(error.GenerateErrorReport()); });

    }


    ///// <summary>
    ///// 1.スクリプトからキーを設定する場合はUpdateUserDataメソッドを用いる
    ///// </summary>
    //private void CreateAccount(string name,string email)
    //{
    //    PlayFabClientAPI.UpdateUserData(
    //    new UpdateUserDataRequest
    //    {
    //        Data = new Dictionary<string, string>()
    //        {
    //            {"Name", name},
    //            {"Email", email},
    //            {"Score","0" }
    //        }
    //    }, result => { Debug.Log("ログイン成功! ! "); },
    //    error => { Debug.Log(error.GenerateErrorReport()); });
    //}

    /// <summary>
    /// プレイヤーデータを取得する
    /// </summary>
    void GetUserData(string id)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            //TODO:取得できるか否か
            PlayFabId = id //←ここがポイント
        }, result => {
            Debug.Log(result.Data["Name"].Value);
        }, error => {
            Debug.Log(error.GenerateErrorReport());
        });
    }




    private void hoge()
    {
        PlayFabClientAPI.AddUsernamePassword(new AddUsernamePasswordRequest
        {
            //TODOメソッドが正しいかどうか、キーの作成、キーに代入する変数の作成
            
        }
        , result => {
            Debug.Log("ログイン成功 ");
        }
        , error => {
            Debug.Log("ログイン失敗");
        });
    }


    public void UpdateUserScoreData(int score)
    {
        PlayFabClientAPI.UpdateUserData(
            new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string>()
                {
                    {"Score", score.ToString()} }
                },
                result => { Debug.Log("スコアの更新 "); },
                error => { Debug.Log(error.GenerateErrorReport());
             });
    }

    //↑試行錯誤中



    public void PressRegisterButton()
    {
        var RegisterData = new RegisterPlayFabUserRequest()
        {
            TitleId = PlayFabSettings.TitleId,
            Email = email,
            Password = pass,
            Username = name
        };

        PlayFabClientAPI.RegisterPlayFabUser(RegisterData, result =>
        {
            Debug.Log("新規登録ok");
            Debug.Log(result.Username);
            Debug.Log(RegisterData.Password);
            Debug.Log(RegisterData.Email);
            PressLoginButton();
        }, error => Debug.Log(error.GenerateErrorReport()));
    }


    public void PressLoginButton()
    {
        var LoginData = new LoginWithEmailAddressRequest()
        {
            TitleId = PlayFabSettings.TitleId,
            Password = pass,
            Email = email
        };
        PlayFabClientAPI.LoginWithEmailAddress(LoginData, result =>
        {
            Debug.Log("ログイン成功！");
            SceneController.Instance.LoadInGameScene();
            
        }, error => Debug.Log(error.GenerateErrorReport()));
    }

    public void InputName()
    {
        name = userNameText.text;
    }
    public void InputPass()
    {
        pass = passText.text;
    }

    public void InputMail()
    {
        email = emailText.text;
    }

    public void PressLoginView()
    {
        loginCanvas.SetActive(true);
        signUpCanvas.SetActive(false);
    }

    public void PressSignupView()
    {
        loginCanvas.SetActive(false);
        signUpCanvas.SetActive(true);
    }

    public void SubmitScore(int playerScore)
    {
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
         {
                Statistics = new List<StatisticUpdate>
             {
                 new StatisticUpdate
                 {
                    StatisticName = "HighScoreRanking",

                    Value = playerScore
                 }
              }
              }, result =>
                {
                    Debug.Log("スコア送信完了! ");
                    SubmitName();
                }, error => {
                    Debug.Log(error.GenerateErrorReport());
          });

    }

    private void SubmitName()
    {
        PlayFabClientAPI.UpdateUserTitleDisplayName(
        new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = name
        },
        result =>
        {
            Debug.Log("Set display name was succeeded.");
        },
        error =>
        {
            Debug.LogError(error.GenerateErrorReport());
        }
        );
    }


    [SerializeField] private GameObject rankingElemnt;
    [SerializeField] private Transform rankingCanvas;
    //[SerializeField]private RankElementView rankElementView;

    public void RequestLeaderboard()
    {
        PlayFabClientAPI.GetLeaderboard(new GetLeaderboardRequest
        {
            StatisticName = "HighScoreRanking",
            StartPosition = 0,
            MaxResultsCount = 20
        }, result => {
            //result.Leaderboard.ForEach(x => Debug.Log($"{x.Position + 1}位:{x.DisplayName} " +
            //$"スコア {x.StatValue}")
            result.Leaderboard.ForEach(x => {
                //Instantiate(rankingElemnt, rankingCanvas, false);
                var rankElementView = Instantiate(rankingElemnt, rankingCanvas, false).GetComponent<RankElementView>();
                rankElementView.RankNameText.text = x.DisplayName;
                rankElementView.RankRankText.text = (x.Position + 1).ToString();
                rankElementView.RankScoreText.text = x.StatValue.ToString();

                Debug.Log($"{x.Position + 1}位:{x.DisplayName} " + $"スコア {x.StatValue}");
           }) ;
        }, error =>
        {
            Debug.Log(error.GenerateErrorReport());
        });
    }

    private void Start()
    {
       // SubmitUserData();
        
    }




}
