using System;
using Confuser.Core;
using Confuser.Protections.ControlFlow;
using ConfuserEx_Additions.Properties;
using dnlib.DotNet;

namespace Confuser.Protections {
	public interface IControlFlowService {
		void ExcludeMethod(ConfuserContext context, MethodDef method);
	}

	internal class ControlFlowProtection : Protection, IControlFlowService {
		public const string _Id = "ctrl flow v2";
		public const string _FullId = "Ki.ControlFlow.v2";
		public const string _ServiceId = "Ki.ControlFlow.v2";

		public override string Name {
			get { return Resources.ControlFlowProtection_Name; }
		}

		public override string Description {
			get { return Resources.ControlFlowProtection_Description; }
		}

		public override string Id {
			get { return _Id; }
		}

		public override string FullId {
			get { return _FullId; }
		}

		public override ProtectionPreset Preset {
			get { return ProtectionPreset.Normal; }
		}

		public void ExcludeMethod(ConfuserContext context, MethodDef method) {
			ProtectionParameters.GetParameters(context, method).Remove(this);
		}

		protected override void Initialize(ConfuserContext context) {
			context.Registry.RegisterService(_ServiceId, typeof(IControlFlowService), this);
		}

		protected override void PopulatePipeline(ProtectionPipeline pipeline) {
			pipeline.InsertPreStage(PipelineStage.OptimizeMethods, new ControlFlowPhase(this));
		}
	}
}