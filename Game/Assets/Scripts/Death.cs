using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Death : MonoBehaviour
{
    private WaterLevel waterLevel;
    [SerializeField] GameObject DeathScreen;
    [SerializeField] TextMeshProUGUI DeathTextUI;
    [SerializeField] string[] DeathTextList;
    public GameObject checkpoint;

    void Start()
    {
        waterLevel = GetComponent<WaterLevel>();
    }

    void Update()
    {
       if (waterLevel.WaterAmount <= 0)
        {
            DeathScreen.SetActive(true);
            Time.timeScale = 0;
        }
        if (Input.GetKeyDown(KeyCode.E) && DeathScreen.activeSelf)
        {
            transform.position = checkpoint.transform.position;
            DeathScreen.SetActive(false);
            DeathTextUI.text = DeathTextList[ Random.Range(0, DeathTextList.Length)];
            Time.timeScale = 1;
            waterLevel.WaterAmount = 100;
            waterLevel.IsInvincible = false;
        }
    }
}
