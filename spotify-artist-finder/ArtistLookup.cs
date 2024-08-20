using MetaBrainz.MusicBrainz;
using MetaBrainz.MusicBrainz.Interfaces.Entities;
using System;

namespace spotify_artist_finder
{
    class ArtistLookup {

        //query used for searching through the database to find artists
        private Query q;

        //list of the members of a band if the user is searching for a band rather than an individual
        private IArtist[] band_members;

        //constructor for creating an object which can search for an artist in the database, and return the nam
        public ArtistLookup()
        {
            //setup the query search using information to authenticate
            q = new Query("spotify-easy-search", "1.0", "https://github.com/Dylan22304");

            //temp size which will be reset later when the number of band members is known
            band_members = new IArtist[1];
        }

        /*
         function to return an array of artists directly related the artist which the user is searching for
         for bands, this means finding works created by other members of the band
         for solo artists, this means finding side projects by the same artists

         param "search_query" is the name of the artist which is searching for
         */
        public IArtist[] FindRelatedArtists(string search_query)
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

            //determine whether the user is searching for a group or a solo artist
            if (artist.Type.Equals("Group"))
            {
                //get the artists related to the band
                var artist_relationships = artist.Relationships;
                if (artist_relationships == null)
                {
                    throw new Exception("THIS BAND DOES NOT HAVE VALID MEMBERS LISTED TO SEARCH FOR");
                }
                else
                {
                    //set the return array to the correct size (will be too large for some bands due to tributes and other relationships)
                    Array.Resize(ref band_members, artist_relationships.Count);

                    //iterate through each relationship and sort out the band members
                    int arr_index = 0;
                    foreach (var relation in artist_relationships)
                    {
                        if (relation == null || relation.Artist == null || relation.Type == null) continue;

                        if (relation.Type.Equals("member of band"))
                        {
                            //add this member to the array
                            band_members[arr_index] = relation.Artist;
                            arr_index++;
                        }
                    }

                    //return the list of band members
                    band_members = band_members.Distinct().ToArray();
                    return band_members;
                }
            }
            else
            {
                throw new Exception("SOLO ARTISTS ARE NOT SETUP YET");
            }
        }
    }
}