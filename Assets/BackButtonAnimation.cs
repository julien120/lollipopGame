using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class BackButtonAnimation : MonoBehaviour
{
    [SerializeField] private Image image;
    // Start is called before the first frame update
    void Start()
    {
        
         image.DOFade(0.3f,0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuart);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
