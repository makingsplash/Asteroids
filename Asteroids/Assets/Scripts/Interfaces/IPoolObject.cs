interface IPoolObject
{
    ObjectPool ParentPool { get; set; }
    void ReturnToPool();
}
