using System;
using Confuser.Core;
using Confuser.Protections.Constants;
using ConfuserEx_Additions.Properties;
using dnlib.DotNet;

namespace Confuser.Protections {
	public interface IConstantService {
		void ExcludeMethod(ConfuserContext context, MethodDef method);
	}

	[BeforeProtection("Ki.ControlFlow"), AfterProtection("Ki.RefProxy")]
	internal class ConstantProtection : Protection, IConstantService {
		public const string _Id = "constants v2";
		public const string _FullId = "Ki.Constants.v2";
		public const string _ServiceId = "Ki.Constants.v2";
		internal static readonly object ContextKey = new object();

		public override string Name {
			get { return Resources.ConstantProtection_Name; }
		}

		public override string Description {
			get { return Resources.ConstantProtection_Description; }
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
			context.Registry.RegisterService(_ServiceId, typeof(IConstantService), this);
		}

		protected override void PopulatePipeline(ProtectionPipeline pipeline) {
			pipeline.InsertPreStage(PipelineStage.ProcessModule, new InjectPhase(this));
			pipeline.InsertPostStage(PipelineStage.ProcessModule, new EncodePhase(this));
		}
	}
}