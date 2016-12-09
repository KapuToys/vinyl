using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{

	[ActionCategory("Jazz")]
	public class LooperHandle : FsmStateAction
	{
		public Slider looperHandle;

		[RequiredField]
		[Tooltip("The GameObject that owns the FSM.")]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.FsmName)]
		[Tooltip("Optional name of FSM on Game Object")]
		public FsmString fsmName;

		[RequiredField]
		[UIHint(UIHint.FsmFloat)]
		[Tooltip("The name of the FSM variable.")]
		public FsmString variableName;

		GameObject goLastFrame;
		string fsmNameLastFrame;

		PlayMakerFSM fsm;

		public override void Reset()
		{
			gameObject = null;
			fsmName = "";
		}

		public override void OnEnter()
		{
			looperHandle.onValueChanged.AddListener(DoSetFsmFloat);
		}

		public override void OnExit()
		{
			base.OnExit ();

			looperHandle.onValueChanged.RemoveListener(DoSetFsmFloat);

			Finish();
		}

		void DoSetFsmFloat(float speed)
		{
			var go = Fsm.GetOwnerDefaultTarget(gameObject);
			if (go == null)
			{
				return;
			}

			// FIX: must check as well that the fsm name is different.
			if (go != goLastFrame || fsmName.Value != fsmNameLastFrame)
			{
				goLastFrame = go;
				fsmNameLastFrame = fsmName.Value;
				// only get the fsm component if go or fsm name has changed

				fsm = ActionHelpers.GetGameObjectFsm(go, fsmName.Value);
			}			

			if (fsm == null)
			{
				LogWarning("Could not find FSM: " + fsmName.Value);
				return;
			}

			var fsmFloat = fsm.FsmVariables.GetFsmFloat(variableName.Value);

			if (fsmFloat != null)
			{
				fsmFloat.Value = speed;
			}
			else
			{
				LogWarning("Could not find variable: " + variableName.Value);
			}
		}

		





	}

}
