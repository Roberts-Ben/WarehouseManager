using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

public class SpriteGenerationTool : ScriptableWizard
{
    public  string              outputDirectory = "/Sprites/";
    public  int                 xresolution      = 256;
	public  int                 yresolution      = 256;
    public  Transform           prefabHolder;

    [MenuItem("Tools/Sprite Generation Tool/Generate Sprites")]
    static void CreateWindow()
    {
        ScriptableWizard.DisplayWizard<SpriteGenerationTool>("Sprite Generation Tool", "Generate", "Refresh");
    }

    private void OnWizardOtherButton()
    {
        OnWizardUpdate();
    }
    private void OnWizardUpdate()
    {
        if (prefabHolder == null)
        {
            try
            {
                prefabHolder = GameObject.Find("PrefabHolder").transform;
            }
            catch
            {
                Debug.LogWarning("PrefabHolder could not be found in the current scene.");
            }
        }

        helpString = "Please ensure all prefabs you wish to generate sprites for are children of the PrefabHolder Game Object in the SpriteGenerationTool scene. Only active Game Objects will be proccessed.";

        if (!Directory.Exists("Assets/" + outputDirectory))
        {
            errorString = "Output directory is not valid.";
        }
        else if (xresolution <= 0 || yresolution <= 0)
        {
            errorString = "Resolution is not valid.";
        }
        else if (prefabHolder == null)
        {
            errorString = "No parent Game Object has been provided. Please Refresh or assign manually.";

        }
        else
        {
            errorString = "";
        }
    }

    void OnWizardCreate()
    {
        Debug.Log("Running Sprite Gen");

        List<GameObject> prefabsToRender = new();
        foreach (Transform t in prefabHolder) //Looking at block prefab holder
        {
            if (t.gameObject != prefabHolder && t.gameObject.GetComponent<LODGroup>() == null && t.gameObject.activeSelf) //looking at the category empties
            {
                Debug.Log("Looking at category: " + t.name);
                foreach (Transform obj in t) //Looking at the prefabs within the category empties
                {
                    if (obj.gameObject.activeSelf)
                    {
                        Debug.Log("Adding prefab to render list: " + obj.name);
                        prefabsToRender.Add(obj.gameObject);
                        obj.gameObject.SetActive(false);
                    }
                }
            }
        }

        RenderTexture rTex = new(xresolution, yresolution, 32, RenderTextureFormat.ARGB32);
        
        Camera.main.targetTexture = rTex;

        Texture2D tex = new(xresolution, yresolution, TextureFormat.ARGB32, false);

        foreach (GameObject prefab in prefabsToRender)
        {
            Debug.Log("Generating sprite for: " + prefab.name);
            prefab.SetActive(true);

            Camera.main.Render();
            RenderTexture.active = rTex;

            tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);

            byte[] bytes = tex.EncodeToPNG();

            File.WriteAllBytes(Application.dataPath + outputDirectory + prefab.name + ".png", bytes);

            prefab.gameObject.SetActive(false);
            Debug.Log("Successfully generated: " + prefab.name);
        }

        Camera.main.targetTexture = null;
        RenderTexture.active = null;
        DestroyImmediate(tex);

        foreach (GameObject prefab in prefabsToRender)
        {
            prefab.SetActive(true);
        }
    }
}
