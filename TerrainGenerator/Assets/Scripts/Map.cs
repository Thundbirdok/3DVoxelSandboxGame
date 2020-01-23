using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{

    public RenderTexture MapTexture;

    // Start is called before the first frame update
    void Start()
    {

        Texture2D texRef = new Texture2D(MapTexture.width, MapTexture.height);        

        for (int x = 0; x < MapTexture.width; ++x)
        {

            for (int y = 0; y < MapTexture.height; ++y)
            {

                texRef.SetPixel(x, y, Color.red);

            }

        }

        texRef.Apply();

        // Copy your texture ref to the render texture
        Graphics.Blit(texRef, MapTexture);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
