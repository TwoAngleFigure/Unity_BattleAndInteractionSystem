public enum EffectType
{
    None,
    Healing,
    Slow
}

public static class EffectMapper
{
    public static BaseEffect GetEffect(EffectData data)
    {
        switch (data.type)
        {
            case EffectType.Healing:
                return new HealingEffect { value = (int)data.value };
            case EffectType.Slow:
                return new SlowEffect { value = data.value };
            default:
                return null;
        }
    }
}
