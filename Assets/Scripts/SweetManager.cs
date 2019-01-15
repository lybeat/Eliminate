using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SweetManager : MonoBehaviour
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

    public GameObject chocolatePrefab;

    private int row = 10;
    private int column = 10;
    private Vector2Int lastClickPosition = Vector2Int.zero;
    private GameObject[,] allSweet;
    private NormalSweet currentSweet;

    private void Awake()
    {
        lastClickPosition.Set(-100, -100);
        allSweet = new GameObject[row, column];
        CreateSweetDict();
        CreateChocolateBackground();
        MatchAllSweet();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2Int clickPosition = GetClickSweetPosition(Input.mousePosition.x, Input.mousePosition.y);
            if (!lastClickPosition.Equals(clickPosition) && IsAdjacentSweet(clickPosition))
            {
                MoveSweet(clickPosition);
                lastClickPosition.Set(-100, -100);
            }
            else
            {
                lastClickPosition = clickPosition;
            }
        }
    }

    private void CreateSweetDict()
    {
        for (int i = 0; i < sweetPrefabs.Length; i++)
        {
            sweetPrefabDict.Add(sweetPrefabs[i].type, sweetPrefabs[i].prefab);
        }
    }

    private void CreateChocolateBackground()
    {
        for (int x = 0; x < column; x++)
        {
            for (int y = 0; y < row; y++)
            {
                GameObject chocolate = Instantiate(chocolatePrefab, CorrectPosition(x, y), Quaternion.identity);
                chocolate.transform.SetParent(transform);

                allSweet[x, y] = SweetCreator.Instance.CreateRandomSweet(CorrectPosition(x, y));
                allSweet[x, y].transform.SetParent(transform);
            }
        }
    }

    private Vector3 CorrectPosition(int x, int y)
    {
        return new Vector3(transform.position.x - column / 2f + x + 0.5f,
            transform.position.y - row / 2f + y + 0.5f, 0);
    }

    private Vector2Int GetClickSweetPosition(float xPosition, float yPosition)
    {
        Vector2Int pos = Vector2Int.zero;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(xPosition, yPosition, 0));
        for (int x = 0; x < column; x++)
        {
            for (int y = 0; y < row; y++)
            {
                if (worldPos.x > transform.position.x - column / 2f + x
                    && worldPos.x < transform.position.x - column / 2f + x + 1
                    && worldPos.y > transform.position.y - row / 2f + y
                    && worldPos.y < transform.position.y - row / 2f + y + 1)
                {
                    pos.Set(x, y);
                    return pos;
                }
            }
        }
        pos.Set(-100, -100);
        return pos;
    }

    private bool IsAdjacentSweet(Vector2Int position)
    {
        return (Mathf.Abs(lastClickPosition.x - position.x) == 1 && lastClickPosition.y == position.y)
            || lastClickPosition.x == position.x && Mathf.Abs(lastClickPosition.y - position.y) == 1;
    }

    private void SwapSweet(Vector2Int lastPosition, Vector2Int position)
    {
        // 交换两个甜品的位置坐标
        Vector3 temp = allSweet[lastPosition.x, lastPosition.y].gameObject.transform.position;
        allSweet[lastPosition.x, lastPosition.y].gameObject.transform.position
            = allSweet[position.x, position.y].gameObject.transform.position;
        allSweet[position.x, position.y].gameObject.transform.position = temp;

        // 交换两个甜品在数组的位置
        GameObject gameObject = allSweet[lastPosition.x, lastPosition.y];
        allSweet[lastPosition.x, lastPosition.y] = allSweet[position.x, position.y];
        allSweet[position.x, position.y] = gameObject;
    }

    private void MoveSweet(Vector2Int position)
    {
        Debug.Log("MoveSweet");
        SwapSweet(lastClickPosition, position);
        bool isMatch = false;
        isMatch = MatchRowSweet(position);
        isMatch |= MatchColumnSweet(position);
        isMatch |= MatchRowSweet(lastClickPosition);
        isMatch |= MatchColumnSweet(lastClickPosition);
        if (!isMatch)
        {
            SwapSweet(position, lastClickPosition);
        }
    }

    private bool MatchRowSweet(Vector2Int position)
    {
        currentSweet = allSweet[position.x, position.y].GetComponent<NormalSweet>();
        int leftX = position.x - 1;
        if (position.x > 0)
        {
            while (true)
            {
                BaseSweet leftBaseSweet = allSweet[leftX, position.y].GetComponent<BaseSweet>();
                if (leftBaseSweet.SweetType == SweetCreator.SweetType.EMPTY)
                {
                    break;
                }
                NormalSweet leftSweet = allSweet[leftX, position.y].GetComponent<NormalSweet>();
                if (currentSweet.SweetType != leftSweet.SweetType)
                {
                    break;
                }
                if (--leftX == -1)
                {
                    break;
                }
            }
        }

        int rightX = position.x + 1;
        if (position.x < column - 1)
        {
            while (true)
            {
                BaseSweet rightBaseSweet = allSweet[rightX, position.y].GetComponent<BaseSweet>();
                if (rightBaseSweet.SweetType == SweetCreator.SweetType.EMPTY)
                {
                    break;
                }
                NormalSweet rightSweet = allSweet[rightX, position.y].GetComponent<NormalSweet>();
                if (currentSweet.SweetType != rightSweet.SweetType)
                {
                    break;
                }
                if (++rightX == column)
                {
                    break;
                }
            }
        }
        if (rightX - leftX > 3)
        {
            // 消除行
            Debug.Log("消除行");
            for (int i = leftX + 1; i < rightX; i++)
            {
                GameObject.Destroy(allSweet[i, position.y]);
                allSweet[i, position.y] = SweetCreator.Instance.CreateEmptySweet(new Vector3(i, position.y, 0));
            }
            return true;
        }
        return false;
    }

    private bool MatchColumnSweet(Vector2Int position)
    {
        int bottomY = position.y - 1;
        if (position.y > 0)
        {
            while (true)
            {
                BaseSweet bottomBaseSweet = allSweet[position.x, bottomY].GetComponent<BaseSweet>();
                if (bottomBaseSweet.SweetType == SweetCreator.SweetType.EMPTY)
                {
                    break;
                }
                NormalSweet bottomSweet = allSweet[position.x, bottomY].GetComponent<NormalSweet>();
                if (currentSweet.SweetType != bottomSweet.SweetType)
                {
                    break;
                }
                if (--bottomY == -1)
                {
                    break;
                }
            }
        }

        int topY = position.y + 1;
        if (position.y < row - 1)
        {
            while (true)
            {
                BaseSweet topBaseSweet = allSweet[position.x, topY].GetComponent<BaseSweet>();
                if (topBaseSweet.SweetType == SweetCreator.SweetType.EMPTY)
                {
                    break;
                }
                NormalSweet topSweet = allSweet[position.x, topY].GetComponent<NormalSweet>();
                if (currentSweet.SweetType != topSweet.SweetType)
                {
                    break;
                }
                if (++topY == row)
                {
                    break;
                }
            }
        }
        if (topY - bottomY > 3)
        {
            // 消除列
            for (int i = bottomY + 1; i < topY; i++)
            {
                GameObject.Destroy(allSweet[position.x, i]);
                allSweet[position.x, i] = SweetCreator.Instance.CreateEmptySweet(new Vector3(position.x, i, 0));
            }
            return true;
        }
        return false;
    }

    private void MatchAllSweet()
    {
        for (int x = 0; x < column; x++)
        {
            for (int y = 0; y < row; y++)
            {
                BaseSweet baseSweet = allSweet[x, y].GetComponent<BaseSweet>();
                if (baseSweet.SweetType != SweetCreator.SweetType.EMPTY)
                {
                    Vector2Int position = Vector2Int.zero;
                    position.Set(x, y);
                    MatchRowSweet(position);
                    MatchColumnSweet(position);
                }
            }
        }
        //FillAllEmptySweet();
    }

    private void FillAllEmptySweet()
    {
        for (int x = 0; x < column; x++)
        {
            for (int y = 0; y < row; y++)
            {
                BaseSweet baseSweet = allSweet[x, y].GetComponent<BaseSweet>();
                if (baseSweet.SweetType == SweetCreator.SweetType.EMPTY)
                {
                    FillEmptySweet(x, y);
                }
            }
        }
    }

    private void FillEmptySweet(int currentX, int currentY)
    {
        for (int y = currentY + 1; y < row; y++)
        {
            BaseSweet baseSweet = allSweet[currentX, y].GetComponent<BaseSweet>();
            if (baseSweet.SweetType != SweetCreator.SweetType.EMPTY)
            {
                Debug.Log("Not Empty: " + baseSweet.SweetType);
                Vector2Int current = Vector2Int.zero;
                current.Set(currentX, currentY);
                Vector2Int notEmpty = Vector2Int.zero;
                notEmpty.Set(currentX, y);
                SwapSweet(current, notEmpty);
                break;
            }
        }
    }
}
