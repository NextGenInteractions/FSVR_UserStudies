# NextGen VR Manager Package

## Installation

### Git install
Unity's Package Manager supports [importing packages through a URL to a Git repo](https://docs.unity3d.com/Manual/upm-ui-giturl.html):  
Use one of the following URLs:  

    https://github.com/NextGenInteractions/NextGenVRManager.git?path=/Assets
    ssh://github.com:NextGenInteractions/NextGenVRManager.git?path=/Assets

### Local File Reference
Unity's Package Manager also supports [Installing a package from a local folder](https://docs.unity3d.com/Manual/upm-ui-local.html)  
This is the recommended approach for local development.

## Dependencies
### Embedded Dependencies
SteamVR  
Leap Motion
### Package Dependencies
Newtonsoft.Json for Unity [https://github.com/jilleJr/Newtonsoft.Json-for-Unity](https://github.com/jilleJr/Newtonsoft.Json-for-Unity)  
OpenVR UnityXR Plugin [https://github.com/ValveSoftware/unity-xr-plugin](https://github.com/ValveSoftware/unity-xr-plugin)  
Text Mesh Pro (through unity package registry)  

## Setup
1) Import the package through one of the methods above.  
2) Import the package dependencies through the links listed above.
3) Copy the StreamingAssets/SteamVR folder into your local StreamingAssets folder
4) Copy the NextGen VR Manager prefab into your scene from the package Prefabs folder

Note that step 3 is required in order for SteamVR inputs to function properly for devices. We are hoping to improve this workflow to not require manual copying of files/folders, but SteamVR is finnicky.