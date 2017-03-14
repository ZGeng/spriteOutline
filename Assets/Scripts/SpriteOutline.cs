using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SpriteOutline : MonoBehaviour
{
    //set a trigger 
    [SerializeField] bool showOutline = false;
    [SerializeField] Material outline_mat;
    [SerializeField] int outlineSortingOrder;

    [Header("Sprites Generated")]
    [SerializeField] int spriteCount = 0;
    SpriteRenderer spriteRenderer;
    GameObject oSprite;
    SpriteRenderer outlineRenderer;
    

    void Start()
    {   
        
        spriteRenderer = GetComponent<SpriteRenderer>();

        //create outline object
        createOutlineGameObject();
    }



    void LateUpdate()
    {
        Sprite sourceSprite = spriteRenderer.sprite;
        if (sourceSprite.name.Substring(0, 7) != "outline")
        {
            //check if the sprite already in the list 
            if (!SpriteStorage.spriteDict.ContainsKey(sourceSprite))
            {
                //do the generation of the new sprite 
                GenerateSprite(sourceSprite);
                spriteCount++;
            }

            if (showOutline)
            {
                //check if the object is enabled
                if (!oSprite.activeInHierarchy)
                {
                    //set the object active 
                    oSprite.SetActive(true);
                    outlineRenderer.sortingOrder = outlineSortingOrder;
                }
                outlineRenderer.sprite = SpriteStorage.spriteDict[sourceSprite];
            }
            else
            {
                if (oSprite.activeInHierarchy)
                {
                    oSprite.SetActive(false);
                }
            }

        }

    }


    void GenerateSprite(Sprite sourceSprite)
    {
        Texture2D sourceTexture = sourceSprite.texture;
        //check if the target texture is already generated
        //if not render the texture
        if (!SpriteStorage.textureDict.ContainsKey(sourceTexture))
        {
            RenderTexture tempRenderTexture = new RenderTexture(sourceTexture.width, sourceTexture.height, 1);


            RenderTexture originalRenderTexture = RenderTexture.active;
            Texture2D destTexture2D = new Texture2D(tempRenderTexture.width, tempRenderTexture.height, TextureFormat.RGBA32, false);

            RenderTexture.active = tempRenderTexture;
            Graphics.Blit(sourceTexture, tempRenderTexture, outline_mat);
            destTexture2D.ReadPixels(new Rect(0, 0, tempRenderTexture.width, tempRenderTexture.height), 0, 0);
            destTexture2D.Apply();
            //put the destTexture2D into the dictionary
            SpriteStorage.textureDict.Add(sourceTexture, destTexture2D);
            //set back the rendertexture
            RenderTexture.active = originalRenderTexture;
        }

        //generate the sprite 
        Rect sRect = sourceSprite.rect;
        float sPixels = sourceSprite.pixelsPerUnit;
        RenderTexture original = RenderTexture.active;
        Texture2D destTexture = SpriteStorage.textureDict[sourceTexture];

        Sprite tempSp = Sprite.Create(destTexture, sRect, new Vector2(0.5f, 0.5f), sPixels);
        tempSp.name = "outline_" + sourceSprite.name;
        //add the sprite to the dict
        SpriteStorage.spriteDict.Add(sourceSprite, tempSp);
    }

    void createOutlineGameObject()
    {
        //create outline sprite
        oSprite = new GameObject("outline" + this.name);
        oSprite.transform.SetParent(this.transform, false);
        outlineRenderer = oSprite.AddComponent<SpriteRenderer>();
        outlineRenderer.sortingOrder = outlineSortingOrder;
    }


}