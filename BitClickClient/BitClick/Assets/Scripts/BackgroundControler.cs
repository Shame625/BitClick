using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundControler : MonoBehaviour
{
	private Renderer myRenderer;

    public Texture2D mainMenuTexture;

	private void Awake()
	{
		myRenderer = GetComponent<Renderer>();
	}

	private void Update ()
	{
		Vector2 offset = new Vector2(Time.time * 0.01f, 0);
		myRenderer.material.mainTextureOffset = offset;
	}

	public void SetTexture(Texture2D text)
	{
		myRenderer.material.mainTexture = text;
	}

    public void SetMainMenu()
    {
        myRenderer.material.mainTexture = mainMenuTexture;
    }
}
