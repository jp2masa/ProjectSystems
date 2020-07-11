﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// The file has been modified

using System;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Build;
using Microsoft.VisualStudio.ProjectSystem;
using Microsoft.VisualStudio.ProjectSystem.Build;
using Microsoft.VisualStudio.ProjectSystem.VS;

namespace XSharp.ProjectSystem.VS.Properties
{

    /// <summary>
    ///     Implementation of <see cref="IProjectGuidStorageProvider"/> that avoids persisting the 
    ///     project GUID property to the project file if isn't already present in the file.
    /// </summary>
    [Export(typeof(IProjectGuidStorageProvider))]
    [AppliesTo(ProjectCapability.XSharp)]
    [Order(Order.Default)]
    internal class AvoidPersistingProjectGuidStorageProvider : IProjectGuidStorageProvider
    {
        private readonly IProjectAccessor _projectAccessor;
        private readonly UnconfiguredProject _project;
        private bool? _isPersistedInProject;

        [ImportingConstructor]
        internal AvoidPersistingProjectGuidStorageProvider(IProjectAccessor projectAccessor, UnconfiguredProject project)
        {
            _projectAccessor = projectAccessor;
            _project = project;
        }

        public Task<Guid> GetProjectGuidAsync()
        {
            // We use the construction model to avoid evaluating during asynchronous project load
            return _projectAccessor.OpenProjectXmlForReadAsync(_project, projectXml =>
            {
                ProjectPropertyElement property = FindProjectGuidProperty(projectXml);
                if (property != null)
                {
                    _isPersistedInProject = true;

                    _ = TryParseGuid(property, out Guid result);
                    return result;
                }
                else
                {
                    _isPersistedInProject = false;
                }

                return Guid.Empty;
            });
        }

        public Task SetProjectGuidAsync(Guid value)
        {
            // Avoid searching for the <ProjectGuid/> if we've already checked previously in GetProjectGuidAsync.
            // This handles project open, avoids us needed to take another read-lock during setting of it.
            //
            // Technically a project could add a <ProjectGuid/> latter down the track by editing the project or 
            // reloading from disk, however, both the solution, CPS and other components within Visual Studio
            // do not handle the GUID changing underneath them.
            if (_isPersistedInProject == false)
                return Task.CompletedTask;

            return _projectAccessor.OpenProjectXmlForUpgradeableReadAsync(_project, async (projectXml, cancellationToken) =>
            {
                ProjectPropertyElement property = FindProjectGuidProperty(projectXml);
                if (property != null)
                {
                    _isPersistedInProject = true;

                    // Avoid touching the project file unless the actual GUID has changed, regardless of format
                    if (!TryParseGuid(property, out Guid result) || value != result)
                    {
                        await _projectAccessor.OpenProjectXmlForWriteAsync(_project, (root) =>
                        {
                            property.Value = ProjectCollection.Escape(value.ToString("B", CultureInfo.InvariantCulture).ToUpperInvariant());

                        }).ConfigureAwait(true);
                    }
                }
                else
                {
                    _isPersistedInProject = false;
                }
            });
        }

        private static ProjectPropertyElement FindProjectGuidProperty(ProjectRootElement projectXml)
        {
            // NOTE: Unlike evaluation, we return the first <ProjectGuid /> to mimic legacy project system behavior
            return projectXml.PropertyGroups.SelectMany(group => group.Properties)
                                            .FirstOrDefault(p => StringComparers.PropertyNames.Equals(BuildProperty.ProjectGuid, p.Name));
        }

        private static bool TryParseGuid(ProjectPropertyElement property, out Guid result)
        {
            string unescapedValue = property.GetUnescapedValue();

            return Guid.TryParse(unescapedValue, out result);
        }
    }
}