using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;

public class Block : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Sprite[] sprite;
    [SerializeField] private BlockType blockType;
    [SerializeField] private ParticleSystem particle;
    private float duration = 1.0f;

    


    //テスト
    public int countID;
    public bool isMatch= false;
    public bool isCombo = false;
    public bool isFever = true;



    public void SetImage(BlockType type)
    {   
        switch (type)
        {
            case BlockType.Bear:
                image.sprite = sprite[0];
                countID = 4; 
                blockType = BlockType.Bear;
                break;

            case (int)BlockType.Cat:
                image.sprite = sprite[1];
                countID = 5;
                blockType = BlockType.Cat;
                break;

            case BlockType.Dog:
                countID = 6;
                image.sprite = sprite[2];
                blockType = BlockType.Dog;
                break;

            case BlockType.Flog:
                countID = 9;
                image.sprite = sprite[3];
                blockType = BlockType.Flog;
                break;
        }

    }
    public void DrawParticle()
    {
        var sequence = DOTween.Sequence();
        sequence

                .SetDelay(0.2f)
                .OnComplete(()=> { particle.Play(); });
       // particle.Play();
    }
    public void StopParticle()
    {
        particle.Stop();
    }

    public BlockType type()
    {
        return blockType;
    }

    //5*5で画面描画し、ユーザーがセルをタッチした場合にそのフリックの距離に合わせて照合や合成をするか、
    //フリック自体はアニメーションででkリウ


    // Start is called before the first frame update
    void Start()
    {
        int count =UnityEngine.Random.Range(0, 4);
        BlockType type = (BlockType)count;
        SetImage(type);
       
    }

    [System.Obsolete]
    void Update()
    {
        SetFeverParticle();
    }

    [System.Obsolete]
    private void SetFeverParticle()
    {
        if (isFever == true)
        {
            float phi = Time.time / duration * 2 * Mathf.PI;
            float amplitude = Mathf.Cos(phi) * 0.5f + 0.5f;
           particle.startColor = Color.HSVToRGB(amplitude, 1, 1);
           // image.color = Color.HSVToRGB(amplitude, 1, 1);
           particle.Play();
        }
    }
}
