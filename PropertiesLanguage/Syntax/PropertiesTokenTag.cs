using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using static PropertiesLanguage.Syntax.PropertiesTokenTypes;

namespace PropertiesLanguage.Syntax {

    [Export(typeof(ITaggerProvider))]
    [ContentType("PropertiesContentType")]
    [TagType(typeof(PropertiesTokenTag))]
    internal class PropertiesTokenTagProvider : ITaggerProvider {
        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag {
            return new PropertiesTokenTagger(buffer) as ITagger<T>;
        }
    }

    public class PropertiesTokenTag : ITag {
        public PropertiesTokenTag(PropertiesTokenTypes type) {
            Type = type;
        }

        public PropertiesTokenTypes Type { get; private set; }
    }

    internal class PropertiesTokenTagger : ITagger<PropertiesTokenTag> {
        private readonly ITextBuffer buffer;
        private readonly Regex commentPattern = new Regex(@"^\s*[#!]");
        private readonly Regex escapedLineEndPattern = new Regex(@"\\$");
        private readonly Regex keyValuePattern = new Regex(@"(?<!\\)((?=[ \t]*)[=:][ \t]*)|(?<=\w)[ \t]+(?![=:][ \t]*)(?=[^ \t])");
        private readonly Regex nonWhitespaceSeparatorPattern = new Regex(@"^[=:]");

        public PropertiesTokenTagger(ITextBuffer buffer) {
            this.buffer = buffer;
            this.buffer.Changed += OnTextBufferChanged;
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public IEnumerable<ITagSpan<PropertiesTokenTag>> GetTags(NormalizedSnapshotSpanCollection spans) {
            foreach (var curSpan in spans) {
                var containingLine = curSpan.Start.GetContainingLine();
                var lineStartLoc = containingLine.Start.Position;
                var lineText = containingLine.GetText();

                if (lineText.Length == 0) {
                    continue;
                }

                var previousIsNotComment = false;

                var previousLine = curSpan.Snapshot.Lines.LastOrDefault(l => l.End <= curSpan.Start);
                if (previousLine != null && escapedLineEndPattern.IsMatch(previousLine.GetText())) {
                     var previousToken = GetTags(new NormalizedSnapshotSpanCollection(previousLine.Extent)).ToList();

                    if (previousToken.Count > 0) {
                        var propertiesTokenTypes = previousToken.Last().Tag.Type;
                        if (propertiesTokenTypes == PropertiesValue) {
                            var separatorCount = nonWhitespaceSeparatorPattern.IsMatch(lineText[0].ToString()) ? 1 : 0;
                            if (separatorCount > 0) {
                                var separatorSpan = new SnapshotSpan(curSpan.Snapshot, new Span(lineStartLoc, 1));
                                yield return new TagSpan<PropertiesTokenTag>(separatorSpan, new PropertiesTokenTag(PropertiesSeparator));
                            }

                            var valueSpan = new SnapshotSpan(curSpan.Snapshot, new Span(lineStartLoc, lineText.Length));
                            yield return new TagSpan<PropertiesTokenTag>(valueSpan, new PropertiesTokenTag(PropertiesValue));
                            continue;
                        }

                        if (propertiesTokenTypes == PropertiesKey && nonWhitespaceSeparatorPattern.IsMatch(lineText)) {
                            var separatorCount = nonWhitespaceSeparatorPattern.IsMatch(lineText[0].ToString()) ? 1 : 0;
                            if (separatorCount > 0) {
                                var separatorSpan = new SnapshotSpan(curSpan.Snapshot, new Span(lineStartLoc, 1));
                                yield return new TagSpan<PropertiesTokenTag>(separatorSpan, new PropertiesTokenTag(PropertiesSeparator));
                            }

                            var valueSpan = new SnapshotSpan(curSpan.Snapshot, new Span(lineStartLoc + separatorCount, lineText.Length - separatorCount));
                            yield return new TagSpan<PropertiesTokenTag>(valueSpan, new PropertiesTokenTag(PropertiesValue));
                            continue;
                        }

                        previousIsNotComment = propertiesTokenTypes != PropertiesComment;
                    }
                }

                if (commentPattern.IsMatch(lineText) && !previousIsNotComment) {
                    var commentSpan = new SnapshotSpan(curSpan.Snapshot, new Span(lineStartLoc, lineText.Length));
                    yield return new TagSpan<PropertiesTokenTag>(commentSpan, new PropertiesTokenTag(PropertiesComment));
                    continue;
                }

                if (keyValuePattern.IsMatch(lineText)) {
                    var splitPosition = keyValuePattern.Split(lineText)[0].Length;
                    var keySpan = new SnapshotSpan(curSpan.Snapshot, new Span(lineStartLoc, splitPosition));
                    yield return new TagSpan<PropertiesTokenTag>(keySpan, new PropertiesTokenTag(PropertiesKey));

                    var separatorCount = nonWhitespaceSeparatorPattern.IsMatch(lineText[splitPosition].ToString()) ? 1 : 0;
                    if (separatorCount > 0) {
                        var separatorSpan = new SnapshotSpan(curSpan.Snapshot, new Span(lineStartLoc + splitPosition, 1));
                        yield return new TagSpan<PropertiesTokenTag>(separatorSpan, new PropertiesTokenTag(PropertiesSeparator));
                    }

                    var valueSpan = new SnapshotSpan(curSpan.Snapshot, new Span(lineStartLoc + splitPosition + separatorCount, lineText.Length - splitPosition - separatorCount));
                    yield return new TagSpan<PropertiesTokenTag>(valueSpan, new PropertiesTokenTag(PropertiesValue));
                } else {
                    var separatorCount = nonWhitespaceSeparatorPattern.IsMatch(lineText[0].ToString()) ? 1 : 0;
                    if (separatorCount > 0) {
                        var separatorSpan = new SnapshotSpan(curSpan.Snapshot, new Span(lineStartLoc, 1));
                        yield return new TagSpan<PropertiesTokenTag>(separatorSpan, new PropertiesTokenTag(PropertiesSeparator));
                    }

                    var keySpan = new SnapshotSpan(curSpan.Snapshot, new Span(lineStartLoc + separatorCount, lineText.Length - separatorCount));
                    yield return new TagSpan<PropertiesTokenTag>(keySpan, new PropertiesTokenTag(PropertiesKey));
                }
            }
        }

        private void OnTextBufferChanged(object sender, TextContentChangedEventArgs e) {
            var snapshot = e.After;

            if (e.Changes.Count == 0) {
                return;
            }

            var min = snapshot.GetLineFromLineNumber(e.Changes.Min(change => snapshot.GetLineFromPosition(change.NewPosition).LineNumber));

            if (min != null) {
                TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(new SnapshotSpan(min.Start, buffer.CurrentSnapshot.Length - min.Start)));
            }
        }
    }

}