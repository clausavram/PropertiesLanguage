using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using static PropertiesLanguage.Syntax.PropertiesTokenTypes;

namespace PropertiesLanguage.Syntax {
    [Export(typeof(ITaggerProvider))]
    [ContentType("PropertiesContentType")]
    [TagType(typeof(PropertiesTokenTag))]
    internal sealed class PropertiesTokenTagProvider : ITaggerProvider {

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag {
            return new PropertiesTokenTagger() as ITagger<T>;
        }
    }

    public class PropertiesTokenTag : ITag {
        public PropertiesTokenTypes Type { get; private set; }

        public PropertiesTokenTag(PropertiesTokenTypes type) {
            Type = type;
        }
    }

    internal sealed class PropertiesTokenTagger : ITagger<PropertiesTokenTag> {
        private readonly Regex keyValuePattern = new Regex(@"(?<!^\s*|\\)([ \t]*[=:][ \t]*|[ \t]+)");
        private readonly Regex commentPattern = new Regex(@"^\s*[#!]");
        private readonly Regex escapedLineEndPattern = new Regex(@"\\$");

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged { add { } remove { } }

        public IEnumerable<ITagSpan<PropertiesTokenTag>> GetTags(NormalizedSnapshotSpanCollection spans) {
            // sadly `spans` gets one line at a time, so previouslyEscapedValue will not get the chance to be used
            var previouslyEscapedValue = false;
            foreach (var curSpan in spans) {
                var containingLine = curSpan.Start.GetContainingLine();
                var lineStartLoc = containingLine.Start.Position;
                var lineText = containingLine.GetText();

                if (previouslyEscapedValue) {
                    var valueSpan = new SnapshotSpan(curSpan.Snapshot, new Span(lineStartLoc, lineText.Length));
                    yield return new TagSpan<PropertiesTokenTag>(valueSpan, new PropertiesTokenTag(PropertiesValue));
                    previouslyEscapedValue = escapedLineEndPattern.IsMatch(lineText);
                    continue;
                }

                if (commentPattern.IsMatch(lineText)) {
                    var commentSpan = new SnapshotSpan(curSpan.Snapshot, new Span(lineStartLoc, lineText.Length));
                    yield return new TagSpan<PropertiesTokenTag>(commentSpan, new PropertiesTokenTag(PropertiesComment));
                    continue;
                }

                if (keyValuePattern.IsMatch(lineText)) {
                    var splitPosition = keyValuePattern.Split(lineText)[0].Length;
                    var keySpan = new SnapshotSpan(curSpan.Snapshot, new Span(lineStartLoc, splitPosition));
                    yield return new TagSpan<PropertiesTokenTag>(keySpan, new PropertiesTokenTag(PropertiesKey));
                    var valueSpan = new SnapshotSpan(curSpan.Snapshot, new Span(lineStartLoc + splitPosition + 1, lineText.Length - splitPosition - 1));
                    yield return new TagSpan<PropertiesTokenTag>(valueSpan, new PropertiesTokenTag(PropertiesValue));
                    previouslyEscapedValue = escapedLineEndPattern.IsMatch(lineText);
                } else {
                    var keySpan = new SnapshotSpan(curSpan.Snapshot, new Span(lineStartLoc, lineText.Length));
                    yield return new TagSpan<PropertiesTokenTag>(keySpan, new PropertiesTokenTag(PropertiesKey));
                }
            }
        }
    }
}
