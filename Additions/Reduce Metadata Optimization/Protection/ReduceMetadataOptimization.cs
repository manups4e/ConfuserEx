using Confuser.Core;
using ConfuserEx_Additions.Properties;
using dnlib.DotNet;

namespace Confuser.Protections
{
    [BeforeProtection("Ki.ControlFlow")]

    internal class ReduceMetadataOptimization : Protection
    {
        public const string _Id = "Reduce MD";

        public const string _FullId = "Ki.ReduceMetadata";

        public override string Name
        {
            get
            {
                return Resources.ReduceMetadataOptimization_Name;
            }
        }

        public override string Description
        {
            get
            {
                return Resources.ReduceMetadataOptimization_Description;
            }
        }

        public override string Id
        {
            get
            {
                return _Id;
            }
        }

        public override string FullId
        {
            get
            {
                return _FullId;
            }
        }

        public override ProtectionPreset Preset
        {
            get
            {
                return ProtectionPreset.Maximum;
            }
        }

        protected override void Initialize(ConfuserContext context)
        {
        }

        protected override void PopulatePipeline(ProtectionPipeline pipeline)
        {
            pipeline.InsertPreStage(PipelineStage.ProcessModule, new ReduceMetadataOptimizationPhase(this));
        }

        private class ReduceMetadataOptimizationPhase : ProtectionPhase
        {
            public ReduceMetadataOptimizationPhase(ReduceMetadataOptimization parent) : base(parent)
            {
                // Null
            }

            public override ProtectionTargets Targets
            {
                get
                {
                    return ProtectionTargets.Methods;
                }
            }

            public override string Name
            {
                get
                {
                    return Resources.ReduceMetadataOptimizationPhase_Name;
                }
            }

            protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
            {
                IMemberDef memberDef = parameters.Targets as IMemberDef;

                TypeDef typeDef;

                if ((typeDef = (memberDef as TypeDef)) != null && !this.IsTypePublic(typeDef))
                {
                    if (typeDef.IsEnum)
                    {
                        int num = 0;
                        while (typeDef.Fields.Count != 1)
                        {
                            if (typeDef.Fields[num].Name != "value__")
                            {
                                typeDef.Fields.RemoveAt(num);
                            }
                            else
                            {
                                num++;
                            }
                        }
                        return;
                    }
                }
                else if (memberDef is EventDef)
                {
                    if (memberDef.DeclaringType != null)
                    {
                        memberDef.DeclaringType.Events.Remove(memberDef as EventDef);
                        return;
                    }
                }
                else if (memberDef is PropertyDef && memberDef.DeclaringType != null)
                {
                    memberDef.DeclaringType.Properties.Remove(memberDef as PropertyDef);
                }
            }

            private bool IsTypePublic(TypeDef type)
            {
                while (type.IsPublic || type.IsNestedFamily || type.IsNestedFamilyAndAssembly || type.IsNestedFamilyOrAssembly || type.IsNestedPublic || type.IsPublic)
                {
                    type = type.DeclaringType;
                    if (type == null)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}
