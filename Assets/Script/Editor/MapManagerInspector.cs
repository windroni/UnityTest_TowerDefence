using UnityEngine;
using UnityEditor;
using System.Collections;

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
