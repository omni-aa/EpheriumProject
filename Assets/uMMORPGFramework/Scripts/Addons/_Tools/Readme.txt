// =======================================================================================
//                            Thank you for your download.
//                 Do not hesitate to post your feedback on Asset Store
// =======================================================================================
// * Discord Support Server........: https://discord.gg/aRCBPGMr7A
// * Website Instructions..........: https://mmo-indie.com/addon/_Tools
// =======================================================================================
// * Addon name....................: Tools
// * Asset Store link..............: https://u3d.as/2FcL
// * Require Asstets...............: https://assetstore.unity.com/packages/templates/systems/ummorpg-remastered-mmorpg-engine-159401
// * Core edit.....................: Yes
// * Prefab Edit...................: Yes
// * Recommendations...............: unmodified ummorpg.
// =======================================================================================
//                                 * Description *
//========================================================================================
// This utility AddOn is required by other AddOns, as it contains several shared functions and UI elements. 
// Most notably are the universal “CastBarUI”, “InfoBoxUI” and a universal “PopupUI”. 
// Those UI elements also come with options that can be edited via the Inspector. 
// Besides that, this AddOn adds a lot of new functions to the core asset, those
// functions are required in order for the AddOns to work as expected.
// =======================================================================================


// =======================================================================================
//                        * Installation and Configuration *
//========================================================================================

1). Open folder "Assets/uMMORPG/Scripts/Addons/_Tools/Imports" and import all package for 2D or 3D
2). Open in menu "Tools/Auto Patch System" if not auto displayed
  a) click for patch all file and wait recompile project

3). Open Scene "World" or your scene "world is in Assets/uMMORPG/Scenes/"
  3.a) Select NetworkManager GameObject
  3.b) Add new component "ToolsConfigurationManager"
  3.c) In inspector in script ToolsConfigurationManager
        c.1) Click on circle "Config Template" and select scriptable "Configuration"
        c.2) Click on cicle "Rules Template" and select scriptable "GameRules"

3). Edit all player prefabs (Warrior, Archer, other)
  a) add script "PlayerAddonsConfigurator"
        a.1) assign all component in component
        a.2) select Player Conponent search and assign playerAddonConponent
  b) add script "Tools_InfoBox"
        b.1) optionnal check force use chat

4). Open hierarchy "Assets/uMMORPG/Scripts/Addons/_Tools/_DEMO/Prefabs/_Tools/UI/Required UI [Drag to Canvas]"
  a) Add all prefabs in "Required UI [Drag to Canvas]" in Canvas
  b) (Optional) Drag and drop optional prefab to the scene



// =======================================================================================
//                                  * Optional *
//========================================================================================

// ======================== Installation Distance Checker ================================

1. Open Scene "World" scene in Assets/uMMORPG/Scenes/World
    a) Select GameObject on scene, for Exemple a house
    b) Add new component "DistanceChecker"
    c) an Sphere collider is added at the same time as DistanceChecker, Drag & drop this "sphere collider" in component "DistanceChecker", in "Collider Triger"
    d) Use slider for chose a distance maximum


// ============================= Installation Floater ====================================

1. Open hierarchy Assets/uMMORPG/Scripts/Addons/_Tools/Utils/Floater
    a) Exemple 1 
     - a) open Prefab folder (in sale folder hierarchy)
     - b) Drag & Drop prefab in the scene

    b) Exemple 2
     - a) in Scene Select a Npc (Alchemist for exemple)
     - b) extend hierarchy and find "NameOverlayPosition"
     - c) Add component Floater component


// ======================================= MySQL =========================================

1. Open Scene "World" scene in Assets/uMMORPG/Scenes/World
    a) Find Network Manager
    b) Find component Database Look at Database Type and Select MySQL
    c) Enter all information Required (Host name, user, password)
        c.1) if you do not have MySQL install https://www.wampserver.com/en/ or https://www.apachefriends.org/index.html
        c.2) for the mysql password, empty is not allowed. Use this link for help https://docs.phpmyadmin.net/en/latest/privileges.html
        c.3) create your scriptable database configuration 
    d) optionnal use thread save, enable log, enable dirty flag


// ================================ Latency And FPS ======================================

1. Open hierarchy Assets/uMMORPG/Scripts/Addons/_Tools/Utils/LatencyAndFps
    a) Extend all folder
    b) Drag and Drop prefab to Canvas in Scene

    
// =============================== Framerate Limiter =====================================
1. Open Scene "World" scene in Assets/uMMORPG/Scenes/World
    a) Search Main Camera
    b) Add new Component "FpsLimiter"
    c) Chose your target FPS and enable/disable VSync


// ============================== Sleep Screen Control ===================================
1. Open Scene "World" scene in Assets/uMMORPG/Scenes/World
    a) Search Main Camera
    b) Add new Component "SleepScreenControl"
    c) Choose number of minutes for screen sleep, or click on NeverSleepControl to disable sleeping


// ================================= UI Visibility =======================================
1. Open Scene "World" scene in Assets/uMMORPG/Scenes/World
    a) Search Canvas
    b) Add new Component "UIVisibility"
    c) Select your conbo key "Left Alt+Z" is default
    d) Add all the children of the Canvas you want hidden


// ============================== New attack detection system ============================
1) Open all entity prefab 
      a) add new GameObject empty and moved it to eye level
      b) in compotenent (Player,Monster,Pet,Mount,Npc) Add this GameObject in HeadPosition
