using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Controller2D))]
[RequireComponent (typeof (PlayerMovement))]
[RequireComponent (typeof (PlayerHealth))]
public class Player : MonoBehaviour {

	[Header("Attack")]
	public AudioClip rangeAttackSound;
	[Range(0,1)]
	public float rangeAttackSoundVolume = 0.5f;
	public AudioClip meleeAttackSound;
	[Range(0,1)]
	public float meleeAttackSoundVolume = 0.5f;

	[Header("Option")]
	public bool allowMeleeAttack;
	public bool allowRangeAttack;

	protected RangeAttack rangeAttack;
	protected MeleeAttack meleeAttack;

	private PlayerMovement playerMovement;
	private PlayerHealth playerHealth;

	void Awake(){
		playerMovement = GetComponent<PlayerMovement> ();
		playerHealth = GetComponent<PlayerHealth> ();
	}

	void Start() {
		rangeAttack = GetComponent<RangeAttack> ();
		meleeAttack = GetComponent<MeleeAttack> ();
	}

	void Update() {
		HandleAttackInput();
	}

	private void HandleAttackInput(){
		if (Input.GetKeyDown (KeyCode.F))
			RangeAttack ();

		if (Input.GetKeyDown (KeyCode.X))
			MeleeAttack ();
	}

	public void MeleeAttack(){
		if (!playerMovement.isPlaying)
			return;
		
		if (allowMeleeAttack && meleeAttack!=null) {
			if (meleeAttack.Attack ()) {
				playerMovement.anim.SetTrigger ("melee_attack");
				SoundManager.PlaySfx (meleeAttackSound, meleeAttackSoundVolume);
			}
		}
	}


	public void RangeAttack(){
		if (!playerMovement.isPlaying)
			return;
		
		if (allowRangeAttack && rangeAttack!=null) {

			if (rangeAttack.Fire ()) {
				playerMovement.anim.SetTrigger ("range_attack");
				SoundManager.PlaySfx (rangeAttackSound, rangeAttackSoundVolume);
			}
		}
	}


	public void RespawnAt(Vector2 pos){
		playerMovement.RespawnAt(pos);
		playerHealth.ResetHealth();
		playerHealth.SetPlayingState(true);
	}

	public void GameFinish(){
		playerMovement.GameFinish ();
		playerHealth.SetPlayingState(false);
	}
} 