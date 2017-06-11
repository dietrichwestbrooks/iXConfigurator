using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Interfaces;
using Wayne.Payment.Products.iXConfigurator.Modules.Core.Commands;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.Services
{
    public class ConfiguratorService : IConfiguratorService
    {
        private IFileService _fileService;
        private IVariableStore _variables;
        private ITaskCommandFactory _commandFactory;
        private ITemplateManager _templateManager;
        private IApplicationSettings _settings;

        public ConfiguratorService(IFileService fileService, IVariableStore variables,
            ITaskCommandFactory commandFactory, ITemplateManager templateManager , IApplicationSettings settings)
        {
            _fileService = fileService;
            _variables = variables;
            _commandFactory = commandFactory;
            _templateManager = templateManager;
            _settings = settings;
        }

        public async Task BuildAsync(string key, string drive, Action<string, string, double> progress)
        {
            try
            {
                var template = await _templateManager.GetTemplateAsync(key);

                _variables[WellKnownVariableNames.InstallDrive] = drive;

                var libraryPath = Path.Combine(_settings.LocalLibraryPath, key);

                _variables[WellKnownVariableNames.LibraryPath] = libraryPath;

                var installPath = Path.Combine(Path.GetTempPath(), "ixconfig\\install");

                _variables[WellKnownVariableNames.InstallPath] = installPath;

                var tempPath = Path.Combine(Path.GetTempPath(), "ixconfig\\temp");

                _variables[WellKnownVariableNames.TempPath] = tempPath;

                progress.Invoke("Building...", "Loading library...", 0);

                await _templateManager.LoadLibraryAsync(key);

                await _fileService.CreateDirectoryAsync(installPath, true);

                await _fileService.CreateDirectoryAsync(tempPath, true);

                var commands = new List<TaskCommand>();

                commands.AddRange(template.Build.Select(_commandFactory.CreateCommand));

                commands.Add(_commandFactory.CreateDeleteCommand($"{drive}", true));

                commands.Add(_commandFactory.CreateCopyCommand($"%{WellKnownVariableNames.InstallPath}%", $"{drive}", true, true));

                for (var i = 0; i < commands.Count; i++)
                {
                    var value = ((double)i) / (commands.Count);
                    var message = $"Executing {i + 1} of {commands.Count} tasks...";
                    var title = "Building...";

                    var command = commands.ElementAt(i);

                    if (i == commands.Count - 1)
                    {
                        title = "Transferring to USB Device...";
                    }

                    progress.Invoke(title, message, value);

                    if (command.CanExecute())
                    {
                        await command.Execute();
                    }
                }

                progress.Invoke("Cleanup...", "Unloading library...", 0);

                await _templateManager.UnloadLibraryAsync(key);

                progress.Invoke("Build Comlete", string.Empty, 1);
            }
            finally
            {
                _variables.RemoveVariable(WellKnownVariableNames.LibraryPath);
                _variables.RemoveVariable(WellKnownVariableNames.InstallPath);
                _variables.RemoveVariable(WellKnownVariableNames.InstallDrive);
                _variables.RemoveVariable(WellKnownVariableNames.TempPath);
            }
        }
    }
}
