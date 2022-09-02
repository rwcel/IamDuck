using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Floor;

public class FloorSpawner : MonoBehaviour
{
    [SerializeField] Transform itemPoint;

    // ť�� �������� ������Ʈ��
    public FloorController FloorController;
    private ItemController itemController;

    private FloorData floorData;

    private PoolingManager _PoolingManager;

    // **�ڵ� �����ؾ��ҵ�? 
    public void SetFirstFloor()
    {
        floorData = LevelData.Instance.FirstFloor;
    }

    public void InitialiseWithData(FloorData floorData, int height)
    {
        if(_PoolingManager == null)
            _PoolingManager = PoolingManager.Instance;

        this.floorData = floorData;
        gameObject.SetActive(floorData.type != EFloorType.None);

        // CalcObstacle();              // => Block
        if(FloorController != null)
        {
            FloorController.InitialiseWithData(floorData);
            CalcItem(height);
        }
    }

    void CalcItem(int height)
    {
        //InGameItemData itemData = null;

        //// *Fever ���� Ȯ��
        //if (InGameManager.Instance.IsFever.Value)
        //{
        //    itemData = Utils.RandomBinary() ? LevelData.Instance.InGameItemMap[EItemType.Coin] : null;
        //}
        //else
        //{
        //    itemData = LevelManager.Instance.GetItemData(height);
        //}

        var itemData = LevelManager.Instance.GetItemData(height);

        if (itemData != null)
        {
            // Debug.Log(itemData);
            itemController = _PoolingManager.Dequeue(itemData.type.ToString(), itemPoint)
                                .GetComponent<ItemController>();
            itemController.OnActive = () => itemController = null;
            itemController.InitialiseWithData(itemData);
        }
    }

    public void EnqueueObjects()
    {
        if (FloorController != null)
        {
            _PoolingManager.Enqueue(FloorController.gameObject, true);
            FloorController = null;
        }
        if (itemController != null)
        {
            _PoolingManager.Enqueue(itemController.gameObject, true);
            itemController = null;
        }
    }
}
