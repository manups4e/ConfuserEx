using System.Linq;
using Confuser.Core;
using Confuser.Core.Helpers;
using Confuser.Core.Services;
using Confuser.Renamer;
using ConfuserEx_Additions.Properties;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections
{
    [BeforeProtection("Ki.ControlFlow")]

    internal class AntiDumpProtection : Protection
    {
        public const string _Id = "Anti Dump v2";
        public const string _FullId = "Ki.AntiDump.v2";

        public override string Name => Resources.AntiDumpProtection_Name;
        public override string Description => Resources.AntiDumpProtection_Description;
        public override string Id => _Id;
        public override string FullId => _FullId;

        public override ProtectionPreset Preset => ProtectionPreset.Maximum;

        protected override void Initialize(ConfuserContext context)
        {
            // Null
        }

        protected override void PopulatePipeline(ProtectionPipeline pipeline)
        {
            pipeline.InsertPreStage(PipelineStage.ProcessModule, new AntiDumpPhase(this));
        }

        private class AntiDumpPhase : ProtectionPhase
        {
            public AntiDumpPhase(AntiDumpProtection parent) : base(parent)
            {
                // Null
            }

            public override ProtectionTargets Targets => ProtectionTargets.Modules;

            public override string Name => Resources.AntiDumpPhase_Name;

            protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
            {
                var rtType = context.Registry.GetService<IRuntimeService>().GetRuntimeType("Confuser.Runtime.AntiDump2");
                var marker = context.Registry.GetService<IMarkerService>();
                var name = context.Registry.GetService<INameService>();

                foreach (var module in parameters.Targets.OfType<ModuleDef>())
                {
                    var members = InjectHelper.Inject(rtType, module.GlobalType, module);
                    var cctor = module.GlobalType.FindStaticConstructor();
                    var init = (MethodDef)members.Single(method => method.Name == "Initialize");

                    cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));

                    foreach (var member in members)
                    name.MarkHelper(member, marker, (Protection)Parent);
                }
            }
        }
    }
}