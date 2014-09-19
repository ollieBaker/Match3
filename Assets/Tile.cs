using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {

	[SerializeField]
	private int id;
	private int size;
	private int colour;
	public int row;
	public int column;
	private Vector2 dir;
	private Material material;
	public GameObject tile;

	// Use this for initialization
	void Start () {
	}

	public void init (int id, int col, int row) {
		this.id = id;
		this.column = col;
		this.row = row;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
