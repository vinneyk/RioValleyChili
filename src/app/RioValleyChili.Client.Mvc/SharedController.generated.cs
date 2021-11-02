// <auto-generated />
// This file was generated by a T4 template.
// Don't change it directly as your change would get overwritten.  Instead, make changes
// to the .tt file (i.e. the T4 template) and save it to regenerate this file.

// Make sure the compiler doesn't complain about missing Xml comments and CLS compliance
// 0108: suppress "Foo hides inherited member Foo. Use the new keyword if hiding was intended." when a controller and its abstract parent are both processed
// 0114: suppress "Foo.BarController.Baz()' hides inherited member 'Qux.BarController.Baz()'. To make the current member override that implementation, add the override keyword. Otherwise add the new keyword." when an action (with an argument) overrides an action in a parent controller
#pragma warning disable 1591, 3008, 3009, 0108, 0114
#region T4MVC

using System;
using System.Diagnostics;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Mvc.Html;
using System.Web.Routing;
using T4MVC;
namespace T4MVC
{
    public class SharedController
    {

        static readonly ViewsClass s_views = new ViewsClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ViewsClass Views { get { return s_views; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ViewsClass
        {
            static readonly _ViewNamesClass s_ViewNames = new _ViewNamesClass();
            public _ViewNamesClass ViewNames { get { return s_ViewNames; } }
            public class _ViewNamesClass
            {
                public readonly string _addressBook = "_addressBook";
                public readonly string _BladesLayout = "_BladesLayout";
                public readonly string _GlobalIncludes = "_GlobalIncludes";
                public readonly string _InitializeRequireJS = "_InitializeRequireJS";
                public readonly string _knockoutDateFormattingScripts = "_knockoutDateFormattingScripts";
                public readonly string _knockoutTemplates = "_knockoutTemplates";
                public readonly string _Layout = "_Layout";
                public readonly string _LayoutMain = "_LayoutMain";
                public readonly string _LoginPartial = "_LoginPartial";
                public readonly string _MainNavigation = "_MainNavigation";
                public readonly string _PageTitle = "_PageTitle";
                public readonly string _qUnitLayout = "_qUnitLayout";
                public readonly string _ReportLayout = "_ReportLayout";
                public readonly string _requireHybridSetup = "_requireHybridSetup";
                public readonly string _WebpackHtmlHead = "_WebpackHtmlHead";
                public readonly string _WebpackLayout = "_WebpackLayout";
                public readonly string Error = "Error";
            }
            public readonly string _addressBook = "~/Views/Shared/_addressBook.cshtml";
            public readonly string _BladesLayout = "~/Views/Shared/_BladesLayout.cshtml";
            public readonly string _GlobalIncludes = "~/Views/Shared/_GlobalIncludes.cshtml";
            public readonly string _InitializeRequireJS = "~/Views/Shared/_InitializeRequireJS.cshtml";
            public readonly string _knockoutDateFormattingScripts = "~/Views/Shared/_knockoutDateFormattingScripts.cshtml";
            public readonly string _knockoutTemplates = "~/Views/Shared/_knockoutTemplates.cshtml";
            public readonly string _Layout = "~/Views/Shared/_Layout.cshtml";
            public readonly string _LayoutMain = "~/Views/Shared/_LayoutMain.cshtml";
            public readonly string _LoginPartial = "~/Views/Shared/_LoginPartial.cshtml";
            public readonly string _MainNavigation = "~/Views/Shared/_MainNavigation.cshtml";
            public readonly string _PageTitle = "~/Views/Shared/_PageTitle.cshtml";
            public readonly string _qUnitLayout = "~/Views/Shared/_qUnitLayout.cshtml";
            public readonly string _ReportLayout = "~/Views/Shared/_ReportLayout.cshtml";
            public readonly string _requireHybridSetup = "~/Views/Shared/_requireHybridSetup.cshtml";
            public readonly string _WebpackHtmlHead = "~/Views/Shared/_WebpackHtmlHead.cshtml";
            public readonly string _WebpackLayout = "~/Views/Shared/_WebpackLayout.cshtml";
            public readonly string Error = "~/Views/Shared/Error.cshtml";
            static readonly _DisplayTemplatesClass s_DisplayTemplates = new _DisplayTemplatesClass();
            public _DisplayTemplatesClass DisplayTemplates { get { return s_DisplayTemplates; } }
            [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
            public partial class _DisplayTemplatesClass
            {
                public readonly string Address = "Address";
                public readonly string Boolean = "Boolean";
                public readonly string SelectList = "SelectList";
            }
            static readonly _EditorTemplatesClass s_EditorTemplates = new _EditorTemplatesClass();
            public _EditorTemplatesClass EditorTemplates { get { return s_EditorTemplates; } }
            [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
            public partial class _EditorTemplatesClass
            {
                public readonly string Boolean = "Boolean";
                public readonly string Color = "Color";
                public readonly string Currency = "Currency";
                public readonly string Date = "Date";
                public readonly string DateTime_Local = "DateTime-Local";
                public readonly string DateTime = "DateTime";
                public readonly string Decimal = "Decimal";
                public readonly string Double = "Double";
                public readonly string Email = "Email";
                public readonly string EmailAddress = "EmailAddress";
                public readonly string GridForeignKey = "GridForeignKey";
                public readonly string HiddenInput = "HiddenInput";
                public readonly string Int32 = "Int32";
                public readonly string Integer = "Integer";
                public readonly string Month = "Month";
                public readonly string Number = "Number";
                public readonly string Password = "Password";
                public readonly string PhoneNumber = "PhoneNumber";
                public readonly string Search = "Search";
                public readonly string SelectList = "SelectList";
                public readonly string String = "String";
                public readonly string TextArea = "TextArea";
                public readonly string Time = "Time";
                public readonly string TransitViewModel = "TransitViewModel";
                public readonly string _Url = "Url";
                public readonly string Week = "Week";
            }
            static readonly _ScriptTemplatesClass s_ScriptTemplates = new _ScriptTemplatesClass();
            public _ScriptTemplatesClass ScriptTemplates { get { return s_ScriptTemplates; } }
            [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
            public partial class _ScriptTemplatesClass
            {
                static readonly _ViewNamesClass s_ViewNames = new _ViewNamesClass();
                public _ViewNamesClass ViewNames { get { return s_ViewNames; } }
                public class _ViewNamesClass
                {
                    public readonly string _addressScriptTemplates = "_addressScriptTemplates";
                    public readonly string _companyScriptTemplates = "_companyScriptTemplates";
                    public readonly string _contactsScriptTemplates = "_contactsScriptTemplates";
                    public readonly string _ContractScriptTemplate = "_ContractScriptTemplate";
                    public readonly string _InventoryAdjustmentScriptTemplates = "_InventoryAdjustmentScriptTemplates";
                    public readonly string _InventoryPickOrderScriptTemplate = "_InventoryPickOrderScriptTemplate";
                    public readonly string _labResultsScriptTemplates = "_labResultsScriptTemplates";
                    public readonly string _LotInventoryScriptTemplates = "_LotInventoryScriptTemplates";
                    public readonly string _LotScriptTemplates = "_LotScriptTemplates";
                    public readonly string _millAndWetdownScriptTemplates = "_millAndWetdownScriptTemplates";
                    public readonly string _notebookScriptTemplates = "_notebookScriptTemplates";
                    public readonly string _PackScheduleScriptTemplates = "_PackScheduleScriptTemplates";
                    public readonly string _ProductIngredientsScriptTemplates = "_ProductIngredientsScriptTemplates";
                    public readonly string _ProductionBatchScriptTemplates = "_ProductionBatchScriptTemplates";
                    public readonly string _ProductionResultsScriptTemplates = "_ProductionResultsScriptTemplates";
                    public readonly string _ProductScriptTemplates = "_ProductScriptTemplates";
                    public readonly string _ProductSpecsScriptTemplates = "_ProductSpecsScriptTemplates";
                    public readonly string _ResultsScriptTemplates = "_ResultsScriptTemplates";
                    public readonly string _ShipmentScriptTemplates = "_ShipmentScriptTemplates";
                }
                public readonly string _addressScriptTemplates = "~/Views/Shared/ScriptTemplates/_addressScriptTemplates.cshtml";
                public readonly string _companyScriptTemplates = "~/Views/Shared/ScriptTemplates/_companyScriptTemplates.cshtml";
                public readonly string _contactsScriptTemplates = "~/Views/Shared/ScriptTemplates/_contactsScriptTemplates.cshtml";
                public readonly string _ContractScriptTemplate = "~/Views/Shared/ScriptTemplates/_ContractScriptTemplate.cshtml";
                public readonly string _InventoryAdjustmentScriptTemplates = "~/Views/Shared/ScriptTemplates/_InventoryAdjustmentScriptTemplates.cshtml";
                public readonly string _InventoryPickOrderScriptTemplate = "~/Views/Shared/ScriptTemplates/_InventoryPickOrderScriptTemplate.cshtml";
                public readonly string _labResultsScriptTemplates = "~/Views/Shared/ScriptTemplates/_labResultsScriptTemplates.cshtml";
                public readonly string _LotInventoryScriptTemplates = "~/Views/Shared/ScriptTemplates/_LotInventoryScriptTemplates.cshtml";
                public readonly string _LotScriptTemplates = "~/Views/Shared/ScriptTemplates/_LotScriptTemplates.cshtml";
                public readonly string _millAndWetdownScriptTemplates = "~/Views/Shared/ScriptTemplates/_millAndWetdownScriptTemplates.cshtml";
                public readonly string _notebookScriptTemplates = "~/Views/Shared/ScriptTemplates/_notebookScriptTemplates.cshtml";
                public readonly string _PackScheduleScriptTemplates = "~/Views/Shared/ScriptTemplates/_PackScheduleScriptTemplates.cshtml";
                public readonly string _ProductIngredientsScriptTemplates = "~/Views/Shared/ScriptTemplates/_ProductIngredientsScriptTemplates.cshtml";
                public readonly string _ProductionBatchScriptTemplates = "~/Views/Shared/ScriptTemplates/_ProductionBatchScriptTemplates.cshtml";
                public readonly string _ProductionResultsScriptTemplates = "~/Views/Shared/ScriptTemplates/_ProductionResultsScriptTemplates.cshtml";
                public readonly string _ProductScriptTemplates = "~/Views/Shared/ScriptTemplates/_ProductScriptTemplates.cshtml";
                public readonly string _ProductSpecsScriptTemplates = "~/Views/Shared/ScriptTemplates/_ProductSpecsScriptTemplates.cshtml";
                public readonly string _ResultsScriptTemplates = "~/Views/Shared/ScriptTemplates/_ResultsScriptTemplates.cshtml";
                public readonly string _ShipmentScriptTemplates = "~/Views/Shared/ScriptTemplates/_ShipmentScriptTemplates.cshtml";
            }
            static readonly _ScriptTestsClass s_ScriptTests = new _ScriptTestsClass();
            public _ScriptTestsClass ScriptTests { get { return s_ScriptTests; } }
            [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
            public partial class _ScriptTestsClass
            {
                static readonly _ViewNamesClass s_ViewNames = new _ViewNamesClass();
                public _ViewNamesClass ViewNames { get { return s_ViewNames; } }
                public class _ViewNamesClass
                {
                    public readonly string _LotHoldUITests = "_LotHoldUITests";
                }
                public readonly string _LotHoldUITests = "~/Views/Shared/ScriptTests/_LotHoldUITests.cshtml";
            }
        }
    }

}

#endregion T4MVC
#pragma warning restore 1591, 3008, 3009, 0108, 0114