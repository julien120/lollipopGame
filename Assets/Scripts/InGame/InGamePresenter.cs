using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class InGamePresenter : MonoBehaviour
{
    [SerializeField] private InGameModel inGameModel;
    [SerializeField] private InGameView inGameView;
    [SerializeField] private PlayFabController playFabController;


    private void Awake()
    {

        inGameModel.Initialize();

        inGameView.IOStartPos.Subscribe(IOStartPos => inGameModel.Idle(IOStartPos));
        inGameView.IOMovePos.Subscribe(IOMovePos => inGameModel.MoveBlock(IOMovePos));

        inGameView.IOTransitionState.Subscribe(_ => inGameModel.TransitionState());
        inGameView.IOMatchBlock.Subscribe(_ => inGameModel.MatchBlock());
        inGameView.IOAddBlock.Subscribe(_ => inGameModel.AddBlock());

        inGameModel.IOTimerCount.Subscribe(IOTimerCount => inGameView.SetTimer(IOTimerCount));
        inGameModel.IOScore.Subscribe(IOScore => inGameView.SetScore(IOScore));

        inGameView.IORequestUserScore.Subscribe(IORequestUserScore => playFabController.UpdateUserScoreData(IORequestUserScore));
    }


    private void Start()
    {
        inGameModel.totalTime = 80;
        this.UpdateAsObservable()
            .Subscribe(_ => {
                inGameModel.IOInGameState.Subscribe(x => inGameView.InGameState(x));

            });
    }

    private void Update()
    {
        inGameModel.SetTimer();
    }
}
