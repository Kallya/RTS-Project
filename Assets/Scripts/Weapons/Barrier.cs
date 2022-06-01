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
        Vector3 spawnPos = new Vector3(transform.root.position.x, _barrierObject.transform.position.y, transform.root.position.z) + transform.root.forward * 5f;
        
        ObjectSpawner.Instance.CmdSpawnNetworkObject(
            _barrierObject.name, 
            spawnPos, 
            Quaternion.Euler(-90f, 0f, transform.root.eulerAngles.y), 
            NetworkClient.connection as NetworkConnectionToClient
        );
        
        OnLimitReached?.Invoke(gameObject);
    }
}
