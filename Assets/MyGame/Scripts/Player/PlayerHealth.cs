using UnityEngine;
using System.Collections;
namespace TAGame
{
	public class PlayerHealth : MonoBehaviour, ICanTakeDamage
	{
		public bool GodMode;
		[Header("Health")]
		public int maxHealth;
		public int Health { get; private set; }
		public GameObject HurtEffect;

		[Header("Sound")]
		public AudioClip hurtSound;
		[Range(0, 1)]
		public float hurtSoundVolume = 0.5f;
		public AudioClip deadSound;
		[Range(0, 1)]
		public float deadSoundVolume = 0.5f;

		public bool isPlaying { get; private set; }
		private Animator anim;
		private Controller2D controller;

		void Start()
		{
			Health = maxHealth;
			isPlaying = true;
			anim = GetComponent<Animator>();
			controller = GetComponent<Controller2D>();
		}

		public void TakeDamage(float damage, Vector2 force, GameObject instigator)
		{
			if (!isPlaying)
				return;

			if (HurtEffect != null)
				Instantiate(HurtEffect, instigator.transform.position, Quaternion.identity);

			if (GodMode)
				return;

			Health -= (int)damage;

			if (Health <= 0)
			{
				Kill();
				return;
			}

			//set force to player
			if (force.x != 0 || force.y != 0 && controller != null)
			{
				var facingDirectionX = Mathf.Sign(transform.position.x - instigator.transform.position.x);
				var facingDirectionY = Mathf.Sign(GetComponent<Rigidbody2D>()?.linearVelocity.y ?? 0);

				GetComponent<Rigidbody2D>()?.AddForce(new Vector2(
					Mathf.Clamp(Mathf.Abs(force.x), 10, 15) * facingDirectionX,
					Mathf.Clamp(Mathf.Abs(force.y), 5, 15) * facingDirectionY * -1
				));
			}
		}

		public void GiveHealth(int hearthToGive, GameObject instigator)
		{
			Health = Mathf.Min(Health + hearthToGive, maxHealth);
		}

		public void Kill()
		{
			if (isPlaying)
			{
				isPlaying = false;
				if (anim != null)
					anim.SetTrigger("dead");
				if (controller != null)
					controller.HandlePhysic = false;
				Health = 0;
			}
		}

		public void SetPlayingState(bool state)
		{
			isPlaying = state;
		}

		public void ResetHealth()
		{
			Health = maxHealth;
		}
	}
}