using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;

public class InGameModel : MonoBehaviour
{
    //値
    private const float generationRate = 0.5f;
    private const int rowStage = 5;
    private const int colStage = 5;
    private const int MachintCount = 4;

    private readonly int[,] stageStates = new int[rowStage, colStage];


    //通知発火
    private readonly ReactiveProperty<int> score = new ReactiveProperty<int>();
    public IObservable<int> IOScore => score;

    private readonly ReactiveProperty<int> feverScore = new ReactiveProperty<int>();
    public IObservable<int> IOfeverScore => feverScore;

    private readonly Subject<(int, int)> changedState = new Subject<(int, int)>();
    public IObservable<(int, int)> IOChangedState => changedState;

    private Subject<InGameState> inGameState = new Subject<InGameState>();
    public IObservable<InGameState> IOInGameState => inGameState;

    public void Initialize()
    {
        for(var j =0;j<rowStage; j++)
        {
            for(var i =0;i<colStage; i++)
            {
                var indexAndstageStates = (i,j);
                changedState.OnNext(indexAndstageStates);

                
            }
        }

        inGameState.OnNext(InGameState.Idle);


    }



    // Start is called before the first frame update
    void Start()
    {
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                

            });
    }




    // Update is called once per frame
    void Update()
    {
        
    }
}
