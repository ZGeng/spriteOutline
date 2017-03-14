using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteChangeInEditor : MonoBehaviour {

    SpriteRenderer spriteRenderer;

    // Use this for initialization
    void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void LateUpdate () {
        if (spriteRenderer.sprite.name.Substring(0, 7) != "outline")
        {
            spriteRenderer.sprite = SpriteStorage.spriteDict[spriteRenderer.sprite];
        }
	}
}
