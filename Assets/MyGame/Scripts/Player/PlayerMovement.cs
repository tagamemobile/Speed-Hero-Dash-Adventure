using UnityEngine;
using System.Collections;
namespace TAGame
{
	[RequireComponent(typeof(Controller2D))]
	public class PlayerMovement : MonoBehaviour
	{
		[Header("Moving")]
		public float moveSpeed = 3;
		float accelerationTimeAirborne = .2f;
		float accelerationTimeGrounded = .1f;

		[Header("Jump")]
		public float maxJumpHeight = 3;
		public float minJumpHeight = 1;
		public float timeToJumpApex = .4f;
		public int numberOfJumpMax = 1;
		int numberOfJumpLeft;
		public GameObject JumpEffect;

		[Header("Wall Slide")]
		public Vector2 wallJumpClimb;
		public Vector2 wallJumpOff;
		public Vector2 wallLeap;

		public float wallSlideSpeedMax = 3;
		public float wallStickTime = .25f;
		float timeToWallUnstick;

		[Header("Sound")]
		public AudioClip jumpSound;
		[Range(0, 1)]
		public float jumpSoundVolume = 0.5f;
		public AudioClip landSound;
		[Range(0, 1)]
		public float landSoundVolume = 0.5f;
		public AudioClip wallSlideSound;
		[Range(0, 1)]
		public float wallSlideSoundVolume = 0.5f;

		bool isPlayedLandSound;

		[Header("Option")]
		public bool allowSlideWall;

		private AudioSource soundFx;

		float gravity;
		float maxJumpVelocity;
		float minJumpVelocity;
		[HideInInspector]
		public Vector3 velocity;
		float velocityXSmoothing;

		bool isFacingRight;
		bool wallSliding;
		int wallDirX;

		public Vector2 input;

		[HideInInspector]
		public Controller2D controller;
		Animator anim;

		public bool isPlaying { get; private set; }
		public bool isFinish { get; set; }

		private bool isJumping;

		void Awake()
		{
			controller = GetComponent<Controller2D>();
			anim = GetComponentInChildren<Animator>();
		}

		void Start()
		{
			gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
			maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
			minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
			isFacingRight = transform.localScale.x > 0;
			numberOfJumpLeft = numberOfJumpMax;
			isPlaying = true;
			isJumping = false;

			soundFx = gameObject.AddComponent<AudioSource>();
			soundFx.loop = true;
			soundFx.playOnAwake = false;
			soundFx.clip = wallSlideSound;
			soundFx.volume = wallSlideSoundVolume;
		}

		void Update()
		{
			HandleInput();
			HandleAnimation();

			wallDirX = (controller.collisions.left) ? -1 : 1;

			float targetVelocityX = input.x * moveSpeed;
			velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);

			wallSliding = false;
			if (allowSlideWall && (controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0)
			{
				wallSliding = true;
				if (!soundFx.isPlaying)
					soundFx.Play();

				if (velocity.y < -wallSlideSpeedMax)
				{
					velocity.y = -wallSlideSpeedMax;
				}

				if (timeToWallUnstick > 0)
				{
					velocityXSmoothing = 0;
					velocity.x = 0;

					if (input.x != wallDirX && input.x != 0)
					{
						timeToWallUnstick -= Time.deltaTime;
					}
					else
					{
						timeToWallUnstick = wallStickTime;
					}
				}
				else
				{
					timeToWallUnstick = wallStickTime;
				}
			}
			else
			{
				if (soundFx.isPlaying)
					soundFx.Stop();
			}

			velocity.y += gravity * Time.deltaTime;

			if (controller.collisions.below && !isPlayedLandSound)
			{
				isPlayedLandSound = true;
				SoundManager.PlaySfx(landSound, landSoundVolume);
			}
			else if (!controller.collisions.below && isPlayedLandSound)
				isPlayedLandSound = false;
		}

		void LateUpdate()
		{
			controller.Move(velocity * Time.deltaTime, input);

			if (controller.collisions.above || controller.collisions.below)
			{
				velocity.y = 0;
			}
		}

		private void HandleInput()
		{
			if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
				MoveLeft();
			else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
				MoveRight();
			else
				StopMove();

			if (Input.GetKeyDown(KeyCode.S))
				FallDown();
			else if (Input.GetKeyUp(KeyCode.S))
				StopMove();

			if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow))
			{
				Jump();
			}

			if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.UpArrow))
			{
				JumpOff();
			}
		}

		private void HandleAnimation()
		{
			if (anim == null) return;

			anim.SetFloat("speed", Mathf.Abs(input.x));
			anim.SetFloat("height_speed", velocity.y);
			anim.SetBool("isGrounded", controller.collisions.below);
			anim.SetBool("isWall", wallSliding);
			anim.SetBool("isJumping", isJumping);
		}

		private void Flip()
		{
			transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
			isFacingRight = transform.localScale.x > 0;
		}

		public void MoveLeft()
		{
			if (isPlaying)
			{
				input = new Vector2(-1, 0);
				if (isFacingRight)
					Flip();
			}
		}

		public void MoveRight()
		{
			if (isPlaying)
			{
				input = new Vector2(1, 0);
				if (!isFacingRight)
					Flip();
			}
		}

		public void StopMove()
		{
			input = Vector2.zero;
		}

		public void FallDown()
		{
			input = new Vector2(0, -1);
		}

		public void Jump()
		{
			if (!isPlaying)
				return;

			isJumping = true;
			if (wallSliding)
			{
				if (wallDirX == input.x)
				{
					velocity.x = -wallDirX * wallJumpClimb.x;
					velocity.y = wallJumpClimb.y;
				}
				else if (input.x == 0)
				{
					velocity.x = -wallDirX * wallJumpOff.x;
					velocity.y = wallJumpOff.y;
					Flip();
				}
				else
				{
					velocity.x = -wallDirX * wallLeap.x;
					velocity.y = wallLeap.y;
					Flip();
				}
				SoundManager.PlaySfx(jumpSound, jumpSoundVolume);
			}
			else if (controller.collisions.below)
			{
				velocity.y = maxJumpVelocity;

				if (JumpEffect != null)
					Instantiate(JumpEffect, transform.position, transform.rotation);
				SoundManager.PlaySfx(jumpSound, jumpSoundVolume);
				numberOfJumpLeft = numberOfJumpMax;
			}
			else
			{
				numberOfJumpLeft--;
				if (numberOfJumpLeft > 0)
				{
					velocity.y = minJumpVelocity;

					if (JumpEffect != null)
						Instantiate(JumpEffect, transform.position, transform.rotation);
					SoundManager.PlaySfx(jumpSound, jumpSoundVolume);
				}
			}
		}

		public void JumpOff()
		{
			isJumping = false;
			if (velocity.y > minJumpVelocity)
			{
				velocity.y = minJumpVelocity;
			}
		}

		public void SetForce(Vector2 force)
		{
			velocity = (Vector3)force;
		}

		public void AddForce(Vector2 force)
		{
			velocity += (Vector3)force;
		}

		public void RespawnAt(Vector2 pos)
		{
			transform.position = pos;
			isPlaying = true;

			ResetAnimation();

			var boxCo = GetComponents<BoxCollider2D>();
			foreach (var box in boxCo)
			{
				box.enabled = true;
			}
			var CirCo = GetComponents<CircleCollider2D>();
			foreach (var cir in CirCo)
			{
				cir.enabled = true;
			}

			controller.HandlePhysic = true;
		}

		void ResetAnimation()
		{
			if (anim == null) return;

			anim.SetFloat("speed", 0);
			anim.SetFloat("height_speed", 0);
			anim.SetBool("isGrounded", true);
			anim.SetBool("isWall", false);
			anim.SetTrigger("reset");
		}

		public void GameFinish()
		{
			StopMove();
			isPlaying = false;
			if (anim != null)
				anim.SetTrigger("finish");
		}
	}
}