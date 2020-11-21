using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Mouse : MonoBehaviour {

	public bool hideNativeCursor = true;
	public int reservedLayer = -1;
	public string cursorUp = "░";
	public string cursorDown = "█";
	public Color cursorColor;
	public float cursorFadeTime = 0.25f;
	public bool fadeToClear = true;

	private bool initialized = false;
	private float screenXMin = 0f;
	private float screenXMax = 0f;
	private float screenYMin = 0f;
	private float screenYMax = 0f;
	private Cell currentCell;
	private Cell currentCellHover;
	private IHoverAction hoverAction;
	private bool dragging = false;
	private Vector2 dragStart;
	private IDragAction dragAction;

	public void Awake(){


		if (hideNativeCursor) {
			Cursor.visible = false;
		}
	}

	public IEnumerator Start (){


		Display display = Display.GET;
		while (!display.initialized) {
			yield return null;
		}

		yield return null;


		Camera mainCamera = display.mainCamera;
		screenXMin = mainCamera.WorldToViewportPoint(display.foreground.GetComponent<MeshRenderer>().bounds.min).x;
		screenXMax = mainCamera.WorldToViewportPoint(display.foreground.GetComponent<MeshRenderer>().bounds.max).x;
		screenYMin = mainCamera.WorldToViewportPoint(display.foreground.GetComponent<MeshRenderer>().bounds.min).y;
		screenYMax = mainCamera.WorldToViewportPoint(display.foreground.GetComponent<MeshRenderer>().bounds.max).y;


		initialized = true;
	}

	public void Update(){


		if (initialized) {

			Display display = Display.GET;
			Camera mainCamera = display.mainCamera;

			Vector3 mousePosition = mainCamera.ScreenToViewportPoint(Input.mousePosition);
			mousePosition = new Vector3 (mousePosition.x, 1f - mousePosition.y);


			mousePosition = new Vector3(
				Mathf.Clamp(mousePosition.x, screenXMin, screenXMax),
				Mathf.Clamp(mousePosition.y, screenYMin, screenYMax));

			Vector2 cellPosition = new Vector2 (
				Mathf.FloorToInt (((mousePosition.x - screenXMin) / (screenXMax - screenXMin)) * display.displayWidth),
				Mathf.FloorToInt (((mousePosition.y - screenYMin) / (screenYMax - screenYMin)) * display.displayHeight));


			cellPosition = new Vector2 (
				Mathf.Clamp (cellPosition.x, 0f, display.displayWidth - 1),
				Mathf.Clamp (cellPosition.y, 0f, display.displayHeight - 1));
			
		
			if (currentCell != null) {

	
				Color clearColor = display.GetBackgroundColorForCell(
					                   (int)currentCell.position.x, 
					                   (int)currentCell.position.y,
					                   reservedLayer);
				
			
				currentCell.SetContent (
					"",
					clearColor, 
					fadeToClear ? display.clearColor : cursorColor, 
					cursorFadeTime, 
					cursorColor, 
					CellFades.DEFAULT_REVERSE);
				currentCell = null;
			}

			
			currentCellHover = null;

	
			currentCell = display.GetCell(reservedLayer, cellPosition.x, cellPosition.y);

	
			Color currentCellBackgroundColor = display.GetBackgroundColorForCell(
				(int)currentCell.position.x, 
				(int)currentCell.position.y,
				reservedLayer);

			
			currentCell.SetContent (
				Input.GetMouseButton (0) || Input.GetMouseButton (1) ? cursorDown : cursorUp,
				currentCellBackgroundColor,
				cursorColor,
				0f,
				cursorColor,
				"");

	
			if (!dragging) {
				for (int i = display.GetNumLayers() - 1; i >= 0; i--) {
					Cell cellHover = display.GetCell (i, currentCell.position.x, currentCell.position.y);

			
					if (cellHover.content != "") {

				
						currentCellHover = cellHover;

				
						if (currentCellHover.hoverAction != null) {

						
							if (hoverAction != currentCellHover.hoverAction) {

							
								if (hoverAction != null) {
									hoverAction.OnHoverExit();
								}

							
								hoverAction = currentCellHover.hoverAction;
								hoverAction.OnHoverEnter();
							}
						}

					
						else if (hoverAction != null) {
							hoverAction.OnHoverExit();
							hoverAction = null;
						}

						break;
					}
				}
			}

		
			if (!dragging) {
				if (Input.GetMouseButtonDown(0) &&
				    currentCellHover != null &&
				    currentCellHover.clickAction != null) {
					currentCellHover.clickAction.OnMouseDown();
				}
			}

		
			if (!dragging &&
			    Input.GetMouseButtonDown(0) &&
			    currentCellHover != null &&
			    currentCellHover.dragAction != null) {
				dragging = true;
				dragStart = currentCell.position;
				dragAction = currentCellHover.dragAction;
				dragAction.OnDragStart();
			}

	
			else if (dragging &&
			         Input.GetMouseButtonUp(0) &&
			         dragAction != null) {
				dragging = false;
				Vector2 dragDelta = currentCell.position - dragStart;
				dragAction.OnDragDelta(dragDelta);
				dragAction = null;
			}

			else if (dragging &&
			         dragAction != null) {
				Vector2 dragDelta = currentCell.position - dragStart;
				dragAction.OnDragDelta(dragDelta);
			}

		
			if (!dragging) {
				if (Input.mouseScrollDelta.y != 0f &&
				    currentCellHover != null &&
				    currentCellHover.scrollAction != null) {
					currentCellHover.scrollAction.OnScrollDelta(Mathf.RoundToInt(Input.mouseScrollDelta.y));	
				}
			}
		}
	}
}
