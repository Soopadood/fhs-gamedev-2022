using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaterLevel : MonoBehaviour
{
    [SerializeField] private float _waterAmount = 100;
    [HideInInspector] public float WaterAmount { get { return _waterAmount; } set { _waterAmount = value; } }
    [SerializeField] private float _dryingRate = 3.5f;
    [SerializeField] private float _iFrameTime = 1.5f;
    [SerializeField] private bool _isInvincible = false; //I-frames
    [HideInInspector] public bool IsInvincible { get { return _isInvincible; } set { _isInvincible = value; } }
    [SerializeField] Slider _healthBarSlider;
    [SerializeField] Image _indicatorUi;
    [SerializeField] List<Sprite> _dropletSprites;
    [SerializeField] ParticleSystem _splashParticles;
    [SerializeField] GameObject _spriteRenderer;
    
    private float _maxWaterAmount;
    private bool _isInLake = false;

    private void Start()
    {
        _maxWaterAmount = _waterAmount;
    }

    private void Update()
    {
        Mathf.Clamp(_waterAmount, 0, 100); //0 is the min, right?
        if (_waterAmount >= 0)
            _waterAmount -= _dryingRate * Time.deltaTime; //Decrease water level by drying rate
        _healthBarSlider.value = _waterAmount / _maxWaterAmount;

        var dropletSpriteIndex = Mathf.Clamp(_dropletSprites.Count - (int)(_waterAmount / _maxWaterAmount * _dropletSprites.Count) - 1, 0, _dropletSprites.Count - 1);
        _indicatorUi.sprite = _dropletSprites[dropletSpriteIndex];

        if (_isInLake)
            _waterAmount += Time.deltaTime * 30f;

        _spriteRenderer.SetActive(!(_isInvincible && _spriteRenderer.activeSelf));
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponentInParent<WaterDroplet>() != null) //Check if the collider's parent has WaterDroplet script(droplet detection)
        {
            WaterDroplet dropletScript = collision.GetComponentInParent<WaterDroplet>();
            _waterAmount += dropletScript.fillAmount;//increse water level by droplet's fillamount
            collision.gameObject.SetActive(false); //set just FX and collider of droplet inactive(not parent w/ script_
        }
        if (collision.GetComponent<Lake>() != null)
            _isInLake = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Lake>() != null)
            _isInLake = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Enemy":
                DecreaseWaterLevel(10);
                break;
            case "Respawn":
                _waterAmount = 0;
                break;
        }
    }

    public void DecreaseWaterLevel(float damage)
    {
        if (!_isInvincible)
        {
            StartCoroutine(IFrameDelay());
            _waterAmount -= damage;
            _splashParticles.Emit(5);
        }
    }

    IEnumerator IFrameDelay()
    {
        _isInvincible = true;
        yield return new WaitForSeconds(_iFrameTime);
        _isInvincible = false;
    }
}
