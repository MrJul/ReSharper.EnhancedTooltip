using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Application;
using JetBrains.Application.Extensions;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.Impl;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace GammaJul.ReSharper.EnhancedTooltip
{
    [ShellComponent]
    public class LiveLoadingRestartRequired : IExtensionRepository
    {
        private readonly SolutionsManager solutionsManager;

        public LiveLoadingRestartRequired(SolutionsManager solutionsManager)
        {
            this.solutionsManager = solutionsManager;
        }

        public bool CanUninstall(string id)
        {
            // We don't uninstall this extension
            return false;
        }

        public void Uninstall(string id, bool removeDependencies, IEnumerable<string> dependencies, Action<LoggingLevel, string> logger)
        {
            // Do nothing
        }

        public bool HasMissingExtensions()
        {
            return false;
        }

        public void RestoreMissingExtensions()
        {
            // Do nothing
        }

        public IEnumerable<string> GetExtensionsRequiringRestart()
        {
            if (RestartRequired())
                yield return "EnhancedTooltip";
        }

        private bool RestartRequired()
        {
            // All of our interesting types are solution-scoped. If we're not in a solution, the types
            // haven't been resolved yet. When they are, we'll be the most derived types and get used.
            // If we're in a solution, then check to see if we've been live loaded - if we have, there
            // will be more than one instance of DaemonImpl
            var solution = solutionsManager.Solution;
            if (solution != null)
            {
                // We can't call GetComponents here, since that verifies cardinality and throws
                // if we try and get multiple instances of a component that should be used with
                // a single instance. We can fake it with Get<IEnumerable<>> though. If we get
                // more than one instance, we're being live loaded, and need a restart
                return solution.GetComponent<IEnumerable<DaemonImpl>>().Count() > 1;
            }

            return false;
        }
    }
}