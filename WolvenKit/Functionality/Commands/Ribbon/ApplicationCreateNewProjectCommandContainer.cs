using System;
using System.IO;
using System.Threading.Tasks;
using Catel;
using Catel.IoC;
using Catel.MVVM;
using Catel.Services;
using WolvenKit.Functionality.Services;
using Orchestra.Services;
using WolvenKit.Common;
using WolvenKit.Common.Model;
using WolvenKit.Common.Services;
using WolvenKit.Functionality.Controllers;
using WolvenKit.Functionality.WKitGlobal;
using WolvenKit.Functionality.WKitGlobal.Helpers;
using WolvenKit.Models.Wizards;
using WolvenKit.MVVM.Model.ProjectManagement.Project;
using WolvenKit.ViewModels.Shell;
using WolvenKit.ViewModels.Wizards;
using WolvenKit.Functionality.Helpers;

namespace WolvenKit.Functionality.Commands
{
    public class ApplicationCreateNewProjectCommandContainer : ProjectCommandContainerBase
    {
        #region Fields

        private readonly ILoggerService _loggerService;
        private readonly ISaveFileService _saveFileService;
        private readonly Cp77Controller _cp77Controller;
        private readonly Tw3Controller _tw3Controller;

        #endregion Fields

        #region Constructors

        public ApplicationCreateNewProjectCommandContainer(
            ICommandManager commandManager,
            IProjectManager projectManager,
            ISaveFileService saveFileService,
            IGrowlNotificationService notificationService,
            ILoggerService loggerService,
            Tw3Controller tw3Controller,
            Cp77Controller cp77Controller
            )
            : base(AppCommands.Application.CreateNewProject, commandManager, projectManager, notificationService, loggerService)
        {
            Argument.IsNotNull(() => loggerService);
            Argument.IsNotNull(() => saveFileService);

            _loggerService = loggerService;
            _saveFileService = saveFileService;
            _tw3Controller = tw3Controller;
            _cp77Controller = cp77Controller;
        }

        #endregion Constructors

        #region Events

        public event Action OnCommandCompleted;

        #endregion Events

        #region Methods

        protected override bool CanExecute(object parameter) => true;

        protected override async void Execute(object parameter)
        {
            try
            {
                var location = parameter as string;
                var filter = "Witcher 3 Project (*.w3modproj)|*.w3modproj| Cyberpunk 2077 Project (*.cpmodproj)|*.cpmodproj";
                if (location == null && parameter is ProjectWizardModel.TypeAndPath res)
                {
                    location = Path.Combine(res.Path, res.Name);
                    if (res.Type == ProjectWizardModel.WitcherGameName)
                    {
                        filter = "Witcher 3 Project (*.w3modproj)|*.w3modproj";
                        location += ".w3modproj";
                    }
                    else if (res.Type == ProjectWizardModel.CyberpunkGameName)
                    {
                        filter = "Cyberpunk 2077 Project (*.cpmodproj)|*.cpmodproj";
                        location += ".cpmodproj";
                    }
                }
                else
                {
                    var result = await _saveFileService.DetermineFileAsync(new DetermineSaveFileContext()
                    {
                        Filter = filter,
                        Title = "Please select a location to save your WolvenKit project",
                        InitialDirectory = location,
                    });

                    if (result.Result)
                    {
                        location = result.FileName;
                    }
                }

                if (!string.IsNullOrWhiteSpace(location))
                {
                    RibbonViewModel.GlobalRibbonVM.StartScreenShown = false;
                    RibbonViewModel.GlobalRibbonVM.BackstageIsOpen = false;
                    using (_pleaseWaitService.PushInScope())
                    {
                        switch (Path.GetExtension(location))
                        {
                            case ".w3modproj":
                            {
                                var np = new Tw3Project(location)
                                {
                                    Name = Path.GetFileNameWithoutExtension(location),
                                    Author = "WolvenKit",
                                    Email = "",
                                    Version = "1.0"
                                    
                                };
                                _projectManager.ActiveProject = np;
                                await _projectManager.SaveAsync();
                                np.CreateDefaultDirectories();
                                saveProjectImg(location);
                                break;
                            }
                            case ".cpmodproj":
                            {
                                var np = new Cp77Project(location)
                                {
                                    Name = Path.GetFileNameWithoutExtension(location),
                                    Author = "WolvenKit",
                                    Email = "",
                                    Version = "1.0"
                                };
                                _projectManager.ActiveProject = np;
                                await _projectManager.SaveAsync();
                                np.CreateDefaultDirectories();
                                saveProjectImg(location);
                                break;
                            }
                            default:
                                _loggerService.LogString("Invalid project path!", Logtype.Error);
                                break;
                        }
                    }

                    await _projectManager.LoadAsync(location);
                    switch (Path.GetExtension(location))
                    {
                        case ".w3modproj":
                            await _tw3Controller.HandleStartup().ContinueWith(t =>
                            {
                                _notificationService.Success(
                                    "Project " + Path.GetFileNameWithoutExtension(location) +
                                    " loaded!");

                            }, TaskContinuationOptions.OnlyOnRanToCompletion);
                            break;
                        case ".cpmodproj":
                            await _cp77Controller.HandleStartup().ContinueWith(
                                t =>
                                {
                                    _notificationService.Success("Project " +
                                                                 Path.GetFileNameWithoutExtension(location) +
                                                                 " loaded!");

                                },
                                TaskContinuationOptions.OnlyOnRanToCompletion);
                            break;
                        default:
                            break;
                    }

                }
            }
            catch (Exception ex)
            {
                _loggerService.LogString(ex.Message, Logtype.Error);
                _loggerService.LogString("Failed to create a new project!", Logtype.Error);
            }

            OnCommandCompleted?.Invoke();
        }

        private void saveProjectImg(string ProjectPath)
        {
            var imagePath = Path.Combine(Path.GetDirectoryName(ProjectPath), Path.GetFileNameWithoutExtension(ProjectPath));
            var _pwvm = ServiceLocator.Default.ResolveType<ProjectWizardViewModel>();
            var src = (System.Windows.Media.Imaging.BitmapSource)_pwvm?.ProfileImageBrush?.ImageSource;
            if (src != null)
            {
                using var fs1 = new FileStream(Path.Combine(imagePath, "img.png"), FileMode.OpenOrCreate);
                var frame = System.Windows.Media.Imaging.BitmapFrame.Create(src);
                var enc = new System.Windows.Media.Imaging.PngBitmapEncoder();
                enc.Frames.Add(frame);
                enc.Save(fs1);
            }
        }

        #endregion Methods
    }
}
