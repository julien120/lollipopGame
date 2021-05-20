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
    [SerializeField] private feverAnimation dropCoinAnimation;


    private void Awake()
    {

        inGameModel.Initialize();

        inGameView.IOStartPos.Subscribe(IOStartPos => inGameModel.Idle(IOStartPos));
        inGameView.IOMovePos.Subscribe(IOMovePos => inGameModel.MoveBlock(IOMovePos));

        inGameView.IOTransitionState.Subscribe(_ => inGameModel.TransitionState().Forget());
        inGameView.IOMatchBlock.Subscribe(_ => inGameModel.MatchBlock().Forget());
        inGameView.IOAddBlock.Subscribe(_ => inGameModel.AddBlock());

        inGameView.IODestroyBlock.Subscribe(_ => inGameModel.DestroyBlockAnimation().Forget());
        inGameView.IOChainBlock.Subscribe(_ => inGameModel.ChainBlock().Forget());
        inGameModel.IOTimerCount.Subscribe(IOTimerCount => inGameView.SetTimer(IOTimerCount));
        inGameModel.IOScore.Subscribe(IOScore => inGameView.SetScore(IOScore));
        inGameModel.IOHighCombo.Subscribe(IOHighCombo => inGameView.SetHigheCombo(IOHighCombo));
        inGameModel.IOSyntheticScore.Subscribe(IOSyntheticScore => inGameView.SetSyntheticScore(IOSyntheticScore));

        inGameView.IOfeverBlock.Subscribe(_ => inGameModel.isFeverTime());
        inGameView.IORequestUserScore.Subscribe(IORequestUserScore => playFabController.SubmitScore(IORequestUserScore));

        inGameModel.IOFeverAnimation.Subscribe(_ => inGameView.SetFeverTextAnimation());
        inGameModel.IODropCoinAnimationn.Subscribe(IODrop => dropCoinAnimation.AddCoins(IODrop.Item1, IODrop.Item2));
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
