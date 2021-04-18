using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Block : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private BlockType blockType;

    public void SetImage(int num)
    {
       int type = (int)(BlockType)Enum.ToObject(typeof(BlockType), num);
        switch (type)
        {
            case (int)BlockType.Bear:
                image.color = Color.red;
                break;

            case (int)BlockType.Cat:
                image.color = Color.blue;
                break;
            case (int)BlockType.Dog:
                image.color = Color.green;
                break;
            case (int)BlockType.Flog:
                image.color = Color.black;
                break;
            default:
                break;
        }


    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
