using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour {

	public float spacing = 0.1f;
	public float tileSize = 1f;
	public GameObject tilePrefab;
	private List<GameObject> grid;
	public int columns = 7;
	public int rows = 7;

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
				ts.init(0, i, j);
				//grid[i * columns + j] = t;
			}
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
	
	public void swapItems(GameObject selectedItem, GameObject target) {
		int a = grid.IndexOf(selectedItem);
		int b = grid.IndexOf(target);
		
		grid[a] = target;
		grid[b] = selectedItem;	
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
