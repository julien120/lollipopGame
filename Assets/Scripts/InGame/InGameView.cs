using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UniRx;
using UniRx.Triggers;

public class InGameView : MonoBehaviour
{
    //スクリプト参照
    [SerializeField] private IInputInterface iInputInterface;
    [SerializeField] private GameObject block;
    [SerializeField] private Transform ParentBlock;
    [SerializeField] private InGameState inGameState;

    //オブジェクト参照
    [SerializeField] private Text timerText;
    [SerializeField] private Text feverText;
    [SerializeField] private Text ScoreText;


    //イベント発火
    private readonly Subject<InputDirection> inputKeySubject = new Subject<InputDirection>();
    public IObservable<InputDirection> InputKeySubject => inputKeySubject;


    // Start is called before the first frame update
    void Start()
    {
        JudgePlatform();

        this.UpdateAsObservable()
            .Subscribe(_ =>{
                ObserveInputKey();

                

            });
    }


    public void InGameState(InGameState state)
    {
        switch (state)
        {
            case global::InGameState.Idle:
                Debug.Log("アイドル");
                break;

            case global::InGameState.MoveBlock:
                Debug.Log("移動");
                break;

            case global::InGameState.MatchBlocks:
                Debug.Log("合成");
                break;

            case global::InGameState.AddBlocks:
                Debug.Log("上から下へ加える");
                break;

        }
    }


    //キー入力:インターフェース化する前


    private void JudgePlatform()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            iInputInterface = new InputOnMobile();
        }
        else if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            iInputInterface = new InputOnPC();
        }
        else
        {
            iInputInterface = new InputOnPC();
        }
    }

    private void ObserveInputKey()
    {
        InputDirection direction = iInputInterface.InputKey();
        switch (direction)
        {
            case InputDirection.Right:
                inputKeySubject.OnNext(InputDirection.Right);
                break;

            case InputDirection.Left:
                inputKeySubject.OnNext(InputDirection.Left);
                break;

            case InputDirection.Up:
                inputKeySubject.OnNext(InputDirection.Up);
                break;

            case InputDirection.Down:
                inputKeySubject.OnNext(InputDirection.Down);
                break;

            case InputDirection.None:
                break;
        }
    }

    public void SetTimer(int time)
    {
        timerText.text = $"Time: {time}";
    }

    public void SetFeverGauge(int feverScore)
    {
        feverText.text = $"Fever: {feverScore}";
    }

    public void ApplyBlock(int x,int y)
    {
        //blocks[index].SetImage(stageValue);
        Instantiate(block, new Vector2(85+x*150, 223+y * 150), Quaternion.identity,ParentBlock);
    }
}
