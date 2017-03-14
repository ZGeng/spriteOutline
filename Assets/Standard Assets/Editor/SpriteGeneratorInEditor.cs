#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SpriteGeneratorInEditor : MonoBehaviour {

    // Use this for initialization

    public Material outline_mat;
    void Start () {
        //process the sprite
        //get the animator 
        Animator animator = GetComponent<Animator>();
        foreach(AnimationClip ac in animator.runtimeAnimatorController.animationClips)
        {
            if (ac != null)
            {
                //get the key frame 
               foreach( EditorCurveBinding ecb in AnimationUtility.GetObjectReferenceCurveBindings(ac))
                {
                    ObjectReferenceKeyframe[] keyframes = AnimationUtility.GetObjectReferenceCurve(ac, ecb);
                    foreach (ObjectReferenceKeyframe keyframe in keyframes)
                    {
                        Sprite frame = (Sprite)keyframe.value;
                        if (!SpriteStorage.spriteDict.ContainsKey(frame))
                        {
                            GenerateSprite(frame);
                        }
                    }
                }
            }

        }
    }

    //helper function
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
        //AssetDatabase.CreateAsset(tempSp, "Assets/"+tempSp.name+".png");
        //add the sprite to the dict
        SpriteStorage.spriteDict.Add(sourceSprite, tempSp);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
#endif