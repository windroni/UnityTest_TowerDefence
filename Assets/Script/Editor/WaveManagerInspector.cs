using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(WaveManager))]
public class WaveManagerInspector : Editor 
{
	public override void OnInspectorGUI()
	{
		WaveManager waveManager = target as WaveManager;
		
		DrawWaveTime(waveManager);
		
		CommonEditorUi.DrawSeparator(Color.cyan);
		
		DrawWaveDelayTime(waveManager);
		
		CommonEditorUi.DrawSeparator(Color.cyan);
		
		DrawWaveInfo(waveManager);
	}
	
	void DrawWaveTime(WaveManager waveManager)
	{
		int waveCount = EditorGUILayout.IntField("Wave Count", waveManager._waveCount);
		//if (waveCount != waveManager._waveCount)
		{
			CommonEditorUi.RegisterUndo("Wave Count", waveManager);
			
			int gap = waveCount - waveManager._waveList.Count;
			if (gap > 0)
			{
				for (int i = 0; i < gap; ++i)
				{
				//	Wave wave =  new Wave()
				//		wave.enemyPrefab = 
					waveManager._waveList.Add(new Wave());
				}
			}
			else
			{
				waveManager._waveList.RemoveRange(waveCount, -gap);
			}
			
			waveManager._waveCount = waveManager._waveList.Count;
		}
	}
	
	void DrawWaveDelayTime(WaveManager waveManager)
	{
		float waveDelayTime = EditorGUILayout.FloatField("Wave Delay Time", waveManager._waveDelayTime);
		if (waveDelayTime != waveManager._waveDelayTime)
		{
			CommonEditorUi.RegisterUndo("Wave Delay time", waveManager);
			waveManager._waveDelayTime = waveDelayTime;
		}
	}
	
	bool fold = true;
	void DrawWaveInfo(WaveManager waveManager)
	{
		fold = EditorGUILayout.Foldout(fold, "Wave List");
		if (fold == false)
			return;
		
		for (int i = 0; i < waveManager._waveList.Count; ++i)
		{
			Wave wave = waveManager._waveList[i];
			
			EditorGUILayout.LabelField("Wave : " + (i + 1));
			
			GUI.color = (wave.enemyPrefab != null) ? Color.green : Color.red;
			GameObject baseTilePrefab = (GameObject)EditorGUILayout.ObjectField("Enemy",
			                                                                    wave.enemyPrefab, typeof(GameObject));
			if (baseTilePrefab != wave.enemyPrefab)
			{
				CommonEditorUi.RegisterUndo("Enemy Prefab", waveManager);
				wave.enemyPrefab = baseTilePrefab;
			}
			GUI.color = Color.white;
			
			
			int spawnEnemyCount =
				EditorGUILayout.IntField("Spawn Enemy Count", wave.spawnEnemyCount);
			if (spawnEnemyCount != wave.spawnEnemyCount)
			{
				CommonEditorUi.RegisterUndo("Spawn Enemy Count", waveManager);
				
				wave.spawnEnemyCount = spawnEnemyCount;
			}
			
			
			float spawnEnemyTime =
				EditorGUILayout.FloatField("Spawn Enemy Time", wave.spawnEnemyTime);
			if (spawnEnemyTime != wave.spawnEnemyTime)
			{
				CommonEditorUi.RegisterUndo("Spawn Enemy Time", waveManager);
				
				wave.spawnEnemyTime = spawnEnemyTime;
			}
			
			CommonEditorUi.DrawSeparator(Color.cyan);
		}
	}
}
