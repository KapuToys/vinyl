using UnityEngine;
using UnityEngine.EventSystems;
using UniRx;
using UniRx.Triggers;


namespace HutongGames.PlayMaker.Actions
{
	public enum OnPointerDownUpType
	{
		OnPointerDown,
		OnPointerUp
	}


	[ActionCategory(ActionCategory.Input)]
	[Tooltip("Proxy for OnPointer down method of Unity UI EventSystem")]
	public class OnPointerDownUp : FsmStateAction
	{
		[RequiredField]
		[Tooltip("Source of OnPointerDown action.")]
		public FsmOwnerDefault gameObject;

		public OnPointerDownUpType PointerEventType;

		[Tooltip("Event to send on pointer event fired.")]
		public FsmEvent sendEvent;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the position of pointer down event.")]
		public FsmVector3 storeWorldPosition;

//		// too many things in question atm
//		[UIHint(UIHint.Variable)]
//		[Tooltip("Store GameObject under pointer. 2D raycast only.")]
//		public FsmGameObject storeTargetObject;

		[Tooltip("Send debug lessages into the Unity console.")]
		public FsmBool debug;


		System.IDisposable eventTrigger;


		public override void OnEnter()
		{
			var target = Fsm.GetOwnerDefaultTarget(gameObject);

			if ( !target )
			{
				LogWarning("OnPointerDown action of " + this.Name + " has no game object set.");

				Finish();
				return;
			}
				
			IObservable<PointerEventData> observable = GetRequestedObservable(target, PointerEventType);

			eventTrigger = observable.TakeUntilDestroy(target)
				.Subscribe(eventData => 
					{
						if (debug != null && debug.Value)
						{
							Log("OnPointerDownUp action triggered on event " + PointerEventType.ToString());		
						}

						if (sendEvent != null)
						{
							Fsm.Event(sendEvent);
							eventTrigger.Dispose();

							if (debug != null && debug.Value)
							{
								Log("OnPointerDownUp action sends " + sendEvent.Name + " event on " + PointerEventType.ToString());		
							}
						}

						if (storeWorldPosition != null)
						{
							StoreWorldPosition(eventData);
						}

//						// too many things in question atm
//						if (storeTargetObject != null)
//						{
//							StoreTargetObject(eventData);
//						}
					});
		}
			
		public override void OnExit()
		{
			if (eventTrigger != null)
			{
				eventTrigger.Dispose();
			}
		}

		IObservable<PointerEventData> GetRequestedObservable(GameObject target, OnPointerDownUpType type)
		{
			if (type == OnPointerDownUpType.OnPointerDown)
			{
				var tmp = target.GetComponent<ObservablePointerDownTrigger>();
				if (tmp == null)
				{
					tmp = target.AddComponent<ObservablePointerDownTrigger>();
				}

				return tmp.OnPointerDownAsObservable();
			}
			else if (type == OnPointerDownUpType.OnPointerUp)
			{
				var tmp = target.GetComponent<ObservablePointerUpTrigger>();
				if (tmp == null)
				{
					tmp = target.AddComponent<ObservablePointerUpTrigger>();
				}

				return tmp.OnPointerUpAsObservable();
			}
			else
			{
				return null;
			}
		}


		void StoreWorldPosition(PointerEventData eventData)
		{
			storeWorldPosition.Value = eventData.enterEventCamera.ScreenToWorldPoint(eventData.position);
		}

		//		// too many things in question atm
		//		void StoreTargetObject(PointerEventData eventData)
		//		{
		//			Vector3 position = eventData.enterEventCamera.ScreenPointToRay(eventData.position);
		//
		//
		//		}

	}

}
