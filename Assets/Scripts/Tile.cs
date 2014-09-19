using UnityEngine;
using System.Collections;

public delegate void TileSelected(Tile tile);

public class Tile : MonoBehaviour {

	private int id;
	private int size;
	private int colour;
	private bool selected = false;
	public int row;
	public int column;

	public event TileSelected OnTileSelected;

	private Vector2 dir;
	private Material material;
	public GameObject tile;

	// Use this for initialization
	void Start () {

	}

	public void init (int id, Material material, int col, int row) {
		this.id = id;
		this.column = col;
		this.row = row;
		this.renderer.material = material;
	}

	void OnMouseDown () {
		if(selected) {
			selected = false;
		} else {
			selected = true;
		}
		OnTileSelected(this);
	}

	public int Id {
		get {
			return id;
		}
	}

	// Update is called once per frame
	void Update () {
	
	}
}
