using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalSweet : MonoBehaviour
{
    public enum NormalSweetType
    {
        DONUT,
        LOLLIPOP,
        RAINBOW,
        MOUSSE,
        SANDWICH,
        STRAWBERRY,
        LEMON
    }

    public Dictionary<NormalSweetType, Sprite> sweetSpriteDict = new Dictionary<NormalSweetType, Sprite>();

    [System.Serializable]
    public struct SweetSprite
    {
        public NormalSweetType type;
        public Sprite sprite;
    }

    public SweetSprite[] sweetSprites;
    private SpriteRenderer spriteRenderer;

    public NormalSweetType SweetType { get; private set; }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        CreateSweetSpriteDict();
    }

    private void CreateSweetSpriteDict()
    {
        for (int i = 0; i < sweetSprites.Length; i++)
        {
            sweetSpriteDict.Add(sweetSprites[i].type, sweetSprites[i].sprite);
        }
    }

    public void SetRandomSweetSprite()
    {
        SweetType = (NormalSweetType)Random.Range(0, 7);
        spriteRenderer.sprite = sweetSpriteDict[SweetType];
    }
}
