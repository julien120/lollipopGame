using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Block : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Sprite[] sprite;
    [SerializeField] private BlockType blockType;
    [SerializeField] private ParticleSystem particle;
   

    //テスト
    public int countID;
    public bool isMatch= false;
    public bool isCombo = false;

 

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
        particle.Play();
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

    void Update()
    {
        
    }
}
