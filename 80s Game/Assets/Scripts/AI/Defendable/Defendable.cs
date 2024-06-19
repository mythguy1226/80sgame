using System.Collections.Generic;
using UnityEngine;

public class Defendable : MonoBehaviour
{

    [SerializeField]
    int _maxHitpoints;

    [Tooltip("Hp Thresholds to switch to the next sprite. Add these in decreasing order")]
    [SerializeField]
    List<float> hpThresholds;

    [SerializeField]
    List<Sprite> damageLevelSprites;

    int _currentHitpoints;
    int _thresholdIndex;

    List<LatchPoint> latchPoints;
    SpriteRenderer sr;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        latchPoints = new List<LatchPoint>();
        foreach(Transform child in transform)
        {
            latchPoints.Add(child.GetComponent<LatchPoint>());
        }
        _thresholdIndex = 0;
        _currentHitpoints = _maxHitpoints;
    }

    public void TakeDamage(int damage)
    {
        _currentHitpoints -= damage;
        for(int i = _thresholdIndex + 1;  i < hpThresholds.Count; i++)
        {
            if (_currentHitpoints < hpThresholds[i])
            {
                _thresholdIndex = i;
                UpdateSprite();
            }
        }
    }

    private void UpdateSprite()
    {
        sr.sprite = damageLevelSprites[_thresholdIndex];
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Filter out non-enemies
        if (!collision.gameObject.CompareTag("Enemy"))
        {
            return;
        }

        List<LatchPoint> openLatches = new List<LatchPoint>();
        // Get a random latch point
        foreach(LatchPoint latch in latchPoints)
        {
            if (latch.isAvailable)
            {
                openLatches.Add(latch);
            }
        }


        // No open latches available
        if (openLatches.Count == 0)
        {
            return;
        }

        int offerLatch = Random.Range(0, openLatches.Count);
        openLatches[offerLatch].LatchTarget(collision.gameObject.GetComponent<Target>());
    }
}