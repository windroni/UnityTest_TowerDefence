using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

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

	public TILESTYLE _editTileStyle = TILESTYLE.NORMAL;
	public List<Transform> _pathList = new List<Transform>();

	public string _fileName;


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
		
		_pathList.Clear();
	}

	public void LoadFile()
	{
		RemoveAllTiles();

		string dataPath = Application.dataPath;
		//string filePath = dataPath + "/Resources/MapData/" + _fileName;
		
		string filePath = "MapData/" + _fileName;
		TextAsset asset = (TextAsset)Resources.Load(filePath, typeof(TextAsset));
		
		if(asset == null)
		{
			Debug.Log("Can not open file on Asset/" + filePath + ".txt");
			return;
		}
		
		Stream stream = new MemoryStream(asset.bytes);
		TextReader textReader  = new StreamReader(stream);
		
		// read.width	
		string text = textReader.ReadLine();
		int spaceIndex = text.IndexOf(' ');
		string widthText = text.Substring(spaceIndex + 1);
		Debug.Log(widthText);
		currentMapWidth_ = int.Parse(widthText);

		// read.height	
		text = textReader.ReadLine();
		string heightText = text.Substring(text.IndexOf(' ') + 1);
		currentMapHeigth_ = int.Parse(heightText);

		// Create Map
		tiles_ = new GameObject[currentMapWidth_, currentMapHeigth_];
		for(int i = 0; i < currentMapWidth_; ++i)
		{
			for(int j = 0; j < currentMapHeigth_; ++j)
			{
				text = textReader.ReadLine();
				string[] infos = text.Split('\t');
				GameObject obj = Instantiate(_baseTilePrefab) as GameObject;
				obj.name = i + "_" + j;
				obj.transform.parent = transform;
				obj.transform.localPosition = GetVector3FromString(infos[0]);
				obj.transform.eulerAngles = GetVector3FromString(infos[1]);

				TileInfo tileInfo = obj.GetComponent<TileInfo>();
				tileInfo.currnetTileStyle = (TILESTYLE)(int.Parse(infos[2]));
				tileInfo.UpdateMaterial();
				tiles_[i,j] = obj;
			}
		}

		// load path 
		text = textReader.ReadLine();
		string pathCountText = text.Substring(text.IndexOf(' '));
		int pathCount = int.Parse(pathCountText);
		for (int i = 0; i < pathCount; ++i)
		{
			text = textReader.ReadLine();
			
			string[] tiles = text.Split('\t');
			int x = int.Parse(tiles[0]);
			int y = int.Parse(tiles[1]);
			
			_pathList.Add(tiles_[x, y].transform);
		}
	}

	public Vector3 GetVector3FromString(string text)
	{
		string newText = text.Replace('(', ' ');
		newText = newText.Replace(')', ' ');
		
		string[] datas = newText.Split(',');
		
		float x = float.Parse(datas[0]);
		float y = float.Parse(datas[1]);
		float z = float.Parse(datas[2]);
		return new Vector3(x, y, z);
	}

#if UNITY_EDITOR
	public void OnValidate()
	{
		Debug.Log("OnValidate");
	}
#endif

}
