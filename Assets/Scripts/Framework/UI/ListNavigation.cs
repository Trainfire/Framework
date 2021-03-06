using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Framework;
using System;
using Framework.UI;
using UnityEngine.Events;

namespace Framework.UI
{
    public class ListNavigation : MonoBehaviour, IInputHandler
    {
        public event UnityAction<UIDataViewList> Focused;
        public event UnityAction<UIDataViewList> Unfocused;

        private InputHoldBehaviour holdBehaviourDown;
        private InputHoldBehaviour holdBehaviourUp;
        private List<DataViewList> lists;
        private int index;

        public void Awake()
        {
            lists = new List<DataViewList>();

            holdBehaviourDown = new InputHoldBehaviour(InputMapCoreActions.Down);
            holdBehaviourDown.OnTrigger += HoldBehaviourDown_OnTrigger;

            holdBehaviourUp = new InputHoldBehaviour(InputMapCoreActions.Up);
            holdBehaviourUp.OnTrigger += HoldBehaviourUp_OnTrigger;
        }

        public void Register(DataViewList list)
        {
            if (lists.Contains(list))
            {
                Debug.LogErrorFormat("List '{0}' is already registered.", list.DataView.name);
            }
            else
            {
                lists.Add(list);
            }
        }

        public void Unregister(DataViewList list)
        {
            if (!lists.Contains(list))
            {
                Debug.LogErrorFormat("List '{0}' has not been registered.", list.DataView.name);
            }
            else
            {
                lists.Remove(list);
            }
        }

        public void Focus(DataViewList list)
        {
            if (!lists.Contains(list))
            {
                Debug.LogWarningFormat("Cannot focus on list '{0}' as it has not been registered. Call Register first.", list.DataView.name);
            }
            else
            {
                index = lists.IndexOf(list);
                Focus(index);
            }
        }

        public void Focus(int index)
        {
            if (index < 0 || index > lists.Count - 1)
            {
                Debug.LogError("Index is out of range.");
            }
            else
            {
                if (Unfocused != null)
                    Unfocused(lists[this.index].DataView);

                var focusable = lists[this.index].DataView.GetComponent<Focusable>();
                if (focusable != null)
                    focusable.Unfocus();

                this.index = index;

                focusable = lists[this.index].DataView.GetComponent<Focusable>();
                if (focusable != null)
                    focusable.Focus();

                if (Focused != null)
                    Focused(lists[this.index].DataView);
            }
        }

        private void HoldBehaviourUp_OnTrigger()
        {
            lists[index].MovePrev();
        }

        private void HoldBehaviourDown_OnTrigger()
        {
            lists[index].MoveNext();
        }

        private void FocusPrev()
        {
            if ((index - 1) >= 0)
            {
                int nextIndex = index - 1;
                Focus(nextIndex);
            }
        }

        private void FocusNext()
        {
            if ((index + 1) < lists.Count)
            {
                int nextIndex = index + 1;
                Focus(nextIndex);
            }
        }

        public void OnDestroy()
        {
            holdBehaviourDown.OnTrigger -= HoldBehaviourDown_OnTrigger;
            holdBehaviourUp.OnTrigger -= HoldBehaviourUp_OnTrigger;

            holdBehaviourDown.Destroy();
            holdBehaviourUp.Destroy();
        }

        void IInputHandler.HandleUpdate(InputUpdateEvent handlerEvent)
        {
            if (lists.Count == 0)
                return;

            var allButtonEvents = handlerEvent.GetAllButtonEvents();
            foreach (var kvp in allButtonEvents)
            {
                var buttonEvent = kvp.Value;

                holdBehaviourDown.HandleInput(buttonEvent);
                holdBehaviourUp.HandleInput(buttonEvent);

                if (buttonEvent.Type == InputActionType.Down)
                {
                    if (buttonEvent.Action == InputMapCoreActions.Up)
                    {
                        lists[index].MovePrev();
                        return;
                    }

                    if (buttonEvent.Action == InputMapCoreActions.Down)
                    {
                        lists[index].MoveNext();
                        return;
                    }

                    if (buttonEvent.Action == InputMapCoreActions.ScrollUp)
                    {
                        lists[index].MovePrev();
                        return;
                    }

                    if (buttonEvent.Action == InputMapCoreActions.ScrollDown)
                    {
                        lists[index].MoveNext();
                        return;
                    }

                    if (buttonEvent.Action == InputMapCoreActions.Right)
                    {
                        FocusNext();
                        return;
                    }

                    if (buttonEvent.Action == InputMapCoreActions.Left)
                    {
                        FocusPrev();
                        return;
                    }
                }
            }
        }
    }
}
