using UnityEngine;
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

		// Various bools for incoming actions, i.e shoot etc. 
		public bool jump;
		public bool sprint;

		public bool shoot;

		public bool bite; 
		public bool zoom; 

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

		void Start()
        {	
			// In order to keep the cursor visible during every scene reload. 
            SetCursorState(true);
        }

#if ENABLE_INPUT_SYSTEM

		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
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

		public void OnShoot(InputValue value)
        {	
			// Example, we retrieve an incoming value for the action (as a value argument)
			ShootInput(value.isPressed); 
			// True, but if it's a release for example, then the action may trigger but return a false bool (trigger on release)
        }

		public void OnBite(InputValue value)
        {
            BiteInput(value.isPressed); 
        }

		public void OnZoom(InputValue value)
        {
            ZoomInput(value.isPressed); 
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

		public void ShootInput(bool newShootState)
        {	
			// The function above, that trigger on associated action, now change the public bools for that action. 
			// That is, the action IS occuring OR is NOT. 
            shoot = newShootState; 
        }

		public void BiteInput(bool newBiteState)
        {
            bite = newBiteState; 
        }
		

		public void ZoomInput(bool newZoomState)
        {
            zoom = newZoomState; 
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
	}
	
}