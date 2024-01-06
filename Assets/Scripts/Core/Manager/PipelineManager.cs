using System.Collections;
using System.Collections.Generic;
using FlatKit;
using UnityEngine;
using UnityEngine.Rendering;

public class PipelineManager : BaseSingleTon<PipelineManager>,IMonoManager
{
    public RenderPipelineAsset mainScenePipelineAsset;
    public RenderPipelineAsset stagePipelineAsset;


    public void OnInit()
    {
        mainScenePipelineAsset = Resources.Load<RenderPipelineAsset>("PipelineAsset/Wanderer-URP Asset");
        stagePipelineAsset= Resources.Load<RenderPipelineAsset>("PipelineAsset/FruitVaseScene-Var-URP Asset");
    }

    public void Update()
    {
        
    }

    public void FixedUpdate()
    {
        
    }

    public void LateUpdate()
    {
        
    }

    public void OnClear()
    {
        
    }
}
