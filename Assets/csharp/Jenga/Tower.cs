using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tower : MonoBehaviour
{
    [Header("Block setting")]
    public Transform[] blockPrefabs;
    public int height = 17;
    public float zBias = 12.0f;
    public float spawnHeight = -3.5f;
    public float spacingBetweenBlocks = 1.0f;

    private List<Transform> blocks;
    public bool bGameRunning = false;
    private bool bDestroying = false;
    private bool bRotating = false;

    [Header("UI setting")]
    public GameObject gameOverUI;
    public GameObject gameStartmenuUI;

    private int[] getRandomBlockType()
    {
        int[] blockTypes = new int[3];
        blockTypes[0] = Random.Range(0, blockPrefabs.Length);
        blockTypes[1] = Random.Range(0, blockPrefabs.Length);
        blockTypes[2] = Random.Range(0, blockPrefabs.Length);
        return blockTypes;
    }

    private int[] getRandomBlockTypeWithoutOverlap()
    {
        int[] blockTypes = new int[3] { 0, 1, 2 };
        for (int i = 0; i < 2; i++)
        {
            int RandomIdx = Random.Range(i, 3);
            int temp = blockTypes[i];
            blockTypes[i] = blockTypes[RandomIdx];
            blockTypes[RandomIdx] = temp;
        }
        return blockTypes;
    }

    private void Start()
    {
        gameOverUI.SetActive(false);
        gameStartmenuUI.SetActive(true);
        StartCoroutine(GameSet());
        gameStartmenuUI.SetActive(false);
    }

    private void Update()
    {
        if (bGameRunning)
        {
            CheckGameOver();
            //if (!bDestroying) CheckBlockInLine();
        }
    }

    private IEnumerator GameSet()
    {
        var wait = new WaitForSeconds(0.1f);
        // Set blocks for game
        blocks = new List<Transform>(new Transform[height * 3]);
        for (int i = 0; i < height; ++i)
        {
            if (i % 2 == 0)
            {
                BuildVerticalLayer(i);
            }
            else
            {
                BuildHorizontalLayer(i);
            }
            for (int j = 0; j < 3; j++)
            {
                Rigidbody body = blocks[3 * i + j].GetComponent<Rigidbody>();
                body.velocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
            }
            yield return wait;
        }
        StartCoroutine(Stabilize());
    }
    public void GameStart()
    {
        StartCoroutine(GameSet());
        gameStartmenuUI.SetActive(false);
    }

    public void GameReset()
    {
        // Game reset
        // destroy current existing blocks
        foreach (var block in blocks)
        {
            Destroy(block.gameObject);
        }
        blocks.Clear();
        // game over ui off
        gameOverUI.SetActive(false);
        // block setting
        StartCoroutine(GameSet());
    }

    private void CheckGameOver()
    {
        int numOfFallingBlocks = 0;
        foreach (var block in blocks)
        {
            Rigidbody body = block.GetComponent<Rigidbody>();
            if (body.velocity.y < -1.0f)
            {
                numOfFallingBlocks += 1;
            }
        }
        // if the number of falling blocks is more than 5
        if (numOfFallingBlocks > 5)
        {
            // GameOver UI Active
            gameOverUI.SetActive(true);
            bGameRunning = false;
        }
    }

    private void CheckBlockInLine()
    {
        List<int[]> blockDestroyIndex = new List<int[]>();
        float refHeight = 0; //기준 높이
        int refBlockType = -1; //이전에 찾은 block의 index
        int iTempBlock = 0; //tempBlock의 block index
        int blockIndex = -1; //block의 index
        bool bDestroy = true;

        for (int i = 0; i < height; ++i)
        {
            int[] tempBlockIndex = new int[3];
            refHeight = Block.height / 2.0f * transform.localScale.y + i * Block.height * transform.localScale.y;
            iTempBlock = 0;
            blockIndex = -1;
            bDestroy = true;

            // Find Block and block's color type
            foreach (var block in blocks)
            {
                if (iTempBlock > 2)
                {
                    break;
                }
                blockIndex += 1;
                //find block located at that height and not falling
                //refheight - 0.1f < BlockHeight < refHeight + 0.1f
                if (refHeight - 0.3f * transform.localScale.y < block.transform.position.y && block.transform.position.y < refHeight + 0.3f * transform.localScale.y)
                {
                    if (!IsStableBlock(block))
                    {
                        continue;
                    }
                    if (iTempBlock == 0)
                    {
                        // 해당 height에서 처음 찾은 블럭
                        tempBlockIndex[iTempBlock] = blockIndex;
                        refBlockType = block.GetComponent<Block>().BlockType;
                    }
                    else
                    {
                        // 해당 height에서 찾은 다음 블럭
                        if (refBlockType == block.GetComponent<Block>().BlockType)
                        {
                            tempBlockIndex[iTempBlock] = blockIndex;
                        }
                        else
                        {
                            bDestroy = false;
                            break;
                        }
                    }
                    iTempBlock += 1;
                }
            }

            // destroy 판정
            if (bDestroy && iTempBlock >= 3)
            {
                blockDestroyIndex.Add(tempBlockIndex);
            }
        }

        //Destroy할 블럭이 존재 -> Destroy
        if (blockDestroyIndex.Count != 0)
        {
            bGameRunning = false;
            DestoryBlocks(blockDestroyIndex);
        }
    }

    private bool IsStableBlock(Transform refBlock)
    {
        Rigidbody body = refBlock.GetComponent<Rigidbody>();
        if (body.velocity.magnitude > 1)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private void DestroyBlockInLine(int[] refIndex)
    {
        //내림차순 정렬
        int temp;
        for (int i = 0; i < refIndex.Length; i++)
        {
            for (int j = i; j < refIndex.Length; j++)
            {
                if (refIndex[i] < refIndex[j])
                {
                    temp = refIndex[j];
                    refIndex[j] = refIndex[i];
                    refIndex[i] = temp;
                }
            }
        }
        int blockLargestIndex = refIndex[0];
        //destroy block
        foreach (var index in refIndex)
        {
            Destroy(blocks[index].gameObject);
            blocks.RemoveAt(index);
        }
        //rotate
        for (int i = blockLargestIndex - 2; i < blocks.Count; i++)
        {
            blocks[i].transform.RotateAround(Vector3.zero, Vector3.up, -90.0f);
        }
    }

    private void DestoryBlocks(List<int[]> blocksIndex)
    {
        blocksIndex.Reverse();
        foreach (int[] blockIndexArr in blocksIndex)
        {
            DestroyBlockInLine(blockIndexArr);
        }
        StartCoroutine(Stabilize());
    }

    private IEnumerator RotateBlocks(int startIdx)
    {
        var wait = new WaitForSeconds(0.05f);
        float sumAngle = 0.0f;
        float speed = 10.0f;
        while (sumAngle <= 90.0f)
        {
            sumAngle += Time.deltaTime * speed;
            for (int i = startIdx; i < blocks.Count; i++)
            {
                blocks[i].transform.RotateAround(Vector3.zero, Vector3.up, -1.0f * Time.deltaTime * speed);
            }
            yield return wait;
        }
    }

    private IEnumerator Stabilize()
    {
        var wait = new WaitForSeconds(0.05f);

        int index = 0;
        foreach (Transform block in blocks)
        {
            Rigidbody body = block.GetComponent<Rigidbody>();
            body.velocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;

            index += 1;
            if (index >= blocks.Count)
            {
                bGameRunning = true;
            }
            else
            {
                yield return wait;
            }
        }
    }

    private void BuildVerticalLayer(int layerIndex)
    {
        float y = (Block.height + Block.deformation + spacingBetweenBlocks) * layerIndex + spawnHeight;

        int[] blockTypes = getRandomBlockType();

        Transform block = Instantiate(blockPrefabs[blockTypes[0]], transform);
        block.localPosition = new Vector3(0.0f, y, zBias);
        block.GetComponent<Block>().BlockType = blockTypes[0];
        blocks[3 * layerIndex + 0] = block;

        block = Instantiate(blockPrefabs[blockTypes[1]], transform);
        block.localPosition = new Vector3(Block.width + Block.deformation, y, zBias);
        block.GetComponent<Block>().BlockType = blockTypes[1];
        blocks[3 * layerIndex + 1] = block;

        block = Instantiate(blockPrefabs[blockTypes[2]], transform);
        block.localPosition = new Vector3(-Block.width - Block.deformation, y, zBias);
        block.GetComponent<Block>().BlockType = blockTypes[2];
        blocks[3 * layerIndex + 2] = block;
    }

    private void BuildHorizontalLayer(int layerIndex)
    {
        Quaternion rotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
        float y = (Block.height + Block.deformation + spacingBetweenBlocks) * layerIndex + spawnHeight;

        int[] blockTypes = getRandomBlockType();

        Transform block = Instantiate(blockPrefabs[blockTypes[0]], transform);
        block.localRotation = rotation;
        block.localPosition = new Vector3(0, y, zBias);
        block.GetComponent<Block>().BlockType = blockTypes[0];
        blocks[3 * layerIndex + 0] = block;

        block = Instantiate(blockPrefabs[blockTypes[1]], transform);
        block.localRotation = rotation;
        block.localPosition = new Vector3(0, y, Block.width + Block.deformation + zBias);
        block.GetComponent<Block>().BlockType = blockTypes[1];
        blocks[3 * layerIndex + 1] = block;

        block = Instantiate(blockPrefabs[blockTypes[2]], transform);
        block.localRotation = rotation;
        block.localPosition = new Vector3(0, y, -Block.width - Block.deformation + zBias);
        block.GetComponent<Block>().BlockType = blockTypes[2];
        blocks[3 * layerIndex + 2] = block;
    }

}
