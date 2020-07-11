﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.ProjectSystem;

using Cosmos.ProjectSystem.VS.PropertyPages.ViewModels;
using Cosmos.ProjectSystem.VS.PropertyPages.Views;

using VSPropertyPages;

namespace Cosmos.ProjectSystem.VS.PropertyPages
{
    [Guid(PageGuid)]
    internal class CosmosPropertyPage : PropertyPageBase
    {
        public const string PageGuid = "18a5712f-d45f-443f-b6ba-5b729fbabde0";

        public override string PageName => "Cosmos";

        public override IPropertyPageUI CreatePropertyPageUI() =>
            new CosmosPropertyPageControl()
            {
                DataContext = new CosmosPropertyPageViewModel(PropertyManager, ProjectThreadingService)
            };

        public override IPropertyManager CreatePropertyManager(
            IReadOnlyCollection<ConfiguredProject> configuredProjects) =>
            new DynamicConfiguredPropertyManager(UnconfiguredProject, configuredProjects);
    }
}
