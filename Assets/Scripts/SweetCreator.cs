using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SweetCreator : MonoBehaviour
{

    public enum SweetType
    {
        EMPTY,
        NORMAL,
        BARRIER,
        ROW_CLEAR,
        COLUMN_CLEAR,
        RAINBOWCANDY
    }

    public Dictionary<SweetType, GameObject> sweetPrefabDict = new Dictionary<SweetType, GameObject>();

    [System.Serializable]
    public struct SweetPrefab
    {
        public SweetType type;
        public GameObject prefab;
    }

    public SweetPrefab[] sweetPrefabs;

    private static SweetCreator _instance;

    public static SweetCreator Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
        CreateSweetDict();
    }

    private void CreateSweetDict()
    {
        for (int i = 0; i < sweetPrefabs.Length; i++)
        {
            sweetPrefabDict.Add(sweetPrefabs[i].type, sweetPrefabs[i].prefab);
        }
    }

    public GameObject CreateRandomSweet(Vector3 position)
    {
        //int type = Random.Range(0, 6);
        //if (type == 1)
        {
            GameObject sweet = Instantiate(sweetPrefabDict[SweetType.NORMAL], position, Quaternion.identity);
            BaseSweet baseSweet = sweet.GetComponent<BaseSweet>();
            baseSweet.SweetType = SweetType.NORMAL;
            NormalSweet normalSweet = sweet.GetComponent<NormalSweet>();
            normalSweet.SetRandomSweetSprite();
            return sweet;
        }
        //else
        //{
        //    return Instantiate(sweetPrefabDict[SweetType.NORMAL], position, Quaternion.identity);
        //}
    }

    public GameObject CreateEmptySweet(Vector3 position)
    {
        GameObject sweet = Instantiate(sweetPrefabDict[SweetType.EMPTY], position, Quaternion.identity);
        BaseSweet baseSweet = sweet.GetComponent<BaseSweet>();
        baseSweet.SweetType = SweetType.EMPTY;
        return sweet;
    }
}
