# InstancedDanmaku

GPU Instanced Bullet System for Unity

![](https://cdn-ak.f.st-hatena.com/images/fotolife/F/FriendSea/20251110/20251110224115.gif)

Used in [MoriyaRhythm](https://store.steampowered.com/app/4127490/_/)

## Collision Targets

Collider and a Component inheriting IBulletCollider is required to detect bullet collision.

```cs
public class BulletCollisionTarget : MonoBehaviour, InstancedDanmaku.IBulletCollider
{
	public bool DeleteBullet => true;

	public void Collide(Bullet bullet)
	{
		// Collision!
	}
}

```

## Custom Bullet

Inherit IBulletBehaviour to make new behaviours for bullets.
We can make more complex behaviours by writing new update method.

```cs
public class SetVelocity : IBulletBehaviour
{
    public void UpdateBullet(ref Bullet bullet)
    {
        //update a bullet
    }
}
```
