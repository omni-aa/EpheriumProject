using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace uMMORPG
{
    
    public class WeatherManager : MonoBehaviour
    {
        public Material SkyBox;
        public Material OtherSkyBox;
    
        public GameObject defaultSun;
    
        [Header("[-=-[ Zone Sun Parameter ]-=-]")]
        public GameObject zoneSun;
        public Color sunColor = Color.yellow;
        [Range(0,3)] public float sunintensity = 1;
    
    
        [Header("[-=-[ Wheather ]-=-]")]
        public GameObject Weather;
    
        [Header("[-=-[ Fog Parameter ]-=-]")]
        public bool enebleFog = true;
        [Range(0,1)] public float density = 0.06f;
        public FogMode fogMode = FogMode.Exponential;
        public Color fogColor;
    
        private void OnTriggerEnter(Collider other)
        {
            // a player might have a root collider and a hip collider.
            // only fire OnTriggerEnter code here ONCE.
            // => only for the collider ON the player
            // => so we check GetComponent. DO NOT check GetComponentInParent.
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                // only call this for local player. not for other
                if (player.isLocalPlayer)
                {
                    RenderSettings.skybox = OtherSkyBox;
                    defaultSun.SetActive(false);
                    zoneSun.SetActive(true);
                    zoneSun.GetComponent<Light>().intensity = sunintensity;
                    zoneSun.GetComponent<Light>().color = sunColor;
    
                    if (Weather != null)
                        Weather.SetActive(true);
                    if (!enebleFog) return;
                    RenderSettings.fog = (true);
                    RenderSettings.fogDensity = density;
                    RenderSettings.fogColor = fogColor;
                    RenderSettings.fogDensity = density;
                }
            }
        }
    
    
    
        private void OnTriggerExit(Collider other)
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                // only call this for local player. not for other
                if (player.isLocalPlayer)
                {
                    RenderSettings.skybox = SkyBox;
                    RenderSettings.fog = (false);
                    defaultSun.SetActive(true);
                    zoneSun.SetActive(false);
                    if (Weather != null)
                        Weather.SetActive(false);
                }
            }
        }
    }
    
}
