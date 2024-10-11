//
//  Outline.cs
//  QuickOutline
//
//  Created by Chris Nolet on 3/30/18.
//  Copyright © 2018 Chris Nolet. All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

[DisallowMultipleComponent]

public class Outline : MonoBehaviour
{
  private static HashSet<Mesh> registeredMeshes = new HashSet<Mesh>();

  public enum Mode
  {
    OutlineAll,
    OutlineVisible,
    OutlineHidden,
    OutlineAndSilhouette,
    SilhouetteOnly
  }

  public Mode OutlineMode
  {
    get { return outlineMode; }
    set
    {
      outlineMode = value;
      needsUpdate = true;
    }
  }

  public Color OutlineColor
  {
    get { return outlineColor; }
    set
    {
      outlineColor = value;
      needsUpdate = true;
    }
  }


  public float OutlineWidth
  {
    get { return outlineWidth; }
    set
    {
      outlineWidth = value;
      needsUpdate = true;
    }
  }

  [Serializable]
  private class ListVector3
  {
    public List<Vector3> data;
  }

  [SerializeField]
  private Mode outlineMode;

  [SerializeField]
  private Color outlineColor = Color.white;

  [SerializeField]
  private float outlineWidth = 2f;

  [Header("Optional")]

  [SerializeField, Tooltip("Precompute enabled: Per-vertex calculations are performed in the editor and serialized with the object. "
  + "Precompute disabled: Per-vertex calculations are performed at runtime in Awake(). This may cause a pause for large meshes.")]
  private bool precomputeOutline;

  [SerializeField, HideInInspector]
  private List<Mesh> bakeKeys = new List<Mesh>();

  [SerializeField, HideInInspector]
  private List<ListVector3> bakeValues = new List<ListVector3>();

  private Renderer[] renderers;
  private Material outlineMaskMaterial;
  private Material outlineFillMaterial;

  private bool needsUpdate;

  void Awake()
  {

    // Cache renderers
    renderers = GetComponentsInChildren<Renderer>();

    // init first mat to clear all instance
    foreach (Renderer renderer in renderers)
    {
      renderer.sharedMaterial = renderer.material;
    }

    // Instantiate outline materials
    outlineMaskMaterial = Instantiate(Resources.Load<Material>(@"Materials/OutlineMask"));
    outlineFillMaterial = Instantiate(Resources.Load<Material>(@"Materials/OutlineFill"));

    outlineMaskMaterial.name = "OutlineMask (Instance)";
    outlineFillMaterial.name = "OutlineFill (Instance)";

    // Retrieve or generate smooth normals
    LoadSmoothNormals();

    // Apply material properties immediately
    needsUpdate = true;
  }

  void OnEnable()
  {
    // foreach (var renderer in renderers)
    // {

    //   // Append outline shaders
    //   var materials = renderer.sharedMaterials.ToList();

    //   materials.Add(outlineMaskMaterial);
    //   materials.Add(outlineFillMaterial);

    //   renderer.materials = materials.ToArray();
    // }


  }

  void OnValidate()
  {
    // #if UNITY_EDITOR
    //     if (!EditorApplication.isPlaying)
    //       UpdateOutlineInEditor();
    // #endif

    // Update material properties
    needsUpdate = true;

    // Clear cache when baking is disabled or corrupted
    if (!precomputeOutline && bakeKeys.Count != 0 || bakeKeys.Count != bakeValues.Count)
    {
      bakeKeys.Clear();
      bakeValues.Clear();
    }

    // Generate smooth normals when baking is enabled
    if (precomputeOutline && bakeKeys.Count == 0)
    {
      Bake();
    }
  }

  public void UpdateOutlineInEditor()
  {
    // Cache renderers
    renderers = GetComponentsInChildren<Renderer>();

    foreach (var renderer in renderers)
    {

      // Remove outline shaders
      var materials = renderer.sharedMaterials.ToList();
      Material mainMat = materials[0];
      materials.Clear();
      materials.Add(mainMat);

      renderer.materials = materials.ToArray();
    }


    DestroyImmediate(outlineMaskMaterial);
    DestroyImmediate(outlineFillMaterial);





    // Instantiate outline materials
    outlineMaskMaterial = Instantiate(Resources.Load<Material>(@"Materials/OutlineMask"));
    outlineFillMaterial = Instantiate(Resources.Load<Material>(@"Materials/OutlineFill"));

    outlineMaskMaterial.name = "OutlineMask (Instance)";
    outlineFillMaterial.name = "OutlineFill (Instance)";

    // Retrieve or generate smooth normals
    LoadSmoothNormals();


    foreach (var renderer in renderers)
    {

      // Append outline shaders
      var materials = renderer.sharedMaterials.ToList();

      materials.Add(outlineMaskMaterial);
      materials.Add(outlineFillMaterial);

      renderer.materials = materials.ToArray();
    }
    UpdateMaterialProperties();
  }

  void Update()
  {
    if (needsUpdate)
    {
      needsUpdate = false;

      UpdateMaterialProperties();
    }
  }

  void OnDisable()
  {
    foreach (var renderer in renderers)
    {

      // Remove outline shaders
      var materials = renderer.sharedMaterials.ToList();

      materials.Remove(outlineMaskMaterial);
      materials.Remove(outlineFillMaterial);

      renderer.materials = materials.ToArray();
    }
  }

  void OnDestroy()
  {

    // Destroy material instances
    Destroy(outlineMaskMaterial);
    Destroy(outlineFillMaterial);
  }

  void Bake()
  {

    // Generate smooth normals for each mesh
    var bakedMeshes = new HashSet<Mesh>();

    foreach (var meshFilter in GetComponentsInChildren<MeshFilter>())
    {

      // Skip duplicates
      if (!bakedMeshes.Add(meshFilter.sharedMesh))
      {
        continue;
      }

      // Serialize smooth normals
      var smoothNormals = SmoothNormals(meshFilter.sharedMesh);

      bakeKeys.Add(meshFilter.sharedMesh);
      bakeValues.Add(new ListVector3() { data = smoothNormals });
    }
  }

  void LoadSmoothNormals()
  {

    // Retrieve or generate smooth normals
    foreach (var meshFilter in GetComponentsInChildren<MeshFilter>())
    {

      // Skip if smooth normals have already been adopted
      if (!registeredMeshes.Add(meshFilter.sharedMesh))
      {
        continue;
      }

      // Retrieve or generate smooth normals
      var index = bakeKeys.IndexOf(meshFilter.sharedMesh);
      var smoothNormals = (index >= 0) ? bakeValues[index].data : SmoothNormals(meshFilter.sharedMesh);

      // Store smooth normals in UV3
      meshFilter.sharedMesh.SetUVs(3, smoothNormals);

      // Combine submeshes
      var renderer = meshFilter.GetComponent<Renderer>();

      if (renderer != null)
      {
        CombineSubmeshes(meshFilter.sharedMesh, renderer.sharedMaterials);
      }
    }

    // Clear UV3 on skinned mesh renderers
    foreach (var skinnedMeshRenderer in GetComponentsInChildren<SkinnedMeshRenderer>())
    {

      // Skip if UV3 has already been reset
      if (!registeredMeshes.Add(skinnedMeshRenderer.sharedMesh))
      {
        continue;
      }

      // Clear UV3
      skinnedMeshRenderer.sharedMesh.uv4 = new Vector2[skinnedMeshRenderer.sharedMesh.vertexCount];

      // Combine submeshes
      CombineSubmeshes(skinnedMeshRenderer.sharedMesh, skinnedMeshRenderer.sharedMaterials);
    }
  }

  List<Vector3> SmoothNormals(Mesh mesh)
  {

    // Group vertices by location
    var groups = mesh.vertices.Select((vertex, index) => new KeyValuePair<Vector3, int>(vertex, index)).GroupBy(pair => pair.Key);

    // Copy normals to a new list
    var smoothNormals = new List<Vector3>(mesh.normals);

    // Average normals for grouped vertices
    foreach (var group in groups)
    {

      // Skip single vertices
      if (group.Count() == 1)
      {
        continue;
      }

      // Calculate the average normal
      var smoothNormal = Vector3.zero;

      foreach (var pair in group)
      {
        smoothNormal += smoothNormals[pair.Value];
      }

      smoothNormal.Normalize();

      // Assign smooth normal to each vertex
      foreach (var pair in group)
      {
        smoothNormals[pair.Value] = smoothNormal;
      }
    }

    return smoothNormals;
  }

  void CombineSubmeshes(Mesh mesh, Material[] materials)
  {

    // Skip meshes with a single submesh
    if (mesh.subMeshCount == 1)
    {
      return;
    }

    // Skip if submesh count exceeds material count
    if (mesh.subMeshCount > materials.Length)
    {
      return;
    }

    // Append combined submesh
    mesh.subMeshCount++;
    mesh.SetTriangles(mesh.triangles, mesh.subMeshCount - 1);
  }

  void UpdateMaterialProperties()
  {
    if (!outlineFillMaterial) return;

    // Apply properties according to mode
    outlineFillMaterial.SetColor("_OutlineColor", outlineColor);

    switch (outlineMode)
    {
      case Mode.OutlineAll:
        outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
        outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
        outlineFillMaterial.SetFloat("_OutlineWidth", outlineWidth);
        break;

      case Mode.OutlineVisible:
        outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
        outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
        outlineFillMaterial.SetFloat("_OutlineWidth", outlineWidth);
        break;

      case Mode.OutlineHidden:
        outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
        outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Greater);
        outlineFillMaterial.SetFloat("_OutlineWidth", outlineWidth);
        break;

      case Mode.OutlineAndSilhouette:
        outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
        outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
        outlineFillMaterial.SetFloat("_OutlineWidth", outlineWidth);
        break;

      case Mode.SilhouetteOnly:
        outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
        outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Greater);
        outlineFillMaterial.SetFloat("_OutlineWidth", 0f);
        break;
    }
  }

  public void RemoveOutline()
  {
    #if UNITY_EDITOR
    if (!EditorApplication.isPlaying)
    {
      foreach (var renderer in renderers)
      {

        // Remove outline shaders
        var materials = renderer.sharedMaterials.ToList();

        materials.Remove(outlineMaskMaterial);
        materials.Remove(outlineFillMaterial);

        DestroyImmediate(outlineMaskMaterial);
        DestroyImmediate(outlineFillMaterial);

        renderer.materials = materials.ToArray();

        return;
      }
    }
    #endif

    foreach (var renderer in renderers)
    {

      // Remove outline shaders
      List<Material> materials = new();

      Material mainMat = renderer.material;
      materials.Add(mainMat);
      renderer.materials = materials.ToArray();
    }
  }

  public void CreateOutline()
  {
    #if UNITY_EDITOR
    if (!EditorApplication.isPlaying)
    {
      UpdateOutlineInEditor();
      return;
    }
    #endif

    foreach (var renderer in renderers)
    {

      // Append outline shaders
      var materials = renderer.sharedMaterials.ToList();

      materials.Add(outlineMaskMaterial);
      materials.Add(outlineFillMaterial);

      renderer.materials = materials.ToArray();
    }

    // foreach (var renderer in renderers)
    // {

    //   // Append outline shaders
    //   var materials = renderer.sharedMaterials.ToList();

    //   materials.Add(outlineMaskMaterial);
    //   materials.Add(outlineFillMaterial);

    //   renderer.materials = materials.ToArray();
    // }



  }
}
