using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Floor;

public class PlayerCollider : MonoBehaviour
{
    [SerializeField] MagnetCollider magnetCollider;
    public MagnetCollider MagnetCollider => magnetCollider;

    // public System.Action OnChangeDir;
    //private bool isChangeDir;

    PlayerController playerController;


    public void OnAwake(PlayerController player)
    {
        playerController = player;

        // isChangeDir = false;

        playerController.OnStateChange += StateAction;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Debug.Log($"{collision.collider.name} / {collision.collider.tag}");
        if (collision.collider.TryGetComponent(out FloorSpawner floorSpawner))
        {
            playerController.EndJump();
            floorSpawner.FloorController?.Active(playerController);
        }
    }

    /// <summary>
    /// 점프중에 벽과 맞은 경우
    /// 한번에 2개에 맞는 경우 벽을 통과하기때문에 방지
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out ItemController itemController))
        {
            // Debug.Log($"Item Check : {itemController.Type}");

            itemController.Active(playerController);
        }
    }

    //public void SwitchMagnetCollider(bool isActive)
    //{
    //    magnetCollider.SetActive(isActive);
    //}

    public void StateAction(EPlayerState state)
    {
        switch(state)
        {
            case EPlayerState.Die:          // or Hit
                magnetCollider.ResetDraggingItems();
                break;
        }
    }
}
