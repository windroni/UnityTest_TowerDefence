using UnityEngine;
using System.Collections;

public class TileInfo : MonoBehaviour {

	public TILESTYLE currnetTileStyle;
	public Material[] tileMaterials;

	// Use this for initialization
	public void UpdateMaterial () 
	{
		renderer.material = tileMaterials[(int)currnetTileStyle];
	}
}
