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
    public Button backTitleButton;
    [SerializeField] private Transform borderDialog;
    [SerializeField] private TextMeshProUGUI totalScoreText;
    [SerializeField] private TextMeshProUGUI totalComboText;
    [SerializeField] private TextMeshProUGUI totalSynthelizeText;
    [SerializeField] private TextMeshProUGUI feverTextAnimation;
    private DOTweenTMPAnimator textAnimator;
    private bool isFlag { get; set; } = true;//TODO変更
    private bool isFeverFlag { get; set; } = true;

    [SerializeField] private GameObject rankingDialog;


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

    //feverBlock
    private readonly Subject<Unit> addFeverBlock = new Subject<Unit>();
    public IObservable<Unit> IOfeverBlock => addFeverBlock;

    //test
    [SerializeField] private int highScore;
    private int highComboScore =0;
    private int syntheticScore = 0;

    private readonly Subject<int> requestUserScore = new Subject<int>();
    public IObservable<int> IORequestUserScore => requestUserScore;

    void Start()
    {
        feverTextAnimation.alpha = 0;
        restartButton.onClick.AddListener(SceneController.Instance.LoadInGameScene);
        // backTitleButton.onClick.AddListener(SceneController.Instance.LoadTitleScene);
        //rankingPage考え中
        backTitleButton.onClick.AddListener(OpenRankignDialog);
        borderDialog.transform.localScale = Vector3.zero;

        if (PlayFabController.names == null)
        {
            return;
        }
        userNameText.text = PlayFabController.names;

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
                DestroyBlocks().Forget();
                break;

            case global::InGameState.AddBlocks:
                AddBlocks().Forget();
                break;

            case global::InGameState.ChainBlocks:
                ChainBlocks().Forget();
                break;

            case global::InGameState.AddFeverBlock:
                AddFeverBlock();
                break;

            case global::InGameState.GameOver:
                GameOver();
                break;

            default:
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

    public void SetHigheCombo(int highCombo)
    {
        if (highComboScore < highCombo) { 
             highComboScore = highCombo;
        }
    }
    public void SetSyntheticScore(int syScore)
    {
        syntheticScore = syScore;
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
        isFeverFlag = true;
        await UniTask.Delay(600);
        chainBlock.OnNext(Unit.Default);
    }

    /// <summary>
    /// modelから通知を受け取り
    /// feverテキストの描画
    /// カット演出
    /// </summary>
    public void SetFeverTextAnimation()
    {
        textAnimator = new DOTweenTMPAnimator(feverTextAnimation);
        var sequence = DOTween.Sequence();
   
        for (int i = 0; i < textAnimator.textInfo.characterCount; i++)
        {
            sequence
                .Append(textAnimator.DOFadeChar(i, 1, 0.1f))
                .Join(textAnimator.DOPunchCharScale(i, 1.5f, 0.1f))
                .Play()
                .OnComplete(() => {
                    DOVirtual.DelayedCall(1.5f, () => feverTextAnimation.text = "");
                    
                });

        }

       
    }

    bool isRequestRanking = true;
    /// <summary>
    /// 1.ポップアップを表示
    /// 2.スコアを表示(スコアオブジェクトにはもう一度遊ぶボタンを付与)
    /// 3.サーバーにscoreを送信する
    /// </summary>
    public void GameOver()
    {
        
        if (isFlag)
        {
            isFlag = false;
            borderDialog.transform.DOScale(1f, 0.6f).SetEase(Ease.OutSine);
            totalComboText.DOCounter(0, highComboScore, 1f).SetEase(Ease.Linear);
            totalSynthelizeText.DOCounter(0, syntheticScore, 1f).SetEase(Ease.Linear).SetDelay(1f);

            totalScoreText.DOCounter(0, highScore, 3f).SetEase(Ease.Linear).SetDelay(2.5f);
            if (isRequestRanking)
            {
                requestUserScore.OnNext(highScore);
                isRequestRanking = false;

            }
        }
        

        
    }

    private void AddFeverBlock()
    {
        if(isFeverFlag == true)
        {
            isFeverFlag = false;
            addFeverBlock.OnNext(Unit.Default);
        }
        
    }
    
    private void OpenRankignDialog()
    {
        rankingDialog.SetActive(true);
    }

}
