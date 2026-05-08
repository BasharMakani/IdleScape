using UnityEngine;
using System.Collections;

public class BonusIcon : MonoBehaviour
{
    [SerializeField] private float boostMultiplier = 2f;
    [SerializeField] private float boostDuration = 10f;
    [SerializeField] private float iconLifetime = 1f;

    [Header("Miss Penalty")]
    [SerializeField] private int healthGainOnMiss = 1;

    private bool wasClicked = false;

    void Start()
    {
        StartCoroutine(DisappearAfterTime());
    }

    void Update()
    {
        Vector2 inputPos = Vector2.zero;
        bool inputDown = false;

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            inputPos = Input.GetTouch(0).position;
            inputDown = true;
        }
        else if (Input.GetMouseButtonDown(0))
        {
            inputPos = Input.mousePosition;
            inputDown = true;
        }

        if (inputDown)
        {
            Ray ray = Camera.main.ScreenPointToRay(inputPos);

            if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.gameObject == gameObject)
            {
                if (!wasClicked)
                {
                    wasClicked = true;
                    StartCoroutine(ApplyBoost());
                }
            }
        }
    }

    IEnumerator DisappearAfterTime()
    {
        yield return new WaitForSeconds(iconLifetime);

        if (!wasClicked)
        {
            ResourceNodeManager.Instance.HealCurrentNode(healthGainOnMiss);
            Destroy(gameObject);
        }
    }

    IEnumerator ApplyBoost()
    {
        GameManager.Instance.bonusDamageMultiplier = boostMultiplier;

        GetComponent<Collider>().enabled = false;

        if (GetComponent<Renderer>() != null)
        {
            GetComponent<Renderer>().enabled = false;
        }

        yield return new WaitForSeconds(boostDuration);

        GameManager.Instance.bonusDamageMultiplier = 1f;

        Destroy(gameObject);
    }
}