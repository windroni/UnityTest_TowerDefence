using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 데이타에서 값을 읽어 범위를 지정하게 할수는 없나?
public class MapManager : MonoBehaviour 
{
	[Range(0, 3)] 
	public int _minWidth = 3;
	[Range(0, 20)]
	public int _maxWidth = 20;
	[Range(0, 3)]
	public int _minHeight = 3;
	[Range(0, 20)]
	public int _maxHeight = 20;

	[Range(0, 20)]
	public int currentMapWidth_ = 8;
	[Range(0, 20)]
	public int currentMapHeigth_ = 11;

	public GameObject[,] tiles_;
	public GameObject _baseTilePrefab;
	

	public void RemoveAllTiles()
	{
		List<GameObject> gameObjectList = new List<GameObject>();
		foreach(Transform child in transform)
		{
			gameObjectList.Add(child.gameObject);
		}

		for(int i = 0; i < gameObjectList.Count; ++i)
		{
			DestroyImmediate(gameObjectList[i]);
		}

		gameObjectList.Clear();
		gameObjectList = null;
		tiles_ = null;

		//pathList_.Clear();
	}
}
