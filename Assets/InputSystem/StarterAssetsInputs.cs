using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif
namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public Vector2 mousePosition;
		public bool jump;
		public bool sprint;
		public bool interact;
		public bool atack;
		public bool throwAxe;
		public bool mightyPunch;
		public bool dash;
		public bool inventoryFirstSlot;
		public bool inventorySecondSlot;
		public bool pause;
		public bool isPaused;
		public bool groundSlam;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = false;
		public bool cursorInputForLook = true;

		private UseInteractor interactor;


#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED

		private void Start()
        {
            interactor = GetComponent<UseInteractor>();
        }

        public void OnMove(InputValue value)
		{
			if(!isPaused)
				MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnMousePosition(InputValue value)
		{
			MousePositionInput(value.Get<Vector2>());
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnInteract(InputValue value)
		{
			if (!isPaused)
				InteractInput(value.isPressed);
			interactor.TryInteract();
		}

		public void OnAtack(InputValue value)
		{
			if (!isPaused)
				AtackInput(value.isPressed);
		}

		public void OnThrowAxe(InputValue value)
		{
			if(TryGetComponent<AnimatorManager>(out AnimatorManager animatorManager)
				&& !animatorManager.GetMightyPunch() && !isPaused && !animatorManager.GetAtack())
				ThrowAxeInput(value.isPressed);
		}

		public void OnMightyPunch(InputValue value)
		{
			if(TryGetComponent<AnimatorManager>(out AnimatorManager animatorManager)
				&& animatorManager.isGrounded() && !isPaused)
			MightyPunchInput(value.isPressed);
		}

		public void OnGroundSlam(InputValue value)
        {
			GroundSlamInput(value.isPressed);
        }

		public void OnInventoryFirstSlot(InputValue value)
		{
			if (!isPaused)
				InventoryFirstSlotInput(value.isPressed);
		}		
		
		public void OnInventorySecondSlot(InputValue value)
		{
			if (!isPaused)
				InventorySecondSlotInput(value.isPressed);
		}		
		
		public void OnPause(InputValue value)
		{
			PauseInput(value.isPressed);
		}

		public void OnDash(InputValue value)
		{
			if (!isPaused)
				DashInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
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
		public void MousePositionInput(Vector2 newMousePosition)
		{
			mousePosition = newMousePosition;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void InteractInput(bool newInteractState)
		{
			interact = newInteractState;
		}

		public void AtackInput(bool newAtackState)
		{
			atack = newAtackState;
		}

		public void ThrowAxeInput(bool newThrowAxeInputState)
		{
			throwAxe = newThrowAxeInputState;
		}

		public void MightyPunchInput(bool newMightyPunchInputState)
		{
			mightyPunch = newMightyPunchInputState;
		}

		public void GroundSlamInput(bool newGroundSlamInputState)
		{
			groundSlam = newGroundSlamInputState;
		}

		public void InventoryFirstSlotInput(bool newInventoryFirstSlotInputState)
		{
			inventoryFirstSlot = newInventoryFirstSlotInputState;
		}

		public void InventorySecondSlotInput(bool newInventorySecondSlotInputState)
		{
			inventorySecondSlot = newInventorySecondSlotInputState;
		}		
		
		public void PauseInput(bool pauseInputState)
		{
			pause = pauseInputState;
		}

		public void DashInput(bool newDashState)
		{
			dash = newDashState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
}