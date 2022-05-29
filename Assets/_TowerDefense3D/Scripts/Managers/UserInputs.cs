using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace TowerDefense3D
{
    public class UserInputs : MonoBehaviour
    {
        public static InGameControls inputData;

        public static UnityEvent<InputAction.CallbackContext> OnPerformActionInputEvent = new UnityEvent<InputAction.CallbackContext>();
        public static UnityEvent<InputAction.CallbackContext> OnTogglePauseInputEvent = new UnityEvent<InputAction.CallbackContext>();
        public static UnityEvent<InputAction.CallbackContext> OnCancelSelectionInputEvent = new UnityEvent<InputAction.CallbackContext>();
        public static UnityEvent<InputAction.CallbackContext> OnSecondaryTouchInputEvent = new UnityEvent<InputAction.CallbackContext>();

        public void OnMousePosition(InputAction.CallbackContext context)
        {
            MousePositionInput(context.action.ReadValue<Vector2>());
        }

        public void OnMiddleMouseButtonHold(InputAction.CallbackContext context)
        {
            if (context.action.phase == InputActionPhase.Canceled)
                MiddleMouseHoldInput(false);
            if (context.action.phase == InputActionPhase.Performed)
                MiddleMouseHoldInput(true);
        }

        public void OnMouseRightClickHold(InputAction.CallbackContext context)
        {
            if (context.action.phase == InputActionPhase.Canceled)
                RightClickHoldInput(false);
            if (context.action.phase == InputActionPhase.Performed)
                RightClickHoldInput(true);
        }

        public void OnMoveCameraSecondary(InputAction.CallbackContext context)
        {
            MoveCameraInputSecondary(context.action.ReadValue<Vector2>());
        }

        public void OnRotateCameraSecondary(InputAction.CallbackContext context)
        {
            RotateCameraInputSecondary(context.action.ReadValue<Vector2>());
        }

        public void OnZoomCamera(InputAction.CallbackContext context)
        {
            ZoomCameraInput(Mathf.Clamp(context.action.ReadValue<float>(), -1, 1));
        }

        public void OnTogglePause(InputAction.CallbackContext context)
        {
            OnTogglePauseInputEvent?.Invoke(context);
        }

        public void OnPerformAction(InputAction.CallbackContext context)
        {
            OnPerformActionInputEvent?.Invoke(context);
        }

        public void OnCancelSelection(InputAction.CallbackContext context)
        {
            OnCancelSelectionInputEvent?.Invoke(context);
        }

        public void OnPrimaryTouchPosition(InputAction.CallbackContext context)
        {
            PrimaryTouchInputPosition(context.action.ReadValue<Vector2>());
        }

        public void OnSecondaryTouchPosition(InputAction.CallbackContext context)
        {
            SecondaryTouchInputPosition(context.action.ReadValue<Vector2>());
        }

        public void OnSecondaryTouchActive(InputAction.CallbackContext context)
        {
            OnSecondaryTouchInputEvent?.Invoke(context);
        }

        private void MiddleMouseHoldInput(bool input)
        {
            inputData.middleMouseButtonHold = input;
        }

        private void RightClickHoldInput(bool input)
        {
            inputData.mouseRightClickHold = input;
        }

        private void MousePositionInput(Vector2 input)
        {
            inputData.mousePosition = input;
            //inputData.moveCameraPrimary = inputData.middleMouseHold ? input : Vector2.zero;
            inputData.moveCameraPrimary = input;
            //inputData.rotateCameraPrimary = inputData.rightClickHold ? input : Vector2.zero;
            inputData.rotateCameraPrimary = input;
        }

        private void PrimaryTouchInputPosition(Vector2 input)
        {
            inputData.primaryTouchPosition = input;
        }

        private void SecondaryTouchInputPosition(Vector2 input)
        {
            inputData.secondaryTouchPosition = input;
        }

        private void MoveCameraInputSecondary(Vector2 input)
        {
            inputData.moveCameraSecondary = input;
        }

        private void RotateCameraInputSecondary(Vector2 input)
        {
            inputData.rotateCameraSecondary = input;
        }

        private void ZoomCameraInput(float input)
        {
            inputData.zoomCamera = input;
        }

        private void TogglePauseInput(bool input)
        {
            inputData.togglePause = input;
        }

        private void PerformActionInput(bool input)
        {
            inputData.performAction = input;
        }

        private void PerformSecondaryTouchInput(bool input)
        {
            inputData.isSecondaryTouchActive = input;
        }
    }

    [System.Serializable]
    public struct InGameControls
    {
        public Vector2 mousePosition;
        public bool middleMouseButtonHold;
        public bool mouseRightClickHold;
        public Vector2 moveCameraPrimary;
        public Vector2 rotateCameraPrimary;
        public Vector2 moveCameraSecondary;
        public Vector2 rotateCameraSecondary;
        public float zoomCamera;
        public bool togglePause;
        public bool performAction;
        public Vector2 primaryTouchPosition;
        public Vector2 secondaryTouchPosition;
        public bool isSecondaryTouchActive;
    }
}
