using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.Timeline;

public class BlockController : MonoBehaviour
{
    [SerializeField] private Block[] blocks;

    public delegate void OnBlockClicked(int index);
    public OnBlockClicked onBlockClicked;   

    // 블록 초기화
    public void InitBlocks()
    {
        for (int i = 0; i < blocks.Length; i++)
        {
            blocks[i].InitMarker(i, blockIndex =>
            {
                onBlockClicked?.Invoke(blockIndex);
            });
        }
    }

    // 특정 블록에 마커 설정
    public void PlaceMarker(int blockIndex, Block.MarkerType marker)
    {
        blocks[blockIndex].SetMarker(marker);
    }
}
