using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPPControler : MonoBehaviour
{

    [SerializeField] private Material PPMaterial = null;
    [SerializeField] private Camera FireCam = null;

    private RenderTexture renderTexture;

    private void Awake() {
        renderTexture = new RenderTexture(Screen.width, Screen.height, 0);
        FireCam.targetTexture = renderTexture;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination) {
        if(PPMaterial != null && renderTexture != null) {
            PPMaterial.SetTexture("_FireAlpha", renderTexture);
            Graphics.Blit(source, destination, PPMaterial);
        } else {
            Graphics.Blit(source, destination);
        }
    }

}
