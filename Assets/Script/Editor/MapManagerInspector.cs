using UnityEngine;
using UnityEditor;
using System.Collections;

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
		//CommonEditorUi.DrawSeparator ();
		//DrawMinMaxValues ();
		//DrawBaseTileObject ();
		DrawGenerateButton();
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


	void OnSceneGUI()
	{
		if(Application.isPlaying)
			return;

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

		int controlID = GUIUtility.GetControlID(FocusType.Passive);
		HandleUtility.AddDefaultControl(controlID);

		Event e = Event.current;
		if(e.isKey)
		{
			Debug.Log("key click");
			if(e.character == '1') mapManager_._editTileStyle = TILESTYLE.NORMAL;
			else if(e.character == '2') mapManager_._editTileStyle = TILESTYLE.STRAIGHT;
			else if(e.character == '3') mapManager_._editTileStyle = TILESTYLE.CORNER;
			else if(e.character == '4') mapManager_._editTileStyle = TILESTYLE.START;
			else if(e.character == '5') mapManager_._editTileStyle = TILESTYLE.END;
		}

		else if(e.type == EventType.mouseDown || e.type == EventType.mouseDrag)
		{
			if(e.alt)
			{
				return;
			}

			Vector2 mousePosition = Event.current.mousePosition;

			Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
			RaycastHit hit;
			bool result  = Physics.Raycast(ray, out hit, 1000.0f);

			if(result)
			{
				GameObject tileObj = hit.transform.gameObject;
				TileInfo tileInfo = tileObj.GetComponent<TileInfo>();
				if(tileInfo == null)
					return;
				if(e.button == 0) //left click
				{
					Debug.Log("edit left click");

					tileInfo.currnetTileStyle = mapManager_._editTileStyle;
					tileInfo.UpdateMaterial();
				}
				else if(e.button == 1) //right click
				{
					hit.transform.localEulerAngles += new Vector3(0.0f, 90.0f, 0.0f);
				}
			}
		}
	}

	void PathAddOrRemove(int buttonCode, Event currentEvent, RaycastHit hit, TileInfo tileInfo)
	{
		if(buttonCode == 0)
		{
			if(currentEvent.shift)
			{
				if(mapManager_._pathList.Contains(hit.transform) == false)
					mapManager_._pathList.Add(hit.transform);
			}
			else
			{
				tileInfo
			}
		}
	}





	void DrawBaseTileObject()
	{
		GUI.color = (mapManager_._baseTilePrefab != null) ? Color.green : Color.red;

		GameObject baseTilePrefab = (GameObject)EditorGUILayout.ObjectField ("Base Field", mapManager_._baseTilePrefab, typeof(GameObject));
		if (baseTilePrefab != mapManager_._baseTilePrefab) 
		{
			CommonEditorUi.RegisterUndo("Map Base Tile", mapManager_);
			mapManager_._baseTilePrefab = baseTilePrefab;
		}

		GUI.color = Color.white;
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
