using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class ApplyPPToCameraOut : MonoBehaviour
{
    public Material PostProcessMat;

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest, PostProcessMat);
    }
}
