using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class RankingView : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    [SerializeField] private GameObject rankingDialog;

    private readonly Subject<Unit> setRankingData = new Subject<Unit>();
    public IObservable<Unit> IOSetRankingData => setRankingData;

    void Start()
    {
        setRankingData.OnNext(Unit.Default);
        closeButton.onClick.AddListener(CloseRankingDialog);
        //setRankingData.OnNext(Unit.Default);
    }

    private void CloseRankingDialog()
    {
        rankingDialog.SetActive(false);
    }

}
