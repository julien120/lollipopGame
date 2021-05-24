using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using DG.Tweening;

public class RankingPresenter : MonoBehaviour
{
    [SerializeField] private RankingView rankingView;
    [SerializeField] private PlayFabController playFabController;

    // Start is called before the first frame update
    void Start()
    {
       // DOVirtual.DelayedCall(2f, () => rankingView.IOSetRankingData.Subscribe(_ => playFabController.RequestRankingData()));
        rankingView.IOSetRankingData.Subscribe(_ => playFabController.RequestRankingData());
        Debug.Log("呼ばれてる");
    }

    // Update is called once per frame
    void Update()
    {
      
    }
}
