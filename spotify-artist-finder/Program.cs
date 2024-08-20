using MetaBrainz.MusicBrainz;
using MetaBrainz.MusicBrainz.Interfaces.Entities;
using System;

namespace spotify_artist_finder
{
    class Program
    {
        public static void Main()
        {
            //output the instructions for using the program
            Console.WriteLine("Welcome to Spotify Easy Search! Here you can easily find new music related to artists you already enjoy!");
            Console.WriteLine("Please enter the name of the music artist which you would like to search for:");

            //store the artists name
            string artistName = Console.ReadLine();

            //create a new object for searching
            ArtistLookup db = new ArtistLookup();

            //get the members of the band which the user is searching for
            IArtist[] artists = db.FindRelatedArtists(artistName);

            //print out the band members for now
            Console.WriteLine("\nHere are the names of related artists: ");
            foreach (var artist in artists)
            {
                if (artist != null && artist.Name != null)
                Console.WriteLine(artist.Name);
            }
        }
    }
}
