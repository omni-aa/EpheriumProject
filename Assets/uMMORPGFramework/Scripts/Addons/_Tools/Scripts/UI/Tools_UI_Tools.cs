using UnityEngine;

namespace uMMORPG
{
    
    // UI TOOLS
    
    public static partial class Tools_UI_Tools
    {
        private static Tools_UI_CanvasOverlay canvasOverlayInstance;
    
        // -----------------------------------------------------------------------------------
        // FadeOutScreen
        // @Client
        // -----------------------------------------------------------------------------------
        public static void FadeOutScreen(bool automatic = true, float fDuration = 0f)
        {
            if (canvasOverlayInstance == null)
                canvasOverlayInstance = GameObject.FindFirstObjectByType<Tools_UI_CanvasOverlay>();
                //canvasOverlayInstance = GameObject.FindObjectOfType<Tools_UI_CanvasOverlay>();
    
            if (canvasOverlayInstance != null)
                if (automatic)
                    canvasOverlayInstance.AutoFadeOut(fDuration);
                else
                    canvasOverlayInstance.FadeOut(fDuration);
        }
    
        // -----------------------------------------------------------------------------------
        // FadeInScreen
        // @Client
        // -----------------------------------------------------------------------------------
        public static void FadeInScreen(float fDelay = 0f)
        {
            if (canvasOverlayInstance == null)
                canvasOverlayInstance = GameObject.FindFirstObjectByType<Tools_UI_CanvasOverlay>();
                //canvasOverlayInstance = GameObject.FindObjectOfType<Tools_UI_CanvasOverlay>();
    
            if (canvasOverlayInstance != null)
                if (fDelay != 0)
                    canvasOverlayInstance.FadeInDelayed(fDelay);
                else
                    canvasOverlayInstance.FadeIn();
        }
    
        // -----------------------------------------------------------------------------------
    }
    
}
