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
    internal sealed class PropertiesKey : ClassificationFormatDefinition {
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
    internal sealed class PropertiesValue : ClassificationFormatDefinition {
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
    internal sealed class PropertiesComment : ClassificationFormatDefinition {
        public PropertiesComment() {
            DisplayName = "Properties Comment";
            ForegroundColor = Colors.Gray;
        }
    }
    #endregion //Format definition
}
