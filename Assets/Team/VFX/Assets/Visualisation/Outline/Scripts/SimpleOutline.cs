using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
[DisallowMultipleComponent]
//[HelpURL("https://docs.unity3d.com/ScriptReference/MonoBehaviour.html")]
public class SimpleOutline : MonoBehaviour
{
    private static HashSet<Mesh> registeredMeshes = new HashSet<Mesh>();
    private static readonly int OutlineColorID = Shader.PropertyToID("_OutlineColor");
    private static readonly int OutlineWidthID = Shader.PropertyToID("_OutlineWidth");

    public Color OutlineColor
    {
        get => outlineColor;
        set
        {
            if (outlineColor != value)
            {
                outlineColor = value;
                UpdateMaterialProperties();
            }
        }
    }

    public float OutlineWidth
    {
        get => outlineWidth;
        set
        {
            if (Math.Abs(outlineWidth - value) > 0.001f)
            {
                outlineWidth = value;
                UpdateMaterialProperties();
            }
        }
    }

    [SerializeField] private Color outlineColor = Color.black;
    [SerializeField, Range(0f, 40f)] private float outlineWidth = 10f;

    private Renderer[] renderers;
    private Material outlineMaskMaterial;
    private Material outlineFillMaterial;
    private bool isInitialized;
    private bool materialsCreated;
    private bool isApplyingMaterials;

#if UNITY_EDITOR
    private void Reset() => EditorApplication.delayCall += Initialize;
#endif

    private void OnEnable()
    {
        Initialize();
        ApplyOutlineMaterials();
    }

    private void OnDisable()
    {
        RemoveOutlineMaterials();
    }

    private void OnDestroy()
    {
        CleanupMaterials();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (isInitialized && !isApplyingMaterials)
        {
            EditorApplication.delayCall += () => {
                if (this != null) UpdateMaterialProperties();
            };
        }
    }
#endif

    private void Initialize()
    {
        if (isInitialized) return;

        
        renderers = GetComponentsInChildren<Renderer>(true);

        
        CreateOutlineMaterials();

        if (outlineMaskMaterial == null || outlineFillMaterial == null)
        {
            //Debug.LogError("Outline materials could not be created. Please check shader names.");
            return;
        }

        
        LoadSmoothNormals();

        isInitialized = true;
        UpdateMaterialProperties();
    }

    private void CreateOutlineMaterials()
    {
        if (materialsCreated) return;

        
        CleanupMaterials();

        
        outlineMaskMaterial = CreateMaterialInstance("Custom/OutlineMask");
        outlineFillMaterial = CreateMaterialInstance("Custom/OutlineFill");

        materialsCreated = outlineMaskMaterial != null && outlineFillMaterial != null;
    }

    private Material CreateMaterialInstance(string shaderName)
    {
        var shader = Shader.Find(shaderName);
        if (shader == null)
        {
            //Debug.LogError($"Shader not found: {shaderName}");
            return null;
        }

        return new Material(shader)
        {
            name = $"{shaderName.Replace("/", "_")}_Instance",
            hideFlags = HideFlags.HideAndDontSave
        };
    }

    private void ApplyOutlineMaterials()
    {
        if (!isInitialized || !materialsCreated || renderers == null)
            return;

        isApplyingMaterials = true;

        try
        {
            foreach (var renderer in renderers)
            {
                if (renderer == null) continue;

                var materials = renderer.sharedMaterials
                    .Where(m => m != null &&
                               !m.name.Contains("OutlineMask") &&
                               !m.name.Contains("OutlineFill"))
                    .ToList();

                
                materials.Add(outlineMaskMaterial);
                materials.Add(outlineFillMaterial);

                
                renderer.sharedMaterials = materials.ToArray();
            }
        }
        finally
        {
            isApplyingMaterials = false;
        }
    }

    private void RemoveOutlineMaterials()
    {
        if (renderers == null) return;

        foreach (var renderer in renderers)
        {
            if (renderer == null) continue;

            var materials = renderer.sharedMaterials
                .Where(m => m != null &&
                           !m.name.Contains("OutlineMask") &&
                           !m.name.Contains("OutlineFill"))
                .ToArray();

            renderer.sharedMaterials = materials;
        }
    }

    private void CleanupMaterials()
    {
        if (outlineMaskMaterial != null)
        {
            if (Application.isPlaying) Destroy(outlineMaskMaterial);
            else DestroyImmediate(outlineMaskMaterial);
        }

        if (outlineFillMaterial != null)
        {
            if (Application.isPlaying) Destroy(outlineFillMaterial);
            else DestroyImmediate(outlineFillMaterial);
        }

        outlineMaskMaterial = null;
        outlineFillMaterial = null;
        materialsCreated = false;
        isInitialized = false;
    }

    private void LoadSmoothNormals()
    {
        foreach (var meshFilter in GetComponentsInChildren<MeshFilter>(true))
        {
            if (meshFilter == null || meshFilter.sharedMesh == null)
                continue;

            if (!registeredMeshes.Add(meshFilter.sharedMesh))
                continue;

            var smoothNormals = GetSmoothNormals(meshFilter.sharedMesh);
            meshFilter.sharedMesh.SetUVs(3, smoothNormals);

            var renderer = meshFilter.GetComponent<Renderer>();
            if (renderer != null)
            {
                CombineSubmeshes(meshFilter.sharedMesh, renderer.sharedMaterials);
            }
        }

        foreach (var skinnedMeshRenderer in GetComponentsInChildren<SkinnedMeshRenderer>(true))
        {
            if (skinnedMeshRenderer == null || skinnedMeshRenderer.sharedMesh == null)
                continue;

            if (!registeredMeshes.Add(skinnedMeshRenderer.sharedMesh))
                continue;

            var smoothNormals = GetSmoothNormals(skinnedMeshRenderer.sharedMesh);
            skinnedMeshRenderer.sharedMesh.SetUVs(3, smoothNormals);

            CombineSubmeshes(skinnedMeshRenderer.sharedMesh, skinnedMeshRenderer.sharedMaterials);
        }
    }

    private List<Vector3> GetSmoothNormals(Mesh mesh)
    {
        if (!mesh.HasVertexAttribute(UnityEngine.Rendering.VertexAttribute.Normal))
        {
            mesh.RecalculateNormals();
        }

        
        var groups = mesh.vertices
            .Select((vertex, index) => new {
                Key = $"{vertex.x:F4}_{vertex.y:F4}_{vertex.z:F4}",
                Index = index,
                Normal = mesh.normals[index]
            })
            .GroupBy(x => x.Key);

        var smoothNormals = new List<Vector3>(mesh.normals);

        foreach (var group in groups)
        {
            if (group.Count() == 1) continue;

            
            var smoothNormal = Vector3.zero;
            foreach (var item in group)
            {
                smoothNormal += item.Normal;
            }
            smoothNormal.Normalize();

            
            foreach (var item in group)
            {
                smoothNormals[item.Index] = smoothNormal;
            }
        }

        return smoothNormals;
    }

    private void CombineSubmeshes(Mesh mesh, Material[] materials)
    {
        if (mesh.subMeshCount == 1 || mesh.subMeshCount > materials.Length) return;

        mesh.subMeshCount++;
        mesh.SetTriangles(mesh.triangles, mesh.subMeshCount - 1);
    }

    public void UpdateMaterialProperties()
    {
        if (!isInitialized || !materialsCreated) return;

        if (outlineFillMaterial != null)
        {
            outlineFillMaterial.SetColor(OutlineColorID, outlineColor);

            if (outlineFillMaterial.HasProperty(OutlineWidthID))
            {
                outlineFillMaterial.SetFloat(OutlineWidthID, outlineWidth);
            }
        }
    }
}