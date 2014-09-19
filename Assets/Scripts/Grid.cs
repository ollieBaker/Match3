using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour {

	public float spacing = 0.1f;
	public float tileSize = 1f;
	public GameObject tilePrefab;
	public List<Material> materials;
	private List<GameObject> grid;
	public int columns = 7;
	public int rows = 7;

	private Tile selectedTile1;

	// Use this for initialization
	void Start () {
		grid = new List<GameObject>( );
		for (int i = 0; i < columns; i++) {
			for (int j = 0; j < rows; j++) {
				GameObject t = Instantiate(tilePrefab, new Vector3(
					(i * tileSize) + (spacing * i) + (spacing/2), 
					-(j * tileSize) - (spacing * j) - (spacing/2), 
					0), Quaternion.identity) as GameObject;
				grid.Add(t);
				Tile ts = (Tile) t.GetComponent(typeof(Tile));
				int rnd = Random.Range(0, materials.Count);
				Material m = materials[rnd];
				ts.init(rnd, m, i, j);
				ts.OnTileSelected += handleTileSelect;
				//grid[i * columns + j] = t;
			}
		}
	}

	void handleTileSelect(Tile tile) {

		if(selectedTile1 == null) {
			selectedTile1 = tile;
			selectedTile1.transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
		} else {
			Debug.Log("swap " + tile.Id);
			selectedTile1.transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);
			if(areNeighbours(selectedTile1, tile)) {
				swapTile(selectedTile1, tile);
			}
			selectedTile1 = null;
				

		}
	} 

	GameObject retrieve(int x, int y) {	
		GameObject item = null;
		if (x >= 0 && y >= 0 && x < columns && y < rows) {
			item = grid[x * columns + y];
		}
		return item;
	}

	public void setNewGridPosition(GameObject tile, int x, int y) {	
		Tile ts = (Tile) tile.GetComponent(typeof(Tile));
		grid[x * columns + y] = tile;	
		ts.column = x;
		ts.row = y;
		
		//retrieve(x,y).filters=[new GlowFilter(getColour(),1,10,10,5)]
	}
	
	public void swapTile(Tile tile1, Tile tile2) {
		int a = grid.IndexOf(tile1.gameObject);
		int b = grid.IndexOf(tile2.gameObject);

		grid[a] = tile2.gameObject;
		grid[b] = tile1.gameObject;	

		//Vector3 tempPoint = new Vector3(tile1.transform.position.x, tile1.transform.position.y, tile1.transform.position.z );
		Vector2 tempColumRow = new Vector2(tile1.column, tile1.row);

		iTween.MoveTo(tile1.gameObject, tile2.transform.position, 0.3f);
		iTween.MoveTo(tile2.gameObject, tile1.transform.position, 0.3f);
		//Actuate.tween(selectedItem, TWEEN_TIME, {x: target.x, y: target.y});
		//Actuate.tween(target, TWEEN_TIME, {x: tempPoInt.x, y: tempPoInt.y});

		tile1.column = tile2.column;
		tile1.row = tile2.row;
		tile2.column = (int)tempColumRow.x;
		tile2.row = (int)tempColumRow.y;
		

	}

	bool areNeighbours(Tile selectedItem, Tile target) {
		bool neighbours = false;
		if (selectedItem.row == target.row || selectedItem.column == target.column) {
			if (Mathf.Abs(selectedItem.row - target.row) == 1 || Mathf.Abs(selectedItem.column - target.column) == 1) {
				neighbours = true;
			}
		}
		return neighbours;
	}

	public void resortAll() 
	{
		List<GameObject> newOrder = new List<GameObject>();
		//newOrder.length = 49;
		
		for (int i = 0; i < 0; i++ ) 
		{			
			GameObject item = grid[i];
			if(item != null) {
				Tile ts = (Tile) item.GetComponent(typeof(Tile));
				newOrder[ts.column * columns + ts.row] = item;
			}
		}			
		grid = new List<GameObject>();
		grid.AddRange(newOrder);				
	}
	
	public void removeItem(int column, int row) 
	{
		GameObject item = retrieve(column, row);			
		if(item != null) {
			//removeChild(item);
			//item.destroy();
			//item = null;
			//grid[column * columns + row] = null;
		}
	}		

	// Update is called once per frame
	void Update () {
	
	}
}
