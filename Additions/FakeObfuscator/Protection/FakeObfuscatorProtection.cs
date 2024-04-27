using Confuser.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConfuserEx_Additions.Properties;

namespace Confuser.Protections.FakeObuscator
{
    public class FakeObfuscatorProtection : Protection
    {
        public override string Name => Resources.FakeObfuscatorProtection_Name;
        public override string Description => Resources.FakeObfuscatorProtection_Description;
        public override string Id => "fake obfuscator";
        public override string FullId => "HoLLy.FakeObfuscator";
        public override ProtectionPreset Preset => ProtectionPreset.Normal;

        protected override void Initialize(ConfuserContext context) { }

        protected override void PopulatePipeline(ProtectionPipeline pipeline)
        {
            pipeline.InsertPostStage(PipelineStage.EndModule, new FakeObfuscatorTypesPhase(this));
            pipeline.InsertPostStage(PipelineStage.EndModule, new FakeObfuscatorAttributesPhase(this));
        }
    }
}
