using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class InGamePresenter : MonoBehaviour
{
    [SerializeField] private InGameModel inGameModel;
    [SerializeField] private InGameView inGameView;


    private void Awake()
    {
        inGameModel.IOChangedState.Subscribe(IOChangedState => inGameView.ApplyBlock(IOChangedState.Item2, IOChangedState.Item1));
        inGameModel.IOInGameState.Subscribe(IOInGameState => inGameView.InGameState(IOInGameState));
        inGameModel.Initialize();

        
    }

    private void Start()
    {
        
    }
}
