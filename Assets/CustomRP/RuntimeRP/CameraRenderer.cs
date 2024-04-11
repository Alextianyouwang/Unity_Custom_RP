using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
public class CameraRenderer 
{
    ScriptableRenderContext _context;
    Camera _camera;
    const string _bufferName = "Render Camera";
    CommandBuffer _buffer = new CommandBuffer { name = _bufferName };
    CullingResults _cullingResults;
    static ShaderTagId _unlitTagID = new ShaderTagId("SRPDefaultUnlit");
    public void Render(ScriptableRenderContext context, Camera camera) 
    {
        _camera = camera;
        _context = context;
        if (!Cull())
            return;
        Setup();
        DrawVisibleGeometry();
        Submit();
    }
    private bool Cull() 
    {
        if (_camera.TryGetCullingParameters(out ScriptableCullingParameters p)) 
        {
            _cullingResults = _context.Cull(ref p);
            return true;
        }
        return false;
    }
    private void Setup() 
    {
        _context.SetupCameraProperties(_camera);
        _buffer.ClearRenderTarget(true, true, Color.clear);
        _buffer.BeginSample(_bufferName);
        ExecuteBuffer();

    }
    private void DrawVisibleGeometry() 
    {
        SortingSettings ss = new SortingSettings(_camera) { criteria = SortingCriteria.CommonOpaque };
        DrawingSettings ds  = new DrawingSettings(_unlitTagID,ss);
        FilteringSettings fs = new FilteringSettings(RenderQueueRange.opaque);

        _context.DrawRenderers(_cullingResults, ref ds, ref fs);
        _context.DrawSkybox(_camera);

        ss.criteria = SortingCriteria.CommonTransparent;
        ds.sortingSettings = ss;
        fs.renderQueueRange = RenderQueueRange.transparent;
        _context.DrawRenderers(_cullingResults, ref ds, ref fs);
    }

    private void Submit() 
    {
        _buffer.EndSample(_bufferName);
        ExecuteBuffer();
        _context.Submit();
    }

    private void ExecuteBuffer() 
    {
        _context.ExecuteCommandBuffer(_buffer);
        _buffer.Clear();
    }
}
