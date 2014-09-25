using UnityEngine;
using System.Collections;

public delegate void TileSelected(Tile tile);

public class Tile : MonoBehaviour {

	public int id;
	private int size;
	private int colour;
	private bool selected = false;
	public int row;
	public int column;

	public bool isDead = false;

	public event TileSelected OnTileSelected;

	public Vector2 dir = new Vector2(0,0);
	private Material material;
	public GameObject tile;

	// Use this for initialization
	void Start () {

	}

	public void init (int id, Material material, int col, int row) {
		this.column = col;
		this.row = row;
		setMaterial(id, material);
	}

	public void setMaterial(int id, Material material) {
		this.id = id;
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
