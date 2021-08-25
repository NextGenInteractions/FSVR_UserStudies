using System;

namespace NextGen.VrManager.Ui
{
    public abstract class ToolInspectorUiWidget : InspectorUiWidget
    {
        public abstract Type ToolType { get; }
    }
}

