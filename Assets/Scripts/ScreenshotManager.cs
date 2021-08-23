/*
 * 	Layered Scene Screenshot 1.0
 * 	============================
 * 
 * 
 * 	HOW TO USE
 * 	==========
 * 
 * 	1. Set your scene objects on separate camera layers (watch the demo scene for example)
 * 	2. Place the script ScreenshotManager.cs on your camera
 * 	3. Set the "In Game Key" to a keyboard key you will use for taking the snapshot at runtime
 * 	4. Set the "Layers" count to the numbers of layers you need
 * 	5. For each Layers item in that array, specify a name, the camera layer mask used for the png, and the transparency mode.
 * 	6. Check "Force Screen Size" to ensure that the png will have the correct game view size
 * 	7. Edit the screenshots "Prefix" if you need.
 * 	8. Play the scene and press your "In Game Key" to take a snapshot. All the layers are generated as PNG in the Assets/ folder.
 * 
 * 
 * 
 * 	CONTACT INFO
 * 	============
 * 
 * 	Anthony Kozak
 * 	contact@exoa.fr
 * 	http://www.exoa.fr
 * 
 * Copyright 2014 exoa.fr
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[ExecuteInEditMode]
public class ScreenshotManager : MonoBehaviour {

	public KeyCode inGameKey = KeyCode.A;
	public List<Layer> layers;
	public int screenWidth = Screen.width;
	public int screenHeight = Screen.height;
	public bool forceScreenSize;
	public string prefix = "Screenshot";

	private CameraClearFlags previousClearFlag;
	private int previousCullingMask;

	void Update () 
	{
		if (Input.GetKeyDown(inGameKey))
		{
			Shot();
		}
	}
	public void Shot()
	{
		if (forceScreenSize)
		{
			screenWidth = Screen.width;
			screenHeight = Screen.height;
		}
		StartCoroutine(ShotCoroutine(prefix));
	}
	IEnumerator ShotCoroutine(string prefix)
	{
		if (!Application.isPlaying)
			print("You should be in play mode !");

		previousClearFlag = Camera.main.clearFlags;
		previousCullingMask = Camera.main.cullingMask;

		int ex = 0;
		foreach (Layer l in layers)
		{
			if(l.transparent)
			{
				PrepareTransparentCamera(Camera.main);
			}
			else
			{
				ResetCamera(Camera.main);
			}

			Camera.main.cullingMask = l.mask;
			yield return new WaitForEndOfFrame();
		
			string sname = prefix + l.name;
			Texture2D tex = new Texture2D(screenWidth, screenHeight, l.transparent ? TextureFormat.ARGB32 : TextureFormat.RGB24, false);
			tex.ReadPixels(new Rect(0, 0, screenWidth, screenHeight), 0, 0);
			tex.Apply();

			// Encode texture into PNG
			byte[] bytes = tex.EncodeToPNG();
			DestroyImmediate(tex);

			File.WriteAllBytes(Application.dataPath + "/" + sname  + ".png", bytes);
			print("Saving " + Application.dataPath + "/" + sname + ".png");
			yield return new WaitForSeconds(0.01f);
			ex++;
		}
		ResetCamera(Camera.main);
	}
	private void ResetCamera(Camera c)
	{
		c.clearFlags = previousClearFlag;
		c.cullingMask = previousCullingMask;
		MonoBehaviour[] bs = c.gameObject.GetComponents<MonoBehaviour>();
		foreach (MonoBehaviour b in bs)
		{
			b.enabled = true;
		}
	}
	private void PrepareTransparentCamera(Camera c)
	{
		c.clearFlags = CameraClearFlags.Color;
		c.backgroundColor = new Color(0,0,0,0);
		MonoBehaviour[] bs = c.gameObject.GetComponents<MonoBehaviour>();
		foreach(MonoBehaviour b in bs)
		{
			b.enabled = false;
		}
	}
	[System.Serializable]
	public class Layer
	{
		public string name;
		public LayerMask mask;
		public bool transparent = true;
	}
}
