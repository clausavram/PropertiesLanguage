using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;

namespace PropertiesLanguage.Syntax.Classification {

    internal class PropertiesClassifier : ITagger<ClassificationTag> {
        private readonly ITagAggregator<PropertiesTokenTag> aggregator;
        private readonly IDictionary<PropertiesTokenTypes, IClassificationType> propertiesTypes;
        private ITextBuffer buffer;

        /// <summary>
        /// Construct the classifier and define search tokens
        /// </summary>
        internal PropertiesClassifier(ITextBuffer buffer,
            ITagAggregator<PropertiesTokenTag> propertiesTagAggregator,
            IClassificationTypeRegistryService typeService) {
            this.buffer = buffer;
            aggregator = propertiesTagAggregator;
            propertiesTypes = new Dictionary<PropertiesTokenTypes, IClassificationType> {
                [PropertiesTokenTypes.PropertiesKey] = typeService.GetClassificationType("PropertiesKeyTypeDefinition"),
                [PropertiesTokenTypes.PropertiesValue] = typeService.GetClassificationType("PropertiesValueTypeDefinition"),
                [PropertiesTokenTypes.PropertiesComment] = typeService.GetClassificationType("PropertiesCommentTypeDefinition"),
                [PropertiesTokenTypes.PropertiesSeparator] = typeService.GetClassificationType("PropertiesSeparatorTypeDefinition")
            };

            aggregator.TagsChanged += (sender, args) => {
                foreach (var span in args.Span.GetSpans(buffer))
                    RaiseTagsChangedEvent(span);
            };
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        /// <summary>
        /// Search the given span for any instances of classified tags
        /// </summary>
        public IEnumerable<ITagSpan<ClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans) {
            return from tagSpan in aggregator.GetTags(spans)
                let tagSpans = tagSpan.Span.GetSpans(spans[0].Snapshot)
                select new TagSpan<ClassificationTag>(tagSpans[0], new ClassificationTag(propertiesTypes[tagSpan.Tag.Type]));
        }

        private void RaiseTagsChangedEvent(SnapshotSpan subjectSpan) {
            var handler = TagsChanged;
            handler?.Invoke(this, new SnapshotSpanEventArgs(subjectSpan));
        }
    }

}