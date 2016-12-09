using UnityEngine;
using UnityEngine.EventSystems;
using UniRx;
using UniRx.Triggers;


namespace HutongGames.PlayMaker.Actions
{

	[ActionCategory(ActionCategory.Input)]
	[Tooltip("Drag game object using drag event taken from.")]
	public class DragEventSystemItem2D : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The GameObject to drag.")]
		public FsmOwnerDefault gameObject;
		[Tooltip("How fast object will follow drag. Higher value means Default is 0.5f")]
		public float DragMultiplier = 0.5f;

		Vector3 dragPosition;
		Vector3 startPosition;

		GameObject target;

		Camera mainCamera;

		ObservableEventTrigger eventTrigger;

		public override void Reset()
		{
			base.Reset();

			gameObject = null;
			target     = null;
		}

		public override void Awake()
		{
			base.Awake ();

			target     = Fsm.GetOwnerDefaultTarget(gameObject);
			mainCamera = Camera.main;

			eventTrigger = target.AddComponent<ObservableEventTrigger>();
		}


		public override void OnEnter()
		{
			eventTrigger.OnDragAsObservable()
				.TakeUntil(eventTrigger.OnEndDragAsObservable())
				.TakeUntilDestroy(target)
				.Subscribe(eventData => 
					{
						dragPosition = mainCamera.ScreenToWorldPoint(eventData.position);
						target.transform.position = Vector2.Lerp(target.transform.position, dragPosition + startPosition, DragMultiplier);
					}, () => { Finish(); });
		}
	}

}
