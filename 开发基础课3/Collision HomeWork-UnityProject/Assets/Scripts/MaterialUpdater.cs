using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MaterialUpdater : MonoBehaviour {

  public Material[] materials;
  public Transform sphere;
  public Transform light;


  void Start () {

  }

  void OnEnable() {
    Camera.onPreRender += UpdateMaterials;
  }

  void OnDisable() {
    Camera.onPreRender -= UpdateMaterials;
  }

  void UpdateMaterials(Camera camera) {
    for (int i = 0; i < materials.Length; i++) {
      materials[i].SetVector("sphereCenter", sphere.position);
      materials[i].SetVector("light", -light.forward);
      materials[i].SetVector("eye", camera.transform.position);
    }
  }
}
