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

    [Header("Texture Generated")]
    [SerializeField] int textureCount = 0;
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
            //check if the sprite already in the list 
            if (!SpriteStorage.textureDict.ContainsKey(sourceSprite.texture))
            {
                //do the generation of the new sprite 
                GenerateSprite(sourceSprite);
                textureCount++;
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


    void GenerateSprite(Sprite sourceSprite)
    {
        Texture2D sourceTexture = sourceSprite.texture;
        //check if the target texture is already generated
        //if not render the texture
        if (!SpriteStorage.textureDict.ContainsKey(sourceTexture))
        {
            RenderTexture tempRenderTexture = new RenderTexture(sourceTexture.width, sourceTexture.height, 1);
            RenderTexture originalRenderTexture = RenderTexture.active;
            Graphics.Blit(sourceTexture, tempRenderTexture, outline_mat);
            SpriteStorage.textureDict.Add(sourceTexture, tempRenderTexture);
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