using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChecker : MonoBehaviour
{
    [SerializeField] private BoxCollider areaToCheck;
    [SerializeField] private bool playerPresent;
    private bool lastPlayerPresent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        playerPresent = false;

        foreach (Collider col in Physics.OverlapBox(
            areaToCheck.transform.position + (areaToCheck.center * areaToCheck.transform.lossyScale.x),
            new Vector3(
                (areaToCheck.size.x * areaToCheck.transform.lossyScale.x) / 2,
                (areaToCheck.size.y * areaToCheck.transform.lossyScale.y) / 2,
                (areaToCheck.size.z * areaToCheck.transform.lossyScale.z) / 2
            )))
        {
            if (col.tag == "PlayerTag")
                playerPresent = true;
        }

        if (!lastPlayerPresent && playerPresent)
            PlayerEnter();

        if (lastPlayerPresent && !playerPresent)
            PlayerExit();

        lastPlayerPresent = playerPresent;
    }

    private void PlayerEnter()
    {
        GetComponent<DynamicTimer>().StartTimer();
    }

    private void PlayerExit()
    {
        GetComponent<DynamicTimer>().StopTimer();
    }
}
