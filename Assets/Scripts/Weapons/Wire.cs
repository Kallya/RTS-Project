using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Wire : MonoBehaviour, IUtility, ILimitedUseEquippable
{
    public Sprite EquipSprite { get => _equipSprite; }
    public int EnergyCost { get; } = 20;
    public event System.Action<GameObject> OnLimitReached;

    [SerializeField] private Sprite _equipSprite;
    [SerializeField] private GameObject _wireObject;
    
    public void Activate()
    {
        ObjectSpawner.Instance.CmdSpawnNetworkObject(
            _wireObject.name, 
            transform.position, 
            _wireObject.transform.rotation * transform.rotation,
            NetworkClient.connection as NetworkConnectionToClient
        );
        
        OnLimitReached?.Invoke(gameObject);
    }
}
