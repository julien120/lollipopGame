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

    //参照
    public Block blockInstance;
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private Transform ParentBlock;

    //test
    private bool isFlag { get; set; } = true;

    public void Initialize()
    {
        blockQueue = new Block[rowStage, colStage];
        
        for (var j =0;j<rowStage; j++)
        {
            for (var i = 0; i < colStage; i++)
            {
                blockInstance = Instantiate(blockPrefab, new Vector2(85 + (j * 150), 223 + i * 150), Quaternion.identity, ParentBlock).GetComponent<Block>();
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

        if (blockQueue[(int)pos.x / 150, ((int)pos.y / 150 - 1)] == null)
        {
            return;
        }

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
    /// 3つ重なると削除
    /// </summary>
    public void MatchBlock()
    {
        for (int i = 1; i < 4; i++)
        {
            for (int j = 0; j < rowStage; j++)
            {
                
                 if (blockQueue[i, j].type() == blockQueue[i + 1, j].type() && blockQueue[i, j].type() == blockQueue[i-1, j].type())
                  {
                        Destroy(blockQueue[i, j].gameObject);
                        Destroy(blockQueue[i + 1, j].gameObject);
                        Destroy(blockQueue[i - 1, j].gameObject);
                        Destroy(blockQueue[i, j]);
                        Destroy(blockQueue[i + 1, j]);
                        Destroy(blockQueue[i - 1, j]);

                        if (isFlag == true) { 
                            SetScore(blockQueue[i, j].countID);
                        
                        }
                        isFlag = false;
                   }
                
            }
        }

        for (int i = 0; i < colStage; i++)
        {
            for (int j = 1; j < 4; j++)
            {

                if (blockQueue[i, j].type() == blockQueue[i , j + 1].type() && blockQueue[i, j].type() == blockQueue[i , j - 1].type())
                {
                    Destroy(blockQueue[i, j].gameObject);
                    Destroy(blockQueue[i , j + 1].gameObject);
                    Destroy(blockQueue[i , j - 1].gameObject);
                    Destroy(blockQueue[i, j]);
                    Destroy(blockQueue[i, j + 1]);
                    Destroy(blockQueue[i, j - 1]);

                    if (isFlag == true)
                    {
                        SetScore(blockQueue[i, j].countID);

                    }
                    isFlag = false;

                }
            }
        }
        inGameState.Value = InGameState.AddBlocks;
        
    }

    //TODO:横縦連鎖のアニメーション
    //ViewはBlockQueueを知らないので一旦modelでアニメーション
    private async UniTask DestroyBlockAnimation(int i, int j)
    {
        //var hoge = blockQueue[i, j].gameObject.GetComponent<Image>();
        if (blockQueue[i, j].gameObject == null) { return; }

        blockQueue[i, j].gameObject.transform.DOScale(new Vector3(0.35f, 0.35f, 0.35f), 0.3f);
        blockQueue[i, j - 1].gameObject.transform.DOScale(new Vector3(0.35f, 0.35f, 0.35f), 0.3f);

        await blockQueue[i, j + 1].gameObject.transform.DOScale(new Vector3(0.35f, 0.35f, 0.35f), 0.3f);

        blockQueue[i, j + 1].gameObject.transform.DOScale(new Vector3(0.0f, 0.0f, 0.0f), 0.2f);
        blockQueue[i, j - 1].gameObject.transform.DOScale(new Vector3(0.0f, 0.0f, 0.0f), 0.2f);
        await blockQueue[i, j].gameObject.transform.DOScale(new Vector3(0.0f, 0.0f, 0.0f), 0.2f);
        Destroy(blockQueue[i, j].gameObject);
        Destroy(blockQueue[i, j + 1].gameObject);
        Destroy(blockQueue[i, j - 1].gameObject);
        Destroy(blockQueue[i, j]);
        Destroy(blockQueue[i, j + 1]);
        Destroy(blockQueue[i, j - 1]);

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

        inGameState.Value = InGameState.Idle;
    }

    private void FindGenerationPos(int x ,int y)
    {
        if (blockQueue[x, y] == null)
        {
            blockInstance = Instantiate(blockPrefab, new Vector2(85 + (x * 150), 223 + y * 150), Quaternion.identity, ParentBlock).GetComponent<Block>();
            blockQueue[x, y] = blockInstance;

        }
        isFlag = true;

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
