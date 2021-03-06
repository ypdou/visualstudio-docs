using System;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;
using System.Resources;
using System.Reflection;
using System.Globalization;

namespace SPProjectServiceAddIn
{
    public class Connect : IDTExtensibility2, IDTCommandTarget
    {
        public Connect()
        {
        }

        public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom)
        {
            _applicationObject = (DTE2)application;
            _addInInstance = (AddIn)addInInst;
            if(connectMode == ext_ConnectMode.ext_cm_UISetup)
            {
                object []contextGUIDS = new object[] { };
                Commands2 commands = (Commands2)_applicationObject.Commands;
                string toolsMenuName = "Tools";

                Microsoft.VisualStudio.CommandBars.CommandBar menuBarCommandBar = ((Microsoft.VisualStudio.CommandBars.CommandBars)_applicationObject.CommandBars)["MenuBar"];

                CommandBarControl toolsControl = menuBarCommandBar.Controls[toolsMenuName];
                CommandBarPopup toolsPopup = (CommandBarPopup)toolsControl;

                try
                {
                    Command command = commands.AddNamedCommand2(_addInInstance, "SPProjectServiceAddIn", "SPProjectServiceAddIn", "Executes the command for SPProjectServiceAddIn", true, 59, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported+(int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);

                    if((command != null) && (toolsPopup != null))
                    {
                        command.AddControl(toolsPopup.CommandBar, 1);
                    }
                }
                catch(System.ArgumentException)
                {
                }
            }

            TestSPService();
        }

        public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
        {
        }

        public void OnAddInsUpdate(ref Array custom)
        {
        }

        public void OnStartupComplete(ref Array custom)
        {
        }

        public void OnBeginShutdown(ref Array custom)
        {
        }
        
        public void QueryStatus(string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus status, ref object commandText)
        {
            if(neededText == vsCommandStatusTextWanted.vsCommandStatusTextWantedNone)
            {
                if(commandName == "SPProjectServiceAddIn.Connect.SPProjectServiceAddIn")
                {
                    status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported|vsCommandStatus.vsCommandStatusEnabled;
                    return;
                }
            }
        }

        public void Exec(string commandName, vsCommandExecOption executeOption, ref object varIn, ref object varOut, ref bool handled)
        {
            handled = false;
            if(executeOption == vsCommandExecOption.vsCommandExecOptionDoDefault)
            {
                if(commandName == "SPProjectServiceAddIn.Connect.SPProjectServiceAddIn")
                {
                    handled = true;
                    return;
                }
            }
        }
        private DTE2 _applicationObject;
        private AddIn _addInInstance;

        private void TestSPService()
        {
            //<Snippet1>
            Microsoft.VisualStudio.Shell.ServiceProvider serviceProvider =
                new Microsoft.VisualStudio.Shell.ServiceProvider(
                _applicationObject as Microsoft.VisualStudio.OLE.Interop.IServiceProvider);

            Microsoft.VisualStudio.SharePoint.ISharePointProjectService projectService = 
                serviceProvider.GetService(typeof(Microsoft.VisualStudio.SharePoint.ISharePointProjectService))
                as Microsoft.VisualStudio.SharePoint.ISharePointProjectService;

            if (projectService != null)
            {
                projectService.Logger.WriteLine("This message was written by using the SharePoint project service.",
                    Microsoft.VisualStudio.SharePoint.LogCategory.Message);
            }
            //</Snippet1>

            projectService.ProjectAdded += new EventHandler<Microsoft.VisualStudio.SharePoint.SharePointProjectEventArgs>(
                projectService_ProjectAdded); 
        }

        //<Snippet2>
        void projectService_ProjectAdded(object sender, Microsoft.VisualStudio.SharePoint.SharePointProjectEventArgs e)
        {
            EnvDTE.Project dteProject = e.Project.ProjectService.Convert<
                Microsoft.VisualStudio.SharePoint.ISharePointProject, EnvDTE.Project>(e.Project);

            if (dteProject != null)
            {
                // Use the Visual Studio automation object model to add a folder to the project.
                dteProject.ProjectItems.AddFolder("Data");
            }
        }
        //</Snippet2>
    }
}