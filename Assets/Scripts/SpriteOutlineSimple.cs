using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SpriteOutlineSimple : MonoBehaviour
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
    MaterialPropertyBlock outlineProperties;
    Texture destTexture;
    

    void Start()
    {   
        spriteRenderer = GetComponent<SpriteRenderer>();
        outlineProperties = new MaterialPropertyBlock();
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
                    //get and set the property 
                    
                    outlineRenderer.sortingOrder = outlineSortingOrder;
                }
                //using property block to change the outline texture
                outlineRenderer.sprite = sourceSprite;
                destTexture = SpriteStorage.textureDict[spriteRenderer.sprite.texture];
                spriteRenderer.GetPropertyBlock(outlineProperties);
                outlineProperties.SetTexture("_MainTex", destTexture);
                outlineRenderer.SetPropertyBlock(outlineProperties);
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
            destTexture2D.Compress(true);//compress the texture to save memory
            //put the destTexture2D into the dictionary
            SpriteStorage.textureDict.Add(sourceTexture, destTexture2D);
            //set back the rendertexture
            RenderTexture.active = originalRenderTexture;
        }
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