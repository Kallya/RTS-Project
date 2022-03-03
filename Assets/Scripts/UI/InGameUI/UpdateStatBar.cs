using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateStatBar : MonoBehaviour
{
    [SerializeField] private Transform _statBarSprite;

    private Quaternion _initRot;
    private Vector3 _initPos;

    private void Awake()
    {
        _initRot = transform.rotation;
    }

    private void Start()
    {
        SubscribeToStatChange();
    }
    
    public virtual void SubscribeToStatChange()
    {
        // subscribe to stat change event here
    }

    // stop hp bar from rotating
    private void LateUpdate()
    {
        transform.rotation = _initRot;
    }

    public void StatChanged(Stat stat)
    {
        float healthPortion = (float)stat.Value/stat.BaseValue;
        _statBarSprite.localScale = new Vector3(1 * healthPortion, 1f, 1f);
        _statBarSprite.localPosition = new Vector3((healthPortion - 1) / 2, 0f, 0f); // adjust position so hp bar reduces to left
    }
}
