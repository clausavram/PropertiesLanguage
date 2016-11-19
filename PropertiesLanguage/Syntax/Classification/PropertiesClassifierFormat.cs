using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace PropertiesLanguage.Syntax.Classification {
    #region Format definition
    /// <summary>
    /// Defines the editor format for the PropertiesKey classification type.
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "PropertiesKeyTypeDefinition")]
    [Name("PropertiesKeyFormat")]
    [Order(Before = Priority.Default)]
    internal class PropertiesKey : ClassificationFormatDefinition {
        public PropertiesKey() {
            DisplayName = "Properties Key";
            ForegroundColor = Color.FromRgb(86, 156, 214);
        }
    }

    /// <summary>
    /// Defines the editor format for the PropertiesValue classification type.
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "PropertiesValueTypeDefinition")]
    [Name("PropertiesValueFormat")]
    [Order(Before = Priority.Default)]
    internal class PropertiesValue : ClassificationFormatDefinition {
        public PropertiesValue() {
            DisplayName = "Properties Value";
            ForegroundColor = Color.FromRgb(214, 157, 113);
        }
    }

    /// <summary>
    /// Defines the editor format for the PropertiesComment classification type.
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "PropertiesCommentTypeDefinition")]
    [Name("PropertiesCommentFormat")]
    [Order(Before = Priority.Default)]
    internal class PropertiesComment : ClassificationFormatDefinition {
        public PropertiesComment() {
            DisplayName = "Properties Comment";
            ForegroundColor = Colors.Gray;
        }
    }

    /// <summary>
    /// Defines the editor format for the PropertiesComment classification type.
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "PropertiesSeparatorTypeDefinition")]
    [Name("PropertiesSeparatorFormat")]
    [Order(Before = Priority.Default)]
    internal class PropertiesSeparator : ClassificationFormatDefinition
    {
        public PropertiesSeparator()
        {
            DisplayName = "Properties Separator";
            ForegroundColor = Colors.LightGray;
        }
    }
    #endregion //Format definition
}
