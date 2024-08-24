using MetaBrainz.MusicBrainz;
using MetaBrainz.MusicBrainz.Interfaces.Entities;
using System;

namespace spotify_artist_finder
{
    class ArtistLookup {

        //query used for searching through the database to find artists
        private Query q;

        //related artists to the artist being searched for, with keys represented by relationship type and values as the artists profile
        private Dictionary<string, List<IArtist>> related_artists;

        //constructor for creating an object which can search for an artist in the database, and return lists of related artists
        public ArtistLookup()
        {
            //setup the query search using information to authenticate
            q = new Query("spotify-easy-search", "1.0", "https://github.com/Dylan22304");

            //key is the relationship type, value is a list of artists with that relationship type
            related_artists = new Dictionary<string, List<IArtist>>();
        }

        /*
         function which returns an entry in the database for an artist which the user is searching for

         param "search_query" is the name of the artist being searched
         */
        public IArtist FindArtist(string search_query)
        {
            if (search_query == null)
            {
                throw new Exception("THAT IS NOT A VALID MUSIC ARTIST");
            }

            //get a list of search results from the data base when just searching for the artists name
            var search_results = q.FindArtists(search_query, simple: true).Results;

            if (search_results == null)
            {
                throw new Exception("THIS SEARCH RETURNED NO ARTISTS IN THE DATABASE");
            }

            //get the databases's id for the top result and access the artist's entry
            var artistID = search_results[0].Item.Id;
            var artist = q.LookupArtist(new Guid($"{artistID}"), Include.ArtistRelationships);

            if (artist == null || artist.Type == null)
            {
                throw new Exception("THIS ARTIST DOES NOT HAVE A VALID ENTRY IN THE DATABASE");
            }

            return artist;
        }

        /*
         function to return a dictionary with related artists

         param "artist" is the entry in the database for the artist
         */
        public Dictionary<string, List<IArtist>> FindRelatedArtists(IArtist artist)
        {
            //get the artists related to the band
            var artist_relationships = artist.Relationships;
            if (artist_relationships == null)
            {
                throw new Exception("THIS ARTIST HAS NO RELATIONSHIPS LISTED IN THE DATABASE");
            }
            else
            {
                //iterate through each relationship
                foreach (var relation in artist_relationships)
                {
                    if (relation == null || relation.Artist == null || relation.Type == null || (relation.Artist.GetType().Equals(typeof(IArtist)) || relation.Artist.GetType().Equals(typeof(ILabel)))) continue;

                    //check if that relation type already exists
                    if (!related_artists.TryGetValue(relation.Type, out List<IArtist>? returned_list))
                    {
                        //add to the dictionary
                        related_artists.Add(relation.Type, new List<IArtist>());
                        related_artists[relation.Type].Add(relation.Artist);
                    }
                    else
                    {
                        returned_list.Add(relation.Artist);
                    }
                }

                //return the list of band members
                return related_artists;
            }
        }
        
        /*
         function returning a list of works made by a specific artist, excluding work made by the original band/person

         param "artist" is the entry in the database for the artist being searched for
         param "original_artist" is the entry in the database for the artist who the user originally searched for
         */
        public List<String> SongsMadeBy(IArtist artist)
        {
            //search for the current artists works
            var artist_works = q.BrowseAllWorks(artist, pageSize: 100, offset: 0);

            if (artist_works == null)
            {
                throw new Exception("THE SELECTED ARTIST DOES NOT HAVE ANY WORKS");
            }

            //convert to a list of strings for sorting
            List<String> related_songs = new List<String>();
            foreach (var song in artist_works)
            {
                related_songs.Add(song.Title);
            }
            related_songs.Sort();

            return related_songs;
        }
    }
}