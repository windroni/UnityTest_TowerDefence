using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class Wave
{
	public GameObject enemyPrefab = null;
	public int spawnEnemyCount = 3;
	public float spawnEnemyTime = 1.0f;
}


public class WaveManager : MonoBehaviour //Singleton<WaveManager> 
{
	//[Range(1, 10)]
	public int _waveCount = 3;
	public float _waveDelayTime = 1.0f;

	public List<Wave> _waveList = new List<Wave>();
		
	int _currentWaveIndex = 0;
	bool _waveProgress = false;

	// Use this for initialization
	void Start () 
	{
		//_waveList.Add(new Wave());
		//_waveList.Add(new Wave());
		//_waveList.Add(new Wave());
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(_waveProgress == false)
		{
			if(_currentWaveIndex < _waveList.Count)
			{
				Wave wave = _waveList[_currentWaveIndex];
				if(wave.enemyPrefab != null)
				{
					StartCoroutine(WaveProcess(wave));
					++_currentWaveIndex;
				}
			}
		}
	}

	IEnumerator WaveProcess(Wave wave)
	{
		_waveProgress = true;
		for(int i = 0; i < wave.spawnEnemyCount; ++i)
		{
			GameObject enemy = Instantiate(wave.enemyPrefab) as GameObject;

			yield return new WaitForSeconds(wave.spawnEnemyTime);
		}

		yield return new WaitForSeconds(_waveDelayTime);
		_waveProgress = false;
	}
}
