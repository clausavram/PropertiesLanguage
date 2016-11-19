using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using PropertiesLanguage.Syntax.Classification;

namespace PropertiesLanguage.Syntax {
    [Export(typeof(ITaggerProvider))]
    [ContentType("PropertiesContentType")]
    [TagType(typeof(ClassificationTag))]
    internal class PropertiesClassifierProvider : ITaggerProvider {

        #pragma warning disable 0649
        [Export]
        [Name("PropertiesContentType")]
        [BaseDefinition("text")]
        internal static ContentTypeDefinition PropertiesContentType;

        [Export]
        [FileExtension(".properties")]
        [ContentType("PropertiesContentType")]
        internal static FileExtensionToContentTypeDefinition PropertiesFileType;

        [Export]
        [FileExtension(".profile")]
        [ContentType("PropertiesContentType")]
        internal static FileExtensionToContentTypeDefinition ProfilePropertyFileType;

        [Import]
        internal IClassificationTypeRegistryService ClassificationTypeRegistry;

        [Import]
        internal IBufferTagAggregatorFactoryService AggregatorFactory;
        #pragma warning restore 0649

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag {
            var propertiesTagAggregator = AggregatorFactory.CreateTagAggregator<PropertiesTokenTag>(buffer);
            return new PropertiesClassifier(buffer, propertiesTagAggregator, ClassificationTypeRegistry) as ITagger<T>;
        }
    }

}
