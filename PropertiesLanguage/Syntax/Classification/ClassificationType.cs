using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace PropertiesLanguage.Syntax.Classification {
    internal static class OrdinaryClassificationDefinition {
        #region Type definition
        #pragma warning disable 0649

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("PropertiesKeyTypeDefinition")]
        internal static ClassificationTypeDefinition PropertiesKeyDefinition;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("PropertiesValueTypeDefinition")]
        internal static ClassificationTypeDefinition PropertiesValueDefinition;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("PropertiesCommentTypeDefinition")]
        internal static ClassificationTypeDefinition PropertiesCommentDefinition;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("PropertiesSeparatorTypeDefinition")]
        internal static ClassificationTypeDefinition PropertiesSeparatorDefinition;

        #endregion
    }
}
