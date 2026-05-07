using UnityEngine;
using System.Collections;

public class BonusIcon : MonoBehaviour
{
    [SerializeField] private float boostMultiplier = 2f;
    [SerializeField] private float boostDuration = 10f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.gameObject == gameObject)
            {
                StartCoroutine(ApplyBoost());
                Destroy(gameObject);
            }
        }
    }

    IEnumerator ApplyBoost()
    {
        GameManager.Instance.bonusDamageMultiplier = boostMultiplier;
        yield return new WaitForSeconds(boostDuration);
        GameManager.Instance.bonusDamageMultiplier = 1f;
    }
}