using System.ComponentModel.Composition;
using Microsoft.VisualStudio.ProjectSystem;

namespace Cosmos.ProjectSystem.VS
{
    [Export(typeof(IProjectTreePropertiesProvider))]
    [AppliesTo(ProjectCapability.Cosmos)]
    [Order(Order.OverrideManaged)]
    internal class ProjectTreePropertiesProvider : IProjectTreePropertiesProvider
    {
        public void CalculatePropertyValues(
            IProjectTreeCustomizablePropertyContext propertyContext,
            IProjectTreeCustomizablePropertyValues propertyValues)
        {
            if (propertyValues.Flags.Contains(ProjectTreeFlags.Common.ProjectRoot))
            {
                propertyValues.Icon = CosmosImagesMonikers.ProjectRootIcon.ToProjectSystemType();
            }
        }
    }
}
