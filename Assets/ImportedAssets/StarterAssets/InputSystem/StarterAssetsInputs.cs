using UnityEngine;
using System.Collections;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    public class StarterAssetsInputs : MonoBehaviour
    {
        [Header("Character Input Values")]
        public Vector2 move;
        public Vector2 look;

        // Various bools for incoming actions, i.e shoot, pickup food, etc. 
        public bool jump;
        public bool sprint;

        public bool primary;

        public bool secondary;
        public bool special;

        [Header("Movement Settings")]
        public bool analogMovement;

        [Header("Mouse Cursor Settings")]
        public bool cursorLocked = true;
        public bool cursorInputForLook = true;


        [Header("SFX")]
        public AudioSource walkSource;
        public AudioSource runSource;
        void Start()
        {
            // In order to keep the cursor visible during every scene reload. 
            SetCursorState(true);
        }

#if ENABLE_INPUT_SYSTEM

		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
      if(move != Vector2.zero){
        if(!walkSource.isPlaying && !sprint){
          walkSource.Play();
        }
        if(!runSource.isPlaying && sprint){

          StartCoroutine(FadeOut(walkSource, 0.2f));
          runSource.Play();
        }
        if(runSource.isPlaying && !sprint){
          StartCoroutine(FadeOut(runSource, 0.2f));
          walkSource.Play();
        }
      }else{
        StartCoroutine(FadeOut(walkSource, 0.3f));
        StartCoroutine(FadeOut(runSource, 0.3f));
      }
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{	
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

		public void OnPrimary(InputValue value)
        {	
			// Example, we retrieve an incoming value for the action (as a value argument)
			PrimaryInput(value.isPressed); 
			// True, but if it's a release for example, then the action may trigger but return a false bool (trigger on release)
        }

		public void OnSecondary(InputValue value)
        {
            SecondaryInput(value.isPressed); 
        }

		public void OnSpecial(InputValue value)
        {
            SpecialInput(value.isPressed); 
        }
#endif


        public void MoveInput(Vector2 newMoveDirection)
        {
            move = newMoveDirection;
        }

        public void LookInput(Vector2 newLookDirection)
        {
            look = newLookDirection;
        }

        public void JumpInput(bool newJumpState)
        {
            jump = newJumpState;
        }

        public void SprintInput(bool newSprintState)
        {
            sprint = newSprintState;
        }

        public void PrimaryInput(bool newPrimaryState)
        {
            // The function above, that trigger on associated action, now change the public bools for that action. 
            // That is, the action IS occuring OR is NOT. 
            primary = newPrimaryState;
        }

        public void SecondaryInput(bool newSecondaryState)
        {
            secondary = newSecondaryState;
        }


        public void SpecialInput(bool newSpecialState)
        {
            special = newSpecialState;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            SetCursorState(cursorLocked);
        }

        public void SetCursorState(bool newState)
        {
            // Make sure to fire this with FALSE, before a GameOver event. 
            // This is why it's a public method. 
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        }


        IEnumerator FadeOut(AudioSource source, float duration)
        {
            float startVolume = source.volume;

            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                source.volume = Mathf.Lerp(startVolume, 0f, t / duration);
                yield return null;
            }

            source.Stop();
            source.volume = startVolume; // reset for next time
        }

    }
}
