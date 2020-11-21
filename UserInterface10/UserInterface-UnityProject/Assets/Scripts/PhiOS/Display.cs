using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Display : MonoBehaviour {

	public static Display GET;

	[Header("COMPONENTS")]
	public BitmapFont font;
	public Camera mainCamera;
	public GameObject displayQuadPrefab;
	public Material backgroundMaterial;
	public DisplayMesh background;
	public DisplayMesh foreground;

	[Header("SIZE")]
	public int displayWidth = 80;
	public bool autoDisplayHeight = true;
	public int displayHeight;

	[Header("TRANSITIONS")]
	public AnimationCurve colorLerpCurve;

	[HideInInspector]
	public bool initialized = false;

	[HideInInspector]
	public Color clearColor;

	private float quadWidth;
	private float quadHeight;
	private int reservedLayers = 1;
	private int initialNumLayers = 3;
	private int numLayers = 0;
	private Dictionary<int, Cell[,]> cells = new Dictionary<int, Cell[,]>();
	private LinkedList<Cell> cellList = new LinkedList<Cell>();
	private LinkedList<int>[,] topLayers;
	private Vector3 zero3 = Vector3.zero;
	private Vector2 zero2 = Vector2.zero;

	public void Awake(){
		GET = this;

	
		clearColor = Color.clear;
	}

	public IEnumerator Start(){
		
		
		while (!font.fontLoaded) {
			yield return null;
		}
	

		quadWidth = 1f;
		quadHeight = 
			(font.GetGlyphHeight() / font.GetGlyphWidth()) * 
			(font.useRexPaintFont ? font.rexPaintQuadHeightScale : font.quadHeightScale);


		int maxDisplayHeight = Mathf.RoundToInt((((float)Screen.height / (float)Screen.width) * (float)displayWidth) / quadHeight);
		if (autoDisplayHeight) {
			displayHeight = maxDisplayHeight;
		}


		int quadMeshFilterIndex = 0;
		MeshFilter[] quadMeshFilters = new MeshFilter[displayWidth * displayHeight];
		for (int y = 0; y < displayHeight; y++) {
			for (int x = 0; x < displayWidth; x++) {


				GameObject quad = (GameObject) GameObject.Instantiate(displayQuadPrefab);
				quad.transform.parent = transform;
				quad.transform.localScale = new Vector3(quadWidth, quadHeight, 1f);
				quad.transform.position = new Vector3(x * quadWidth + quadWidth * 0.5f, -y * quadHeight - quadHeight * 0.5f, 0f);


				quadMeshFilters[quadMeshFilterIndex] = quad.GetComponent<MeshFilter>();
				quadMeshFilterIndex++;
			}
		}


		CombineInstance[] combineInstances = new CombineInstance[quadMeshFilters.Length];
		for (int i = 0; i < quadMeshFilters.Length; i++) {
			combineInstances[i].mesh = quadMeshFilters[i].sharedMesh;
			combineInstances[i].transform = quadMeshFilters[i].transform.localToWorldMatrix;
		}


		background.CombineQuads(combineInstances, "background_quads", backgroundMaterial, 0.001f);
		foreground.CombineQuads(combineInstances, "foreground_quads", font.GetFontMaterial(), 0f);


		mainCamera.orthographicSize = Mathf.Max(maxDisplayHeight * quadHeight * 0.5f, background.transform.position.y);

		yield return null;


		for (int i = quadMeshFilters.Length - 1; i >= 0; i--) {
			GameObject.Destroy(quadMeshFilters[i].gameObject);
		}
		quadMeshFilters = null;

		Debug.Log ("DISPLAY SIZE: " + displayWidth + "x" + displayHeight);


		topLayers = new LinkedList<int>[displayWidth, displayHeight];
		for (int y = 0; y < displayHeight; y++) {
			for (int x = 0; x < displayWidth; x++) {
				topLayers[x,y] = new LinkedList<int>();
			}
		}

	
		for (int layerIndex = -reservedLayers; layerIndex < initialNumLayers; layerIndex++) {
			Cell[,] layer = new Cell[displayWidth, displayHeight];
			for (int y = 0; y < displayHeight; y++) {
				for (int x = 0; x < displayWidth; x++) {
					layer[x, y] = CreateCell(layerIndex, x, y);
				}
			}
			cells.Add (layerIndex, layer);
		}
		numLayers = initialNumLayers;

		yield return null;


		initialized = true;
	}

	private Cell CreateCell(int layerIndex, int x, int y){


		Cell cell = new Cell();
		cell.layer = layerIndex;
		cell.position = new Vector2 (x, y);
		cellList.AddLast(cell);
		return cell;
	}

	public int GetNumLayers(){
		if (initialized) {
			return numLayers;
		} else {
			throw new UnityException("Display not yet initialized!");
		}
	}

	public Cell GetCell(int layer, float x, float y){

		if (initialized) {

			Cell[,] layerCells = null;
			if (!cells.TryGetValue(layer, out layerCells)) {
				layerCells = new Cell[displayWidth, displayHeight];
				cells.Add(layer, layerCells);
				numLayers = Mathf.Max(layer + 1, numLayers);
			}

			if (x >=0 && y >= 0 && x < displayWidth && y < displayHeight) {
				Cell cell = layerCells[(int)x, (int)y];
				if (cell == null) {			
					cell = layerCells[(int)x, (int)y] = CreateCell(layer, (int)x, (int)y);
				}

				return cell;
			} 


			else {
				return null;
			}

		} else {
			throw new UnityException("Display not yet initialized!");
		}
	}

	public void AddCellAsTopLayer(Cell c){

		if (initialized) {


			LinkedList<int> topLayersForCell = topLayers[(int)c.position.x, (int)c.position.y];


			if (topLayersForCell.Count == 0 ||
			    (topLayersForCell.Last.Value >= 0 && topLayersForCell.Last.Value < c.layer) ||
			    (topLayersForCell.Last.Value >= 0 && c.layer < 0) ||
			    (topLayersForCell.Last.Value < 0 && c.layer < 0 && c.layer < topLayersForCell.Last.Value)) {
				topLayersForCell.AddLast(c.layer);
				return;
			}


			else if (
				(topLayersForCell.First.Value >= 0 && c.layer >= 0 && topLayersForCell.First.Value > c.layer) ||
				(topLayersForCell.First.Value < 0 && c.layer >= 0) ||
				(topLayersForCell.First.Value < 0 && c.layer < 0 && topLayersForCell.First.Value < c.layer)) {
				topLayersForCell.AddFirst(c.layer);
				return;
			}


			LinkedListNode<int> current = topLayersForCell.First;
			while (current.Next != null) {

			
				if (current.Value == c.layer || current.Next.Value == c.layer) {
					return;
				}

				if ((current.Value >= 0 && c.layer >= 0 && current.Next.Value >= 0 && current.Value < c.layer && current.Next.Value > c.layer) ||
				    (current.Value >= 0 && c.layer >= 0 && current.Next.Value < 0 && current.Value < c.layer) ||
				    (current.Value >= 0 && c.layer < 0 && current.Next.Value < 0 && current.Next.Value < c.layer) ||
				    (current.Value < 0 && c.layer < 0 && current.Value > c.layer && current.Next.Value < c.layer)) {
					topLayersForCell.AddAfter(current, c.layer);
					return;
				}

				current = current.Next;
			}
		} else {
			throw new UnityException("Display not yet initialized!");
		}
	}

	public void RemoveCellAsTopLayer(Cell c){

		if (initialized) {

	
			LinkedList<int> topLayersForCell = topLayers[(int)c.position.x, (int)c.position.y];

		
			if (topLayersForCell.Contains(c.layer)) {
				topLayersForCell.Remove(c.layer);
			}
		} else {
			throw new UnityException("Display not yet initialized!");
		}
	}

	public LinkedList<int> GetTopLayersForCell(int x, int y){
		if (initialized) {
			if (x >= 0 && y >= 0 && x < displayWidth && y < displayHeight) {
				return topLayers[x, y];
			} else {
				return new LinkedList<int>();
			}
		} else {
			throw new UnityException("Display not yet initialized!");
		}
	}

	public Color GetBackgroundColorForCell(int x, int y, params int[] excludedLayers){

		if (initialized) {
			List<int> excludedLayersList = new List<int>(excludedLayers);


			LinkedList<int> topLayer = GetTopLayersForCell(x, y);
			if (topLayer.First != null && !excludedLayersList.Contains(topLayer.First.Value)) {
				LinkedListNode<int> topLayerNode = topLayer.Last;
				while (excludedLayersList.Contains(topLayerNode.Value)) {
					topLayerNode = topLayerNode.Previous;
				}
				Cell cell = GetCell(topLayerNode.Value, x, y);
				return (cell != null) ? cell.backgroundColor : clearColor;
			}

			return clearColor;
		} else {
			throw new UnityException("Display not yet initialized!");
		}
	}

	public void Update(){
		
	
		if (initialized) {

	
			LinkedListNode<Cell> cellNode = cellList.First;
			while(cellNode!=null){				
				cellNode.Value.Update();
				cellNode = cellNode.Next;
			}

			
			for (int y = 0; y < displayHeight; y++) {
				for (int x = 0; x < displayWidth; x++) {

			
					LinkedList<int> topLayersForCell = topLayers[x,y];

		
					Cell cell = null;
					if (topLayersForCell.First != null) {
						cell = cells[topLayersForCell.Last.Value][x, y];
					}

					if (cell == null || cell.content == "") {
						for (int i = 0; i < 4; i++) {

				
							foreground.meshVertices[(y * displayWidth + x) * 4 + i] = zero3;
							foreground.meshUVs[(y * displayWidth + x) * 4 + i] = zero2;
							foreground.meshColors[(y * displayWidth + x) * 4 + i] = clearColor;
							background.meshColors[(y * displayWidth + x) * 4 + i] = cell != null ? cell.backgroundColor : clearColor;
						}
					}

				
					else {
						BitmapFontGlyph glyph = font.GetGlyph(cell.content);
						for (int i = 0; i < 4; i++) {

				
							foreground.meshVertices[(y * displayWidth + x) * 4 + i] = new Vector3(
								x * quadWidth + glyph.vertices[i].x * quadWidth, 
								-y * quadHeight + glyph.vertices[i].y * quadHeight - quadHeight,
								0f);
							foreground.meshUVs[(y * displayWidth + x) * 4 + i] = glyph.uvs[i];
							foreground.meshColors[(y * displayWidth + x) * 4 + i] = cell.color;
							background.meshColors[(y * displayWidth + x) * 4 + i] = cell.backgroundColor;
						}
					}
				}
			}

			background.UpdateMesh();
			foreground.UpdateMesh();


			if (Input.GetKeyDown(KeyCode.P)) {
				ScreenCapture.CaptureScreenshot("ascii_" + Random.Range(0, int.MaxValue) + ".png");
			}
		}
	}

	public IEnumerator Quit(float delay){
		yield return new WaitForSeconds(delay);
		Application.Quit();
	}

	public static bool IsInitialized(){
		return GET.initialized;
	}

	public static Cell CellAt(int layer, float x, float y){		
		return GET.GetCell(layer, x, y);
	}

	public static int GetDisplayWidth(){
		if (GET.initialized) {
			return GET.displayWidth;
		} else {
			throw new UnityException("Display not yet initialized!");
		}
	}

	public static int GetDisplayHeight(){
		if (GET.initialized) {
			return GET.displayHeight;
		} else {
			throw new UnityException("Display not yet initialized!");
		}
	}
}
