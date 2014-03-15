using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

/*
 * 텍스처의 이해 (파이프라인에 대한 이해)
 * 텍스처 + 쉐이더 = 머터리얼 
 * 텍스처나 쉐이더가  스위칭하면 드로우콜 발생 
 *
 */

[CustomEditor(typeof(MapManager))]
public class MapManagerInspector : Editor
{
	MapManager mapManager_;

	public void OnEnable()
	{
		mapManager_ = target as MapManager;
	}

	public void CreateTiles()
	{
		int heigth = mapManager_.currentMapHeigth_;
		int width = mapManager_.currentMapWidth_;
		
		mapManager_.tiles_ = new GameObject[width, heigth];
		for(int i = 0;  i < width; ++i)
		{
			for(int j = 0; j < heigth; ++j)
			{
				GameObject obj = Instantiate(mapManager_._baseTilePrefab) as GameObject;
				
				obj.transform.parent = mapManager_.transform;
				obj.transform.localPosition = new Vector3(i, 0.0f, j);
				obj.name = i + "_" + j;
				mapManager_.tiles_[i,j] = obj;

				TileInfo tileInfo = mapManager_.tiles_[i,j].GetComponent<TileInfo>();
				tileInfo.currnetTileStyle = mapManager_._editTileStyle;
				tileInfo.UpdateMaterial();
			}
		}
	}

	public override void OnInspectorGUI()
	{
		this.DrawDefaultInspector ();
		DrawPathData();
		DrawGenerateButton();
		DrawSaveLoadButton();
	}


	void DrawGenerateButton()
	{
		EditorGUILayout.BeginHorizontal ();
		if (GUILayout.Button ("Generate Map Data")) 
		{
			CreateTiles();
		}

		if (GUILayout.Button ("Generate Map Data")) 
		{
			mapManager_.RemoveAllTiles();
		}

		EditorGUILayout.EndHorizontal();
	}


	void DrawUI()
	{
		Handles.BeginGUI();
		if(GUI.Button(new Rect(10, 10, 100, 30), "NORMAL"))
		{
			mapManager_._editTileStyle = TILESTYLE.NORMAL;
		}
		if(GUI.Button(new Rect(10, 50, 100, 30), "STRAIGHT"))
		{
			mapManager_._editTileStyle = TILESTYLE.STRAIGHT;
		}
		if(GUI.Button(new Rect(10, 90, 100, 30), "CORNER"))
		{
			mapManager_._editTileStyle = TILESTYLE.CORNER;
		}
		if(GUI.Button(new Rect(10, 130, 100, 30), "START"))
		{
			mapManager_._editTileStyle = TILESTYLE.START;
		}
		if(GUI.Button(new Rect(10, 170, 100, 30), "END"))
		{
			mapManager_._editTileStyle = TILESTYLE.END;
		}
		
		GUI.color = Color.green;
		GUI.Label(new Rect(120, 10, 500, 30), "Eidt Mode : " + mapManager_._editTileStyle);
		GUI.color = Color.white;
		
		Handles.EndGUI();
	}

	void ProcessHotKey(char value)
	{
		if(value == '1') mapManager_._editTileStyle = TILESTYLE.NORMAL;
		else if(value == '2') mapManager_._editTileStyle = TILESTYLE.STRAIGHT;
		else if(value == '3') mapManager_._editTileStyle = TILESTYLE.CORNER;
		else if(value == '4') mapManager_._editTileStyle = TILESTYLE.START;
		else if(value == '5') mapManager_._editTileStyle = TILESTYLE.END;
	}

	void OnSceneGUI()
	{
		if(Application.isPlaying)
			return;
		
		DrawUI();
		
		int controlID = GUIUtility.GetControlID(FocusType.Passive);
		HandleUtility.AddDefaultControl(controlID);
		
		Event e = Event.current;
		if(e.isKey)
		{
			ProcessHotKey(e.character);
		}	
		else if(e.type == EventType.mouseDown || e.type == EventType.mouseDrag)
		{
			if(e.alt)
			{
				return;
			}
			
			// 현재 마우스 버튼의 좌료를 얻는다.(2D)
			Vector2 mousePosition = Event.current.mousePosition;
			Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition); // 카메라의 점과 마우스가 찍은 점을 연결시켜 레이를 만든다.
			
			RaycastHit hit;
			bool result  = Physics.Raycast(ray, out hit, 1000.0f); //레이가 지나가면서 충돌된 물체는 hit에 담긴다.
			
			GameObject tileObj = null;
			TileInfo tileInfo = null;
			
			if(result)
			{
				tileObj = hit.transform.gameObject;
				tileInfo = tileObj.GetComponent<TileInfo>();
				
				if(tileInfo == null)
					return;
				
				if(e.button == 0) //left click
				{
					if(e.shift)
					{
						if(mapManager_._pathList.Contains(hit.transform) == false)
						{
							mapManager_._pathList.Add(hit.transform);
							tileInfo.currnetTileStyle = mapManager_._editTileStyle;
							tileInfo.UpdateMaterial();
						}
					}
					else
					{
						tileInfo.currnetTileStyle = mapManager_._editTileStyle;
						tileInfo.UpdateMaterial();
					}
				}
				else if(e.button == 1) //right click
				{
					if(e.shift)
					{
						if(mapManager_._pathList.Contains(hit.transform) == true)
						{
							mapManager_._pathList.Remove(hit.transform);
						}
					}
					else
					{
						hit.transform.localEulerAngles += new Vector3(0.0f, 90.0f, 0.0f);
					}
				}


			}
		}
		
		DrawPathList();
	}



	void DrawPathList()
	{
		for(int i = 0; i < mapManager_._pathList.Count; ++i)
		{
			Transform obj = mapManager_._pathList[i];
			TileInfo tileInfo = obj.GetComponent<TileInfo>();
			if(tileInfo.currnetTileStyle == TILESTYLE.START)
			{
				Handles.color = Color.green;
			}
			else if(tileInfo.currnetTileStyle == TILESTYLE.END)
			{
				Handles.color = Color.red;
			}


			Handles.DrawSphere(i, obj.position + Vector3.up * 0.5f,
			                   obj.rotation, 0.5f);
			
			Handles.color = Color.white;
		}

		if(mapManager_._pathList.Count > 2)
		{
			Handles.color = Color.magenta;
			for(int i= 0; i < mapManager_._pathList.Count - 1; ++i)
			{
				Transform obj1 = mapManager_._pathList[i];
				Transform obj2 = mapManager_._pathList[i + 1];
				Vector3 pos1 = obj1.position + Vector3.up * 0.5f;
				Vector3 pos2 = obj2.position + Vector3.up * 0.5f;
				Handles.DrawLine(pos1, pos2);
			}

			Handles.color = Color.white;
		}
	}

	void DrawPathData()
	{
		if(GUILayout.Button("Remove All Path Data"))
		{
			mapManager_._pathList.Clear();
		}

		string pathLabel = "Path List : "  + mapManager_._pathList.Count;
		EditorGUILayout.LabelField(pathLabel);

		for(int i = 0; i < mapManager_._pathList.Count; ++i)
		{
			string pathName = "Path Index : " + i;
			Transform pathTransform = mapManager_._pathList[i];
			EditorGUILayout.ObjectField(pathName, pathTransform, typeof(GameObject));
		}
	}



	public void DrawSaveLoadButton()
	{
		if(string.IsNullOrEmpty(mapManager_._fileName))
			mapManager_._fileName = "Map";

		string fileName = EditorGUILayout.TextField("Map File Name", mapManager_._fileName);

		if(fileName != mapManager_._fileName)
		{
			CommonEditorUi.RegisterUndo("Map File Name", mapManager_);
			mapManager_._fileName = fileName;
		}

		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("Save Map Data"))
		{
			SaveFile();
		}

		if(GUILayout.Button("Laod Map Data"))
		{
			mapManager_.LoadFile();
		}

		EditorGUILayout.EndHorizontal();
	}

	void SaveFile()
	{
		string dataPath = Application.dataPath;
		string fullPath = dataPath + "/Resources/MapData/" + mapManager_._fileName + ".txt";

		//string path = Path.Combine(dataPath, "/Resources/MapData/"); // 사이사이에 경로 "/" 를 넣어준다.
		//path += mapManager_._fileName;

		FileStream fs = new FileStream(fullPath, FileMode.Create);
		TextWriter textWriter = new StreamWriter(fs);

		int width = mapManager_.currentMapWidth_;
		int height = mapManager_.currentMapHeigth_;

		textWriter.Write("width " + width + "\n");
		textWriter.Write("height " + height + "\n");

		for(int i = 0; i < width; ++i)
		{
			for(int j = 0; j < height; ++j)
			{
				Transform tile = mapManager_.tiles_[i,j].transform;
				textWriter.Write(tile.position + "\t");
				textWriter.Write(tile.eulerAngles + "\t");

				TileInfo tileInfo = tile.GetComponent<TileInfo>();
				textWriter.Write((int)tileInfo.currnetTileStyle + "\t");
				textWriter.Write("\n");
			}
		}


		//save Path
		int pathCount = mapManager_._pathList.Count;
		textWriter.Write("PathCount " + pathCount + "\n");

		for(int i = 0; i < pathCount; ++i)
		{
			string [] tileIndex = mapManager_._pathList[i].name.Split('_');
			textWriter.Write(tileIndex[0] + "\t");
			textWriter.Write(tileIndex[1]);
			textWriter.Write("\n");
		}

		textWriter.Close();
	}











	void DrawBaseTileObject()
	{
		/*GUI.color = (mapManager_._baseTilePrefab != null) ? Color.green : Color.red;

		GameObject baseTilePrefab = (GameObject)EditorGUILayout.ObjectField ("Base Field", mapManager_._baseTilePrefab, typeof(GameObject));
		if (baseTilePrefab != mapManager_._baseTilePrefab) 
		{
			CommonEditorUi.RegisterUndo("Map Base Tile", mapManager_);
			mapManager_._baseTilePrefab = baseTilePrefab;
		}

		GUI.color = Color.white;*/
	}

	void DrawMinMaxValues()
	{
		GUI.color = ( mapManager_._minWidth >= 3 &&
		             mapManager_._minWidth <= 20) ?
			Color.green : Color.red;
		int minWidth =
			EditorGUILayout.IntField("Map Min Width", mapManager_._minWidth);
		if( minWidth != mapManager_._minWidth )
		{
			CommonEditorUi.RegisterUndo( "Map Min Width", mapManager_ );
			
			mapManager_._minWidth = minWidth;
		}
		
		GUI.color = Color.white;
		
		GUI.color = ( mapManager_._minHeight >= 3 &&
		             mapManager_._minHeight <= 20) ?
			Color.green : Color.red;
		int minHeight =
			EditorGUILayout.IntField("Map Min Height", mapManager_._minHeight);
		if( minHeight != mapManager_._minHeight )
		{
			CommonEditorUi.RegisterUndo( "Map Min Height", mapManager_ );
			
			mapManager_._minHeight = minHeight;
		}		
		
		GUI.color = Color.white;
		
		CommonEditorUi.DrawSeparator();
		
		GUI.color = ( mapManager_._maxWidth >= 3 &&
		             mapManager_._maxWidth <= 20) ?
			Color.green : Color.red;
		int maxWidth =
			EditorGUILayout.IntField("Map Max Width", mapManager_._maxWidth);
		if( maxWidth != mapManager_._maxWidth )
		{
			CommonEditorUi.RegisterUndo( "Map Max Width", mapManager_ );
			
			mapManager_._maxWidth = maxWidth;
		}		
		
		GUI.color = Color.white;
		
		GUI.color = ( mapManager_._maxHeight >= 3 &&
		             mapManager_._maxHeight <= 20) ?
			Color.green : Color.red;;
		int maxHeight =
			EditorGUILayout.IntField("Map Max Height", mapManager_._maxHeight);
		if( maxHeight != mapManager_._maxHeight )
		{
			CommonEditorUi.RegisterUndo( "Map Max Height", mapManager_ );
			
			mapManager_._maxHeight = maxHeight;
		}		
		
		GUI.color = Color.white;
	}		
}
