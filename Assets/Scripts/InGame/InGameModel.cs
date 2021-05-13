using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
//MVP分離前にunitaskDOTweenをこっちで試す
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine.UI;

public class InGameModel : MonoBehaviour
{
    //値
    public const int rowStage = 5;
    public const int colStage = 5;
    private const int MachCount = 3;
    public float totalTime = 90;

    //通知発火
    private readonly ReactiveProperty<int> score = new ReactiveProperty<int>();
    public IObservable<int> IOScore => score;

    private readonly ReactiveProperty<int> feverScore = new ReactiveProperty<int>();
    public IObservable<int> IOfeverScore => feverScore;

    private ReactiveProperty<int> timerCount= new ReactiveProperty<int>(90);
    public IObservable<int> IOTimerCount => timerCount;

    private readonly Subject<Vector2> changedState = new Subject<Vector2>();
    public IObservable<Vector2> IOChangedState => changedState;

    public ReactiveProperty<InGameState> inGameState = new ReactiveProperty<InGameState>();
    public IObservable<InGameState> IOInGameState => inGameState;

    //Block座標
    private Block[,] blockQueue;
    private Block startPosition;
    private Block endPosition;

    [SerializeField] private Transform blockPos;

    //参照
    public Block blockInstance;
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private Transform ParentBlock;

    //test
    private bool isHorFlag { get; set; } = true;
    private bool isVerFlag { get; set; } = true;
    private bool isChainFlag { get; set; } = false;

    private int combo { get; set; } = 0;

   [SerializeField]private Font font;


    private readonly ReactiveProperty<int> highCombo = new ReactiveProperty<int>();
    public IObservable<int> IOHighCombo => highCombo;

    private readonly ReactiveProperty<int> syntheticScore = new ReactiveProperty<int>();
    public IObservable<int> IOSyntheticScore => syntheticScore;

    public void Initialize()
    {
        blockQueue = new Block[rowStage, colStage];
        
        for (var j =0;j<rowStage; j++)
        {
            for (var i = 0; i < colStage; i++)
            {
                //mainCameraのrenderModeを元に戻す場合はこれ
              blockInstance = Instantiate(blockPrefab, new Vector2(85 + (j * 150), 223 + i * 150), Quaternion.identity, ParentBlock).GetComponent<Block>();
              //  blockInstance = Instantiate(blockPrefab, new Vector3(blockPos.position.x * j + 85, blockPos.position.y * i + blockPos.position.y, blockPos.position.z), Quaternion.identity, ParentBlock).GetComponent<Block>();
                blockQueue[j, i] = blockInstance;
                
            }
        }
        inGameState.Value = InGameState.Idle;
    }

    /// <summary>
    /// 1.ユーザーがタッチした場所を行列に反映させ、当該ブロックをStartPosに格納する
    /// </summary>
    /// <param name="hoge"></param>
    public void Idle(Vector2 pos)
    {
        //var X = (int)pos.x / 150;
        //var Y = (int)pos.y / 150 - 1;
        if ((int)pos.x / 150 >= 5 || ((int)pos.y / 150 - 1) >= 5) { return; }
        if ((int)pos.x / 150 < 0 || ((int)pos.y / 150 - 1) < 0) { return; }

        for (var j = 0; j < rowStage; j++)
        {
            for (var i = 0; i < colStage; i++)
            {
                if (blockQueue[i, j] == blockQueue[(int)pos.x / 150, ((int)pos.y / 150 - 1)])
                {
                    if (blockQueue[i, j] == null)
                    {
                        return;
                    }

                    startPosition = BlockPosition(pos);
                }
            }
        }
        
        highCombo.Value = 0;
        combo = 0;
        inGameState.Value = InGameState.MoveBlock;
    }

    /// <summary>
    /// 早期リターンを試行錯誤するうちにネストが深くなちゃったから後でこれに置換する
    /// </summary>
    /// <param name="pos"></param>
    private void CheckBlockPos(Vector2 pos)
    {
        for (var j = 0; j < rowStage; j++)
        {
            for (var i = 0; i < colStage; i++)
            {
                if(blockQueue[i, j] == blockQueue[(int)pos.x / 150, ((int)pos.y / 150 - 1)])
                {
                    if(blockQueue[(int)pos.x / 150, ((int)pos.y / 150 - 1)] == null)
                    {
                        return;
                    }

                    BlockPosition(pos);
                }
            }
        }
    }

    private Block BlockPosition(Vector2 pos)
    {
        Block matchBlockPos;
        matchBlockPos = blockQueue[(int)pos.x / 150, ((int)pos.y / 150 - 1)];

        return matchBlockPos;
    }

    /// <summary>

    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    public void MoveBlock(Vector2 pos)
    {
        if ((int)pos.x / 150 >= 5 || ((int)pos.y / 150 - 1) >= 5) { return; }
        if ((int)pos.x / 150 < 0 || ((int)pos.y / 150 - 1) < 0) { return; }

        for (var j = 0; j < rowStage; j++)
        {
            for (var i = 0; i < colStage; i++)
            {
                
                 if (blockQueue[i, j] == blockQueue[(int)pos.x / 150, ((int)pos.y / 150 - 1)])
                {
                    
                    if (blockQueue[i, j] == null)
                    {
                        return;
                    }
                    
                    //endPosition
                    endPosition = BlockPosition(pos);
                }
            }
        }
        if (blockQueue[(int)pos.x / 150, ((int)pos.y / 150 - 1)] == null)
        {
            return;
        }
        
        if (endPosition != startPosition)
        {
            startPosition.transform.DOScale(new Vector3(0.4f, 0.4f, 0.4f), 0.1f);
            ChangeBlock(startPosition, endPosition);
        }
        

    }

    public async UniTaskVoid TransitionState()
    {
        await ReturnScale();
        inGameState.Value = InGameState.MatchBlocks;
    }

    public async UniTask ReturnScale()
    {
        startPosition.transform.DOScale(new Vector3(0.3f, 0.3f, 0.3f), 0.1f);
        await UniTask.Delay(600);
    }

    public async UniTask TestDelay()
    {
       
        await UniTask.Delay(600);
    }


    /// <summary>
    /// 引数1と引数2を移動させる
    /// </summary>
    /// <param name="startBlock"></param>
    /// <param name="endBlock"></param>
    private void ChangeBlock(Block startBlock,Block endBlock)
    {
        MoveBlockAsync(startBlock,endBlock).Forget();


        var startBlockPos = GetBlockPos(startBlock);
        var endBlockPos = GetBlockPos(endBlock);
        blockQueue[(int)startBlockPos.x, (int)startBlockPos.y] = endBlock;
        blockQueue[(int)endBlockPos.x, (int)endBlockPos.y] = startBlock;
    }

    //TODO:animationmethod
    // MoveBlockAsync().Forget();
    private async UniTaskVoid MoveBlockAsync(Block startBlock, Block endBlock)
    {
       
        //awaitをつけなかったらいい感じにanimationするようになったが、一方で早く動かすとアウト！
        endBlock.transform.DOMove(startBlock.transform.position, 0.3f);
       
        await startBlock.transform.DOMove(endBlock.transform.position, 0.3f);
        //await UniTask.DelayFrame(1);

        //await startBlock.transform.DOScale(new Vector3(0.3f, 0.3f, 0.3f), 0.3f);
    }




    private Vector2 GetBlockPos(Block blockPos)
    {
        for (int i = 0; i < colStage; i++)
        {
            for (int j = 0; j < rowStage; j++)
            {
                if (blockQueue[i, j] == blockPos)
                {
                    return new Vector2(i, j);
                }
            }
        }
        return Vector2.zero;
    }

    /// <summary>
    /// 照合メソッドと削除メソッドとで分割し、MatchBlockでは消すブロックの風呂ぐを立てるだけにする。
    /// </summary>
    public async UniTaskVoid MatchBlock()
    {
        isMatchBlock().Forget();
        inGameState.Value = InGameState.DestroyBlock;

    }

    private async UniTaskVoid isMatchBlock()
    {
        for (int i = 1; i < 4; i++)
        {
            for (int j = 0; j < rowStage; j++)
            {

                if (blockQueue[i, j].type() == blockQueue[i + 1, j].type() && blockQueue[i, j].type() == blockQueue[i - 1, j].type())
                {
                    blockQueue[i, j].isMatch = true;
                    blockQueue[i + 1, j].isMatch = true;
                    blockQueue[i - 1, j].isMatch = true;


                    if (isHorFlag == true)
                    {
                        
                        SetScore(blockQueue[i, j].countID);

                    }
                    isHorFlag = false;
                }

            }
        }

        for (int i = 0; i < colStage; i++)
        {
            for (int j = 1; j < 4; j++)
            {

                if (blockQueue[i, j].type() == blockQueue[i, j + 1].type() && blockQueue[i, j].type() == blockQueue[i, j - 1].type())
                {
                    blockQueue[i, j].isMatch = true;
                    blockQueue[i, j + 1].isMatch = true;
                    blockQueue[i, j - 1].isMatch = true;
                    
                    if (isVerFlag == true)
                    {
                        
                        SetScore(blockQueue[i, j].countID);

                    }
                    isVerFlag = false;

                }
            }
        }
    }


    //TODO:横縦連鎖のアニメーション
    //async UniTask 一個ずつ処理する場合はシーケンスをawaitする。
    public async UniTask DestroyBlockAnimation()
    {
        //var hoge = blockQueue[i, j].gameObject.GetComponent<Image>();

        //await UniTask.Delay(600);
        foreach (Block block in blockQueue)
        {
            if (block == null) { return; }
            if (block.isMatch)
            {
                //await block.gameObject.transform
                //    .DOScale(new Vector3(0.35f, 0.35f, 0.35f), 0.15f)
                //    .SetDelay(0.2f)
                //    .OnComplete(() => Destroy(block.gameObject));
                //await UniTask.Delay(100);
                
                Sequence seq = DOTween.Sequence();
                if (block == null) { return; }
                await seq
                //seq.Kill();
                    .Append(block.gameObject.transform.DOScale(new Vector3(0.35f, 0.35f, 0.35f), 0.05f))
                    .SetDelay(0.05f)
                    //.Append(block.gameObject.transform.DOScale(new Vector3(0f, 0f, 0f), 0.1f))
                    //.SetDelay(0.05f)
                    .OnComplete(() =>
                    {
                        
                        DestroyBlock(block).Forget();
                    });
                
                //   .OnComplete(() => Destroy(block.gameObject));
            }
        }

        inGameState.Value = InGameState.AddBlocks;
        //await UniTask.Delay(600);

    }

    /// <summary>
    /// 1.コンボ表示
    /// 2.パーティクル
    /// 3.削除
    /// </summary>
    private async UniTask DestroyBlock(Block block)
    {
        block.DrawParticle();
        //await UniTask.Delay(600);
        //Destroy(block.gameObject);
        
        if (block.isCombo==false)
        {
            
            combo++;
            syntheticScore.Value++;
            if (highCombo.Value < combo / 3 )
            {
                highCombo.Value = combo / 3 + combo % 3;


                //TODOコンボcountを表示
                //textをblockの場所に生成し、text内容をcombo / 3 + combo % 3にする
                //block.transform.position
                
                CreateComboText(block.gameObject.transform, combo / 3 + combo % 3);
               // }
            }
           

            block.isCombo = true;

        }

        Sequence seq = DOTween.Sequence();

        await seq
            .SetDelay(0.4f)
            .OnComplete(() =>
            {

                block.StopParticle();
                Destroy(block.gameObject);//このメソッドの呼び出しが終わるまでOnCompleteが終わらないからnullなのにDOTween呼び出しエラーになると
            });

    }

    private void CreateComboText(Transform pos,int count)
    {
        Text comboText = new GameObject("ComboText").AddComponent<Text>();
        Canvas canvas = FindObjectOfType<Canvas>();
        comboText.transform.SetParent(canvas.transform);
        comboText.font =  font;
        comboText.alignment = TextAnchor.MiddleCenter;
        comboText.color = Color.red;
        comboText.fontSize = 40;
        comboText.fontStyle = FontStyle.Bold;
        comboText.text = count.ToString();
        comboText.rectTransform.position = new Vector2(pos.position.x,pos.position.y);
      //  comboText.gameObject.AddComponent<>();
        
    }

    //TODO:fever
    //何回かコンボするとfeverになる
    //fever玉を中心に3*3削除する
    //普通のidleに戻る



    /// <summary>
    /// Chain時にIdleに行かずMatch-destory-addでループする処理
    /// 
    /// </summary>
    public async UniTaskVoid ChainBlock()
    {
        
        isMatchBlock().Forget();
        await UniTask.Delay(700);
        int count = 0;
        isChainFlag = false;
        foreach (Block block in blockQueue)
        {
            count++;
            
            if (block.isMatch)
            {
                isChainFlag = true;
                inGameState.Value = InGameState.DestroyBlock;
               
               
            }
            if (isChainFlag == false && count >= 24)
            {
                inGameState.Value = InGameState.Idle;
            }
        }

       
    }



    public void SetScore(int blockValue)
    {
        score.Value += blockValue*20;
    }

    /// <summary>
    /// 1.ピースの生成位置を決める
    /// 2.生成位置にピースを生成する
    /// 3.ステートをIdleに戻す
    /// </summary>
    public void AddBlock()
    {

        for (int i = 0; i < colStage; i++)
        {
            for (int j = 0; j < rowStage; j++)
            {
                FindGenerationPos(i, j);

            }
        }

        inGameState.Value = InGameState.ChainBlocks;
    }


    private void FindGenerationPos(int x ,int y)
    {
        if (blockQueue[x, y] == null)
        {
            blockInstance = Instantiate(blockPrefab, new Vector2(85 + (x * 150), 223 + y * 150), Quaternion.identity, ParentBlock).GetComponent<Block>();
            blockQueue[x, y] = blockInstance;

        }
        isHorFlag = true;
        isVerFlag = true;

    }

    public void SetTimer()
    {
        if (totalTime >= 0)
        {
            totalTime -= Time.deltaTime;
            timerCount.Value = (int)totalTime;
        }
        else
        {
            //ゲームオーバー
            inGameState.Value = InGameState.GameOver;
        }  
    }




}
