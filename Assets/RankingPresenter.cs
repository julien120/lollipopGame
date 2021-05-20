using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class RankingPresenter : MonoBehaviour
{
    [SerializeField] private RankingView rankingView;
    [SerializeField] private PlayFabController playFabController;

    // Start is called before the first frame update
    void Start()
    {
        rankingView.IOSetRankingData.Subscribe(_ => playFabController.RequestRankingData());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
