using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class InGameView : MonoBehaviour
{
    //スクリプト参照
    [SerializeField] private IInputInterface iInputInterface;
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private Transform ParentBlock;
    [SerializeField] private InGameState inGameState;

    //オブジェクト参照
    [SerializeField] private Text timerText;
    [SerializeField] private Text feverText;
    [SerializeField] private Text ScoreText;
    [SerializeField] private Image fillImage; //タイマーゲージ
    //[SerializeField] private ParticleSystem gaugeMaxEffect;

    //イベント発火 //まだ使用してない
    private readonly Subject<InputDirection> inputKeySubject = new Subject<InputDirection>();
    public IObservable<InputDirection> InputKeySubject => inputKeySubject;

    //idle
    private readonly Subject<Vector2> startPos = new Subject<Vector2>();
    public IObservable<Vector2> IOStartPos => startPos;

    //move
    private readonly Subject<Vector2> movePos = new Subject<Vector2>();
    public IObservable<Vector2> IOMovePos => movePos;

    //EndMove
    private readonly Subject<Unit> transitionState = new Subject<Unit>();
    public IObservable<Unit> IOTransitionState => transitionState;

    //MatchBlock
    private readonly Subject<Unit> matchBlock = new Subject<Unit>();
    public IObservable<Unit> IOMatchBlock => matchBlock;

    //addBlock
    private readonly Subject<Unit> addBlock = new Subject<Unit>();
    public IObservable<Unit> IOAddBlock => addBlock;

    //デバック
    [SerializeField] private Text stateUI;


    // Start is called before the first frame update
    void Start()
    {
        //JudgePlatform();

        this.UpdateAsObservable()
            .Subscribe(_ =>{
          //     InGameState(gameState.Value);

            });
    }

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

            case global::InGameState.AddBlocks:
                AddBlocks();
                break;

            case global::InGameState.GameOver:
                GameOver();
                break;
        }
        stateUI.text = state.ToString();
    }


    public void SetTimer(int time)
    {
        timerText.text = $"Timer: {time}";
        //値が変わるごとにfillが変わっており、アニメーションとして好ましくないのでDOTweenでそれらしいメソッドを探す
        fillImage.fillAmount -= 0.009f;

    }

    public void SetScore(int score)
    {
        ScoreText.text = $"Score: {score}";
    }

    public void SetFeverGauge(int feverScore)
    {
        feverText.text = $"Fever: {feverScore}";
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
        await UniTask.Delay(1000); // 千分の一秒単位で2秒
        addBlock.OnNext(Unit.Default);
    }


    /// <summary>
    /// 1.ポップアップを表示
    /// 2.スコアを表示(スコアオブジェクトにはもう一度遊ぶボタンを付与)
    /// 3.
    /// </summary>
    public void GameOver()
    {
       
    }


    //test書き
    // MoveBlockAsync().Forget();
    private async UniTaskVoid MoveBlockAsync()
    {
        await transform.DOMove(new Vector3(0, 2, 0), 5).ToUniTask();
        await UniTask.Delay(500);
    }




}
