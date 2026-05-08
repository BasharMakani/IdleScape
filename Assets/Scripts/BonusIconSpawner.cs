using UnityEngine;
using System.Collections;

public class BonusIconSpawner : MonoBehaviour
{
    [SerializeField] private GameObject bonusIconPrefab;

    [SerializeField] private float spawnCheckInterval = .1f;
    [SerializeField] private float spawnChance = 0.8f;

    [Header("Spawn Area")]
    [SerializeField] private float leftScreenPadding = 50f;
    [SerializeField] private float rightScreenPadding = 50f;
    [SerializeField] private float topUIPadding = 130f;
    [SerializeField] private float bottomUIPadding = 170f;
    [SerializeField] private float distanceFromCamera = 10f;

    void Start()
    {
        StartCoroutine(SpawnBonusIcon());
    }

    IEnumerator SpawnBonusIcon()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnCheckInterval);

            if (Random.value <= spawnChance)
            {
                Vector3 spawnPosition = GetRandomScreenSpawnPosition();
                Instantiate(bonusIconPrefab, spawnPosition, Quaternion.identity);
            }
        }
    }

    Vector3 GetRandomScreenSpawnPosition()
    {
        float randomX = Random.Range(leftScreenPadding, Screen.width - rightScreenPadding);
        float randomY = Random.Range(bottomUIPadding, Screen.height - topUIPadding);

        Vector3 screenPosition = new Vector3(randomX, randomY, distanceFromCamera);
        return Camera.main.ScreenToWorldPoint(screenPosition);
    }
}