using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour {
	
	public float spacing = 0.1f;
	public float tileSize = 1f;
	public GameObject tilePrefab;
	public List<Material> materials;
	private List<GameObject> grid;
	private List<GameObject> itemsToReplace;
	public int columns = 7;
	public int rows = 7;
	
	private List<Vector2> neighbours;
	
	public Tile selectedTile;
	private int multiplier = 0;
	
	// Use this for initialization
	void Start () {
		
		neighbours = new List<Vector2>();
		neighbours.Add(new Vector2(1, 0));
		neighbours.Add(new Vector2(0, 1));
		
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
		
		updateLogic();
	}
	
	void handleTileSelect(Tile tile) {
		
		if(selectedTile == null) {
			selectedTile = tile;
			selectedTile.transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
		} else {
			//Debug.Log("swap " + tile.Id);
			//selectedTile.transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);
			if(areNeighbours(selectedTile, tile)) {
				swapTile(selectedTile, tile);
				updateLogic();
				if(selectedTile.dir.x == 0 && selectedTile.dir.y == 0) {
					Tile sl = selectedTile;
					Debug.Log("swap back " + selectedTile.dir);
					StartCoroutine(waitAndSwap(sl, tile));
				}
				selectedTile.dir = new Vector2(0,0);
			}
			selectedTile = null;
			
			
		}
	}
	
	IEnumerator waitAndSwap(Tile tile1, Tile tile2) {
		yield return new WaitForSeconds(0.3f);
		swapTile(tile1, tile2);
	}
	
	void updateLogic() {
		multiplier++;
		itemsToReplace = new List<GameObject>();
		for (int x = 0; x < columns; x++) {
			for (int y = 0; y < rows; y++) {
				GameObject item = retrieve(x, y);
				if (item != null) {
					check(item, x, y);
				}
			}
		}
		if (itemsToReplace.Count > 1) {
			StartCoroutine(waitAndRemoveItems());
		} else {
			multiplier = 0;
			updateScore(0);
		}
	}
	
	void updateScore(int num) {
	}
	
	IEnumerator waitAndRemoveItems() {
		yield return new WaitForSeconds(0.3f);
		foreach (GameObject go in itemsToReplace) {
			Tile ts = (Tile) go.GetComponent(typeof(Tile));
			removeItem(ts.column, ts.row);
			//ts = null;
		}
		//itemsToReplace = null;
		dropColumns();
	}
	
	void dropColumns() {
		for (int i = 0; i < columns; i ++) {
			int spaces = 0;
			for (int j = 0; j < rows; j++) {
				Tile tile = retrieveScript(i, (rows - 1) - j);
				if (tile.isDead) {
					spaces++;
				} else {
					if (spaces > 0) {
						//_disableControls = true;
						iTween.MoveTo(tile.gameObject, iTween.Hash(
							"position", new Vector3(tile.gameObject.transform.position.x, (tile.gameObject.transform.position.y - spaces) - (spacing * spaces), tile.gameObject.transform.position.z),
							"time", 0.3f
							));
						tile.row += spaces;
						resortAll();
					}
				}
			}
			//for (int k = 0; k < spaces; k++) {
			addItems(i, spaces-1);
			//}
			spaces = 0;
		}
		
		StartCoroutine(waitAndUpdateLogic());
	}
	
	IEnumerator waitAndUpdateLogic() {
		yield return new WaitForSeconds(0.6f);
		updateLogic();
	}
	
	void addItems(int column, int gaps) {
		//Debug.Log("I am adding new tiles in column " + column);
		for(int i = 0; i < itemsToReplace.Count; i++) {
			GameObject go = itemsToReplace[i];
			Tile tile = (Tile) go.GetComponent(typeof(Tile));
			if(tile.column == column && tile.isDead) {
				tile.column = column;
				tile.row = gaps;
				tile.isDead = false;
				//Debug.Log("gaps " + gaps + " i " + i);
				float newY = -((gaps * tileSize) + (gaps * spacing) + (spacing/2));
				//Debug.Log(newY);
				int rnd = Random.Range(0, materials.Count);
				Material m = materials[rnd];
				tile.setMaterial(rnd, m);
				iTween.MoveTo(tile.gameObject, iTween.Hash(
					"position", new Vector3(tile.gameObject.transform.position.x, newY, tile.gameObject.transform.position.z),
					"time", 0.3f
					));
				gaps--;
			}
		}
	}
	
	GameObject retrieve(int x, int y) {	
		GameObject item = null;
		if (x >= 0 && y >= 0 && x < columns && y < rows) {
			//Debug.Log("retrieve " + (x * columns + y) + " " + grid.Count);
			item = grid[x * columns + y];
		}
		return item;
	}
	
	Tile retrieveScript(int x, int y) {
		GameObject go = retrieve(x, y);
		if( go ) {
			return (Tile) go.GetComponent(typeof(Tile));
		}
		return null;
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
		
		Vector2 tempColumRow = new Vector2(tile1.column, tile1.row);
		//Debug.Log(tile1.transform.position);
		iTween.MoveTo(tile1.gameObject, iTween.Hash(
			"position", tile2.transform.position,
			"time", 0.3f
			));
		iTween.MoveTo(tile2.gameObject, iTween.Hash( 
		                                            "position", tile1.transform.position, 
		                                            "time", 0.3f
		                                            ));
		
		tile1.column = tile2.column;
		tile1.row = tile2.row;
		tile2.column = (int)tempColumRow.x;
		tile2.row = (int)tempColumRow.y;
		
		
	}
	
	void check(GameObject go, int x, int y ) {
		//Debug.Log("check for lines " +  x + " " + y);
		Tile tile = (Tile) go.GetComponent(typeof(Tile));
		Vector2 dir;
		for (int i = 0; i < neighbours.Count; i++) {
			Tile neighbour = retrieveScript(x + (int)neighbours[i].x, y + (int)neighbours[i].y);
			dir = neighbours[i];
			if (neighbour != null) {
				if (tile.id == neighbour.id) {
					if (!isPartOfLine(neighbour, dir)) {
						List<GameObject> matchedItems = new List<GameObject>( new GameObject [] {tile.gameObject, neighbour.gameObject} );
						checkDirection(neighbour.column, neighbour.row, dir, tile.id, matchedItems);
					}
				}
			}
		}
		
	}
	
	void checkDirection(int x, int y, Vector2 dir, int id, List<GameObject> matchedItems) {
		Tile next = retrieveScript(x + (int) dir.x, y + (int) dir.y);
		if(next) {
			if(next.id == id) {
				if(selectedTile) {
					selectedTile.dir = dir;
				}
				matchedItems.Add(next.gameObject);
				next.dir = new Vector2(dir.x, dir.y);
				Debug.Log("next tile "+ next + " next id " + id + " id " + id); 
				checkDirection(next.column, next.row, dir, id, matchedItems);
			} else {
				finaliseLine(matchedItems);
			}
		} else {
			finaliseLine(matchedItems);
		}
		
	}
	
	void finaliseLine(List<GameObject> matchedItems) {
		if (matchedItems.Count > 2) {
			for (int j = 0; j < matchedItems.Count; j++) {
				GameObject item = matchedItems[j];
				itemsToReplace.Add(item);
				updateScore(30);
			}
		}
	}
	
	bool isPartOfLine(Tile tile, Vector2 dir) {
		bool partOfLine = false;
		if (tile.dir.magnitude > 0) {
			if (tile.dir.x == dir.x && tile.dir.y == dir.y) {
				partOfLine = true;
			}
		}
		return partOfLine;
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
		
		for (int i = 0; i < 49; i++ ) 
		{			
			GameObject item = grid[i];
			if(item != null) {
				//Tile ts = (Tile) item.GetComponent(typeof(Tile));
				newOrder.Add(item);
			}
		}			
		grid = new List<GameObject>();
		grid.AddRange(newOrder);				
	}
	
	public void removeItem(int column, int row) 
	{
		Tile item = retrieveScript(column, row);			
		if(item != null) {
			//removeChild(item);
			//item.destroy();
			//TODO add object pool here
			//item = null;
			item.gameObject.transform.localScale -= new Vector3(0.2f,0.2f,0.2f);
			item.gameObject.transform.position = new Vector3(item.gameObject.transform.position.x, 1.45f, item.gameObject.transform.position.z);
			item.isDead = true;
			//grid[column * columns + row] = null;
		}
	}		
	
	// Update is called once per frame
	void Update () {
		
	}
}
