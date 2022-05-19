using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class Barrier : MonoBehaviour, IUtility, ILimitedUseEquippable
{
    public Sprite EquipSprite { get => _equipSprite; }
    public int EnergyCost { get; } = 20;
    public event System.Action<GameObject> OnLimitReached;

    [SerializeField] private Sprite _equipSprite;
    [SerializeField] private GameObject _barrierObject;
    
    public void Activate()
    {
        Vector3 spawnPos = transform.parent.position + transform.parent.forward*5f;
        ObjectSpawner.Instance.CmdSpawnNetworkObject(
            _barrierObject.name, 
            spawnPos, 
            _barrierObject.transform.rotation, 
            NetworkClient.connection as NetworkConnectionToClient
        );
        
        OnLimitReached?.Invoke(gameObject);
    }
}
