namespace RioValleyChili.Client.Mvc.SolutionheadLibs.MvcClientSideBinding
{
    public class EditorViewConfig
    {
        public bool HideSaveButton { get; set; }

        public bool HideEndEditButton { get; set; }

        public static EditorViewConfig Default
        {
            get { return new EditorViewConfig(); }
        }
    }

    public class EditorViewMode
    {
        public static EditorViewConfig Default
        {
            get { return EditorViewConfig.Default; }
        }

        public static EditorViewConfig Component
        {
            get { return new EditorViewConfig
                             {
                                 HideSaveButton = true,
                                 HideEndEditButton = true,
                             }; }
        }
    }
}