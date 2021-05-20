using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;


public class feverAnimation : MonoBehaviour
{
	//References
	[Header("アタッチする内容")]
//	[SerializeField] TMP_Text coinUIText;
	[SerializeField] private GameObject animatedCoinPrefab;
	[SerializeField] private Transform target;
	[SerializeField] private Transform parentGroup;

	[Space]
	[Header("取得量")]
	[SerializeField] private int maxCoins;
	private Queue<GameObject> coinsQueue = new Queue<GameObject>();


	[Space]
	[Header("アニメーション設定")]
	[SerializeField] [Range(0.5f, 0.9f)] private float minAnimDuration;
	[SerializeField] [Range(0.9f, 2f)] private float maxAnimDuration;

	[SerializeField] private Ease easeType;
	[SerializeField] private float spread;

	Vector3 targetPosition;


	private int _c = 0;

	public int Coins
	{
		get { return _c; }
		set
		{
			_c = value;
			//coinUIText.text = Coins.ToString();
		}
	}

	void Awake()
	{
		targetPosition = target.position;

		//prepare pool
		PrepareCoins();
	}

    private void Update()
    {
		
		

	}

    void PrepareCoins()
	{
		GameObject coin;
		//50=maxCoinsとは別に一旦仮設定
		for (int i = 0; i < maxCoins; i++)
		{
			coin = Instantiate(animatedCoinPrefab);
			coin.transform.parent = transform;
			
			coin.SetActive(false);
			//coin.transform.SetParent(parentGroup);
			coinsQueue.Enqueue(coin);
		}
	}

	void Animate(Vector3 collectedCoinPosition, int amount)
	{
		for (int i = 0; i < amount; i++)
		{
			//coinsQueue.Count
			if (coinsQueue.Count > 0)
			{
				
				GameObject coin = coinsQueue.Dequeue();
				coin.SetActive(true);
				
				
				coin.transform.position = collectedCoinPosition + new Vector3(Random.Range(-spread, spread), 0f, 0f);

				//animate coin to target position
				float duration = Random.Range(minAnimDuration, maxAnimDuration);
				coin.transform.DOMove(targetPosition, duration)
				.SetEase(easeType)
				.OnComplete(() => {
					coin.SetActive(false);
					coinsQueue.Enqueue(coin);

					Coins++;
				});
			}
		}
	}

	public void AddCoins(Vector3 collectedCoinPosition, int amount)
	{
		Animate(collectedCoinPosition, amount);
	}
}
