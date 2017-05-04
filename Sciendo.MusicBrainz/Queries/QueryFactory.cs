﻿using System;
using Neo4jClient;
using Sciendo.MusicMatch.Contracts;

namespace Sciendo.MusicBrainz.Queries
{
    public class QueryFactory : IQueryFactory
    {
        public MatchingQuery Get(QueryType queryType, GraphClient graphClient)
        {
            switch (queryType)
            {
                case QueryType.ArtistMatching:
                    return new MatchingArtistQuery(graphClient);
                case QueryType.AlbumMatching:
                    return new MatchingIndividualAlbumQuery(graphClient);
                case QueryType.TitleMatching:
                    return new MatchingTitleInIndividualAlbumQuery(graphClient);
                case QueryType.CollectionAlbumMatching:
                    return new MatchingCollectionAlbumQuery(graphClient);
                case QueryType.TitleInCollectionMatching:
                    return new MatchingTitleInCollectionAlbumQuery(graphClient);
                default:
                    return new MatchingArtistQuery(graphClient);
            }
        }
    }
}
