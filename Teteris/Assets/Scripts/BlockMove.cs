using UnityEngine;

public class BlockMove : MonoBehaviour
{
    [SerializeField]
    private int width, height;
    [SerializeField]
    private GameObject[] blocks;
    [SerializeField]
    private BlockStamps[] rotStamps;
    [SerializeField]
    private GameObject foreview;
    [SerializeField]
    private GameObject[] foreviewBlocks;

    [System.Serializable]
    public struct BlockStamps
    {
        public Vector2[] pos;
    }

    public float maxGroundTime;

    private GameObject instant;
    private float curGroundTime = 0;
    [System.NonSerialized]
    public bool canControl = true;
    private int rotIndex = 0;
    private int delayer = 1;
    private int delay = 0;
    private int[,] checkPosPool = new int[14, 2] { { 0, 0 }, { 1, 0 }, { -1, 0 }, { 0, -1 }, { -1, -1 }, { 1, -1 }, { -2, 0 },
                                    { 2, 0 }, { 0, -2 }, { -1, -2 }, { 1, -2 }, { 0, 1 }, { -1, 1 }, { 1, 1 } };
    private int checkIndex = 0;
    private float maxMoveDelay = 0.1f;
    private float moveDelay = 0;
    private float maxDownDelay = 0.1f;
    private float downDelay = 0;
    private bool onLeft, onRight, onDown = false;
    private bool isTSpin = false;
    private int difficulty = 0;
    public void ChangeDifficulty(int value)
    {
        if (value <= 0) difficulty = 0;
        else if (value >= 16) difficulty = 16;
        else difficulty = value;
    }
    private float[] autoDownDelay = new float[14] { 2.0f, 1.6f, 1.2f, 0.9f, 0.7f, 0.5f, 0.4f, 0.3f, 0.25f, 0.2f, 0.15f, 0.1f, 0.05f, 0f };
    private float curDownDelay = 0;

    private void FixedUpdate()
    {
        if (!canControl) return;
        delay++;
        if (delay > delayer)
        {
            delay = 0;
            return;
        }
        KeepMoveHorizon();
        KeepMoveDown();
    }

    private void Update()
    {
        CheckDestroy();
        if (!canControl) return;
        PressButton();
        PressUpdateButton();
        AutoDownBlock();
        ForeviewBlockDown();
    }

    private void PressButton()
    {
        if (!canControl) return;
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            if (CheckChildrens(-1, 0))
            {
                transform.position += new Vector3(-1, 0);
                onLeft = true;
                if (onRight) onRight = false;
                isTSpin = false;
                AudioManager.instance.PlaySound("Move");
            }
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            onLeft = false;
            AudioManager.instance.StopSound("Move");
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
            if (CheckChildrens(1, 0))
            {
                transform.position += new Vector3(1, 0);
                onRight = true;
                if (onLeft) onLeft = false;
                isTSpin = false;
                AudioManager.instance.PlaySound("Move");
            }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            onRight = false;
            AudioManager.instance.StopSound("Move");
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
            if (CheckChildrens(0, -1))
            {
                transform.position += new Vector3(0, -1);
                onDown = true;
                AudioManager.instance.PlaySound("Move");
                isTSpin = false;
            } else
                curGroundTime = maxGroundTime;
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            onDown = false;
            AudioManager.instance.StopSound("Move");
        }
        if (onLeft || onRight) moveDelay += Time.deltaTime;
        else moveDelay = 0;
        if (onDown) downDelay += Time.deltaTime;
        else downDelay = 0;
    }

    private void AutoDownBlock()
    {
        curDownDelay += Time.deltaTime;
        if (curDownDelay > (autoDownDelay[difficulty] * 1.5f) + 0.5f)
            InsertBlock();
        if (curDownDelay < autoDownDelay[difficulty]) return;
        if(CheckChildrens(0, -1))
        {
            transform.position += Vector3.down;
            curDownDelay = 0;
        } else if (onLeft || onRight) curDownDelay = 0;
    }

    private void KeepMoveHorizon()
    {
        if (!canControl) return;
        if (moveDelay < maxMoveDelay) return;
        if(onLeft)
        {
            if (CheckChildrens(-1, 0))
            {
                transform.position += new Vector3(-1, 0);
                isTSpin = false;
            }
        } else if (onRight)
        {
            if (CheckChildrens(1, 0))
            {
                transform.position += new Vector3(1, 0);
                isTSpin = false;
            }
        }
    }

    private void KeepMoveDown()
    {
        if (!canControl) return;
        if (downDelay < maxDownDelay) return;
        if (onDown)
            if (CheckChildrens(0, -1))
            {
                transform.position += new Vector3(0, -1);
                isTSpin = false;
            }
    }

    private void ForeviewBlockDown()
    {
        if (!canControl) return;
        if (foreview == null) return;
        foreview.transform.position = transform.position + Vector3.forward;
        for (int i = 0; i < height; i++)
        {
            if (!CheckPreviewChildrens(-i))
            {
                foreview.transform.localPosition += Vector3.down * i;
                foreview.transform.localPosition += Vector3.up;
                break;
            }
        }
    }

    private void PressUpdateButton()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.X))
        {
            if (CheckChildrensRotate(true))
            {
                AudioManager.instance.PlaySound("Turn");
                isTSpin = true;
                RotateChildBlock(true);
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.Z))
        {
            if (CheckChildrensRotate(false))
            {
                AudioManager.instance.PlaySound("Turn");
                isTSpin = true;
                RotateChildBlock(false);
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < height; i++)
            {
                if (!CheckChildrens(0, -i))
                {
                    transform.position += Vector3.down * i;
                    transform.position += Vector3.up;
                    curGroundTime = maxGroundTime;
                    if(i > 1)
                        isTSpin = false;
                    break;
                }
            }
        }
        if (!CheckChildrens(0, -1))
        {
            curGroundTime += Time.deltaTime;
            if (curGroundTime > maxGroundTime)
                InsertBlock();
        }
    }

    public bool CheckChildrens(int x, int y)
    {
        if (!canControl) return false;
        foreach (var obj in blocks)
        {
            var xPos = obj.transform.position.x + x;
            var yPos = obj.transform.position.y + y;
            if (xPos < 0 || xPos >= width || yPos < 0 || yPos >= height)
            {
                return false;
            }
            if(CreateBlock.blockMap.TryGetValue(new Vector2(xPos, yPos), out instant))
            {
                return false;
            }
        }
        return true;
    }

    public bool CheckPreviewChildrens(int y)
    {
        if (!canControl) return false;
        if (foreview == null) return false;
        foreach (var obj in foreviewBlocks)
        {
            var xPos = obj.transform.position.x;
            var yPos = obj.transform.position.y + y;
            if (xPos < 0 || xPos >= width || yPos < 0 || yPos >= height)
            {
                return false;
            }
            if (CreateBlock.blockMap.TryGetValue(new Vector2(xPos, yPos), out instant))
            {
                return false;
            }
        }
        return true;
    }

    public bool CheckChildrensRotate(bool right)
    {
        if (!canControl) return false;
        int index;
        if (right)
            index = rotIndex + 1 < rotStamps.Length ? rotIndex + 1 : 0;
        else
            index = rotIndex - 1 < 0 ? rotStamps.Length - 1 : rotIndex - 1;
        bool check;
        for (int j = 0; j < 14; j++)
        {
            check = true;
            checkIndex = j;
            for (var i = 0; i < 4; i++)
            {
                var xLocalPos = rotStamps[index].pos[i].x + checkPosPool[j, 0];
                var yLocalPos = rotStamps[index].pos[i].y + checkPosPool[j, 1];
                float xPos = transform.position.x + xLocalPos;
                float yPos = transform.position.y + yLocalPos;
                if (xPos < 0 || xPos >= width || yPos < 0 || yPos >= height)
                {
                    check = false;
                }
                if (CreateBlock.blockMap.TryGetValue(new Vector2(xPos, yPos), out instant))
                {
                    check = false;
                }
            }
            if (check)
                return true;
        }
        return false;
    }

    public void RotateChildBlock(bool right)
    {
        if (!canControl) return;
        if (foreview == null) return;
        int i = 0;
        if (right)
            rotIndex = rotIndex + 1 < rotStamps.Length ? rotIndex + 1 : 0;
        else
            rotIndex = rotIndex - 1 < 0 ? rotStamps.Length - 1 : rotIndex - 1;
        for(var j = 0; j < blocks.Length; j++)
        {
            var xPos = rotStamps[rotIndex].pos[i].x;
            var yPos = rotStamps[rotIndex].pos[i].y;
            blocks[j].transform.localPosition = new Vector3(xPos, yPos);
            foreviewBlocks[j].transform.localPosition = new Vector3(xPos, yPos);
            i++;
        }
        transform.position += new Vector3(checkPosPool[checkIndex, 0], checkPosPool[checkIndex, 1]);
        CheckAutoDown();
        return;
    }

    private void CheckAutoDown()
    {
        if (!CheckChildrens(0, -1))
            curDownDelay = 0;
    }

    private void InsertBlock()
    {
        if (!canControl) return;
        if (isTSpin) CheckTSpin();
        foreach (var obj in blocks)
            CreateBlock.blockMap.Add(new Vector2(obj.transform.position.x, obj.transform.position.y), obj);
        foreach (var obj in blocks)
            CreateBlock.instance.BeforeClearLine((int)obj.transform.position.y);
        foreach (var obj in blocks)
            CreateBlock.instance.CheckLine((int)obj.transform.position.y);
        canControl = false;
        CreateBlock.instance.SpawnBlock();
        CreateBlock.instance.EndInsertBlock(this.name, isTSpin);
        AudioManager.instance.PlaySound("HardMove");
        Destroy(foreview);
    }

    private void CheckTSpin()
    {
        int tSpinCounter = 0;
        var x = transform.position.x;
        var y = transform.position.y;
        if (CreateBlock.blockMap.ContainsKey(new Vector2(x + 1, y + 1))) tSpinCounter++;
        if (CreateBlock.blockMap.ContainsKey(new Vector2(x + 1, y - 1))) tSpinCounter++;
        if (CreateBlock.blockMap.ContainsKey(new Vector2(x - 1, y + 1))) tSpinCounter++;
        if (CreateBlock.blockMap.ContainsKey(new Vector2(x - 1, y - 1))) tSpinCounter++;
        if (tSpinCounter < 3) isTSpin = false;
    }

    private void CheckDestroy()
    {
        for(int i = 0; i < blocks.Length; i++)
        {
            if (blocks[i] != null)
                return;
        }
        Destroy(gameObject);
    }
}
