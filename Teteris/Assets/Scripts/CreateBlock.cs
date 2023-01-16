using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreateBlock : MonoBehaviour
{
    [SerializeField]
    private GameObject[] blockPool;
    [SerializeField]
    private GameObject[] previews;
    [SerializeField]
    private GameObject holdBlockView;
    [SerializeField]
    private Vector2 spawnPoint = new Vector2(4, 20);
    [SerializeField]
    private GameObject gameoverSign;
    [SerializeField]
    private GameObject lineRemoveEffect;

    public static CreateBlock instance;
    public static Dictionary<Vector2, GameObject> blockMap;

    private GameObject instant;
    private bool gameOver = false;
    private GameObject[] blockStack = new GameObject[7];
    private int stackIndex = 0;
    private List<GameObject> blockList = new List<GameObject>();
    private ForeviewBlockChanger[] foreviews;
    private ForeviewBlockChanger holdBlockChanger;
    private GameObject holdInstant;
    private GameObject holdInstantPrefab;
    private GameObject holdPrefab;
    private bool canHold = true;
    private int combo = 0;
    private int countClearLine = 0;
    private int difficulty = 0;
    private int sumClearLine = 0;

    private void Awake()
    {
        instance = GetComponent<CreateBlock>();
        blockMap = new Dictionary<Vector2, GameObject>();
        for (int i = 0; i < blockPool.Length; i++)
            blockStack[i] = blockPool[i];
        ShakeStack();
        while(stackIndex < blockStack.Length)
        {
            blockList.Add(blockStack[stackIndex]);
            stackIndex++;
        }
        ShakeStack();
        foreviews = new ForeviewBlockChanger[previews.Length];
        for(int i = 0; i < foreviews.Length; i++)
            foreviews[i] = previews[i].GetComponent<ForeviewBlockChanger>();
        holdBlockChanger = holdBlockView.GetComponent<ForeviewBlockChanger>();
        holdBlockChanger.ChangeBlock("N");
    }

    private void Start()
    {
        SpawnBlock();
    }

    private void Update()
    {
        if (!canHold) return;
        if (Input.GetKeyDown(KeyCode.C))
            ExHold();
    }

    private void ExHold()
    {
        Destroy(holdInstant);
        if(holdPrefab != null)
        {
            var clone = Instantiate(holdPrefab, spawnPoint, Quaternion.identity);
            clone.GetComponent<BlockMove>().ChangeDifficulty(difficulty);
        }
        else
        {
            var clone = Instantiate(blockList[0], spawnPoint, Quaternion.identity);
            clone.GetComponent<BlockMove>().ChangeDifficulty(difficulty);
            blockList.RemoveAt(0);
            blockList.Add(blockStack[stackIndex]);
            ShowBlock();
            if (++stackIndex >= 7)
                ShakeStack();
        }
        holdPrefab = holdInstantPrefab;
        holdBlockChanger.ChangeBlock(holdPrefab.name);
        canHold = false;
    }

    private void ShakeStack()
    {
        stackIndex = 0;
        int rand, nexRand;
        for(int i = 0; i < 20; i++)
        {
            rand = Random.Range(0, 7);
            nexRand = Random.Range(0, 7);
            var inst = blockStack[rand];
            blockStack[rand] = blockStack[nexRand];
            blockStack[nexRand] = inst;
        }
    }

    private void ShowBlock()
    {
        for(int i = 0; i < blockList.Count - 1; i++)
            foreviews[i].ChangeBlock(blockList[i].name);
    }

    public void SpawnBlock()
    {
        if (gameOver) return;
        if(blockMap.ContainsKey(new Vector2(3, 19)) || blockMap.ContainsKey(new Vector2(4, 19)) || blockMap.ContainsKey(new Vector2(5, 19)))
        {
            gameOver = true;
            gameoverSign.SetActive(true);
            Invoke("Restart", 3);
            return;
        }
        canHold = true;
        holdInstantPrefab = blockList[0];
        holdInstant = Instantiate(holdInstantPrefab, spawnPoint, Quaternion.identity);
        holdInstant.GetComponent<BlockMove>().ChangeDifficulty(difficulty);
        blockList.RemoveAt(0);
        blockList.Add(blockStack[stackIndex]);
        ShowBlock();
        if(++stackIndex >= 7)
            ShakeStack();
    }

    private void Restart()
    {
        SceneManager.LoadScene("StartScene");
    }

    public void CheckLine(int lineNum)
    {
        for(int i = 0; i < 10; i++)
            if(!blockMap.ContainsKey(new Vector2(i, lineNum)))
                return;
        for(int i = 0; i < 10; i++)
        {
            var vec = new Vector2(i, lineNum);
            var obj = blockMap[vec];
            Destroy(obj.gameObject);
            blockMap.Remove(vec);
            for(int j = lineNum + 1; j < 22; j++)
            {
                var pos = new Vector2(i, j);
                if (blockMap.TryGetValue(pos, out instant))
                {
                    instant.transform.position += Vector3.down;
                    blockMap.Remove(pos);
                    pos.y -= 1;
                    blockMap.Add(pos, instant);
                }
            }
        }
        sumClearLine++;
        if (sumClearLine % 10 == 0)
        {
            difficulty = sumClearLine / 10;
            TextManager.instance.ChangeDifficulty(difficulty);
        }

        countClearLine++;
    }

    public void BeforeClearLine(int lineNum)
    {
        for (int i = 0; i < 10; i++)
            if (!blockMap.ContainsKey(new Vector2(i, lineNum)))
                return;
        Instantiate(lineRemoveEffect, new Vector3(4.5f, lineNum), Quaternion.identity);
    }

    private bool PerfectClear()
    {
        for (int i = 0; i < 10; i++)
            if (blockMap.ContainsKey(new Vector2(i, 0)))
                return false;
        return true;
    }
    
    public void EndInsertBlock(string blockName, bool isTSpin)
    {
        if (countClearLine == 0)
        {
            combo = 0;
            TextManager.instance.ChangeCombo(combo);
            return;
        }
        else
        {
            combo++;
            if(isTSpin && blockName == "T(Clone)")
            {
                switch(countClearLine)
                {
                    case 1:
                        TextManager.instance.ChangeText("T-Spin\nSingle");
                        break;
                    case 2:
                        TextManager.instance.ChangeText("T-Spin\nDouble");
                        break;
                    case 3:
                        TextManager.instance.ChangeText("T-Spin\nTriple");
                        break;
                    default:
                        Debug.Log("에바");
                        return;
                }
            }
            if(countClearLine >= 4)
            {
                TextManager.instance.ChangeText("Tetris");
            }
            if(PerfectClear())
            {
                TextManager.instance.ChangeText("Perfect\nClear");
            }
        }
        countClearLine = 0;
        TextManager.instance.ChangeCombo(combo);
        TextManager.instance.ChangeLine(sumClearLine);
    }
}
