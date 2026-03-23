public interface IHitable
{
    public EntityState State();

    public int MaxHp { get; }

    public int Hp { get; set; }

    public void OnHit(int damage);
}

public enum EntityState
{
    Alive,
    Hit,
    Attack,
    Air,
    Dead,
}