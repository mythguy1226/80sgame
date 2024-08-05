public class Overheat
{
    private float _maxHeat;
    private float _maxOverheat;
    private float _currentHeat;
    private float _heatLossFactor;
    private float _originalHeadAddFactor;
    private float _heatAddFactor;
    
    public Overheat(float maxHeat, float maxOverheat, float lossFactor, float addFactor)
    {
        _maxHeat = maxHeat;
        _maxOverheat = maxOverheat;
        _currentHeat = 0;
        _heatLossFactor = lossFactor;
        _originalHeadAddFactor = addFactor;
        _heatAddFactor = addFactor;
    }

    public float AddHeat()
    {
        _currentHeat += _heatAddFactor;
        if (_currentHeat > _maxOverheat)
        {
            _currentHeat = _maxOverheat;
        }
        return _currentHeat;
    }

    public void OverchargeHeatAdd()
    {
        _heatAddFactor = _originalHeadAddFactor*2;
    }

    public void ResetHeadAdd()
    {
        _heatAddFactor -= _originalHeadAddFactor;
    }

    public float ReduceHeat(float timeDelta)
    {
        _currentHeat -= timeDelta * _heatLossFactor;
        if ( _currentHeat < 0)
        {
            _currentHeat = 0;
        }
        return _currentHeat;
    }

    public bool CanShoot() { 
        return _currentHeat <= _maxHeat;
    }

    public float GetHeatProportion()
    {
        return _currentHeat / _maxHeat;
    }
}