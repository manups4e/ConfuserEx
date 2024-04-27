using Confuser.Core;
using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfuserEx_Additions.Properties;

namespace Confuser.Protections {

    [AfterProtection("Cx.Watermark")]
	public class AntiWatermarkProtection : Protection {
		public override string Name => Resources.AntiWatermarkProtection_Name;
		public override string Description => Resources.AntiWatermarkProtection_Description;
		public string Author => "HoLLy ";
		public override string Id => "anti watermark";
		public override string FullId => "HoLLy.AntiWatermark";
		public override ProtectionPreset Preset => ProtectionPreset.Normal;

		protected override void Initialize(ConfuserContext context) { }

		protected override void PopulatePipeline(ProtectionPipeline pipeline) {
			//watermark is added in the inspection stage, this executes right after
			pipeline.InsertPostStage(PipelineStage.EndModule, new AntiWatermarkPhase(this));
		}

		public class AntiWatermarkPhase : ProtectionPhase {
			public override ProtectionTargets Targets => ProtectionTargets.Modules;
			public override string Name => Resources.AntiWatermarkPhase_Name;

			public AntiWatermarkPhase(ConfuserComponent parent) : base(parent) { }
			protected override void Execute(ConfuserContext context, ProtectionParameters parameters) {
				foreach (var m in parameters.Targets.Cast<ModuleDef>().WithProgress(context.Logger)) {
					//look for watermark and remove it
					var attr = m.CustomAttributes.Find("ConfusedByAttribute");
					if (attr != null) {
						m.CustomAttributes.Remove(attr);
						m.Types.Remove((TypeDef)attr.AttributeType);
					}
				}
			}
		}
	}
}
