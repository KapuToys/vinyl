using UnityEngine;
using UnityEngine.EventSystems;
using UniRx;
using UniRx.Triggers;


namespace HutongGames.PlayMaker.Actions
{

	[ActionCategory(ActionCategory.Input)]
	[Tooltip("Proxy for OnPointer down method of Unity UI EventSystem")]
	public class OnDrop : FsmStateAction
	{
		[RequiredField]
		[Tooltip("Source of OnPointerDown action.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("Event to send on pointer event fired.")]
		public FsmEvent sendEvent;

		[UIHint(UIHint.Variable)]
		public FsmGameObject storeDroppedObject;

//		// too many things in question atm
//		[UIHint(UIHint.Variable)]
//		[Tooltip("Store GameObject under pointer. 2D raycast only.")]
//		public FsmGameObject storeTargetObject;

		System.IDisposable eventTrigger;

		IObservable<PointerEventData> observable;

		public override void OnEnter()
		{
			var target = Fsm.GetOwnerDefaultTarget(gameObject);

			if ( !target )
			{
				LogWarning("OnPointerDown action of " + this.Name + " has no game object set.");


				return;
			}

			if (observable == null)
			{
				observable = target.AddComponent<ObservableDropTrigger>().OnDropAsObservable();
			}

			eventTrigger = observable.TakeUntilDestroy(target)
				.Take(1)
				.Subscribe(eventData => 
					{
						Debug.Log(" Object " + eventData.pointerDrag.name + " dropped on " + target.name);

						if (storeDroppedObject != null)
						{
							storeDroppedObject.Value = eventData.pointerDrag;
						}

						if (sendEvent != null)
						{
							Fsm.Event(sendEvent);
						}



						Finish();
					});
		}
	}

}
