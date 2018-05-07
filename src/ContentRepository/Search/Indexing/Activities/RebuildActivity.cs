﻿using System;
using System.Linq;
using SenseNet.ContentRepository.Storage;
using SenseNet.Search;
using SenseNet.Search.Querying;

namespace SenseNet.ContentRepository.Search.Indexing.Activities
{
    [Serializable]
    internal class RebuildActivity : IndexingActivityBase
    {
        private static readonly int[] EmptyIntArray = new int[0];
        protected override bool ProtectedExecute()
        {
            // getting common versioning info
            var head = NodeHead.Get(NodeId);
            var versioningInfo = new VersioningInfo
            {
                Delete = EmptyIntArray,
                Reindex = EmptyIntArray,
                LastDraftVersionId = head.LastMinorVersionId,
                LastPublicVersionId = head.LastMajorVersionId
            };

            // delete documents by NodeId
            IndexManager.DeleteDocuments(new[] { new SnTerm(IndexFieldName.NodeId, NodeId)}, versioningInfo);

            // add documents of all versions
            var docs = IndexManager.LoadIndexDocumentsByVersionId(head.Versions.Select(v => v.VersionId).ToArray());
            foreach (var doc in docs)
                IndexManager.AddDocument(doc, versioningInfo);

            return true;
        }

        protected override string GetExtension()
        {
            return null;
        }
        protected override void SetExtension(string value)
        {
            // do nothing
        }
    }
}