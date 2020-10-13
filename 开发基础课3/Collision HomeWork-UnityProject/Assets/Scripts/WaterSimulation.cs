using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class WaterSimulation : MonoBehaviour, IPointerClickHandler, IDragHandler {
    public Transform sphereTrans;
  public CustomRenderTexture texture;
  public float dropRadius = 1f; 
  public bool pause = false;
  public int initialDropCount = 20;

  private CustomRenderTextureUpdateZone[] zones = null;
  private Collider collider;
  private CustomRenderTextureUpdateZone defaultZone, normalZone, waveZone;

  void Start() {
    texture.Initialize();
    collider = GetComponent<Collider>();

    defaultZone = new CustomRenderTextureUpdateZone();
    defaultZone.needSwap = true;
    defaultZone.passIndex = 0; 
    defaultZone.rotation = 0f;
    defaultZone.updateZoneCenter = new Vector2(0.5f, 0.5f);
    defaultZone.updateZoneSize = new Vector2(1f, 1f);

    normalZone = new CustomRenderTextureUpdateZone();
    normalZone.needSwap = true;
    normalZone.passIndex = 2;
    normalZone.rotation = 0f;
    normalZone.updateZoneCenter = new Vector2(0.5f, 0.5f);
    normalZone.updateZoneSize = new Vector2(1f, 1f);

    waveZone = new CustomRenderTextureUpdateZone();
    waveZone.needSwap = true;
    waveZone.passIndex = 1; 
    waveZone.rotation = 0f;

    waveZone.updateZoneSize = new Vector2(dropRadius, dropRadius);

    var waves = new List<CustomRenderTextureUpdateZone>();
    for (int i = 0; i < initialDropCount; i++) {
      waveZone.updateZoneCenter = new Vector2(Random.Range(0f, 1f),
                                              Random.Range(0f, 1f));

      waves.Add(waveZone);
    }
    zones = waves.ToArray();
  }

  public void OnDrag(PointerEventData ped) {
    AddWave(ped);
  }

  public void OnPointerClick(PointerEventData ped) {
    AddWave(ped);
  }

  void AddWave(PointerEventData ped) {

    Vector2 localCursor;
    var rt = GetComponent<RectTransform>();
    if (rt == null || !RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, ped.position, ped.pressEventCamera, out localCursor))
      return;


    Vector2 uv = Rect.PointToNormalized(rt.rect, localCursor);

    var leftClick = ped.button == PointerEventData.InputButton.Left;


    AddWave(uv);
  }

  void AddWave(Vector2 uv) {
    waveZone.updateZoneCenter = new Vector2(uv.x, 1f - uv.y);

    if (pause) {
      zones = new CustomRenderTextureUpdateZone[] { waveZone, normalZone };
    } else {
      zones = new CustomRenderTextureUpdateZone[] { defaultZone, defaultZone, waveZone, normalZone };
    }

  }

  void Update() {
    if (Input.GetKeyDown(KeyCode.Space))
      pause = !pause;
    UpdateZones();
    if (zones != null) {
      texture.SetUpdateZones(zones);
      zones = null;
      if (pause)
        texture.Update(1);
    } else {
      texture.SetUpdateZones(new CustomRenderTextureUpdateZone[] { defaultZone, defaultZone, normalZone });
    }
    if (! pause
        || Input.GetKeyDown(KeyCode.N))
      texture.Update(1);
  }
    float timer = 0;
  void UpdateZones()
  {
    if (collider == null) return;
    if (sphereTrans.GetComponent<CollosionTest>().preV==Vector3.zero) return;

    RaycastHit hit;
    Ray ray = new Ray(sphereTrans.position, Vector3.down);

    if (collider.Raycast(ray, out hit, 10f)) {

      AddWave(hit.textureCoord2);
    }
  }

}
