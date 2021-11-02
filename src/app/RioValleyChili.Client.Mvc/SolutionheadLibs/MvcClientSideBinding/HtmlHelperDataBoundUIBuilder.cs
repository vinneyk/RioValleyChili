namespace RioValleyChili.Client.Mvc.SolutionheadLibs.MvcClientSideBinding
{
    //public abstract class DataBoundUIBuilderBase<TModel> : IDataBoundUIBuilder<TModel>
    //{
        
    //    public abstract ViewDataDictionary<TModel> ViewData { get; }

    //    public MvcHtmlString BoundFieldSet(string expression, string dataBindingArgument = null)
    //    {
    //        if (string.IsNullOrWhiteSpace(dataBindingArgument)) { dataBindingArgument = expression; }

    //        var labelBuilder = new TagBuilder("div");
    //        labelBuilder.AddCssClass("editor-label");
    //        labelBuilder.InnerHtml = Label(expression).ToHtmlString();

    //        var inputBuilder = new TagBuilder("div");
    //        inputBuilder.AddCssClass("editor-field");
    //        inputBuilder.InnerHtml = BoundEditor(expression, dataBindingArgument).ToHtmlString();
    //        inputBuilder.InnerHtml += ValidationMessage(expression).ToHtmlString();

    //        return new MvcHtmlString(String.Format("{0}{1}", labelBuilder, inputBuilder));
    //    }

    //    public MvcHtmlString BoundDisplayFieldSet(string expression, string dataBindingArgument = null)
    //    {
    //        if (string.IsNullOrWhiteSpace(dataBindingArgument)) { dataBindingArgument = expression; }

    //        var labelBuilder = new TagBuilder("div");
    //        labelBuilder.AddCssClass("display-label");
    //        labelBuilder.InnerHtml = Label(expression).ToHtmlString();

    //        var inputBuilder = new TagBuilder("div");
    //        inputBuilder.AddCssClass("display-field");
    //        inputBuilder.InnerHtml = BoundDisplay(expression, dataBindingArgument).ToHtmlString();

    //        return new MvcHtmlString(String.Format("{0}{1}", labelBuilder, inputBuilder));
    //    }

    //    public MvcHtmlString BoundFieldSetFor<TValue>(Expression<Func<TModel, TValue>> expression)
    //    {
    //        var lambda = (LambdaExpression) expression;

    //        if (lambda.Body.NodeType == ExpressionType.Parameter)
    //        {
    //            return BoundFieldSetFor(expression, string.Empty);
    //        }

    //        if (lambda.Body.NodeType == ExpressionType.MemberAccess)
    //        {
    //            return BoundFieldSetFor(expression, ((MemberExpression) lambda.Body).Member.Name);
    //        }

    //        throw new NotSupportedException("The expression is not supported.");
    //    }

    //    public MvcHtmlString BoundFieldSetFor<TValue>(Expression<Func<TModel, TValue>> expression, string dataBoundArgument)
    //    {
    //        var labelBuilder = new TagBuilder("div");
    //        labelBuilder.AddCssClass("editor-label");
    //        labelBuilder.InnerHtml = LabelFor(expression).ToHtmlString();

    //        var inputBuilder = new TagBuilder("div");
    //        inputBuilder.AddCssClass("editor-field");
    //        inputBuilder.InnerHtml = BoundEditorFor(expression, dataBoundArgument).ToHtmlString();
    //        inputBuilder.InnerHtml += ValidationMessageFor(expression).ToHtmlString();

    //        return new MvcHtmlString(String.Format("{0}{1}", labelBuilder, inputBuilder));
    //    }

    //    public abstract MvcHtmlString BoundFieldsetDisplayFor<TValue>(Expression<Func<TModel, TValue>> expression);

    //    public MvcHtmlString BoundEditor(string expression, string dataBoundArgument = null)
    //    {
    //        if (string.IsNullOrWhiteSpace(dataBoundArgument)) { dataBoundArgument = expression; }

    //        var valueAttribute = new KeyValuePair<string, object>("value", dataBoundArgument);
    //        ClientSideDataBindingHelper.SetDataBindingAttribute(expression, ViewData, valueAttribute);
    //        var binding = ClientSideDataBindingHelper.ViewData(expression, ViewData);
    //        return Editor(expression, binding);
    //    }

    //    public MvcHtmlString BoundDisplay(string expression, string dataBoundArgument = null)
    //    {
    //        if (string.IsNullOrWhiteSpace(dataBoundArgument)) { dataBoundArgument = expression; }
    //        var binding = new
    //                          {
    //                              data_bind = string.Format("text:{0}", dataBoundArgument)
    //                          };
    //        return Display(expression, binding);
    //    }
        
    //    public MvcHtmlString BoundEditorViewFor<TValue>(Expression<Func<TModel, TValue>> expression)
    //    {
    //        var container = new TagBuilder("div");
    //        container.AddCssClass("editor-control");

    //        // readonly container
    //        var readonlyContainer = new TagBuilder("div");
    //        readonlyContainer.AddCssClass("editor-readonly-view");
    //        readonlyContainer.InnerHtml += BoundFieldsetDisplayFor(expression);

    //        var editButton = InputButton("Edit", new[] { "editor-state-command", "edit-command", "link" });
    //        readonlyContainer.InnerHtml += editButton.ToHtmlString();

    //        container.InnerHtml = readonlyContainer.ToString();

    //        // editable container
    //        var editableContainer = new TagBuilder("div");
    //        editableContainer.AddCssClass("editor-editable-view");
    //        editableContainer.InnerHtml += BoundEditorFor(expression);

    //        var saveButton = InputButton("Save", new[] { "editor-state-command", "save-command", "link" });
    //        editableContainer.InnerHtml += saveButton.ToHtmlString();

    //        var cancelButton = InputButton("Cancel", new[] { "editor-state-command", "cancel-command", "link" });
    //        editableContainer.InnerHtml += cancelButton.ToHtmlString();

    //        container.InnerHtml += editableContainer.ToString();

    //        return new MvcHtmlString(container.ToString());
    //    }

    //}

    //public class HtmlHelperDataBoundUIBuilder<TModel> : DataBoundUIBuilderBase<TModel>
    //{
    //    #region constructors

    //    private readonly HtmlHelper<TModel> _htmlHelper;

    //    public HtmlHelperDataBoundUIBuilder(HtmlHelper<TModel> htmlHelper)
    //        : this(htmlHelper, ClientDataBinding.ClientBoundContainerBuilder, ClientDataBinding.ClientContextBinding) { }

    //    public HtmlHelperDataBoundUIBuilder(HtmlHelper<TModel> htmlHelper, IUIContainerBuilder clientBoundContainerBuilder, IClientContextBinding clientContextBinding)
    //        : base(clientBoundContainerBuilder, clientContextBinding)
    //    {
    //        if (htmlHelper == null) { throw new ArgumentNullException("htmlHelper"); }
    //        _htmlHelper = htmlHelper;
    //    }

    //    #endregion

    //}
}