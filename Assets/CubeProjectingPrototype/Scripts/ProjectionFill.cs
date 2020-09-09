using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectionFill : MonoBehaviour {


    public Camera CubeFaceCamera
    {
        get; private set;
    }
    Texture2D projectionTexture;
    Color32[] textureColors;


    public void InitialiseTextures(RenderTexture targetRenderTexture, Camera camera )
    {
        projectionTexture = new Texture2D(targetRenderTexture.width, targetRenderTexture.height);
        textureColors = new Color32[targetRenderTexture.width * targetRenderTexture.height];
        CubeFaceCamera = camera;
    }

    public IEnumerator ReadProjectionPixels(RenderTexture targetRenderTexture)
    {
        yield return new WaitForEndOfFrame();

        RenderTexture.active = targetRenderTexture;
        projectionTexture.ReadPixels(new Rect(0, 0, targetRenderTexture.width, targetRenderTexture.height), 0, 0, false);

        //Debug.Log("projectionTexture.width " + projectionTexture.width + " projectionTexture.height " + projectionTexture.height);
        int counter = 0;
        Color backgroundColor = new Vector4(projectionTexture.GetPixel(0, 0).r, projectionTexture.GetPixel(0, 0).g, projectionTexture.GetPixel(0, 0).b, projectionTexture.GetPixel(0, 0).a);

        Color previousColor = backgroundColor;
        Color currentColor = backgroundColor;
        Color targetColor = Color.magenta;

        int? idofstartDraw = null;
        for (int y = 0; y < projectionTexture.height; y++)
        {
            
            for (int x = 0; x < projectionTexture.width; x++)
            {
                currentColor = projectionTexture.GetPixel(x, y);

                if (previousColor == backgroundColor && currentColor != backgroundColor)
                {
                    // First edge onto filament edge
                    textureColors[counter] = targetColor;
                    if (idofstartDraw !=null)
                    {
                        for (int i = (int)idofstartDraw; i <counter; i++)
                        {
                            textureColors[i] = targetColor;
                        }
                    }
                    //edgeCount++;
                    //ontoFilamentCount++;

                }
                else if (previousColor != backgroundColor && currentColor == backgroundColor /*&& inside && crossedEdge == 0*/)
                {
                    idofstartDraw = counter;
                    // second edge - inside filament
                }
                else
                {
                    textureColors[counter] = backgroundColor;
                }
                counter++;

                previousColor = currentColor;
            }
            idofstartDraw = null;
        }
        //Debug.Log("insideCount " + insideCount + " edgeCOunt " + edgeCount + " finalEdge " + finalEdge + " crossedEdge " + crossedEdge);
        projectionTexture.SetPixels32(textureColors);
        projectionTexture.Apply();
        GetComponent<Renderer>().material.mainTexture = projectionTexture;
        RenderTexture.active = null;
    }
}
