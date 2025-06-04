using UnityEngine;
using UnityEngine.UI;

namespace uMMORPG
{
    
    // CANVAS OVERLAY - UI
    
    public class Tools_UI_CanvasOverlay : MonoBehaviour
    {
        public Image fadeInOut;
        // -----------------------------------------------------------------------------------
        // Awake
        // @Client
        // -----------------------------------------------------------------------------------
        private void Awake()
        {
            //LeanTween.init();
           // LeanTween.alpha(this.gameObject.GetComponent<RectTransform>(), 0f, 0f);
        }
    
        // -----------------------------------------------------------------------------------
        // FadeOut
        // @Client
        // -----------------------------------------------------------------------------------
        public void FadeOut(float fDuration = 0f)
        {
            // LeanTween.alpha(this.gameObject.GetComponent<RectTransform>(), 1f, fDuration);
            if(fadeInOut != null)
                fadeInOut.color = new Color(fadeInOut.color.r, fadeInOut.color.g, fadeInOut.color.b, 1);
            //GetComponent<SpriteRenderer>().color += new Color(0, 0, 0, 1);
        }
    
        // -----------------------------------------------------------------------------------
        // AutoFadeOut
        // @Client
        // -----------------------------------------------------------------------------------
        public void AutoFadeOut(float fDuration = 0f)
        {
            //LeanTween.alpha(this.gameObject.GetComponent<RectTransform>(), 1f, fDuration);
            if (fadeInOut != null)
                fadeInOut.color = new Color(fadeInOut.color.r, fadeInOut.color.g, fadeInOut.color.b, 1);
            //GetComponent<SpriteRenderer>().color += new Color(0, 0, 0, 1);
            Invoke("FadeIn", 0.5f);
        }
    
        // -----------------------------------------------------------------------------------
        // FadeIn
        // @Client
        // -----------------------------------------------------------------------------------
        public void FadeIn(float fDuration = 0.5f)
        {
            //LeanTween.alpha(this.gameObject.GetComponent<RectTransform>(), 0f, fDuration);
            if (fadeInOut != null)
                fadeInOut.color = new Color(fadeInOut.color.r, fadeInOut.color.g, fadeInOut.color.b, 0);
           // GetComponent<SpriteRenderer>().color += new Color(0, 0, 0, 0);
        }
    
        // -----------------------------------------------------------------------------------
        // FadeIn
        // Same method without parameters for Invoke
        // @Client
        // -----------------------------------------------------------------------------------
        public void FadeIn()
        {
            if (fadeInOut != null)
                fadeInOut.color = new Color(fadeInOut.color.r, fadeInOut.color.g, fadeInOut.color.b, 0);
            //GetComponent<SpriteRenderer>().color += new Color(fadeInOut.color.r, fadeInOut.color.g, fadeInOut.color.b, 0);
            //LeanTween.alpha(this.gameObject.GetComponent<RectTransform>(), 0f, 0.5f);
        }
    
        // -----------------------------------------------------------------------------------
        // FadeInDelayed
        // @Client
        // -----------------------------------------------------------------------------------
        public void FadeInDelayed(float fDelay = 0f)
        {
            Invoke("FadeIn", fDelay);
        }
    
        // -----------------------------------------------------------------------------------
    }
    
}
