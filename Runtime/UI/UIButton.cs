using System;
using CollieMollie.Editor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CollieMollie.UI
{
    public class UIButton : BaseUI
    {
        #region Variable Field
        public event Action<UIEventArgs> OnDefault = null;
        public event Action<UIEventArgs> OnHovered = null;
        public event Action<UIEventArgs> OnPressed = null;
        public event Action<UIEventArgs> OnSelected = null;
        public event Action<UIEventArgs> OnDisabled = null;

        [Header("Button")]
        [SerializeField] private ButtonType _type = ButtonType.Button;
        [SerializeField] private UIColorFeature _colorFeature = null;
        [SerializeField] private UIAudioFeature _audioFeature = null;

        [SerializeField] private bool _interactable = true;
        public bool IsInteractable
        {
            get => _interactable;
        }

        [ReadOnly]
        [SerializeField] private bool _hovering = false;
        public bool IsHovering
        {
            get => _hovering;
        }

        [ReadOnly]
        [SerializeField] private bool _pressed = false;
        public bool IsPressed
        {
            get => _pressed;
        }

        [ReadOnly]
        [SerializeField] private bool _selected = false;
        public bool IsSelected
        {
            get => _selected;
        }

        [ReadOnly]
        [SerializeField] private bool _dragging = false;
        public bool IsDragging
        {
            get => _dragging;
        }
        #endregion

        private void Start()
        {
            _hovering = _pressed = _selected = false;
            if (_interactable)
                DefaultButton(true);
            else
                DisabledButton(true);
        }

        #region Public Functions
        /// <summary>
        /// Change button state with event invocation.
        /// </summary>
        public void ChangeState(ButtonState state)
        {
            switch (state)
            {
                case ButtonState.Default: InvokeExitAction(); break;
                case ButtonState.Hovered: InvokeEnterAction(); break;
                case ButtonState.Pressed: InvokeDownAction(); break;
                case ButtonState.Selected: InvokeClickAction(); break;
                case ButtonState.Disabled: InvokeDisableAction(); break;
            }
        }

        /// <summary>
        /// Change button state only visually.
        /// </summary>
        public void ChangeStateQuietly(ButtonState state)
        {
            switch (state)
            {
                case ButtonState.Default: DefaultButton(); break;
                case ButtonState.Hovered: HoveredButton(); break;
                case ButtonState.Pressed: PressedButton(); break;
                case ButtonState.Selected: SelectedButton(); break;
                case ButtonState.Disabled: DisabledButton(); break;
            }
        }
        #endregion

        #region Button Interaction Publishers
        protected override sealed void InvokeEnterAction(PointerEventData eventData = null)
        {
            if (!_interactable) return;

            _hovering = true;
            if (_selected) return;

            HoveredButton();

            OnHovered?.Invoke(new UIEventArgs(this));
            //Debug.Log("[UIButton] Invoke Hovered");
        }

        protected override sealed void InvokeExitAction(PointerEventData eventData = null)
        {
            if (!_interactable) return;

            _hovering = false;
            if (_selected) return;

            DefaultButton();

            OnDefault?.Invoke(new UIEventArgs(this));
            //Debug.Log("[UIButton] Invoke Default");
        }

        protected override sealed void InvokeDownAction(PointerEventData eventData = null)
        {
            if (!_interactable) return;

            PressedButton();

            OnPressed?.Invoke(new UIEventArgs(this));
            //Debug.Log("[UIButton] Invoke Pressed");
        }

        protected override sealed void InvokeUpAction(PointerEventData eventData = null)
        {
            if (!_interactable) return;

            _pressed = false;
            CancelInteraction();

            void CancelInteraction()
            {
                if (!_selected && !_hovering)
                {
                    ChangeColors(ButtonState.Default);
                }
                else if (_selected && !_hovering)
                {
                    ChangeColors(ButtonState.Selected);
                }
            }
        }

        protected override sealed void InvokeClickAction(PointerEventData eventData = null)
        {
            if (!_interactable) return;

            SelectedButton();

            if (_selected)
            {
                OnSelected?.Invoke(new UIEventArgs(this));
                //Debug.Log("[UIButton] Invoke Selected");
            }
            else if (_hovering)
            {
                OnHovered?.Invoke(new UIEventArgs(this));
                //Debug.Log("[UIButton] Invoke Hovered");
            }
            else
            {
                OnDefault?.Invoke(new UIEventArgs(this));
                //Debug.Log("[UIButton] Invoke Default");
            }
        }

        private void InvokeDisableAction()
        {
            DisabledButton();

            OnDisabled?.Invoke(new UIEventArgs(this));
            //Debug.Log("[UIButton] Invoke Disabled");
        }
        #endregion

        #region Button Behaviors
        private void DefaultButton(bool instantChange = false)
        {
            _selected = _pressed = _hovering = false;
            ChangeColors(ButtonState.Default, instantChange);
            PlayAudio(ButtonState.Default);
        }

        private void HoveredButton(bool instantChange = false)
        {
            _hovering = true;
            ChangeColors(ButtonState.Hovered, instantChange);
            PlayAudio(ButtonState.Hovered);
        }

        private void PressedButton(bool instantChange = false)
        {
            _pressed = true;
            ChangeColors(ButtonState.Pressed, instantChange);
            PlayAudio(ButtonState.Pressed);
        }

        private void SelectedButton(bool instantChange = false)
        {
            _selected = _type switch
            {
                ButtonType.Radio => true,
                ButtonType.Checkbox => !_selected,
                _ => false
            };

            if (_selected)
            {
                ChangeColors(ButtonState.Selected, instantChange);
                PlayAudio(ButtonState.Selected);
            }
            else if (_hovering)
            {
                ChangeColors(ButtonState.Hovered, instantChange);
                PlayAudio(ButtonState.Hovered);
            }
            else
            {
                ChangeColors(ButtonState.Default, instantChange);
                PlayAudio(ButtonState.Default);
            }
        }

        private void DisabledButton(bool instantChange = false)
        {
            _interactable = false;
            ChangeColors(ButtonState.Disabled, instantChange);
            PlayAudio(ButtonState.Disabled);
        }
        #endregion

        #region Button Features
        private void ChangeColors(ButtonState state, bool instantChange = false)
        {
            if (_colorFeature == null) return;

            if (instantChange)
                _colorFeature.ChangeInstantly(state);
            else
                _colorFeature.ChangeGradually(state);
        }

        private void ChangeSprites(ButtonState state)
        {

        }

        private void PlayAudio(ButtonState state)
        {
            if (_audioFeature == null) return;

            _audioFeature.Play(state);
        }
        #endregion
    }

    public enum ButtonType { Button, Radio, Checkbox }
    public enum ButtonState { Default, Hovered, Pressed, Selected, Disabled }
}
