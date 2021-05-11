using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;

public class InGameView : MonoBehaviour
{
    //スクリプト参照
    [SerializeField] private IInputInterface iInputInterface;
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private Transform parentBlock;

    //オブジェクト参照
    [SerializeField] private Text timerText;
    [SerializeField] private Text userNameText;
    [SerializeField] private Text scoreText;
    [SerializeField] private Image fillImage; //タイマーゲージ
    //[SerializeField] private ParticleSystem gaugeMaxEffect;

    //オブジェクト参照：ダイアログ
    [SerializeField] private Button restartButton;
    [SerializeField] private Button backTitleButton;
    [SerializeField] private Transform borderDialog;
    [SerializeField] private TextMeshProUGUI totalScoreText;
    [SerializeField] private TextMeshProUGUI totalComboText;
    [SerializeField] private TextMeshProUGUI totalSynthelizeText;
    private bool isFlag { get; set; } = true;//TODO変更


    //インターフェース使ってPC/スマホ対応するとき用 //まだ使用してない
    private readonly Subject<InputDirection> inputKeySubject = new Subject<InputDirection>();
    public IObservable<InputDirection> InputKeySubject => inputKeySubject;

    //-idle
    private readonly Subject<Vector2> startPos = new Subject<Vector2>();
    public IObservable<Vector2> IOStartPos => startPos;

    //-move
    private readonly Subject<Vector2> movePos = new Subject<Vector2>();
    public IObservable<Vector2> IOMovePos => movePos;

    //-EndMove
    private readonly Subject<Unit> transitionState = new Subject<Unit>();
    public IObservable<Unit> IOTransitionState => transitionState;

    //-MatchBlock
    private readonly Subject<Unit> matchBlock = new Subject<Unit>();
    public IObservable<Unit> IOMatchBlock => matchBlock;

    //-DestroyBlock
    private readonly Subject<Unit> destroyBlock = new Subject<Unit>();
    public IObservable<Unit> IODestroyBlock => destroyBlock;

    //-addBlock
    private readonly Subject<Unit> addBlock = new Subject<Unit>();
    public IObservable<Unit> IOAddBlock => addBlock;

    //-ChainBlock
    private readonly Subject<Unit> chainBlock = new Subject<Unit>();
    public IObservable<Unit> IOChainBlock => chainBlock;

    //test
    [SerializeField] private int highScore;
    private readonly Subject<int> requestUserScore = new Subject<int>();
    public IObservable<int> IORequestUserScore => requestUserScore;

    void Start()
    {
        restartButton.onClick.AddListener(SceneController.Instance.LoadInGameScene);
        backTitleButton.onClick.AddListener(SceneController.Instance.LoadTitleScene);
        borderDialog.transform.localScale = Vector3.zero;

        if (PlayFabController.name == null)
        {
            return;
        }
        userNameText.text = PlayFabController.name;

        //後々使うかも
        this.UpdateAsObservable()
            .Subscribe(_ =>{
          

            });
    }

    //デバック時はmodelのインスペクター上で確認する
    public void InGameState(InGameState state)
    {
        switch (state)
        {
            case global::InGameState.Idle:
                Idle();
                break;

            case global::InGameState.MoveBlock:
                MoveBlock();
                break;

            case global::InGameState.MatchBlocks:
                MatchBlocks();
                break;

            case global::InGameState.DestroyBlock:
                DestroyBlocks();
                break;

            case global::InGameState.AddBlocks:
                AddBlocks();
                break;

            case global::InGameState.ChainBlocks:
                ChainBlocks();
                break;

            case global::InGameState.GameOver:
                GameOver();
                break;
        }
        //stateUI.text = state.ToString();
    }


    public void SetTimer(int time)
    {
        timerText.text = $"Timer: {time}";
        //値が変わるごとにfillが変わっており、アニメーションとして好ましくないのでDOTweenでそれらしいメソッドを探す
        fillImage.fillAmount -= 0.009f;

    }

    public void SetScore(int score)
    {
        scoreText.text = $"Score: {score}";
        highScore = score;
    }

    public void SetFeverGauge(int feverScore)
    {
        // $"Fever: {feverScore}";
    }




    private void Idle()
    {
        if (Input.GetMouseButton(0))
        {
            //modelの方でnullになる前にリターンする判定が上手く行かなかったので仮置き
            if (Input.mousePosition.y < 900) { 
            startPos.OnNext(Input.mousePosition);
            }
        }
    }

    private void MoveBlock()
    {
        if (Input.GetMouseButton(0))
        {
            movePos.OnNext(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            transitionState.OnNext(Unit.Default);
        }
    }

    private void MatchBlocks()
    {
        matchBlock.OnNext(Unit.Default);
    }

    private async UniTask AddBlocks()
    {
        await UniTask.Delay(1000); 
        addBlock.OnNext(Unit.Default);
    }

    private async UniTask DestroyBlocks()
    {
        await UniTask.Delay(600);
        destroyBlock.OnNext(Unit.Default);
    }

    private async UniTask ChainBlocks()
    {
        await UniTask.Delay(600);
        chainBlock.OnNext(Unit.Default);
    }


    /// <summary>
    /// 1.ポップアップを表示
    /// 2.スコアを表示(スコアオブジェクトにはもう一度遊ぶボタンを付与)
    /// 3.サーバーにscoreを送信する
    /// </summary>
    public void GameOver()
    {
        
        if (isFlag)
        { 
            borderDialog.transform.DOScale(1f, 0.6f).SetEase(Ease.OutSine);
            totalComboText.DOCounter(0, 999, 1f).SetEase(Ease.Linear);
            totalSynthelizeText.DOCounter(0, 999, 1f).SetEase(Ease.Linear).SetDelay(1f);

            totalScoreText.DOCounter(0, highScore, 3f).SetEase(Ease.Linear).SetDelay(2.5f); ;
        }
        isFlag = false;
        if (!isFlag)
        {
            requestUserScore.OnNext(highScore);
        }
    }


}
