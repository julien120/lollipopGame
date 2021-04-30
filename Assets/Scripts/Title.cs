using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UniRx;
using UniRx.Triggers;

public class Title : MonoBehaviour
{
    [SerializeField] private Transform borderDialog;

    // Start is called before the first frame update
    void Start()
    {
        borderDialog.transform.localScale = Vector3.zero;

        this.UpdateAsObservable()
            .First(_ => Input.GetMouseButtonDown(0))
            .Subscribe(_ => {
                ShowSignupUI();
            });
    }



    void ShowSignupUI()
    {
        borderDialog.transform.DOScale(1f, 0.6f).SetEase(Ease.OutSine);
    }
}
