using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using UnityEngine.Profiling;
 partial class CameraRenderer 
{
    partial void DrawGizmos();
    partial void DrawUnsupportedGeometry();
    partial void PrepareForSceneWindow();
    partial void PrepareBuffer();
    string _sampleName { get; set; }
#if UNITY_EDITOR
    static ShaderTagId[] _legacyShaderTagIds = {
        new ShaderTagId("Always"),
        new ShaderTagId("ForwardBase"),
        new ShaderTagId("PrepassBase"),
        new ShaderTagId("Vertex"),
        new ShaderTagId("VertexLMRGBM"),
        new ShaderTagId("VertexLM")
    };
    static Material _errorMat;


    partial void DrawUnsupportedGeometry() 
    {
        if (_errorMat == null)
        {
            _errorMat = new Material(Shader.Find("Hidden/InternalErrorShader"));
        }
        DrawingSettings ds = new DrawingSettings(_legacyShaderTagIds[0], new SortingSettings(_camera))
        {
            overrideMaterial = _errorMat
        };
        for (int i = 1; i < _legacyShaderTagIds.Length; i++)
        {
            ds.SetShaderPassName(i, _legacyShaderTagIds[i]);
        }
        FilteringSettings fs = FilteringSettings.defaultValue;
        _context.DrawRenderers(_cullingResults, ref ds, ref fs);
    
    }
    partial void DrawGizmos()
    {
        if (Handles.ShouldRenderGizmos()) 
        {
            _context.DrawGizmos(_camera, GizmoSubset.PreImageEffects);
            _context.DrawGizmos(_camera, GizmoSubset.PostImageEffects);
        }
    }
    partial void PrepareForSceneWindow()
    {
        if (_camera.cameraType == CameraType.SceneView) 
        {
            ScriptableRenderContext.EmitWorldGeometryForSceneView(_camera);
        }
    }

    partial void PrepareBuffer()
    {
        Profiler.BeginSample("EditorOnly");
       _buffer.name = _sampleName= _camera.name;
        Profiler.EndSample();
    }
#else
    const string _sampleName = _bufferName;
#endif
}
