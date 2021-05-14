using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using System;

public class ComboTextAnimation : MonoBehaviour
{
    private DOTweenTMPAnimator textAnimator;
    [SerializeField] private Transform comboImage;
    [SerializeField] TextMeshProUGUI comboText;
    /// <summary>
    ///1.アルファ値を0から1にゆるっとさせて
    ///2.snakeはじめ縮尺買えるアニメーションを発生させ
    ///3.最後にアタッチされたゲームオブジェクトを破壊
    /// </summary>



    public void Initialize(int text)
    {
        comboText.text = text.ToString();
    }

    void Start()
    {
        TextMeshProUGUI comboText = this.gameObject.GetComponent<TextMeshProUGUI>();
       // comboText.alpha = 0;
        ComboAnimation();
    }

   private void ComboAnimation()
    {
        textAnimator = new DOTweenTMPAnimator(comboText);
        var sequence = DOTween.Sequence();
        for (int i = 0; i < textAnimator.textInfo.characterCount; i++)
        {
            sequence
                .Append(textAnimator.DOFadeChar(i, 1, 1.5f))
                .Join(comboImage.DOPunchScale(Vector3.one, 1.5f))
                //.Join(comboImage.DOFade(0f, 1.5f).SetEase(Ease.Flash))
                //(0.1f, 1.2f).SetEase(Ease.InSine).SetLoops(-1, LoopType.Yoyo))
                .Join(textAnimator.DOPunchCharScale(i, 1.5f, 1.5f))
                .Play()
                //.SetDelay(1.8f)
                .OnComplete(() => { Destroy(this.gameObject); });
        }
    }
}
