public class Overheat
{
    private float _maxHeat;
    private float _maxOverheat;
    private float _currentHeat;
    private float _heatLossFactor;
    private float _originalHeadAddFactor;
    private float _heatAddFactor;
    private float _emergencyVentFactor;
    private bool _bIsVenting;
    
    public Overheat(float maxHeat, float maxOverheat, float lossFactor, float addFactor)
    {
        _maxHeat = maxHeat;
        _maxOverheat = maxOverheat;
        _currentHeat = 0;
        _heatLossFactor = lossFactor;
        _emergencyVentFactor = 2 * _heatLossFactor;
        _originalHeadAddFactor = addFactor;
        _heatAddFactor = addFactor;
        _bIsVenting = false;
    }

    public float AddHeat()
    {
        _currentHeat += _heatAddFactor;
        if (_currentHeat > _maxOverheat)
        {
            _currentHeat = _maxOverheat;
        }
        if (_currentHeat >= _maxHeat)
        {
            _bIsVenting = true;
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
        float factorToUse = _heatLossFactor;
        if ( _bIsVenting)
        {
            factorToUse = _emergencyVentFactor;
        }
        _currentHeat -= timeDelta * factorToUse;
        if ( _currentHeat < 0)
        {
            _currentHeat = 0;
            _bIsVenting = false;
        }
        return _currentHeat;
    }

    public bool CanShoot() { 
        return _currentHeat <= _maxHeat && !_bIsVenting;
    }

    public bool IsVenting()
    {
        return _bIsVenting;
    }

    public float GetHeatProportion()
    {
        return _currentHeat / _maxHeat;
    }
}