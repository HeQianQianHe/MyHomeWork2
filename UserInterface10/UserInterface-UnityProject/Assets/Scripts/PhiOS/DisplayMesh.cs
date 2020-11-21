using UnityEngine;
using System.Collections;

public class DisplayMesh : MonoBehaviour {

	[HideInInspector]
	public Mesh mesh;

	[HideInInspector]
	public Vector3[] meshVertices;

	[HideInInspector]
	public Vector2[] meshUVs;

	[HideInInspector]
	public Color[] meshColors;

	private MeshRenderer meshRenderer;

	public void CombineQuads(CombineInstance[] combineInstances, string name, Material material, float z){

	
		MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
		mesh = meshFilter.mesh = new Mesh();
		mesh.name = name;
		mesh.CombineMeshes(combineInstances, true, true);

	
		meshRenderer = gameObject.AddComponent<MeshRenderer>();
		meshRenderer.sharedMaterial = material;
		meshRenderer.enabled = false;
		meshRenderer.receiveShadows = false;
		meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

		meshVertices = mesh.vertices;


		meshUVs = mesh.uv;


		meshColors = new Color[mesh.vertexCount];

		mesh.RecalculateBounds();

		gameObject.transform.position = new Vector3(
			-meshRenderer.bounds.extents.x,
			meshRenderer.bounds.extents.y,
			z);
	}

	public void UpdateMesh(){


		mesh.vertices = meshVertices;
		mesh.uv = meshUVs;
		mesh.colors = meshColors;
		meshRenderer.enabled = true;
	}
}
