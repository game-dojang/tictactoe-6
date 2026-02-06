using UnityEngine;

public class BlockController : MonoBehaviour
{
    [SerializeField] private GameObject blockPrefab;

    // 블록 초기화
    public void InitBlocks()
    {
        for (int i = 0; i < 9; i++)
        {
            GameObject blockObject = Instantiate(blockPrefab, transform);
            Block block = blockObject.GetComponent<Block>();
            block.InitMarker(i);
        }
    }
}
