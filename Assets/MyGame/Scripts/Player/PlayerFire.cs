using UnityEngine;
using System.Collections;
namespace TAGame
{
	public class PlayerFire : MonoBehaviour
	{
		public Transform FirePoint;
		public GameObject bulletPrefabs; 
        private Projectile projectile;
        [Tooltip("fire projectile after this delay, useful to sync with the animation of firing action")]
		public float fireDelay;
		public float fireRate;
		public bool inverseDirection = false;

		[Header("Animation")]
		private Animator anim;
		private AudioSource soundFx;
		public AudioClip rangeAttackSound;
		[Range(0,1)]
		public float rangeAttackSoundVolume = 0.5f;

		float nextFire = 0;

		private void Awake()
		{
			anim = GetComponentInChildren<Animator>();
			soundFx = gameObject.AddComponent<AudioSource>();
		}
        private void Update()
        {
			if (Input.GetKeyDown(KeyCode.F))
				Fire();
        }
        private void Start()         //phuong thuc duoc them khi tao dan
		{
			//  BulletHolder.Instance.GetPickedBullets();
			//GameObject bullet = BulletHolder.Instance.bulletPicked;
		  // GameObject bullet = Instantiate(bulletPrefabs, FirePoint.position, Quaternion.identity);
           //Projectile = bullet.GetComponent<Projectile>();
		}
		public bool Fire()
		{
			//if (GameManager.Instance.Bullet>0 && Time.time > nextFire) {
			nextFire = Time.time + fireRate;
			//GameManager.Instance.Bullet--;
			StartCoroutine(DelayAttack(fireDelay));
			return true;
			//} else
			//return false;
		}

		IEnumerator DelayAttack(float time)
		{
			yield return new WaitForSeconds(time);

			var direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

			if (inverseDirection)
				direction *= -1;
            GameObject bullet = Instantiate(bulletPrefabs, FirePoint.position, Quaternion.identity);
            projectile = bullet.GetComponent<Projectile>();
			projectile.Initialize(gameObject, direction, Vector2.zero);

			// Play animation and sound
			if (anim != null)
				anim.SetTrigger("range_attack");
			
			if (soundFx != null && rangeAttackSound != null)
				soundFx.PlayOneShot(rangeAttackSound, rangeAttackSoundVolume);
		}
	}
}
