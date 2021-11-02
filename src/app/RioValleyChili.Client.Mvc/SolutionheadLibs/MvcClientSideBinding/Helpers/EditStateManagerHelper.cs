using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace RioValleyChili.Client.Mvc.SolutionheadLibs.MvcClientSideBinding.Helpers
{
    public class EditStateManagerHelper
    {
        #region fields and constructors
        
        private readonly string _editStateManagerName;

        public EditStateManagerHelper(string editStateManager)
        {
            _editStateManagerName = editStateManager;
            CommandClassName = "editor-state-command";
            CommandNames = new Dictionary<Command, string>
                               {
                                   { Command.BeginEdit, "beginEditingCommand" },
                                   { Command.EndEdit, "endEditingCommand" },
                                   { Command.Reset, "resetEditsCommand" },
                                   { Command.Save, "saveEditsCommand" },
                                   { Command.Cancel, "cancelEditsCommand" },
                               };

            CommandDisplayValues = new Dictionary<Command, string>
                                       {
                                           { Command.BeginEdit, "Edit" },
                                           { Command.EndEdit, "End Edit" },
                                           { Command.Reset, "Undo Changes" },
                                           { Command.Save, "Save" },
                                           { Command.Cancel, "Cancel" },
                                       };

            CommandCssClasses = new Dictionary<Command, string[]>
                                    {
                                        { Command.BeginEdit, new [] {"edit-command"} },
                                        { Command.EndEdit, new [] {"end-edit-command"} },
                                        { Command.Reset, new [] {"reset-command"} },
                                        { Command.Save, new [] {"save-command"} },
                                        { Command.Cancel, new [] {"cancel-command"} },
                                    };
        }

        #endregion

        public enum Command
        {
            BeginEdit,
            EndEdit,
            Cancel,
            Reset,
            Save,
        }

        public string CommandClassName { get; set; }

        public IDictionary<Command, string> CommandNames { get; set; }

        public IDictionary<Command, string> CommandDisplayValues { get; set; }

        public IDictionary<Command, string[]> CommandCssClasses { get; set; }


        public string VisibleWhenReadonlyBinding
        {
            get
            {
                return string.Format("visible: {0}.isReadonly", _editStateManagerName);
            }
            
        }

        public string VisibleWhenEditingBinding
        {
            get
            {
                return string.Format("visible: {0}.isEditing", _editStateManagerName);
            }
            
        }

        public string VisibleWhenDirtyBinding
        {
            get
            {
                return string.Format("visible: {0}.isDirty", _editStateManagerName);
            }
            
        }

        public string VisibleWhenEditingOrDirtyBinding
        {
            get
            {
                return string.Format("visible: {0}.isDirty() || {0}.isEditing()", _editStateManagerName);
            }
            
        }

        public string CreateCommand(Command command)
        {
            var comandName = CommandNames[command];
            var displayText = CommandDisplayValues[command];
            var cssClassNames = CommandCssClasses[command];
            return CreateCommand(comandName, displayText, cssClassNames.ToArray(), CommandVisiblity(command));
        }

        private string CreateCommand(string commandName, string commandText, string[] cssClassNames, string visibilityBinding)
        {   
            var commandBindings = string.Format("command: {0}.{1}, {2}", 
                                                _editStateManagerName, 
                                                commandName,
                                                visibilityBinding);
            var buttonAttributes = new Dictionary<string, string>
                                       {
                                           { "type", "button" },
                                           { "value", String.IsNullOrWhiteSpace(commandText) ? "Button" : commandText },
                                           { "data-bind", commandBindings },
                                       };

            var inputButton = new TagBuilder("input");
            inputButton.MergeAttributes(buttonAttributes);
            inputButton.AddCssClass(CommandClassName);
            if (cssClassNames != null && cssClassNames.Any())
            {
                cssClassNames.ToList().ForEach(inputButton.AddCssClass);
            }

            return inputButton.ToString();
        }

        private string CommandVisiblity(Command command)
        {
            switch (command)
            {
                case Command.BeginEdit:
                    return VisibleWhenReadonlyBinding;
                case Command.EndEdit:
                case Command.Reset:
                    return VisibleWhenEditingBinding;
                case Command.Save:
                    return VisibleWhenEditingOrDirtyBinding;
                case Command.Cancel:
                    return VisibleWhenEditingOrDirtyBinding;
                default:
                    return null;
            }
        }
    }
}